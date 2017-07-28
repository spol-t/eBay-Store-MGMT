using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMGMT
{
    public class Shipment
    {
        private string country;
        private int minWeight;
        private int maxWeight;
        private int registered;
        private double price;

        public string Country { get; set; }
        public int MinWeight { get; set; }
        public int MaxWeight { get; set; }
        public int Registered { get; set; }
        public double Price { get; set; }

    }
}
