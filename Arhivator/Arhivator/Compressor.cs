using System;
using System.IO;

namespace Arhivator
{
    /// <summary>
    /// Производит упаковку/распаковку файлов
    /// </summary>
    public class Compressor:IDisposable
    {
        private const int BufSize = 512;

        private readonly string _iFile;
        private readonly string _oFile;
        private readonly string _method;

        private Stream _iStream;

        private Stream _oStream;
        private bool _isComplete;

        private bool _isDisposed;

        /// <summary>
        /// Выполняет инициализацию класса, производящего упаковку/распаковку файлов
        /// </summary>
        /// <param name="method">
        /// Указывает какое действие выполнять.
        /// p - упаковка файла
        /// u - распаковка файла
        /// </param>
        /// <param name="iFile">Входной файл</param>
        /// <param name="oFile">Выходдной файл</param>
        public Compressor(string method, string iFile, string oFile)
        {
            _method = method;
            _iFile = iFile;
            _oFile = oFile;
            _isComplete = false;
        }

        /// <summary>
        /// Запускает процесс обработки файла
        /// </summary>
        /// <returns>Возвращает true если упаковка прошла успешно, в противном случае - false</returns>
        public bool Run()
        {
            if (
                !ValidateMethod()
                ||!ValidateInput()
                ||!ValidateOutput()
                ||!OpenReader()
                ||!OpenWriter()
                ) 
                return false;

            Console.WriteLine(Strings.Compressor_Run_Start);

            using (var st = _method == "p" ? (ParallelStream)new ParallelCompress(_oStream) : new ParallelDecompress(_oStream))
            {

                var buf = new byte[BufSize];
                try
                {
                    int len;
                    while ((len = _iStream.Read(buf, 0, BufSize)) > 0)
                    {
                        if (_isDisposed)
                            return false;
                        st.Write(buf, 0, len);
                        if (st.Exception != null)
                        {
                            Console.WriteLine(Strings.Compressor_Run_WorckError);
                            Console.WriteLine(st.Exception.Message);
                            return false;
                        }
                        if (_isDisposed)
                            return false;
                    }
                    _isComplete = true;
                    Console.WriteLine(Strings.Compressor_Run_Finish);
                }
                catch (Exception e)
                {
                    Console.WriteLine(Strings.Compressor_Run_WorckError);
                    Console.Write(e.Message);
                    return false;
                }
            }

            return true;

        }

        private bool OpenWriter()
        {
            try
            {
                
                _oStream = File.Create(_oFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(Strings.ErrorCreateOutputFile);
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private bool OpenReader()
        {
            try
            {
                _iStream = File.OpenRead(_iFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(Strings.ErrorOpenInputFile);
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private bool ValidateOutput()
        {
            if (!ValidatePath(_oFile))
            {
                Console.WriteLine(Strings.ErrorOutputFileName);
                PrintInfo();
                return false;
            }
            return true;
        }

        private bool ValidateInput()
        {
            if (!ValidatePath(_iFile) || !File.Exists(_iFile))
            {
                Console.WriteLine(Strings.ErrorInputFileName);
                PrintInfo();
                return false;
            }
            
            return true;
        }

        private bool ValidateMethod()
        {

            if (_method != "p" && _method != "u")
            {
                Console.WriteLine(Strings.ErrorSyntax);
                PrintInfo();
                return false;
            }
            return true;

        }

        private bool ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return false;
            var lastSlash = path.LastIndexOf('\\');
            if (lastSlash == path.Length - 1)
                return false;
            if (path.IndexOfAny(Path.GetInvalidFileNameChars(), lastSlash + 1) >= 0)
                return false;
            return true;

        }


        public static void PrintInfo()
        {
            Console.WriteLine(Strings.CommandInfo);
        }



        public void Dispose()
        {
            _isDisposed = true;
            if(_iStream!=null)
                _iStream.Dispose();
            if(_oStream!=null)
                _oStream.Dispose();
            if(!_isComplete)
                try
                {
                    File.Delete(_oFile);
                }
                catch (Exception)
                {
                    
                }

        }
    }
}