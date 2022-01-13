using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DAM.Models;
using MySql.Data.MySqlClient;
using System.Dynamic;
using System.Reflection;

namespace DAM.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }
    public static object GetPropValue(object src, string propName)
    {
      return src.GetType().GetProperty(propName).GetValue(src, null);
    }
    public IActionResult Index()
    {
      //List<Customer> customers = new List<Customer>();

      //starter
      //init db
      //DBManager db = DBManagerSingleton.InitDBManager("mysql");

      //connect db
      //db.Connect("Server=localhost;Uid=root;Pwd=;Database=sql_store;");
      //Customer onePerson = new Customer();
      //onePerson.first_name = "a";
      //onePerson.last_name = "b";
      //onePerson.city = "Vietnam";
      //onePerson.customer_id = 136;
      //onePerson.birth_date = new DateTime(2015, 12, 25);
      //onePerson.points = 2;
      //onePerson.state = "NA";
      //onePerson.address = "d";
      //onePerson.phone = "123";
      //onePerson.setTableName("customers");
      //onePerson.setPrimaryKey("")
      //onePerson.save();
      //db.update("customers", onePerson);
      //var maps = db.getAll("customers");
      //foreach (Dictionary<string, dynamic> row in maps)
      //{
      //  var customer = new Customer();
      //  customer.customer_id = row["customer_id"];
      //  customer.first_name = row["first_name"];
      //  customer.last_name = row["last_name"];
      //  customer.city = row["city"];
      //  customer.birth_date = row["birth_date"];
      //  customer.points = row["points"];
      //  customers.Add(customer);
      //}

      //db.Close();
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public IActionResult Tables(DatabaseModel dbModel)
    {
      //Console.WriteLine(dbModel.databaseType + " " + dbModel.connectionString + " " + dbModel.password);
      DBManager db = DBManagerSingleton.InitDBManager(dbModel.databaseType);

      db.Connect(dbModel.connectionString + "Pwd=" + dbModel.password);
      //query
      MySqlConnection connect = new MySqlConnection(dbModel.connectionString + "Pwd=" + dbModel.password);
      connect.Open();
      MySqlCommand cmd = new MySqlCommand("SHOW TABLES", connect);
      List<string> tableList = new List<string>();
      MySqlDataReader reader = cmd.ExecuteReader();

      while (reader.Read())
      {
        tableList.Add(reader.GetString(0));
      }

      reader.Close();
      return View(tableList);
    }

    public IActionResult Handle(Customer customer)
    {
      string tableName = this.RouteData.Values["id"].ToString();
      //DBManager db = DBManagerSingleton.InitDBManager("mysql");
      //db.Connect("Server=localhost;Uid=root;Pwd=;Database=sql_store;");
      //db.Query("DESCRIBE customers");
      
      if (Request.HasFormContentType) {
        switch (Request.Form["handle"])
        {
          case "Update": 
            Console.WriteLine("Update");
            break;
          case "Insert": 
            Console.WriteLine("Insert");
            break;
          default:   
            Console.WriteLine("Invalid");
            break;
        }
      }
      
      MySqlConnection connect = new MySqlConnection("Server=localhost;Uid=root;Pwd=;Database=sql_store;");
      connect.Open();

      var maps = new List<Dictionary<string, dynamic>>();
      var colNames = new List<string>();
      MySqlCommand cmd = new MySqlCommand("select * from " + tableName, connect);
      MySqlCommand cmdReadCol = new MySqlCommand("DESCRIBE " + tableName, connect);
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

      reader.Close();
      ViewBag.ColNames = colNames;
      ViewBag.Data = maps;

      return View("customers", ViewBag);
    }
  }
}

