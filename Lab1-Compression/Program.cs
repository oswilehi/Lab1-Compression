using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab1_Compression
{
    class Program
    {
        static void Main(string[] args)
        {
            string entry = "", filePath = "", type = "";
            bool exit = true;
            RLE rle = new RLE();
            while (exit)
            {

                Console.WriteLine("c:/rle>rle.exe");
                Console.SetCursorPosition(15, Console.CursorTop - 1);
                entry = Console.ReadLine();
                if (!Validation(entry, ref filePath, ref type))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("¡Error! Asegúrese de haber escrito correctamente los comandos del progama. Verifique que exista el archivo.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    switch (type.ToLower())
                    {
                        case "-c":
                            rle.compression(filePath);
                            messageC();
                            Console.WriteLine("\nEstadísticas del archivo generado:");
                            Console.WriteLine("-Tamaño original: " + rle.sizeOriginalSize);
                            Console.WriteLine("-Tamaño final: " + rle.sizeCompressedFile);
                            Console.WriteLine("-Ratio de compresión: " + rle.compressionRatio().ToString());
                            Console.WriteLine("-Factor de compresión: " + rle.compressionFactor().ToString());
                            Console.WriteLine("-Porcentaje ahorrado: " + rle.savingPercentage().ToString() + "%\n\n");
                            break;
                        case "-d":
                            rle.decompression(filePath);
                            messageD();
                            break;
                        case "exit":
                            exit = false;
                            break;
                        default:
                            Console.WriteLine("Si desea dejar de usar el programa escriba 'exit'");
                            Console.Clear();
                            break;
                    }
                    
                }

                exit = true;
            }
            Console.ReadLine();
        }

        //validar entrada, comandos y la ruta del archivo
        static bool Validation(string entry, ref string filePath, ref string type)
        {
            string[] current = entry.Split(' ');
            if (current[0].ToLower() == "exit")
            {
                type = "exit";
                return true;
            }                
            else
            {
                if (!(current.Length > 2))
                {
                    if (!(current[0] == "-c" || current[0] == "-d"))
                        return false;
                    type = current[0];
                    string[] path = current[1].Split('"');
                    if (!(path.Length > 3))
                    {
                        if (!(path[0] == "-f" && File.Exists(path[1])))
                            return false;
                        filePath = path[1];
                    }
                    else
                        return false;
                    return true;

                }
                else
                    return false;
            }
            
        }
        //Mensajes de operación exitosa
        static void messageC()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Compresión finalizada exitosamente.\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void messageD()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Decompresión finalizada exitosamente.\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
