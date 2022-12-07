using System;
using System.IO;
using System.Linq;

namespace FXRTestBed
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string effectZipPath = @"Assets\effect.zip";
            string effectPath = @"effect";
            string outputPath = @"effectOutput";
            string logDirPath = @"logs";
            string readLogPath = @"logs\readLog.txt";
            string writeLogPath = @"logs\writeLog.txt";

            int readCount = 0;
            int readExceptions = 0;
            int writeCount = 0;
            int writeExceptions = 0;

            // unzip effects
            if (!Directory.Exists(effectPath))
            {
                Directory.CreateDirectory(effectPath);
                System.IO.Compression.ZipFile.ExtractToDirectory(effectZipPath, effectPath);
            }

            // get effects
            string[] files = Directory.GetFiles(effectPath, "*.fxr");

            // make output directory
            if (Directory.Exists(outputPath)) Directory.GetFiles(outputPath).ToList().ForEach(outputFile => File.Delete(outputFile));
            else Directory.CreateDirectory(outputPath);

            // clean logs
            if (!Directory.Exists(logDirPath)) Directory.CreateDirectory(logDirPath);
            if (File.Exists(readLogPath)) File.Delete(readLogPath);
            if (File.Exists(writeLogPath)) File.Delete(writeLogPath);

            FileStream fsr = File.OpenWrite(readLogPath);
            StreamWriter swr = new StreamWriter(fsr);

            FileStream fsw = File.OpenWrite(writeLogPath);
            StreamWriter sww = new StreamWriter(fsw);

            Console.WriteLine($"Reading {files.Length} FXRs...");

            foreach (string file in files)
            {
                Fxr3ivi fxr;
                try
                {
                    fxr = Fxr3ivi.Read(file);
                    readCount++;
                }
                catch (Exception e)
                {
                    var line = $"{Path.GetFileName(file)}: {e.Message} - {e.StackTrace}";
                    readExceptions++;
                    swr.WriteLine(line);
                    Console.WriteLine(line);
                    break;
                }

                try
                {
                    // Console.WriteLine(fxr.Section14s.Count);
                    fxr.Write($@"{outputPath}\{Path.GetFileName(file)}");
                    writeCount++;
                }
                catch (Exception e)
                {
                    var line = $"{Path.GetFileName(file)}: {e.Message} - {e.StackTrace}";
                    writeExceptions++;
                    sww.WriteLine(line);
                    Console.WriteLine(line);
                    break;
                }

                try
                {
                    byte[] comp1 = File.ReadAllBytes(file);
                    byte[] comp2 = File.ReadAllBytes($@"{outputPath}\{Path.GetFileName(file)}");

                    if (Utils.ByteArrayCompare(comp1,comp2)) continue;

                    Console.WriteLine($"Binary check failed on {Path.GetFileName(file)}");

                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{Path.GetFileName(file)}: {e.Message} - {e.StackTrace}");
                    break;
                }
            }

            Console.WriteLine($"Read {readCount}/{files.Length} FXRs, received {readExceptions} exceptions");
            Console.WriteLine($"Wrote {writeCount}/{files.Length} FXRs, received {writeExceptions} exceptions");
        }
    }
}