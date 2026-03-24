using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handler.Connect.Camera
{
    public class CameraManager
    {
        /// <summary>
        /// 前壳定位相机
        /// </summary>
        public static CameraHelper CameraFrontHolder = new CameraHelper("前壳定位相机",
            (IConnectDLL.IConnect)ConnectFactory.tcpConnect_Camera,
            () => ConnectFactory.tcpConnect_Camera.IsConnected);

        /// <summary>
        /// PCB定位相机
        /// </summary>
        public static CameraHelper CameraPCB = new CameraHelper("PCB定位相机",
            (IConnectDLL.IConnect)ConnectFactory.tcpConnect_Camera,
            () => ConnectFactory.tcpConnect_Camera.IsConnected);

    }
}
