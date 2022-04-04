using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTabletkiUa
{
    public class DrugBlock
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }

        public String PriceString { get; set; }
        public double Price { get; set; }
        public string Stores { get; set; }
        public string ImageSource { get; set; }

        public string LinkStores { get; set; }


        override public String ToString()
        {
            return String.Format("{0} | {1} | {2} | {3} | {4} | {5}", Name, Manufacturer,
                PriceString, Stores, ImageSource, LinkStores);
        }
    }

    
}
