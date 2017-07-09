using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreMGMT
{
    public class DbAccess
    {
        #region Constructor + members

        protected OleDbConnection _conn = null;

        //constructor
        public DbAccess(string connectionString)
        {
            //create a new OleDbConnection object
            _conn = new OleDbConnection(connectionString);
        }

        #endregion

        #region Protected Methods
        //Open the connection if it is not open
        protected void Connect()
        {
            //opern the connection if it is not open
            if( _conn.State != ConnectionState.Open )
            {
                _conn.Open();
            }
        }

        //Closing the connection
        protected void Disconnect()
        {
            _conn.Close();
        }

        //Send a query without return
        protected void ExecuteSimpleQuery(OleDbCommand command)
        {
            //Locking database chanel
            lock (_conn)
            {
                Connect();
                command.Connection = _conn;
            }
            try
            {
                //Execute command.text qery of th DB
                command.ExecuteNonQuery();
            }
            finally
            {
                //Closing the connection and restoring the lock
                Disconnect();
            }
        }

        //submiting a qery with an argument and returning an integer
        protected int ExecuteScalarIntQuery(OleDbCommand command)
        {
            int ret = -1;
            lock(_conn)
            {
                Connect();
                command.Connection = _conn;
                try
                {
                    // calling a query that returns Executescalar - 1
                    ret = (int)command.ExecuteScalar();
                }
                finally
                {
                    Disconnect();
                }
                return ret;
            }
        }

        //submiting a query and return mulitipal arguments
        protected DataSet GetMulitpaleQuery (OleDbCommand command)
        {
            DataSet dataset = new DataSet();

            lock(_conn)
            {
                Connect();
                command.Connection = _conn;
                try
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.Fill(dataset);
                }
                finally
                {
                    Disconnect();
                }
            }
            return dataset;
        }

        //submiting a qery with an argument and returning an EbayStore
        protected EbayStore ExecuteStoreQuery(OleDbCommand command)
        {
            EbayStore store = new EbayStore();
            lock (_conn)
            {
                Connect();
                command.Connection = _conn;
                try
                {
                    // calling a query that returns the store details
                    store = (EbayStore)command.ExecuteScalar();
                }
                finally
                {
                    Disconnect();
                }
                return store;
            }
        }
        #endregion
    }
}
