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
    public static DBManager GetInstance()
    {
      if (_instance == null)
      {
        _instance = new PostgreSqlManager();
      }
      return _instance;

    }
    private string stringConnection { get; set; }
    private  NpgsqlConnection connection { get; set; }
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
    override public bool Insert(DBObject obj)
    {
      // crreate insert string
      string tableName = obj.getTableName();
      string keys = "";
      string values = "";
      foreach (PropertyInfo prop in obj.GetType().GetProperties())
      {
        Console.WriteLine(prop.PropertyType.Name);
        keys += prop.Name + ", ";
        values += getValueProp(prop, obj) + ", ";
      }
      keys = keys.Remove(keys.Length - 2, 2);
      values = values.Remove(values.Length - 2, 2);
      // execute  
      string insertString = $"INSERT INTO {tableName}({keys}) VALUES({values})";
      Console.WriteLine(insertString);
      var cmd = new NpgsqlCommand();
      cmd.CommandText = insertString;
      cmd.Connection = connection;
      cmd.ExecuteNonQuery();
      return true;
    }
    // delete
    override public bool Delete(DBObject obj)
    {
      // create delete string
      string tableName = obj.getTableName();
      string key = "";
      string value = "";
      foreach (PropertyInfo prop in obj.GetType().GetProperties())
      {
        key = prop.Name;
        value = prop.GetValue(obj, null).ToString();
        break;
      }
      string deleteString = $"DELETE FROM {tableName} WHERE {key} = {value}";
      Console.WriteLine(deleteString);
      var cmd = new NpgsqlCommand();
      cmd.CommandText = deleteString;
      cmd.Connection = connection;
      cmd.ExecuteNonQuery();
      return true;
    }
    // update
    override public bool Update(DBObject obj)
    {
      string tableName = obj.getTableName();
      string updateString = "";
      string key_value = "";
      string condition = "";
      foreach (PropertyInfo prop in obj.GetType().GetProperties())
      {
        if (condition == "")
        {
          condition = prop.Name + "=" + getValueProp(prop, obj) + ", ";
        }
        else
        {
          key_value += prop.Name + "=" + getValueProp(prop, obj) + ", ";
        }
      }
      key_value = key_value.Remove(key_value.Length - 2, 2);
      condition = condition.Remove(condition.Length - 2, 2);
      updateString = $"update {tableName} set {key_value} where {condition}";
      Console.WriteLine(updateString);
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

