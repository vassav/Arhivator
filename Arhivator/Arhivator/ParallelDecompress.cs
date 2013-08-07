using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Arhivator
{
    /// <summary>
    /// Предоставляет функционал параллельной распаковки файла
    /// </summary>
    public class ParallelDecompress:ParallelStream
    {
        public ParallelDecompress(Stream oStream) : base(oStream)
        {
        }

        /// <summary>
        /// Определяет поведение заполнения входного буфера рабочего элемента.
        /// </summary>
        /// <param name="buffer">
        /// Массив байтов. Этот метод копирует байты <paramref name="count"/> из параметра <paramref name="buffer"/> в текущий поток.
        /// </param>
        /// <param name="offset">
        /// Смещение байтов (начиная с нуля) в <paramref name="buffer"/>, с которого начинается копирование байтов в буфер текущего рабочего элемента.
        /// </param>
        /// <param name="count">Количество байтов, которое необходимо записать в буфер текущего рабочего элемента.</param>
        /// <param name="idx">Индекс текущего рабочего элемента</param>
        /// <returns>Возвращает количество считанных байт</returns>
        protected override int FillBuffer(byte[] buffer, int offset, int count, int idx)
        {
            var item = Pools[idx];
            var headOffset=0;
            if (item.Ordinal < 0)
            {
                if (buffer.Length < offset + 8)
                {
                    Buffer.BlockCopy(buffer,offset,item.InputBuffer,0,count);
                    item.InputOffset = count;
                    CurrentInput = idx;
                    return count;
                }
                headOffset = 8 - item.InputOffset;

                Buffer.BlockCopy(buffer,offset,item.InputBuffer,item.InputOffset,headOffset);
                item.InputOffset = 0;
                item.InputSize = BitConverter.ToInt32(item.InputBuffer, 0) - 8;

                item.Ordinal = BitConverter.ToInt32(item.InputBuffer, 4);

                offset += headOffset;
                count -= headOffset;
            }
            if(item.Ordinal!=LastInput)
                throw new FormatException(Strings.ErrorFileFormat);
            int limit = (item.InputSize - item.InputOffset) > count
                ? count
                : item.InputSize - item.InputOffset;
            Buffer.BlockCopy(buffer, offset, item.InputBuffer, item.InputOffset, limit);
            item.InputOffset += limit;
            if (item.InputSize == item.InputOffset)
            {

                if (!ThreadPool.QueueUserWorkItem(_WriteItem, item))
                    throw new Exception(Strings.ErrorAddTask);
                CurrentInput = -1;
            }
            else
            {
                CurrentInput = idx;
            }
            return limit+headOffset;

            
        }


        /// <summary>
        /// Производит расспаковку данных их буфера рабочего элемента и записывает их в выходной поток
        /// </summary>
        /// <param name="item">Рабочий элемент</param>
        protected override void ItemFlush(WorckItem item)
        {
            item.MemoryStream.Position = 0;
            
                using (var st = new GZipStream(new MemoryStream(item.InputBuffer,0,item.InputOffset), CompressionMode.Decompress,true))
                {
                    st.CopyTo(item.MemoryStream);
                    item.OutputSize = (int) item.MemoryStream.Position;
                }


        }
    }
}