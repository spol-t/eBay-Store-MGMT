using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMGMT
{
    public class DBSQL : DbAccess
    {
        public DBSQL(string connectionString) : base(connectionString)
        {
        }

        //insert an Item for sale in to the data base
        public void InsertItemForSale(Item item)
        {
            string cmdStr = "INSERT INTO itemsForSale (barcode,description,weight,costILS)VALUES (@barcode,@description,@weight,@costILS)";
            using (OleDbCommand command = new OleDbCommand(cmdStr))
            {
                command.Parameters.AddWithValue("@barcode", item.Barcode);
                command.Parameters.AddWithValue("@description", item.Description);
                command.Parameters.AddWithValue("@weight", item.Weight);
                command.Parameters.AddWithValue("@costILS", item.CostILS);
                base.ExecuteSimpleQuery(command);
            }
        }

        //insert a pack in to the data base
        public void InsertPack(Item item)
        {
            string cmdStr = "INSERT INTO packs (barcode,description,weight,costILS)VALUES (@barcode,@description,@weight,@costILS)";
            using (OleDbCommand command = new OleDbCommand(cmdStr))
            {
                command.Parameters.AddWithValue("@barcode", item.Barcode);
                command.Parameters.AddWithValue("@description", item.Description);
                command.Parameters.AddWithValue("@weight", item.Weight);
                command.Parameters.AddWithValue("@costILS", item.CostILS);
                base.ExecuteSimpleQuery(command);
            }
        }

        //insert a new client in to the data base
        public void InsertClient(Person client)
        {
            string cmdStr = "INSERT INTO address (email,firstName,lastName,address1,address2,city,state_province,zip_postal,country,phoneNumber)VALUES (@email,@firstName,@lastName,@address1,@address2,@city,@state_province,@zip_postal,@country,@phoneNumber)";
            using (OleDbCommand command = new OleDbCommand(cmdStr))
            {
                command.Parameters.AddWithValue("@email", client.Email);
                command.Parameters.AddWithValue("@firstName", client.FirstName);
                command.Parameters.AddWithValue("@lastName", client.LastName);
                command.Parameters.AddWithValue("@address1", client.Address1);
                command.Parameters.AddWithValue("@address2", client.Address2);
                command.Parameters.AddWithValue("@city", client.City);
                command.Parameters.AddWithValue("@state_province", client.State_province);
                command.Parameters.AddWithValue("@zip_postal", client.Zip_postal);
                command.Parameters.AddWithValue("@country", client.Country);
                command.Parameters.AddWithValue("@phoneNumber", client.Phone);
                base.ExecuteSimpleQuery(command);
            }
        }

        //retrive eBay store type and fees 
        public EbayStore GetStoreDetails(string storeType)
        {
            EbayStore store = new EbayStore();
            string cmdStr = "SELECT * FROM ebay WHERE type=@storeType";
            using (OleDbCommand command = new OleDbCommand(cmdStr))
            {
                command.Parameters.AddWithValue("@storeType", storeType);
                base.ExecuteStoreQuery(command);
            }
            return store;
        }
    }
}
