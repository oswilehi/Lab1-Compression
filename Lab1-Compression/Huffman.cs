using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab1_Compression
{
    class Huffman
    {
        /// <summary>
        /// Atributes
        /// </summary>
        private HuffmanNode root;
        public int sizeOriginalSize;
        public int sizeCompressedFile;
        private Dictionary<char, int> frequencies;
        private int lengthOfPrefixCodes;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">File path</param>
        public Huffman()
        {
            root = null;
            frequencies = new Dictionary<char, int>();
            sizeOriginalSize = 0;

        }
        public void Compression(string filePath)
        {
            ReadFrequencies(filePath);
        }
       
        /// <summary>
        /// Method to compress file
        /// </summary>
        /// <param name="filepath">File path</param>
        /// <param name="bytes">File byte array</param>
        private void compression(string filepath, byte[] bytes)
        {
            StreamWriter file = new StreamWriter("C:\\Users\\Oscar\\Desktop\\LAB1.txt" + ".comp");
            file.WriteLine("1," + Path.GetFileName(filepath));
            Dictionary<byte, string> encode = PrefixCode();
            foreach (KeyValuePair<byte, string> item in encode)
            {
                file.Write((byte)item.Key);
                file.Write('-');
                file.Write(item.Value);
                file.Write('|');
            }
            file.WriteLine();
            file.Flush();
            file.Close();
            using (var outputFile = new FileStream("C:\\Users\\Oscar\\Desktop\\LAB1.txt" + ".comp", FileMode.Append))
            {
                using (var writer = new BinaryWriter(outputFile, Encoding.ASCII))
                {
                    string pivot = "";
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        pivot += encode[bytes[i]];
                    }
                    lengthOfPrefixCodes = pivot.Length;
                    var bit = 0;

                    for (int i = 0; i < pivot.Length; i++)
                    {
                        if (pivot.Length - i >= 8)
                        {
                            string current = pivot.Substring(i, 8);
                            bit = Convert.ToInt16(current, 2);
                            i = i + 7;
                        }
                        else
                        {
                            string current = pivot.Substring(i, (pivot.Length % 8));
                            bit = Convert.ToInt16(current, 2);
                            i = pivot.Length;
                        }

                        byte bits = (byte)int.Parse(bit.ToString());
                        writer.Write(bits);
                    }

                }
                sizeCompressedFile = (int)outputFile.Length;
            }
        }
        /// <summary>
        /// Creat a dictionary (char,int) with the frequencies of each character
        /// </summary>
        /// <param name="filepath"></param>
        private void ReadFrequencies(string filepath)
        {
            using (var file = new FileStream(filepath, FileMode.Open))
            {
                using (var binaryFile = new BinaryReader(file))
                {
                    var bytes = binaryFile.ReadBytes((int)file.Length);
                    sizeOriginalSize = (int)file.Length;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        if (!frequencies.ContainsKey((char)bytes[i]))
                            frequencies.Add((char)bytes[i], 0);
                        frequencies[(char)bytes[i]]++;

                    }
                    HuffmanTree(frequencies);
                    compression(filepath, bytes);
                }
            }
        }
        /// <summary>
        /// Get the add of two probabilities
        /// </summary>
        /// <param name="n1">frequency number one</param>
        /// <param name="n2">frequency number one</param>
        /// <returns>The sum of both frequencies</returns>
        private double SumOfProbabilities(double n1, double n2)
        {
            return n1 + n2;
        }
        /// <summary>
        /// Creat Huffman tree 
        /// </summary>
        /// <param name="frequencies">Dictionary of frequencies</param>
        private void HuffmanTree(IEnumerable<KeyValuePair<char, int>> frequencies)
        {
            HuffmanQueue<HuffmanNode> priorityQueue = new HuffmanQueue<HuffmanNode>();

            foreach (KeyValuePair<char, int> item in frequencies)
            {
                priorityQueue.Queue(new HuffmanNode { value = item.Key, probability = GetProbability(item.Value) }, GetProbability(item.Value));
            }
            while (priorityQueue.Count > 1)
            {
                HuffmanNode n1 = priorityQueue.Dequeue();
                HuffmanNode n2 = priorityQueue.Dequeue();
                HuffmanNode newNode = new HuffmanNode { left = n1, right = n2, probability = SumOfProbabilities(n1.probability, n2.probability) };
                n1.parent = newNode;
                n2.parent = newNode;
                priorityQueue.Queue(newNode, newNode.probability);
            }
            root = priorityQueue.Dequeue();

        }
        private double GetProbability(int value)
        {
            double v = value;
            double s = sizeOriginalSize;
            return v / s;
        }
        /// <summary>
        /// Dictionary with the prefix code and the characters
        /// </summary>
        /// <returns></returns>
        private Dictionary<byte, string> PrefixCode()
        {
            Dictionary<byte, string> prefixcode = new Dictionary<byte, string>();
            Encode(root, prefixcode, "");
            return prefixcode;
        }
        /// <summary>
        ///Method to encode
        /// </summary>
        /// <param name="node">Root</param>
        /// <param name="prefixcode"></param>
        /// <param name="prefix"></param>
        private void Encode(HuffmanNode node, Dictionary<byte, string> prefixcode, string prefix)
        {
            if (node.left != null)
            {
                Encode(node.left, prefixcode, prefix + "0");
                Encode(node.right, prefixcode, prefix + "1");
            }
            else
                prefixcode.Add((byte)node.value, prefix);
        }

        public void Decompress(string path)
        {
            Dictionary<string, int> prefijos = new Dictionary<string, int>();
            StreamReader reader = new StreamReader(path);
            string firstLine = reader.ReadLine();
            var infoOfFile = firstLine.Split(',');
            string fileName = infoOfFile[1];
            string secondLine = reader.ReadLine();
            secondLine = secondLine.TrimEnd('|');
            var separatedSecondLine = secondLine.Split('|');
            int sizeToJump = firstLine.Length + secondLine.Length + 4 /* Ese cuatro es por los dos saltos de lineas y los dos enters*/ + 1 /* y ese uno es por el palito que se quita del final*/;

            string fileInBinary = "";


            for (int i = 0; i < separatedSecondLine.Length; i++)
            {
                var valueAndPrefix = separatedSecondLine[i].Split('-');
                int value = int.Parse(valueAndPrefix[0]);
                string prefix = valueAndPrefix[1];
                prefijos[prefix] = value;
            }

            reader.Close();

            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var currentFile = new BinaryReader(file))
                {
                    var bytes = currentFile.ReadBytes((int)file.Length);

                    for (int i = sizeToJump; i < bytes.Length; i++)
                    {
                        string partOfFile = Convert.ToString(bytes[i], 2);
                        if (partOfFile.Length < 8 && i != bytes.Length - 1)
                            partOfFile = partOfFile.PadLeft(8, '0');
                        else if (i == bytes.Length - 1)
                            partOfFile = partOfFile.PadLeft(lengthOfPrefixCodes % 8, '0');
                        fileInBinary += partOfFile;
                    }
                }
            }

            string pathToSave = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            using (StreamWriter outputFile = new StreamWriter(pathToSave + "\\D" + fileName))
            {
                    int start = 0;
                    for (int i = 0; i <= fileInBinary.Length; i++)
                    {

                        if (prefijos.ContainsKey(fileInBinary.Substring(0, i)))
                        {
                            outputFile.Write((char)prefijos[fileInBinary.Substring(0, i)]);
                            fileInBinary = fileInBinary.Remove(start, i);
                            i = 0;
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
            return sizeAfter / sizeBefore;
        }

        /// <summary>
        /// Gives the compression factor
        /// </summary>
        /// <returns></returns>
        public double compressionFactor()
        {
            double sizeAfter = sizeCompressedFile;
            double sizeBefore = sizeOriginalSize;
            return sizeBefore / sizeAfter;
        }

        /// <summary>
        /// Gives the saving percentage
        /// </summary>
        /// <returns></returns>
        public double savingPercentage()
        {
            int sizeAfter = sizeCompressedFile;
            int sizeBefore = sizeOriginalSize;
            return (sizeBefore - sizeAfter) / sizeBefore * 100;
        }

    }
}


