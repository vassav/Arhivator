using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Arhivator
{
    /// <summary>
    /// Базовый абстрактый класс, предоставляющий функционал параллельной обработки файла
    /// </summary>
    public abstract class ParallelStream : Stream
    {
        #region Private filds

        private Exception _exeption;
        private long _position;
        private bool _isClosed;
        private readonly Stream _oStream;
        private bool _isFirstFrite;
        private Queue<int> _toInput;
        private Queue<int> _toWrite;
        private int _latestPack;
        private AutoResetEvent _resetEvent;
        private readonly object _latestLock = new object();
        private bool _isWritting;
        private int _lastWriten;

        #endregion

        protected List<WorckItem> Pools;
        protected int LastInput;
        protected int CurrentInput;


        /// <summary>
        /// Базовая инициализация потока параллельной обработки
        /// </summary>
        /// <param name="oStream">Поток, в который будет производится запись.</param>
        protected ParallelStream(Stream oStream)
        {
            _oStream = oStream;

        }

        private const int PoolByThread = 4;

        #region Property

        /// <summary>
        /// Возвращает значение, показывающее, поддерживает ли текущий поток возможность чтения.
        /// </summary>
        /// <returns>
        /// Значение true, если поток поддерживает чтение; в противном случае — значение false.
        /// </returns>
        public override bool CanRead
        {
            get { return false; }
        }

        /// <summary>
        /// Возвращает значение, которое показывает, поддерживает ли текущий поток возможность записи.
        /// </summary>
        /// <returns>
        /// Значение true, если поток поддерживает запись; в противном случае — значение false.
        /// </returns>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Возвращает значение, которое показывает, поддерживается ли в текущем потоке возможность поиска.
        /// </summary>
        /// <returns>
        /// Значение true, если поток поддерживает поиск; в противном случае — значение false.
        /// </returns>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Получает или задает позицию в текущем потоке.
        /// </summary>
        /// <returns>
        /// Текущее положение в потоке.
        /// </returns>
        public override long Position
        {
            get { return _position; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Возвращает последнюю ошибку
        /// </summary>
        public Exception Exception { get { return _exeption; } }


        #endregion

        /// <summary>
        /// Очищает все буферы данного потока и вызывает запись данных буферов в базовое устройство.
        /// </summary>
        public override void Flush()
        {
            _Flush(false);
        }


        /// <summary>
        /// Закрывает текущий поток и отключает все ресурсы (например, сокеты и файловые дескрипторы), 
        /// связанные с текущим потоком. Вместо вызова данного метода, убедитесь в том, что поток надлежащим образом ликвидирован.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public override void Close()
        {
            if (_isClosed) return;
            
            _Flush(true);

            _isClosed = true;
            _oStream.Dispose();
        }

        public new void Dispose()
        {
            Close();
            Dispose(true);
        }


        /// <summary>
        /// Записывает последовательность байтов в текущий поток.
        /// </summary>
        /// <param name="buffer">
        /// Массив байтов. Этот метод копирует байты <paramref name="count"/> из параметра <paramref name="buffer"/> в текущий поток.
        /// </param>
        /// <param name="offset">
        /// Смещение байтов (начиная с нуля) в <paramref name="buffer"/>, с которого начинается копирование байтов в текущий поток.
        /// </param>
        /// <param name="count">Количество байтов, которое необходимо записать в текущий поток.
        /// </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_isClosed)
                throw new InvalidOperationException();
            if (_exeption != null)
                throw _exeption;
            if (!_isFirstFrite)
            {
                InitPools();
                _isFirstFrite = true;
            }
            _position += count;
            var isWait = false;
            do
            {
                WriteBuffers(false,isWait);
                isWait = false;
                int idx;
                if (CurrentInput >= 0)
                {
                    idx = CurrentInput;
                }
                else
                {
                    if (_toInput.Count == 0)
                    {
                        isWait = true;
                        continue;
                    }
                    idx = _toInput.Dequeue();
                    LastInput++;
                }
                var limit = FillBuffer(buffer, offset, count, idx);
                count -= limit;
                offset += limit;


            } while (count>0);
            
        }

        /// <summary>
        /// При перепреоделении в производном классе определяет поведение заполнения входного буфера рабочего элемента.
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
        protected abstract int FillBuffer(byte[] buffer, int offset, int count, int idx);
        
        
        /// <summary>
        /// При переопределении в производном классе, определяет поведение записи данных из рабочего элемента в выходной поток
        /// </summary>
        /// <param name="item">Рабочий элемент</param>
        protected abstract void ItemFlush(WorckItem item);

        #region Private methods

        private void _Flush(bool lastInput)
        {
            if (_isClosed)
                throw new InvalidOperationException();

            if (_isWritting) return;

            if (CurrentInput >= 0)
            {
                var item = Pools[CurrentInput];
                _WriteItem(item);
                CurrentInput = -1; 
            }

            WriteBuffers(lastInput, false);
        }

        private void InitPools()
        {
            _resetEvent=new AutoResetEvent(false);
            _toInput = new Queue<int>();
            _toWrite = new Queue<int>();
            Pools = new List<WorckItem>();
            var poolCount = Environment.ProcessorCount*PoolByThread;
            for (var i = 0; i < poolCount; i++)
            {
                Pools.Add(new WorckItem(i));
                _toInput.Enqueue(i);
            }
            LastInput = -1;
            _latestPack = -1;
            CurrentInput = -1;
            _lastWriten = -1;
            


        }


        protected void _WriteItem(object objectItem)
        {
            try
            {
                var item = (WorckItem) objectItem;

                lock (_latestLock)
                {
                    if (item.Ordinal > _latestPack)
                        _latestPack = item.Ordinal;
                }


                ItemFlush(item);



                lock (_toWrite)
                {
                    _toWrite.Enqueue(item.Index);
                }
                _resetEvent.Set();
            }
            catch (Exception e)
            {
                _exeption = e;
            }
        }

        private void WriteBuffers(bool isAll, bool isWait)
        {
            if (_isWritting) return;
            _isWritting = true;

            do
            {

                if (isWait || isAll)
                {
                    _resetEvent.WaitOne();
                }

                int firstSkip = -1;
                int waitMs = isAll ? 200 : (isWait ? -1 : 0);

                int nextToWrite;
                do
                {
                    if (Monitor.TryEnter(_toWrite, waitMs))
                    {
                        nextToWrite = -1;
                        try
                        {
                            if (_toWrite.Count > 0)
                                nextToWrite = _toWrite.Dequeue();

                        }
                        finally
                        {
                            Monitor.Exit(_toWrite);
                        }
                        if (nextToWrite >= 0)
                        {
                            var item = Pools[nextToWrite];
                            if (item.Ordinal != _lastWriten + 1)
                            {
                                lock (_toWrite)
                                {
                                    _toWrite.Enqueue(nextToWrite);
                                }

                                if (firstSkip == nextToWrite)
                                {
                                    _resetEvent.WaitOne();
                                    firstSkip = -1;
                                }
                                else if (firstSkip == -1)
                                    firstSkip = nextToWrite;

                                continue;

                            }
                            firstSkip = -1;
                            _oStream.Write(item.OutputBuffer, 0, item.OutputSize);
                            item.InputOffset = 0;
                            _lastWriten = item.Ordinal;
                            item.Ordinal = -1;
                            _toInput.Enqueue(item.Index);

                            if (waitMs == -1) waitMs = 0;

                        }
                    }
                    else
                        nextToWrite = -1;



                } while (nextToWrite >= 0);
            } while (isAll && (_lastWriten != _latestPack));
            _isWritting = false;

        }

        #endregion

        #region NotSupported

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}