using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Reflection;

namespace DAM.Models
{
  public class MySQLManager
  {
    private string stringConnection { get; set; }
    private MySqlConnection connection { get; set; }
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
    public bool connect(string stringConection)
    {
      try
      {
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
    // string Query = "insert into student.studentinfo(idStudentInfo,Name,Father_Name,Age,Semester) values('" + this.IdTextBox.Text + "','" + this.NameTextBox.Text + "','" + this.FnameTextBox.Text + "','" + this.AgeTextBox.Text + "','" + this.SemesterTextBox.Text + "');";
    public void insert(string tableName, object obj)
    {
      // crreate insert string
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
      var cmd = new MySqlCommand();
      cmd.CommandText = insertString;
      cmd.Connection = connection;
      cmd.ExecuteNonQuery();
    }
    // delete
    public void delete(string tableName, object obj)
    {
      // create delete string
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
      var cmd = new MySqlCommand();
      cmd.CommandText = deleteString;
      cmd.Connection = connection;
      cmd.ExecuteNonQuery();
    }
    // update
    public void update(string tableName, object obj)
    {
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
      var cmd = new MySqlCommand();
      cmd.CommandText = updateString;
      cmd.Connection = connection;
      cmd.ExecuteNonQuery();
    }
    // query
    public List<Dictionary<string, dynamic>> getAll(string tableName)
    {
      var maps = new List<Dictionary<string, dynamic>>();
      var colNames = new List<string>();
      MySqlCommand cmd = new MySqlCommand("select * from " + tableName, connection);
      MySqlCommand cmdReadCol = new MySqlCommand("DESCRIBE " + tableName, connection);
      MySqlDataReader reader = cmdReadCol.ExecuteReader();
      while (reader.Read())
      {
        colNames.Add(reader.GetString(0));
      }
      reader.Close();
      reader = cmd.ExecuteReader();
      while (reader.Read())
      {
        var map = new Dictionary<string, dynamic>();
        for (int i = 0; i < colNames.Count; i++)
        {
          map[colNames[i].ToString()] = reader[colNames[i]];
        }
        maps.Add(map);
      }
      // string Query = "insert into student.studentinfo(idStudentInfo,Name,Father_Name,Age,Semester) values('" + this.IdTextBox.Text + "','" + this.NameTextBox.Text + "','" + this.FnameTextBox.Text + "','" + this.AgeTextBox.Text + "','" + this.SemesterTextBox.Text + "');";
      reader.Close();
      return maps;
    }
    // close
    public void close()
    {
      connection.Close();
    }
  }
}
