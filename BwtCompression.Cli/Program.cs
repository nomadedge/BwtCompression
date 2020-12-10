using BwtCompression.Cli.Enums;
using BwtCompression.Coders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BwtCompression.Cli
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            try
            {
                var mode = args[0] == "e" ? Mode.Encoding : Mode.Decoding;
                var fileFullName = args[1];
                if (!File.Exists(fileFullName))
                {
                    throw new FileNotFoundException("File not found.");
                }

                var stopwatch = new Stopwatch();

                Console.WriteLine("Starting the task...");
                stopwatch.Start();

                var fileBytes = File.ReadAllBytes(fileFullName).ToList();

                var resultFileBytes = new List<byte>();
                var resultFileFullName = string.Empty;

                switch (mode)
                {
                    case Mode.Encoding:
                        resultFileBytes = Encode(fileBytes);
                        resultFileFullName = $"{fileFullName}Encoded";
                        Console.WriteLine("Encoding has been finished successfully.");
                        break;
                    case Mode.Decoding:
                        resultFileBytes = Decode(fileBytes);
                        resultFileFullName = $"{fileFullName}Decoded";
                        Console.WriteLine("Decoding has been finished successfully.");
                        break;
                }

                File.WriteAllBytes(resultFileFullName, resultFileBytes.ToArray());
                Console.WriteLine($"File was saved as \"{Path.GetFileName(resultFileFullName)}\" in the same directory.");

                stopwatch.Stop();
                Console.WriteLine($"It took {stopwatch.Elapsed.TotalSeconds} seconds.");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private static List<byte> Encode(List<byte> fileBytes)
        {
            var encoder = new Encoder(fileBytes);
            return encoder.Encode();
        }

        private static List<byte> Decode(List<byte> fileBytes)
        {
            var decoder = new Decoder(fileBytes);
            return decoder.Decode();
        }
    }
}
