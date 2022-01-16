using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npgsql;

namespace DAM.Models
{
    public class PostgreSqlManager : DBManager
    {
        private PostgreSqlManager() { }
        public static DBManager CreateInstance()
        {
            if (instance == null)
            {
                instance = new PostgreSqlManager();
            }
            return instance;

        }
        private string stringConnection { get; set; }
        private NpgsqlConnection connection { get; set; }
        private string getValueProp(PropertyInfo prop, Object obj)
        {
            string result = "";
            if (prop.PropertyType.Name == "String")
            {
                result += "'" + prop.GetValue(obj, null) + "'";
            }
            else if (prop.PropertyType.Name == "DateTime")
            {
                result += "'" + ((DateTime)prop.GetValue(obj, null)).ToString("yyyy-MM-dd H:mm:ss") + "'";
            }
            else
            {
                result += prop.GetValue(obj, null);
            }
            return result;
        }
        // connect
        override public bool Connect(string stringConection)
        {
            try
            {
                connection = new NpgsqlConnection(stringConection);
                Console.WriteLine("connnected");

                connection.Open();
                return true;
            }
            catch (NpgsqlException ex)
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
            var cmd = new NpgsqlCommand();
            cmd.CommandText = insertString;
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            return true;
        }
        // delete
        override public bool ExecuteDelete(string deleteString)
        {
            var cmd = new NpgsqlCommand();
            cmd.CommandText = deleteString;
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            return true;
        }
        // update
        override public bool ExecuteUpdate(string updateString)
        {
            var cmd = new NpgsqlCommand();
            cmd.CommandText = updateString;
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            return true;
        }
        // query
        public override List<Dictionary<string, dynamic>> Query(String queryStr)
        {
            var maps = new List<Dictionary<string, dynamic>>();
            NpgsqlCommand cmd = new NpgsqlCommand(queryStr, connection);
            NpgsqlDataReader reader = cmd.ExecuteReader();
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

