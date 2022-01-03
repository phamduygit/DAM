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
    public void insert(string tableName, object obj) {
      // crreate insert string
      string keys = "";
      string values = "";
      foreach (PropertyInfo prop in obj.GetType().GetProperties())
      {
        Console.WriteLine(prop.PropertyType.Name);
        keys += prop.Name + ", ";
        if (prop.PropertyType.Name == "String") {
          values += "'" + prop.GetValue(obj, null) + "'" + ", ";
        } else if (prop.PropertyType.Name == "DateTime") {
          values += "'" + ((DateTime)prop.GetValue(obj, null)).ToString("yyyy-MM-dd H:mm:ss") + "'" + ", ";
        } 
        else {
          values += prop.GetValue(obj, null) + ", ";
        }
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
    public void delete(string tableName, object obj) {
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
