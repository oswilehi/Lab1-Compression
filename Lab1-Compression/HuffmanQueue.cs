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
        private SortedDictionary<double, Queue<HuffmanNode>> sortedCharactersDictionary = new SortedDictionary<double, Queue<HuffmanNode>>(); //este diccionario contendrá las probabilidades y por cada probabilidad una cola de los caracteres que repiten esa probabilidad
        public int Count { get; set; }
        /// <summary>
        /// Enqueue a new item 
        /// </summary>
        /// <param name="node">HuffmanNode that contains a value character and a frequency</param>
        /// <param name="priorityKey">Probability of the node</param>
        public void Queue(HuffmanNode node, double priorityKey)
        {
            if (!sortedCharactersDictionary.ContainsKey(priorityKey)) //busca la llave dentro de la sortedCharactersDictionary si no existe la ingresa 
                sortedCharactersDictionary[priorityKey] = new Queue<HuffmanNode>(); //y crea su cola de caracteres 

            sortedCharactersDictionary[priorityKey].Enqueue(node); //añade el caracter que tiene la misma probabilidad
            Count++;

        }
        /// <summary>
        /// Dequeue an element
        /// </summary>
        /// <returns>El primer nodo de la cola de nodos según su llave</returns>
        public HuffmanNode Dequeue()
        {
            var item = sortedCharactersDictionary.First(); //take the first element from the dictionary 'sortedCharactersDictionary'
            if (item.Value.Count == 1)
                sortedCharactersDictionary.Remove(item.Key); //elimina la probabilidad que ya no se repite dentro de la sortedCharactersDictionary

            Count--;
            return item.Value.Dequeue(); //elimina un elemento de la cola de una probabilidad en específico
        }
    }
}
