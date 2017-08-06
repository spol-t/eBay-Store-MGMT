using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMGMT
{
    class Sale
    {
        private int number;
        private int numOfItems;
        private double totalItemsCost;
        private int numOfPacks;
        private double totalPacksCost;
        private double totalWeight;
        private double totalEbayFees;
        private double totalPayPalFees;
        private string clientEmail;
        private double shipping;
        private double income;
        private double totalCost;
        private double profit;

        public int Number { get; set; }
        public int NumOfItems { get; set; }
        public double TotalItemsCost { get; set; }
        public int NumOfPacks { get; set; }
        public double TotalPacksCost { get; set; }
        public double TotalWeight { get; set; }
        public double TotalEbayFees { get; set; }
        public double TotalPayPalFees { get; set; }
        public string ClientEmail { get; set; }
        public double Shipiing { get; set; }
        public double Income { get; set; }
        public double TotalCost { get; set; }
        public double Profit { get; set; }

    }
}
