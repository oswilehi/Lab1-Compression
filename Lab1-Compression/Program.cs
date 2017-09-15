using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Compression
{
    class Program
    {
        static void Main(string[] args)
        {
            Huffman huffman = new Huffman("C:\\Users\\Oscar\\Desktop\\LAB1.txt");

            huffman.Decompress("C:\\Users\\Oscar\\Desktop\\LAB1.txt.comp");
        }
    }
}
