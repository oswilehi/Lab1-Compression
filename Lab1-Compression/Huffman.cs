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
            ReadFrequencies(filePath);

        }
        /// <summary>
        /// Method to compress file
        /// </summary>
        /// <param name="filepath">File path</param>
        /// <param name="bytes">File byte array</param>
        private void compression(string filepath, byte[] bytes)
        {
            //escribir el encabezado: 1 = algoritmo huffman, nombre del archivo
            StreamWriter file = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.GetFileName(filepath) + ".comp"));
            file.WriteLine("1," + Path.GetFileName(filepath));
            Dictionary<char, string> encode = PrefixCode(); //diccionario que contiene los caracteres y el codigo prefijo de cada uno
            foreach (KeyValuePair<char, string> item in encode) //escribo en el archivo el caracter y el código prefijo para que pueda ser descomprimido despues
            {
                //formato: s-001|
                file.Write(item.Key);
                file.Write('-');
                file.Write(item.Value);
                file.Write('|');
                
            }
            file.WriteLine(); //salto de línea
            file.Flush();
            file.Close();
            //creo el archivo de salida para escribirlo en ascii
            using (var outputFile = new FileStream(filepath + ".comp", FileMode.Append))
            {
                using (var writer = new BinaryWriter(outputFile, Encoding.ASCII))
                {
                    string pivot = ""; //va a contener la información de todo el archivo comprimido
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        pivot += encode[(char)bytes[i]]; 
                    }
                    var bit = 0;

                    for (int i = 0; i < pivot.Length; i++) //escribo en el archivo
                    {
                        if (pivot.Length - i > 8)
                        {
                            string current = pivot.Substring(i, 8);
                            bit = Convert.ToInt16(current, 2); //convierto a binario los 8 bits que tomo del 'pivot'
                            i = i + 7;
                        }
                        else
                        {
                            string current = pivot.Substring(i, (pivot.Length - i));
                            bit = Convert.ToInt16(current, 2);
                            i = pivot.Length;
                        }
                        
                        var bits = int.Parse(bit.ToString()); //convierto a int el binario obtenido previamente
                      
                        writer.Write(bits); //escribo en el archivo
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
                    fileSize = (int)file.Length; //asigno el length al atributo fileSize de la clase
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        if (!frequencies.ContainsKey((char)bytes[i])) //si en el diccionario "frequencies" no está el caracter actual lo añade
                            frequencies.Add((char)bytes[i], 0);
                        frequencies[(char)bytes[i]]++; // si se encuentra dentro del diccionario, incrementa su frecuencia/probabilidad

                    }
                    HuffmanTree(frequencies); //crea el árbol con el algoritmo de huffman
                    compression(filepath, bytes); // comprime el archivo
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
            HuffmanQueue<HuffmanNode> priorityQueue = new HuffmanQueue<HuffmanNode>(); //cola de probabilidades la cual siempre irá de menor a mayor (por ello el nombre priority)

            foreach (KeyValuePair<char, int> item in frequencies) //añado los elementos del diccionario "frequencies" a la cola 
            {
                priorityQueue.Queue(new HuffmanNode { value = item.Key, probability = GetProbability(item.Value) }, GetProbability(item.Value));
            }
            while (priorityQueue.Count > 1)
            {
                HuffmanNode n1 = priorityQueue.Dequeue(); //elimina el primer elemento de la cola (siempre será el de menor probabilidad)
                HuffmanNode n2 = priorityQueue.Dequeue(); //elimina el segundo elemento de la cola (tendra la misma o mayor probabilidad que el anterior)
                HuffmanNode newNode = new HuffmanNode { left = n1, right = n2, probability = SumOfProbabilities(n1.probability, n2.probability) }; //crea un nuevo nodo con la suma de ambas probabilidades
                n1.parent = newNode; // se le asigna a los 2 nodos de la tabla de frecuencia el nuevo nodo como padre
                n2.parent = newNode;
                priorityQueue.Queue(newNode, newNode.probability); //se añade el nuevo nodo a la cola de prioridad para repetir el algoritmo hasta que el tamaño de la cola sea 1
            }
            root = priorityQueue.Dequeue(); //se asigna el último elemento de la cola de prioridad al nodo root, el cual deberá tener una probabilidad de 1

        }
        /// <summary>
        /// Get the probability of a character
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double GetProbability(int value)
        {
            double v = value;
            double s = fileSize;
            return v / s;
        }
        /// <summary>
        /// Dictionary with the prefix code and the characters
        /// </summary>
        /// <returns></returns>
        private Dictionary<char, string> PrefixCode()
        {
            Dictionary<char, string> prefixcode = new Dictionary<char, string>(); //diccionario de caracteres con su código prefijo
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
                Encode(node.left, prefixcode, prefix + "0"); //concatena 0 si es a la izquierda
                Encode(node.right, prefixcode, prefix + "1"); //concatena 1 si es a la derecha
            }
            else
                prefixcode.Add(node.value, prefix); //nodo hoja
        }
    }
}
