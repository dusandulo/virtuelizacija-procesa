using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FileHandle : IDisposable
    {
        private bool disposedValue;
        public string FileName { get; set; }
        public MemoryStream MemoryStream { get; set; }
        public FileHandle(MemoryStream memoryStream, string fileName)
        {
            MemoryStream = memoryStream;
            FileName = fileName;
        }

        public FileHandle()
        {
            MemoryStream = new MemoryStream();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                disposedValue = true;
            }
        }

        ~FileHandle()
        {
             Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
