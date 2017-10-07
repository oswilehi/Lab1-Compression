using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Compression
{
    class HuffmanNode
    {
        //Atributes
        public double probability { get; set; } //probabilidad de repetirse dentro del documento 
        public byte value { get; set; } //caracter especial
        public HuffmanNode right { get; set; } // nodo derecho
        public HuffmanNode left { get; set; } //nodo izquiero
        public HuffmanNode parent { get; set; } //nodo padre

        //Constructor
        public HuffmanNode()
        {
            probability = 0;
            right = null;
            left = null;
            parent = null;

        }
    }
}
