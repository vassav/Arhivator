using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Arhivator
{
    /// <summary>
    /// Предоставляет функционал параллельной упаковки файла
    /// </summary>
    public class ParallelCompress : ParallelStream
    {
        /// <summary>
        /// Инициализирует класс параллельной упаковки файла
        /// </summary>
        /// <param name="oStream">Поток, в который будет производится запись.</param>
        public ParallelCompress(Stream oStream)
            : base(oStream)
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
            item.Ordinal = LastInput;
            int length = item.InputBuffer.Length-200;
            int limit = (length - item.InputOffset) > count
                            ? count
                            : length - item.InputOffset;
            Buffer.BlockCopy(buffer, offset, item.InputBuffer, item.InputOffset, limit);
            item.InputOffset += limit;
            if (length == item.InputOffset)
            {

                if (!ThreadPool.QueueUserWorkItem(_WriteItem, item))
                    throw new Exception(Strings.ErrorAddTask);
                CurrentInput = -1;
            }
            else
            {
                CurrentInput = idx;
            }
            return limit;
        }

        /// <summary>
        /// Производит упаковку данных их буфера рабочего элемента и записывает их в выходной поток
        /// </summary>
        /// <param name="item">Рабочий элемент</param>
        protected override void ItemFlush(WorckItem item)
        {
            item.MemoryStream.Position = 8;
            
            
            using (var st = new GZipStream(item.MemoryStream, CompressionMode.Compress, true))
                st.Write(item.InputBuffer, 0, item.InputOffset);
            item.OutputSize = (int)item.MemoryStream.Position;

            Buffer.BlockCopy(BitConverter.GetBytes(item.OutputSize), 0, item.OutputBuffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(item.Ordinal), 0, item.OutputBuffer, 4, 4);

        }
    }
}