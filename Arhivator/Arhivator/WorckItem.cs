using System.IO;
using System.IO.Compression;

namespace Arhivator
{
    public class WorckItem
    {
        public WorckItem(int idx)
        {
            InputBuffer=new byte[BufferSize];
            OutputBuffer=new byte[BufferSize+BufferSize/2];
            Index = idx;
            MemoryStream=new MemoryStream(OutputBuffer);
            Ordinal = -1;
        }

        /// <summary>
        /// Размер буфера
        /// </summary>
        public const int BufferSize = 512*1024;

        /// <summary>
        /// Входной буфер
        /// </summary>
        public byte[] InputBuffer { get; set; }

        public int InputOffset { get; set; }

        public int InputSize { get; set; }

        public byte[] OutputBuffer { get; set; }

        public int Index { get; set; }

        public int Ordinal { get; set; }

        public int OutputSize { get; set; }

        public Stream MemoryStream { get; set; }

        
    }
}