using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab1_Compression
{
    class RLE
    {
        private static int sizeCompressedFile { get; set; }
        private static int sizeOriginalSize { get; set; }
        /// <summary>
        /// This method compress a file with the RLE method
        /// </summary>
        /// <param name="pathFileToCompress"></param>
        public static void compression(string pathFileToCompress)
        {
            StreamWriter outputfile = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.GetFileNameWithoutExtension(pathFileToCompress)));
            outputfile.WriteLine("0," + Path.GetFileName(pathFileToCompress));
            outputfile.Flush();
            outputfile.Close();
            using (var file = new FileStream(pathFileToCompress, FileMode.Open))
            {
                sizeOriginalSize = (int)file.Length;
                using (var reader = new BinaryReader(file))
                {
                    var bytes = reader.ReadBytes((int)file.Length);

                    int counterOfEquals = 1;
                    for (int i = 0; i < file.Length; i++)
                    {
                        if (i != file.Length - 1 && bytes[i] == bytes[i + 1])
                        {
                            counterOfEquals++;
                        }
                        else
                        {
                            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.GetFileNameWithoutExtension(pathFileToCompress));
                            using (var outputFile = new FileStream(path + ".comp", FileMode.Append))
                            {
                                using (var writer = new BinaryWriter(outputFile, Encoding.ASCII))
                                {
                                    var counterOfEqualsInChar = (char)counterOfEquals;
                                    writer.Write(counterOfEqualsInChar);
                                    writer.Write(bytes[i]);
                                    counterOfEquals = 1;
                                }
                            }

                        }
                    }
                }
            }
        }


        /// <summary>
        /// This method decompress a file that was compressed by RLE method
        /// </summary>
        /// <param name="filepath"></param>
        public static void decompression(string filepath)
        {
            StreamReader reader1 = new StreamReader(filepath);
            string firstLine = reader1.ReadLine();
            var info = firstLine.Split(',');
            string fileName = info[1];
            int firsLineLength = firstLine.Length;
            reader1.Close();
            using (var file = new FileStream(filepath, FileMode.Open))
            {
                sizeCompressedFile = (int)file.Length;
                using (var currentFile = new BinaryReader(file))
                {
                    var bytes = currentFile.ReadBytes((int)file.Length);
                    int count = 0;
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                    using (var outputFile = new FileStream(path + "\\D" + fileName, FileMode.Append))
                    {
                        using (var writer = new BinaryWriter(outputFile, Encoding.ASCII))
                        {
                            for (int i = firsLineLength + 2 /*Empiezo a leer despues de la linea donde esta la infoDelArchivo y el +2 es para saltarme el salto de linea y el ENTER*/; i < bytes.Length; i++)//fijo ya se que el primero es un número 
                            {
                                count = (int)bytes[i];
                                while (count > 0)
                                {
                                    writer.Write(bytes[i + 1]);
                                    count--;
                                }
                                i++;

                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gives the ratio of compression
        /// </summary>
        /// <returns></returns>
        public static int compressionRatio()
        {
            int sizeAfter = sizeCompressedFile;
            int sizeBefore = sizeOriginalSize;
            return sizeAfter / sizeBefore;
        }

        /// <summary>
        /// Gives the compression factor
        /// </summary>
        /// <returns></returns>
        public static int compressionFactor()
        {
            int sizeAfter = sizeCompressedFile;
            int sizeBefore = sizeOriginalSize;
            return sizeBefore / sizeAfter;
        }

        /// <summary>
        /// Gives the saving percentage
        /// </summary>
        /// <returns></returns>
        public static double savingPercentage()
        {
            int sizeAfter = sizeCompressedFile;
            int sizeBefore = sizeOriginalSize;
            return (sizeBefore - sizeAfter) / sizeBefore * 100;
        }


    }
}
