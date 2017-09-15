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
        public double probability { get; set; }
        public char value { get; set; }
        public HuffmanNode right { get; set; }
        public HuffmanNode left { get; set; }
        public HuffmanNode parent { get; set; }

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
