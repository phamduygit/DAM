using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
namespace DAM.Models
{
    //public class MySQLManager : DBManager
    public class MySqLManager : DBManager
    {
        private MySqLManager() { }
        public static DBManager CreateInstance()
        {
            if (instance == null)
            {
                instance = new MySqLManager();
            }
            return instance;

        }
        private string stringConnection { get; set; }
        private MySqlConnection connection { get; set; }

        // connect
        override public bool Connect(string stringConection)
        {
            try
            {
                Console.WriteLine(stringConection);
                connection = new MySql.Data.MySqlClient.MySqlConnection(stringConection);

                connection.Open();
                Console.WriteLine("Connected to db");
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Didn't connect to db");
                return false;
            }
        }
        // insert
        override public bool ExecuteInsert(string insertString)
        {
            // crreate insert string

            var cmd = new MySqlCommand();
            cmd.CommandText = insertString;
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            return true;
        }
        // delete
        override public bool ExecuteDelete(string deleteString)
        {
            // create delete string
            var cmd = new MySqlCommand();
            cmd.CommandText = deleteString;
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            return true;
        }
        // update
        override public bool ExecuteUpdate(string updateString)
        {

            var cmd = new MySqlCommand();
            cmd.CommandText = updateString;
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            return true;
        }
        // query
        public override List<Dictionary<string, dynamic>> Query(String queryStr)
        {
            var maps = new List<Dictionary<string, dynamic>>();
            MySqlCommand cmd = new MySqlCommand(queryStr, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                maps.Add(Enumerable.Range(0, reader.FieldCount)
                         .ToDictionary(reader.GetName, reader.GetValue));
            }
            reader.Close();
            return maps;
        }
        // close
        override public bool Close()
        {
            connection.Close();
            return true;
        }

    }
}
