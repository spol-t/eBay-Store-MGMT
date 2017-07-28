using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMGMT
{
    public class EbayStore
    {
        private string type;
        private int listings;
        private double monthlyPrice;
        private double insertionFee;
        private double finalValueFee;
        private double internationalSiteFee;

        public string Type { get; set; }
        public int Listings { get; set; }
        public double MonthlyPrice { get; set; }
        public double InsertionFee { get; set; }
        public double FinalValueFee { get; set; }
        public double InternationalSiteFee { get; set; }
    }

}
