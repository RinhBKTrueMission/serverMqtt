using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Server
{
    public class Process : BsonData.Document
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int MemorySize { get; set; } = 1;
        public int MainThreadInterval { get; set; } = 100;
        public bool IsRunning { get; set; }
        public string ShareDataPath { get; set; }


        public Process() { }
        public Process(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var s = args[i];
                switch (i)
                {
                    case 0: MemorySize = int.Parse(s); break;
                    case 1: MainThreadInterval = int.Parse(s); break;
                    case 2: ShareDataPath = Vst.Encoding.FromBase64(s); break;
                }
            }
        }
        public string GetStartArguments()
        {
            var path = Vst.Encoding.ToBase64(ShareDataPath);
            return string.Format("{0} {1} {2}",
                MemorySize,
                MainThreadInterval,
                path);
        }
        
    }
}
