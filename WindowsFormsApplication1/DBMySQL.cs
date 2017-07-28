using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMGMT
{
    class DBMySQL : DBConnect
    {

        #region eBay Store Querys
        public EbayStore GetEbayStore(string storeType)
        {
            EbayStore s = new EbayStore();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ArrayList storeDetails = new ArrayList();
            string cmdStr = "SELECT * FROM ebay_details WHERE type=@storeType";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }
            foreach (DataRow tType in dt.Rows)
            {
                storeDetails.Add(tType[0]);
            }

            s.Type = storeDetails[0].ToString();
            s.Listings = Convert.ToInt32(storeDetails[1]);
            s.MonthlyPrice = Convert.ToDouble(storeDetails[2]);
            s.InsertionFee = Convert.ToInt32(storeDetails[3]);
            s.FinalValueFee = Convert.ToDouble(storeDetails[4]);
            s.InsertionFee = Convert.ToDouble(storeDetails[5]);

            return s;
        }
        #endregion

        #region Item/Pack Querys

        //get all items barcodes
        public string[] GetItemsBarcodes(string item)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ArrayList barcodes = new ArrayList();
            string cmdStr = "SELECT barcode FROM " + item;
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }
            foreach (DataRow tType in dt.Rows)
            {
                barcodes.Add(tType[0].ToString());
            }
            return (string[])barcodes.ToArray(typeof(string));
        }

        //check if an item or pack exist
        public bool IsItemPackExist(string barcode, string table)
        {
            string str = "";
            string cmdStr = "SELECT * FROM " + table + " WHERE barcode=@barcode";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@barcode", barcode);
            str = ExecuteScalarStringQuery(command);
            if (str != "" && str != null)
                return true;
            else
                return false;
        }

        //update item info. barcode cant be changed
        public void UpdateItem(Item item, string table)
        {
            string cmdStr = "UPDATE " + table +
                " SET description=@description, weight=@weight, cost=@cost, quantity=@quantity " +
                "WHERE barcode=@barcode";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@barcode", item.Barcode);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@weight", item.Weight);
            command.Parameters.AddWithValue("@cost", item.Cost);
            command.Parameters.AddWithValue("@quantity", item.Quantity);
            ExecuteSimpleQuery(command);
        }

        //insert an item to th 'Item For Sale' table
        public void InsertItemForSale(Item item)
        {
            string cmdStr = "INSERT INTO items (barcode,description,weight,cost,quantity)"
                + "VALUES (@barcode,@description,@weight,@costILS,@quantity)";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@barcode", item.Barcode);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@weight", item.Weight);
            command.Parameters.AddWithValue("@costILS", item.Cost);
            command.Parameters.AddWithValue("@quantity", item.Quantity);
            ExecuteSimpleQuery(command);

        }

        //insert a pack to the 'Packs' table
        public void InsertPack(Item item)
        {
            string cmdStr = "INSERT INTO packs (barcode,description,weight,cost,quantity)"
                + "VALUES (@barcode,@description,@weight,@costILS,@quantity)";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@barcode", item.Barcode);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@weight", item.Weight);
            command.Parameters.AddWithValue("@costILS", item.Cost);
            command.Parameters.AddWithValue("@quantity", item.Quantity);
            ExecuteSimpleQuery(command);


        }

        #endregion

        #region Client Querys

        //get Clients country
        public string GetClientCountry(string email)
        {
            string str = "";
            string cmdStr = "SELECT country FROM clients WHERE email = @email";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@email", email);
            str = ExecuteScalarStringQuery(command);
            return str;
        }

        //check if client exist
        public bool IsClientExist(Person client)
        {
            string str = "";
            string cmdStr = "SELECT * FROM clients WHERE email=@email";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@email", client.Email);
            str = ExecuteScalarStringQuery(command);
            if (str != "" && str != null)
                return true;
            else
                return false;
        }

        //update existing client
        public void UpdateClient(Person client)
        {
            string cmdStr = "UPDATE  clients SET " +
                "firstname=@firstname, lastname=@lastname, address1=@address1, " +
                "city=@city, state_province=@state, zip_postal=@zip, country=@country, " +
                "phonenumber=@phone WHERE email=@email";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@email", client.Email);
            command.Parameters.AddWithValue("@firstname", client.FirstName);
            command.Parameters.AddWithValue("@lastname", client.LastName);
            command.Parameters.AddWithValue("@address1", client.Address1);
            command.Parameters.AddWithValue("@address2", client.Address2);
            command.Parameters.AddWithValue("@city", client.City);
            command.Parameters.AddWithValue("@state", client.State_province);
            command.Parameters.AddWithValue("@zip", client.Zip_postal);
            command.Parameters.AddWithValue("@country", client.Country);
            command.Parameters.AddWithValue("@phone", client.Phone);
            ExecuteSimpleQuery(command);
        }

        //insert new client
        public void InsertClient(Person client)
        {
            string cmdStr = "INSERT INTO clients (email,firstname,lastname,address1," +
                "address2,city,state_province,zip_postal,country,phonenumber)" +
                "VALUES (@email,@firstname,@lastname,@address1,@address2,@city," +
                "@state_province,@zip_postal,@country,@phonenumber)";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@email", client.Email);
            command.Parameters.AddWithValue("@firstname", client.FirstName);
            command.Parameters.AddWithValue("@lastname", client.LastName);
            command.Parameters.AddWithValue("@address1", client.Address1);
            command.Parameters.AddWithValue("@address2", client.Address2);
            command.Parameters.AddWithValue("@city", client.City);
            command.Parameters.AddWithValue("@state_province", client.State_province);
            command.Parameters.AddWithValue("@zip_postal", client.Zip_postal);
            command.Parameters.AddWithValue("@country", client.Country);
            command.Parameters.AddWithValue("@phonenumber", client.Phone);
            ExecuteSimpleQuery(command);
        }

        #endregion

        #region shipping Querys

        //retrive all the countrys with shipping ditails
        public string[] GetCountrys()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ArrayList country = new ArrayList();
            string cmdStr = "SELECT DISTINCT country FROM shipments ORDER BY country";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }
            foreach (DataRow tType in dt.Rows)
            {
                country.Add(tType[0].ToString());
            }
            return (string[])country.ToArray(typeof(string));
        }

        //find an existing shipping methode in the database
        public bool IsShipmentExist(Shipment ship)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ArrayList shipment = new ArrayList();
            string cmdStr = "SELECT * FROM shipments WHERE country=@country" +
                " AND (min_weight=@min OR max_weight=@max) AND registered=@reg";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@country", ship.Country);
            command.Parameters.AddWithValue("@min", ship.MinWeight);
            command.Parameters.AddWithValue("@max", ship.MaxWeight);
            command.Parameters.AddWithValue("@reg", ship.Registered);
            ds = GetMultipleQuery(command);
            if (ds.Tables[0].Rows.Count == 0)
                return false;
            return true;
        }

        //update shipping
        public void UpdateShipping(Shipment ship)
        {
            string cmdStr = "UPDATE shipments " +
                "SET min_weight=@min, max_weight=@max, price=@price " +
                "WHERE country=@country AND (min_weight=@min OR max_weight=@max) " +
                "AND registered=@reg";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@country", ship.Country);
            command.Parameters.AddWithValue("@min", ship.MinWeight);
            command.Parameters.AddWithValue("@max", ship.MaxWeight);
            command.Parameters.AddWithValue("@reg", ship.Registered);
            command.Parameters.AddWithValue("@price", ship.Price);
            ExecuteSimpleQuery(command);
        }

        //insert an new shippmet option
        public void InsertShipment(Shipment ship)
        {
            string cmdStr = "INSERT INTO shipments " +
                " (country, min_weight, max_weight, registered, price) " +
                "VALUES (@country, @min, @max, @reg, @price)";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@country", ship.Country);
            command.Parameters.AddWithValue("@min", ship.MinWeight);
            command.Parameters.AddWithValue("@max", ship.MaxWeight);
            command.Parameters.AddWithValue("@reg", ship.Registered);
            command.Parameters.AddWithValue("@price", ship.Price);
            ExecuteSimpleQuery(command);
        }
        #endregion

        #region Sales Querys

        //get the index of the last sale made from the database and return the following number
        public int GetNextSaleNum()
        {
            string cmdStr = "SELECT  * FROM sales_sam ORDER BY number DESC limit 1";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            int num = ExecuteScalarIntQuery(command) + 1;
            return num;
        }

        //insert new lines into 'sales_details' table
        public void UpdateSaleDetails(int saleNum, string barcode, int quantity, string type)
        {
            string cmdStr = "";

            //if the item is already in the list - update the quantity
            //else insert new line
            if (IsItemPackExist(barcode, "sales_details"))
            {
                cmdStr = "UPDATE sales_details SET quantity=quantity+@quantity WHERE barcode=@barcode";
            }
            else
            {
                cmdStr = "INSERT INTO sales_details (number,barcode,quantity," +
                "type) VALUES (@saleNum,@barcode,@quantity,@type)";
            }

            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@saleNum", saleNum);
            command.Parameters.AddWithValue("@barcode", barcode);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@quantity", quantity);
            ExecuteSimpleQuery(command);


            //update the item's stock table
            String cmdStr2 = "UPDATE " + type  + "s" + " SET quantity=quantity-@quantity WHARE barcode=@barcode";
            MySqlCommand command2 = new MySqlCommand(cmdStr2, connection);
            command2.Parameters.AddWithValue("@barcode", barcode);
            command2.Parameters.AddWithValue("@quantity", quantity);
            ExecuteSimpleQuery(command2);
        }

        //get the number of items from a spcific type in the sale
        public int GetNumOfItemsInSale(int saleNum, string itemType)
        {
            int numOfItems = 0;
            string cmdStr = "SELECT SUM(quantity) FROM sales_details WHERE number=@saleNum" +
                " AND type=@itemType";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@saleNum", saleNum);
            command.Parameters.AddWithValue("@itemType", itemType);
            numOfItems = ExecuteScalarIntQuery(command);
            return numOfItems;
        }

        //get the cost of all the items from a spcific type in the sale
        public double GetTotalItemsCost(int saleNum, string itemType)
        {
            double sum = 0;
            string cmdStr = "";
            if (itemType == "item")
            {
                cmdStr = "SELECT SUM(items.cost * sales_details.quantity) FROM items, sales_details " +
                    "WHERE items.barcode=sales_details.barcode AND sales_details.number=@saleNum";
            }
            else if (itemType == "pack")
            {
                cmdStr = "SELECT SUM(packs.cost * sales_details.quantity) FROM packs,sales_details " +
                "WHERE packs.barcode=sales_details.barcode AND sales_details.number=@saleNum";
            }
            
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@saleNum", saleNum);
            command.Parameters.AddWithValue("@type", itemType);
            sum = ExecuteScalarDoubleQuery(command);
            return sum;
        }

        //get the total weight of the items and pack in the sale
        public double GetTotalWeight(int saleNumber)
        {
            double totalWeight = 0;

            for (int i = 0; i < 2; i++)
            {
                
                if (i == 0)
                {
                    string cmdStr = "SELECT SUM(items.weight * sd.quantity) " +
                        "FROM items, sales_details sd " +
                        "where items.barcode = sd.barcode and sd.number = @saleNumber";
                    MySqlCommand command = new MySqlCommand(cmdStr, connection);
                    command.Parameters.AddWithValue("@saleNumber", saleNumber);
                    totalWeight += ExecuteScalarDoubleQuery(command);
                }

                else
                {
                    string cmdStr = "SELECT SUM(packs.weight * sd.quantity) " +
                        "FROM packs, sales_details sd " +
                        "where packs.barcode = sd.barcode and sd.number = @saleNumber";
                    MySqlCommand command = new MySqlCommand(cmdStr, connection);
                    command.Parameters.AddWithValue("@saleNumber", saleNumber);
                    totalWeight += ExecuteScalarDoubleQuery(command);
                }
                    
                
            }
            return totalWeight;
        }

        //return a list of cliens email's
        public ArrayList GetClientEmail()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ArrayList emails = new ArrayList();
            string cmdStr = "SELECT email FROM clients";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }
            foreach (DataRow tType in dt.Rows)
            {
                emails.Add(tType[0].ToString());
            }
            return emails;
        }

        //return ebay total fees per sale
        public double GetEbayFees(int saleNum, string storeType, double paymentReceived, bool internationalSite )
        {
            string cmdStr = "";

            //if ther is an innternation site fee add it to the query
            if (internationalSite)
            {
                cmdStr = "select ((ed.monthly_price / ed.listings) + ed.insertion "
                + "+ ed.international_site) " + "+ (ed.finalvalue * @pr)" +
                "from ebay_details ed where type=@type";
            }
            else
            {
                cmdStr = "select ((ed.monthly_price / ed.listings) + ed.insertion "
                 + "+ (ed.finalvalue * @pr) FROM ebay_details ed where type=@type";
            }

            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@type", storeType);
            command.Parameters.AddWithValue("@pr", paymentReceived);

            return ExecuteScalarDoubleQuery(command);
        }

        //get the shipping costs for a sale
        public double GetShippingCosts(double weight, int registered, string country)
        {
            double price = 0;
            string cmdStr = "SELECT price FROM shipments " +
                "WHERE country=@country AND min_weight<@weight and max_weight>@weight " +
                "AND registered=@reg";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@country", country);
            command.Parameters.AddWithValue("@weight", weight);
            command.Parameters.AddWithValue("@reg", registered);

            price = ExecuteScalarDoubleQuery(command);

            return price;
        }

        //check if there is an ongoing sale that isnt complit
        public string IsSaleEmpty(int saleNum)
        {
            string str = "No";
            string cmdStr = "SELECT number FROM sales_details WHERE number=@saleNum";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@saleNum", saleNum);

            string cmdStr2 = "SELECT quantity from sales_details WHERE " +
                "number=@saleNum AND type=@type";
            MySqlCommand command2 = new MySqlCommand(cmdStr2, connection);
            command2.Parameters.AddWithValue("@saleNum", saleNum);
            command2.Parameters.AddWithValue("@type", "item");

            string cmdStr3 = "SELECT quantity from sales_details WHERE " +
                "number=@saleNum AND type=@type";
            MySqlCommand command3 = new MySqlCommand(cmdStr3, connection);
            command3.Parameters.AddWithValue("@saleNum", saleNum);
            command3.Parameters.AddWithValue("@type", "pack");

            if (ExecuteScalarIntQuery(command) > 0)
            {
                if (ExecuteScalarIntQuery(command2) > 0)
                    if (ExecuteScalarIntQuery(command3) > 0)
                        return str;
                    else
                        str += "There is No pack in the sale.";
                else
                    str += "There are no items in the sale.";
                return str;
            }
            else
            {
                str = "yes";
                return str;
            }

                

        }

        //chec kif a sale contains items and packs
        public string CheckSaleItemsPacks(int saleNum)
        {

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ArrayList types = new ArrayList();
            string cmdStr = "SELECT DISTINCT type FROM sales_details WHERE number=@saleNum";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@saleNum", saleNum);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }
            foreach (DataRow tType in dt.Rows)
            {
                types.Add(tType[0].ToString());
            }
            if (types.Contains("item"))
            {
                if (types.Contains("pack"))
                    return "OK";
                else
                    return "There are no packs in the sale";
            }
            else
                return "There are no items in the sale";
        }

        //retrive items and packs of an unfinshied sale
        public DataTable GetExistingSaleDetails(int saleNum)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            //retriving the items in the sale 
            string cmdStr = "SELECT sd.number AS 'Sale Number', sd.barcode AS Barcode, " +
                "i.description AS Description, sum(sd.quantity) AS Quantity " +
                "FROM sales_details sd, items i " +
                "WHERE sd.number=@saleNum AND i.barcode=sd.barcode " +
                "group by barcode ";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@saleNum", saleNum);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }

            //retriving the packs in the sale
            string cmdStr2 = "SELECT sd.number AS 'Sale Number', sd.barcode AS Barcode, " +
                "i.description AS Description, sum(sd.quantity) AS Quantity " +
                "FROM sales_details sd, packs i " +
                "WHERE sd.number=@saleNum AND i.barcode=sd.barcode " +
                "group by barcode ";
            MySqlCommand command2 = new MySqlCommand(cmdStr2, connection);
            command2.Parameters.AddWithValue("@saleNum", saleNum);
            ds = GetMultipleQuery(command2);
            try
            {
                dt2 = ds.Tables[0];
            }
            catch { }

            dt.Merge(dt2);
            return dt;
        }

        //update the sale_sam table with a new sale
        public void FinalizeSale(Sale s)
        {
            string cmdStr = "INSERT INTO sales_sam (number,num_of_items,total_items_cost," +
                "num_of_packs,total_packs_cost,total_weight,total_ebay_fees,total_paypal_fees," +
                "client_email,shipping,income,total_cost,profit) " +
                "VALUES (@num,@numI,@iCost,@numP,@pCost,@weight,@ebay,@paypal," +
                "@email,@shipping,@in,@cost,@profit)";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@num", s.Number);
            command.Parameters.AddWithValue("@numI", s.NumOfItems);
            command.Parameters.AddWithValue("@iCost", s.TotalItemsCost);
            command.Parameters.AddWithValue("@numP", s.NumOfPacks);
            command.Parameters.AddWithValue("@pCost", s.TotalPacksCost);
            command.Parameters.AddWithValue("@weight", s.TotalWeight);
            command.Parameters.AddWithValue("@ebay", s.TotalEbayFees);
            command.Parameters.AddWithValue("@paypal", s.TotalPayPalFees);
            command.Parameters.AddWithValue("@email", s.ClientEmail);
            command.Parameters.AddWithValue("@shipping", s.Shipiing);
            command.Parameters.AddWithValue("@in", s.Income);
            command.Parameters.AddWithValue("@cost", s.TotalCost);
            command.Parameters.AddWithValue("@profit", s.Profit);
            ExecuteSimpleQuery(command);
        }

        //clear items from an unfinished sale
        public void ClearSale(int saleNum)
        {
            string cmdStr = "DELETE FROM sales_details WHERE number=@saleNum";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@saleNum", saleNum);
            ExecuteSimpleQuery(command);
        }

        public DataTable GetSalesDetails()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            string cmdStr = "SELECT * FROM sales_sam";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }

            return dt;
        }

        #endregion

        #region CDS

        //get the number of items in the sale
        public int GetNumOfDiffItems()
        {
            string cmdStr = "SELECT COUNT(barcode) FROM sales_details WHERE type='item'";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            return ExecuteScalarIntQuery(command);
        }

        //get all the finished sales numbers
        public string[] GetSalesNums()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ArrayList salesNums = new ArrayList();
            string cmdStr = "SELECT number FROM sales_sam ORDER BY number";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);

            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }
            foreach (DataRow tType in dt.Rows)
            {
                salesNums.Add(tType[0].ToString());
            }
            return (string[])salesNums.ToArray(typeof(string));
        }

        public string[] GetClientBySaleNum(int saleNum)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ArrayList client = new ArrayList();
            int numOfCells = 0;
            string cmdStr = "SELECT email,firstname,lastname,address1,address2," +
                "city,state_province,zip_postal,country,phonenumber " +
                "FROM clients, sales_sam " +
                "WHERE sales_sam.number=@saleNum AND sales_sam.client_email=clients.email";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@saleNum", saleNum);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }
            numOfCells = dt.Columns.Count;
            for (int i = 1; i < numOfCells; i++)
            {
                client.Add(Convert.ToString(dt.Rows[0].ItemArray[i]));
            }
            client.Add(dt.Rows[0].ItemArray[0]);
            return (string[])client.ToArray(typeof(string));
        }

        //get items in the sale
        public DataTable GetItemfForCDS(int saleNum)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();


            //retriving the items in the sale 
            string cmdStr = "SELECT i.description, sum(sd.quantity), " +
                "(i.weight * sum(sd.quantity)), (i.cost * sum(sd.quantity)) " +
                "FROM items i, sales_details sd " +
                "WHERE sd.number=@saleNum AND i.barcode=sd.barcode " +
                "GROUP BY description";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@saleNum", saleNum);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }

            return dt;
        }

        #endregion

        #region Reports

        //all items in stock
        public DataTable GetItemsPacksInStock(string type)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            string cmdStr = "SELECT * FROM " + type;
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }

            return dt;
        }

        //items that are bout to end under 5 units
        public DataTable GetItemsPacksEnding(string type)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            string cmdStr = "SELECT * FROM " + type + " WHERE quantity<5";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }

            return dt;
        }

        public DataTable GetBestSellers()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            string cmdStr = "SELECT DISTINCT i.barcode, i.description, sd.quantity " +
                "FROM items i, sales_details sd " +
                "WHERE i.barcode = sd.barcode " +
                "ORDER BY sd.quantity DESC";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }

            return dt;
        }

        public DataTable GetClients()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            string cmdStr = "SELECT * FROM clients";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            ds = GetMultipleQuery(command);
            try
            {
                dt = ds.Tables[0];
            }
            catch { }

            return dt;
        }

        #endregion
    }
}
