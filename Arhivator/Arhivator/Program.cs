using System;

namespace Arhivator
{
    class Program
    {
        private static Compressor _compressor;

        static void Main(string[] args)
        {

            Console.CancelKeyPress += CancelWorck;

            if (args.Length < 3)
            {
                Console.WriteLine(Strings.ErrorSyntax);
                Compressor.PrintInfo();
                return;
            }

            using (_compressor = new Compressor((args[0] ?? "").ToLowerInvariant(), args[1], args[2]))
            {
                _compressor.Run();
            }
            
        }



        private static void CancelWorck(object sender, ConsoleCancelEventArgs e)
        {
            if(_compressor!=null)
                _compressor.Dispose();
            Console.WriteLine(Strings.Cancel);

        }

    }
}
