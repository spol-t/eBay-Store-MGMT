using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;

namespace StoreMGMT
{
    class DBConnect
    {
        protected MySqlConnection connection;
        private string server;
        private string port;
        private string database;
        private string uid;
        private string password;

        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            port = "3306";
            database = "storemgmt";
            uid = "Manager";
            password = "!qaz2wsx";
            string connectionString;
            connectionString = "SERVER=" + server + ";" +
                "PORT=" + port + ";" +
                "DATABASE=" + database + ";" +
                "UID=" + uid + ";" +
                "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        #region connection

        //open connection to database if it is not open
        protected void OpenConnection()
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    connection.Open();

                }
                catch (MySqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 0:
                            MessageBox.Show("Cannot connect to server.  Contact administrator");
                            break;

                        case 1045:
                            MessageBox.Show("Invalid username/password, please try again");
                            break;
                    }
                }
            }
        }

        //Close connection
        protected void Disconnect()
        {
            try
            {
                connection.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        //Send a query without return
        protected void ExecuteSimpleQuery(MySqlCommand command)
        {
            //Locking database chanel
            lock (connection)
            {
                OpenConnection();
                command.Connection = connection;
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
        }

        //return a string from the database
        protected string ExecuteScalarStringQuery(MySqlCommand command)
        {
            string str = "";
            //Locking database chanel
            lock (connection)
            {
                OpenConnection();
                command.Connection = connection;
                try
                {
                    //Execute command.text qery of th DB
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        str = Convert.ToString(result);
                    }
                }
                finally
                {
                    //Closing the connection and restoring the lock
                    Disconnect();
                }
                return str;
            }
        }

        //return an Integerfrom the database
        protected int ExecuteScalarIntQuery(MySqlCommand command)
        {
            int num = 0;
            //Locking database chanel
            lock (connection)
            {
                OpenConnection();
                command.Connection = connection;
                try
                {
                    //Execute command.text qery of th DB
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        num = Convert.ToInt32(result);
                    }
                }
                finally
                {
                    //Closing the connection and restoring the lock
                    Disconnect();
                }
                return num;
            }
        }

        //send a query that returns a number of arguments
        protected DataSet GetMultipleQuery(MySqlCommand command)
        {
            DataSet dataset = new DataSet();
            //Locking database chanel
            lock (connection)
            {
                OpenConnection();
                command.Connection = connection;
                try
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    adapter.SelectCommand = command;
                    adapter.Fill(dataset);
                }
                finally
                {
                    //Closing the connection and restoring the lock
                    Disconnect();
                }
            }
            return dataset;
        }
    }
}
