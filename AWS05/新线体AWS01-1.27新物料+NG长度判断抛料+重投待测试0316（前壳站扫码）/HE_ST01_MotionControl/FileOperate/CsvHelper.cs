using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Handler.FileOperate
{
    public class CsvHelper : System.IDisposable
    {
        public Func<string> GetFilePathFunc { get; set; }
        public Func<string> GetFileNameFunc { get; set; }
        public Func<string> GetFullPathFunc { get; set; }

        private StreamReader _reader;
        private StreamWriter _writer;

        public string[] HeaderArray;

        public CsvHelper(Func<string> filePath, Func<string> fileName)
        {
            if(filePath==null) throw new ArgumentNullException(nameof(filePath));
            if(fileName == null) throw new ArgumentNullException(nameof(fileName));

            this.GetFilePathFunc = filePath;
            this.GetFileNameFunc = fileName;
            this.GetFullPathFunc = ()=>System.IO.Path.Combine(this.GetFilePathFunc(), this.GetFileNameFunc());
            CheckFileExistAndWriteHeader();
        }


        public void SetCSVHeader(params string[] items)
        {
            HeaderArray=items;
        }

        public void CheckFileExistAndWriteHeader()
        {
            if (!Directory.Exists(GetFilePathFunc()))
            {
                Directory.CreateDirectory(GetFilePathFunc());
               
            }
            if (!File.Exists(this.GetFullPathFunc()))
            {
                if (this.HeaderArray != null)
                {
                    using (_writer = new StreamWriter(GetFullPathFunc(), true))
                    {
                        foreach (var item in HeaderArray)
                        {
                            _writer.Write(item);
                            _writer.Write(",");
                        }
                        _writer.WriteLine();
                    }
                  
                }
            }
           

        }

        public void Write(params string[] items)
        {
            foreach (var item in items)
            {
                Write(item);
                Write(",");
            }
           
        }
        public void Write(string item)
        {
            CheckFileExistAndWriteHeader();

            using (_writer = new StreamWriter(GetFullPathFunc(), true))
            {
                _writer.Write(item);
            }
        }
        public void WriteLine(string item)
        {
            CheckFileExistAndWriteHeader();

            using (_writer = new StreamWriter(GetFullPathFunc(), true))
            {
                _writer.WriteLine(item);
            }
        }
        public void WriteLine(params string[] items)
        {
            foreach (var item in items)
            {
                Write(item);
                Write(",");
               
            }
            using (_writer = new StreamWriter(GetFullPathFunc(), true))
            {
                _writer.WriteLine();
            }
        }

        public void ReadAll()
        {
            using (_reader = new StreamReader(GetFullPathFunc(), System.Text.Encoding.GetEncoding("GB2313")))
            {
                List<string> list = new List<string>();
                while (_reader.Peek() >= 0)
                {
                    list.Add(_reader.ReadLine());
                }
            }
        }


        public void Close()
        {

        }
        public void Dispose()
        {
            _writer?.Dispose();
            _reader?.Dispose();
        }
    }
}
