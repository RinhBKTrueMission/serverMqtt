using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Mvc;
using System.Runtime.InteropServices;

namespace Vst.Server
{
    using SE = System.Text.Encoding;
    public class Memory
    {
        Mutex _mutex;
        MemoryMappedFile _mmf;

        string _name;
        public string Name => _name;

        public Memory(string name)
        {
            _name = name.ToLower();
        }

        public void Create(long size)
        {
            try
            {
                string mutexName = _name + "-mutex";

                if (!Mutex.TryOpenExisting(mutexName, out _mutex))
                {
                    bool b;
                    _mutex = new Mutex(true, mutexName, out b);
                    _mutex.ReleaseMutex();

                    _mmf = MemoryMappedFile.CreateNew(_name, size);
                }
                else
                {
                    _mmf = MemoryMappedFile.OpenExisting(_name);
                }
                //_mutex.WaitOne();
                //_mutex.ReleaseMutex();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static bool IsExists(string name)
        {
            try
            {
                using (MemoryMappedFile.OpenExisting(name.ToLower())) { }
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool IsCreated => IsExists(_name);

        public Memory Open()
        {
            try
            {
                _mmf = MemoryMappedFile.OpenExisting(_name);
                _mutex = Mutex.OpenExisting(_name + "-mutex");
                return this;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
            return null;
        }

        public void Dispose()
        {
            _mutex?.Dispose();
            _mmf?.Dispose();
        }
        public void Streaming(Action<MemoryMappedViewStream> callback)
        {
            _mutex.WaitOne();
            using (var stream = _mmf.CreateViewStream())
            {
                callback.Invoke(stream);
            }
            _mutex.ReleaseMutex();
        }
        public void Streaming(Action<MemoryMappedViewStream, int> callback)
        {
            _mutex.WaitOne();
            using (var stream = _mmf.CreateViewStream())
            {
                int len = 0;
                for (int i = 0; i < 4; i++)
                {
                    len |= (stream.ReadByte() << (i << 3));
                }

                callback.Invoke(stream, len);
            }
            _mutex.ReleaseMutex();
        }
    }
    public class ShareMemory : Memory
    {
        public void SetDataLength(MemoryMappedViewStream s, int v)
        {
            s.Position = 0;
            for (int i = 0; i < 4; i++, v >>= 8)
            {
                s.WriteByte((byte)(v & 255));
            }
        }
        public ShareMemory(string name) : base(name) { }
        public List<T> ReadObject<T>()
        {
            List<T> lst = null;
            Streaming((stream, len) => {
                if (len == 0) { return; }

                var buff = new byte[len];
                stream.Read(buff, 0, len);

                try
                {
                    var v = SE.UTF8.GetString(buff);
                    lst = Json.GetObject<List<T>>(v);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                SetDataLength(stream, 0);
            });
            return lst;
        }
        public void WriteBytes(byte[] v)
        {
            Streaming((stream, len) => {
                if (len > 0)
                {
                    stream.Position = (--len) + 4;
                }

                stream.WriteByte((byte)(len == 0 ? '[' : ','));
                stream.Write(v, 0, v.Length);
                stream.WriteByte((byte)']');

                SetDataLength(stream, len + v.Length + 2);
            });
        }
        public void WriteObject(object value)
        {
            WriteBytes(SE.UTF8.GetBytes(Json.GetString(value)));
        }
        public void AsyncReading<T>(int interval, Action<List<T>> callback)
        {
            var ts = new ThreadStart(() => {
                while (true)
                {
                    var s = this.ReadObject<T>();
                    if (s != null)
                    {
                        callback.Invoke(s);
                    }
                    Thread.Sleep(interval);
                }
            });
            new Thread(ts).Start();
        }
        new public ShareMemory Open()
        {
            return (ShareMemory)base.Open();
        }
    }
}