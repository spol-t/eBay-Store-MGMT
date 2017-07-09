using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMGMT
{
    //Discribs all the itms in the store including packeges
    public class Item
    {
        private string barcode;
        private string description;
        private double weight;
        private double costILS;
        private int quantity;

        public string Barcode { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public double CostILS { get; set; }
        public int Quantity { get; set; }

    }
}
