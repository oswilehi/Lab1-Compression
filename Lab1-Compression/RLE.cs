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
        public  int sizeCompressedFile { get; set; }
        public  int sizeOriginalSize { get; set; }
        /// <summary>
        /// This method compress a file with the RLE method
        /// </summary>
        /// <param name="pathFileToCompress"></param>
        public  void compression(string pathFileToCompress)
        {
            StreamWriter outputfile = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.GetFileNameWithoutExtension(pathFileToCompress) + ".rlex"));
            string data = "0," + Path.GetFileName(pathFileToCompress);
            outputfile.WriteLine(data);
            int count = data.Length;
            outputfile.Flush();
            outputfile.Close();
            using (var file = new FileStream(pathFileToCompress, FileMode.Open))
            {
                sizeOriginalSize = (int)file.Length;
                using (var reader = new BinaryReader(file))
                {
                    var bytes = reader.ReadBytes((int)file.Length);
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.GetFileNameWithoutExtension(pathFileToCompress));
                    using (var outputFile = new FileStream(path + ".rlex", FileMode.Append))
                    {
                        int counterOfEquals = 1;
                        using (var writer = new BinaryWriter(outputFile, Encoding.ASCII))
                        {
                            for (int i = 0; i < file.Length; i++)
                            {
                                if (i != file.Length - 1 && bytes[i] == bytes[i + 1])
                                {
                                    counterOfEquals++;
                                }
                                else
                                {
                                    var counterOfEqualsInChar = (char)counterOfEquals;
                                    writer.Write(counterOfEqualsInChar);
                                    writer.Write(bytes[i]);
                                    counterOfEquals = 1;
                                }
                            }
                            sizeCompressedFile = (int)outputFile.Length - count;
                        }

                    }

                }
            }
        }


        /// <summary>
        /// This method decompress a file that was compressed by RLE method
        /// </summary>
        /// <param name="filepath"></param>
        public void decompression(string filepath)
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
        public double compressionRatio()
        {
            double sizeAfter = sizeCompressedFile;
            double sizeBefore = sizeOriginalSize;
            return Math.Round(sizeAfter/sizeBefore,2);
        }

        /// <summary>
        /// Gives the compression factor
        /// </summary>
        /// <returns></returns>
        public double compressionFactor()
        {
            double sizeAfter = sizeCompressedFile;
            double sizeBefore = sizeOriginalSize;
            return Math.Round(sizeBefore / sizeAfter, 2);
        }

        /// <summary>
        /// Gives the saving percentage
        /// </summary>
        /// <returns></returns>
        public double savingPercentage()
        {
            double sizeAfter = sizeCompressedFile;
            double sizeBefore = sizeOriginalSize;
            return Math.Round((sizeBefore - sizeAfter) / sizeBefore * 100, 2);
        }


    }
}
