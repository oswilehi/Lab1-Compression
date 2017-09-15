using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Compression
{
    class HuffmanQueue<HuffmanNode>
    {
        //Atributes
        private SortedDictionary<double, Queue<HuffmanNode>> sortedCharactersDictionary = new SortedDictionary<double, Queue<HuffmanNode>>();
        public int Count { get; set; }
        /// <summary>
        /// Enqueue a new item 
        /// </summary>
        /// <param name="node">HuffmanNode that contains a value character and a frequency</param>
        /// <param name="priorityKey">Probability of the node</param>
        public void Queue(HuffmanNode node, double priorityKey)
        {
            if (!sortedCharactersDictionary.ContainsKey(priorityKey))
                sortedCharactersDictionary[priorityKey] = new Queue<HuffmanNode>();

            sortedCharactersDictionary[priorityKey].Enqueue(node);
            Count++;

        }
        /// <summary>
        /// Dequeue an element
        /// </summary>
        /// <returns>El primer nodo de la cola de nodos según su llave</returns>
        public HuffmanNode Dequeue()
        {
            var item = sortedCharactersDictionary.First();
            if (item.Value.Count == 1)
                sortedCharactersDictionary.Remove(item.Key);

            Count--;
            return item.Value.Dequeue();
        }
    }
}
