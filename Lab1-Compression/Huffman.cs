﻿using System;
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
        private int fileSize;
        private Dictionary<char, int> frequencies;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">File path</param>
        public Huffman(string filePath)
        {
            root = null;
            frequencies = new Dictionary<char, int>();
            ReadFrequencies(filePath); //creat the dictionary with the frequency of each character

        }
        /// <summary>
        /// Method to compress file
        /// </summary>
        /// <param name="filepath">File path</param>
        /// <param name="bytes">File byte array</param>
        private void compression(string filepath, byte[] bytes)
        {
            //header : 1 = huffman algorithm, file name
            StreamWriter file = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.GetFileNameWithoutExtension(filepath)));
            file.WriteLine("1," + Path.GetFileName(filepath));
           
            file.Flush();
            file.Close();

            using (var outputFile = new FileStream("C:\\Users\\jsala\\Desktop\\prueba2.txt" + ".comp", FileMode.Append))
            {
                using (var writer = new BinaryWriter(outputFile, Encoding.ASCII))
                {
                    Dictionary<char, string> encode = PrefixCode(); //dictionary value - prefix code to compress the file
                    string pivot = "";
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        pivot += encode[(char)bytes[i]]; //compress data 
                    }

                    //write prefix table
                    foreach (KeyValuePair<char, string> item in encode)
                    {
                        writer.Write(item.Key + ',' + item.Value);
                    }
                    //write the compressed document in the file
                    var bit = 0;
                    for (int i = 0; i < pivot.Length; i++)
                    {
                        if (pivot.Length - i > 8)
                        {
                            string current = pivot.Substring(i, 8); //group of 8 bits from the original file
                            bit = Convert.ToInt16(current, 2); // convert to binary 
                            i = i + 7;
                        }
                        else
                        {
                            string current = pivot.Substring(i, (pivot.Length - i));
                            bit = Convert.ToInt16(current, 2);
                            i = pivot.Length;
                        }
                        var bits = int.Parse(bit.ToString()); //convert to int and write the value in the file
                        writer.Write(bits);

                    }
                }
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
                    fileSize = (int)file.Length;
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
        /// Get the probability 
        /// </summary>
        /// <param name="n1">frequency number one</param>
        /// <param name="n2">frequency number one</param>
        /// <returns>The sum of both frequencies</returns>
        private double SumOfProbabilities(double n1, double n2)
        {
            double r1 = n1 / fileSize;
            double r2 = n2 / fileSize;
            return r1 + r2;
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
                priorityQueue.Queue(new HuffmanNode { value = item.Key, probability = item.Value }, item.Value);
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
        /// <summary>
        /// Dictionary with the prefix code and the characters
        /// </summary>
        /// <returns></returns>
        private Dictionary<char, string> PrefixCode()
        {
            Dictionary<char, string> prefixcode = new Dictionary<char, string>();
            Encode(root, prefixcode, "");
            return prefixcode;
        }
        /// <summary>
        ///Method to encode
        /// </summary>
        /// <param name="node">Root</param>
        /// <param name="prefixcode"></param>
        /// <param name="prefix"></param>
        private void Encode(HuffmanNode node, Dictionary<char, string> prefixcode, string prefix)
        {
            if (node.left != null)
            {
                Encode(node.left, prefixcode, prefix + "0"); 
                Encode(node.right, prefixcode, prefix + "1");
            }
            else
                prefixcode.Add(node.value, prefix);
        }
    }
}
