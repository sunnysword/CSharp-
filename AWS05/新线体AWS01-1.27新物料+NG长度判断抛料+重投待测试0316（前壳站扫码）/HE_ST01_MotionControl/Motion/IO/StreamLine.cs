using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Handler.Motion.IO
{
    public class StreamLine
    {
        public static readonly StreamLine Cur = new StreamLine();
        private StreamLine()
        {
        }
        public static StreamLine CreateInstance()
        {
            return Cur;
        }

        //可用，建议优化流水线，让调用启停的case或方法一定只使用一次
        public ConveyorController ReturnFrontConveyor = new ConveyorController(() => StaticIOHelper.前段回流线启停.Out_ON(), () => { }, () => StaticIOHelper.前段回流线启停.Out_OFF(), "回流前流水线");
        public ConveyorController ReturnBackConveyor = new ConveyorController(() => StaticIOHelper.后段回流线启停.Out_ON(), () => { }, () => StaticIOHelper.后段回流线启停.Out_OFF(), "回流后流水线");

        public ConveyorController ConveyorFront = new ConveyorController(() => StaticIOHelper.前段输送线启停.Out_ON(), () => { }, () => StaticIOHelper.前段输送线启停.Out_OFF(), "流水线前段");
        public ConveyorController ConveyorBack = new ConveyorController(() => StaticIOHelper.后段输送线启停.Out_ON(), () => { }, () => StaticIOHelper.后段输送线启停.Out_OFF(), "流水线后段");


    }

    public enum Direction
    {
        Forward,
        Reverse
    }

    public class ConveyorController
    {
        public enum State { Stopped, Forward, Reverse, Paused }

        private State _state = State.Stopped;
        private int _userCount = 0;
        private Direction? _lastDirection = null;
        private readonly object _sync = new object();

        private readonly Action _startForward;
        private readonly Action _startReverse;
        private readonly Action _stop;
        private string Name = "";

        public ConveyorController(Action startForward, Action startReverse, Action stop, string name)
        {
            _startForward = startForward ?? throw new ArgumentNullException(nameof(startForward));
            _startReverse = startReverse ?? throw new ArgumentNullException(nameof(startReverse));
            _stop = stop ?? throw new ArgumentNullException(nameof(stop));

            //订阅外部状态机事件
            StaticInitial.Motion.actionPauseEventHandler += PauseConveyor;
            StaticInitial.Motion.actionStartEventHandler += ResumeOrStart;
            StaticInitial.Motion.actionStopEventHandler += StopConveyor;
            Name = name;
        }

        private void ResumeOrStart()
        {
            return;
            lock (_sync)
            {
                if (_state == State.Paused && _lastDirection.HasValue)
                {
                    //暂停后继续
                    _state = _lastDirection == Direction.Forward ? State.Forward : State.Reverse;
                    InvokeStart(_lastDirection.Value);
                }
                else
                {
                    //全新启动，清空计数
                    _state = State.Stopped;
                    _userCount = 0;
                }
            }
        }

        private void PauseConveyor()
        {
            return;
            lock (_sync)
            {
                Thread.Sleep(1000);
                if (_state == State.Forward || _state == State.Reverse)
                {
                    _lastDirection = (_state == State.Forward) ? Direction.Forward : Direction.Reverse;
                    _state = State.Paused;
                    _stop();
                }
            }
        }

        private void StopConveyor()
        {
            lock (_sync)
            {
                _userCount = 0;
                _state = State.Stopped;
                _lastDirection = null;
                _stop();
            }
        }

        public State GetCurrentState()
        {
            return _state;
        }

        public void RequestRun(Direction dir)
        {
            lock (_sync)
            {
                //暂停时直接恢复
                if (_state == State.Paused)
                {
                    ResumeOrStart();
                    return;
                }

                switch (_state)
                {
                    case State.Stopped:
                        //停止=>运行
                        _state = dir == Direction.Forward ? State.Forward : State.Reverse;
                        _userCount = 1;
                        _lastDirection = dir;
                        InvokeStart(dir);
                        break;

                    case State.Forward when dir == Direction.Forward:
                    case State.Reverse when dir == Direction.Reverse:
                        //同方向，计数++
                        _userCount++;
                        break;

                    case State.Forward when dir == Direction.Reverse:
                    case State.Reverse when dir == Direction.Forward:
                        //不允许在有用户时直接反转，抛出异常或错误处理
                        throw new InvalidOperationException(
                            $"Cannot switch direction from {_state} to {dir} while there are active users: {_userCount}.");

                    default:
                        //其他情况当作全新请求处理
                        _state = dir == Direction.Forward ? State.Forward : State.Reverse;
                        _userCount = 1;
                        _lastDirection = dir;
                        InvokeStart(dir);
                        break;
                }
            }
        }

        public void RequestStop()
        {
            lock (_sync)
            {
                if (_state == State.Stopped) return;

                _userCount--;
                if (_userCount <= 0)
                {
                    _userCount = 0;
                    _state = State.Stopped;
                    _lastDirection = null;
                    _stop();
                }
            }
        }

        private void InvokeStart(Direction dir)
        {
            if (dir == Direction.Forward)
                _startForward();
            else
                _startReverse();
        }
    }
}
