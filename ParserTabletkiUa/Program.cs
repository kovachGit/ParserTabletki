using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTabletkiUa
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;

            Parser.Host = @"https://tabletki.ua/uk/";
            Parser.SearchByName("Парацетамол");
            //Parser.SearchByName("Аспирин-кардио");
            

            long i = 0;
            while (true)
            {
                if (Parser.IsFound)
                {
                    Parser.IsFound = false;
                    foreach (DrugBlock db in Parser.ListOfDrugsBlocks)
                    {
                        Console.WriteLine("Назва: " + db.Name);
                        Console.WriteLine("Фото: " + db.ImageSource);
                        Console.WriteLine("Виробник: " + db.Manufacturer);
                        Console.WriteLine("Ціна: " + db.PriceString);
                        Console.WriteLine("" + db.Stores);
                        Console.WriteLine("Варіанти аптек, замовлення: " + db.LinkStores);
                        Console.WriteLine();
                    }
                    break;
                }
                i++;
                if (i > 10000000000)
                {
                    Console.WriteLine("Time is out!");
                    break;
                }
            }
            Console.ReadKey();  
        }
    }
}
