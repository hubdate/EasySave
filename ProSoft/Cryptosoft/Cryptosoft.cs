using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace ProSoft.Cryptosoft {

    public class Cryptosoft {
        private readonly string __key;
        private static Cryptosoft __instance;
        private readonly Stopwatch __stopwatch;
        private readonly HashSet<string> __extensions;

        private Cryptosoft() { }

        private Cryptosoft(string key) {
            __key = key;
            __stopwatch = new Stopwatch();
            __extensions = new HashSet<string>();
        }

        private Cryptosoft(string key, HashSet<string> extensions) {
            __key = key;
            __stopwatch = new Stopwatch();
            __extensions = extensions;
        }

        public static Cryptosoft Init(string key, string[] extensions = null) {
            if (key.Length < 8) { throw new Exception("Key must be at least 64 bits long"); }

            if (extensions != null) 
            { __instance ??= new Cryptosoft(key, extensions.ToHashSet()); }
            
            else 
            { __instance ??= new Cryptosoft(key); }

            return __instance;
        }

        public long ProcessFile(string inputFile, string outputFile = null, bool isLargeFile = false) {
            try {
                if (outputFile == null) { 
                    if (inputFile.EndsWith(".cryptosoft")) { 
                        outputFile = inputFile[..inputFile.IndexOf(".")] + "_2" + inputFile[(inputFile.IndexOf(".") +1)..].Replace(".cryptosoft", ""); 
                    } 
                    else { outputFile = inputFile + ".cryptosoft"; }
                }

                __stopwatch.Restart();
                using var fin = new FileStream(inputFile, FileMode.Open);
                using var fout = new FileStream(outputFile, FileMode.Create);
                var buffer = new byte[4096];

                while (true) {
                    var read = fin.Read(buffer);
                    if (read == 0) { break; }

                    for (var i = 0; i < read; i++) { buffer[i] ^= (byte)(buffer[i] ^__key[i % __key.Length]); }
                    fout.Write(buffer, 0, read);
                }
                __stopwatch.Stop();
                return __stopwatch.ElapsedMilliseconds;
            } 
            
            catch { return -1; }
        }

        public Dictionary<string, long> ProcessFiles(string[] inputFiles, string outputDirectory = null) {
            Dictionary<string, long> results = new Dictionary<string, long>();
            List<Task> tasks = new List<Task>();
            foreach (string file in inputFiles) {
                string outputFile;
                if (outputDirectory == null) {
                    outputFile = file;
                    if (file.EndsWith(".cryptosoft")) { 
                        outputFile = $"{file[..file.IndexOf(".")]}_2{file[(file.IndexOf(".") +1)..].Replace(".cryptosoft", "")}"; 
                    } 
                    else { outputFile += ".cryptosoft"; }
                }
                else { outputFile = $"{outputDirectory}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(file)}.cryptosoft"; }

                if (__extensions.Count == 0 || __extensions.Contains(Path.GetExtension(file))) {
                    try {
                        tasks.Add(Task.Run(() => {
                            try {
                                long time = ProcessFile(file, outputFile, new FileInfo(file).Length > (200 * 1024 * 1024));
                                results.Add(outputFile, time);
                            }
                            catch { results.Add(outputFile, -1); }
                        }));
                    }
                    catch { results.Add(outputFile, -1); }
                }
                else { results.Add(outputFile, -2); }
            }
            Task.WaitAll(tasks.ToArray());

            return results;
        }

        public static void Main(string[] args) {
            // if (args.Length < 1) { throw new Exception("Usage: cryptosoft <source> <desitnation> <key>"); }
            // if (args.Length > 3) { throw new Exception("Usage: cryptosoft <source> <desitnation> <key>"); }

            // string key;
            // if (args.Length == 3) { key = args[2]; }
            // else {
            //     var seed = new Random();
            //     const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            //     var _ = new char[96];
            //     for (var i = 0; i < 96; i++) { _[i] = chars[seed.Next(chars.Length)]; }
            //     key = new string(_);
            // }

            // Cryptosoft cs = Init(key);
            // cs.ProcessFile(args[0], null);

            string extension = Path.GetExtension(@"C:\Users\acer\Desktop\Test\origin\Test").ToLower();

            Console.WriteLine($"{extension}");
        }
    }
}