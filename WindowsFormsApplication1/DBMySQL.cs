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
        //check if an item or pack exist
        public bool IsItemPackExist(Item item, string table)
        {
            string str = "";
            string cmdStr = "SELECT * FROM " + table + " WHERE barcode=@barcode";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@barcode", item.Barcode);
            str = ExecuteScalarStringQuery(command);
            if (str != "" && str != null)
                return true;
            else
                return false;         
        }
        
        public void UpdateItem(Item item, string table)
        {
            string cmdStr = "UPDATE " + table +
                " SET description=@description, weight=@weight, cost_ils=@cost, quantity=@quantity";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@weight", item.Weight);
            command.Parameters.AddWithValue("@costILS", item.CostILS);
            command.Parameters.AddWithValue("@quantity", item.Quantity);
            ExecuteSimpleQuery(command);
        }
        
        //insert an item to th 'Item FOr Sale' table
        public void InsertItemForSale(Item item)
        {
            string cmdStr = "INSERT INTO items (barcode,description,weight,cost_ils)"
                + "VALUES (@barcode,@description,@weight,@costILS)";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@barcode", item.Barcode);
            command.Parameters.AddWithValue("@description", item.Description);
            command.Parameters.AddWithValue("@weight", item.Weight);
            command.Parameters.AddWithValue("@costILS", item.CostILS);
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
            command.Parameters.AddWithValue("@costILS", item.CostILS);
            command.Parameters.AddWithValue("@quantity", item.Quantity);
            ExecuteSimpleQuery(command);


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
            foreach ( DataRow tType in dt.Rows)
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
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ArrayList shipment = new ArrayList();
            string cmdStr = "UPDATE shipments " +
                "SET min_weight=@min, max_weight=@max, price=@price " +
                "WHERE country=@country AND (min_weight=@min OR max_weight=@max) " +
                "AND registered=@reg"; 
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            command.Parameters.AddWithValue("@country", ship.Country);
            command.Parameters.AddWithValue("@min", ship.MinWeight);
            command.Parameters.AddWithValue("@max", ship.MaxWeight);
            command.Parameters.AddWithValue("@reg", ship.Registered);
            command.Parameters.AddWithValue("@reg", ship.Price);
            ExecuteSimpleQuery(command);
        }

        public int GetNextSaleNum()
        {
            string cmdStr = "SELECT  * FROM sales_sam ORDER BY number DESC limit 1";
            MySqlCommand command = new MySqlCommand(cmdStr, connection);
            int num = ExecuteScalarIntQuery(command) + 1;
            return num;
        }
    }
}
