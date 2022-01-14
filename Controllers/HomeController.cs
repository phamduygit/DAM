using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DAM.Models;
using MySql.Data.MySqlClient;

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
      // List<Customer> customers = new List<Customer>();
      // //starter
      // //init db
      // DBManager db = DBManagerSingleton.InitDBManager("mysql");
      // //connect db
      // db.Connect("Server=localhost;Uid=root;Pwd=minhduy999*;Database=store;");
      // Customer onePerson = new Customer(customer_id: 16, first_name: "a", last_name: "b", city: "Vietnam", birth_date: new DateTime(2015, 12, 25), address: "abc", state: "hihi", points: 2, phone: "1234");
      // Console.WriteLine("Ahihi");
      // onePerson.customer_id = 20;
      // onePerson.setTableName("customers");
      // Console.WriteLine("Ahihi");
      // onePerson.setPrimaryKey("");
      // Console.WriteLine("Ahihi");

      // // Select toàn bộ bảng customers
      // // ----------------------------
      // var tableCustomers = db.Query("SELECT * FROM customers");
      // // In tất cả các key của row 1
      // foreach (KeyValuePair<string, dynamic> kvp in tableCustomers[0])
      // {
      //   Console.Write("{0} ", kvp.Key);
      // }
      // Console.WriteLine("");
      // // In tất cả các value row 1
      // foreach (KeyValuePair<string, dynamic> kvp in tableCustomers[0])
      // {
      //   Console.Write("{0} ", kvp.Value);
      // }
      // Console.WriteLine("");

      // // Select ID và ten vn bảng customers
      // // ----------------------------
      // var idAndName = db.Query("SELECT customer_id, first_name FROM customer");
      // // In tất cả các key của row 1
      // foreach (KeyValuePair<string, dynamic> kvp in idAndName[0])
      // {
      //   Console.Write("{0} ", kvp.Key);
      // }
      // Console.WriteLine("");
      // // In tất cả các value row 1
      // foreach (KeyValuePair<string, dynamic> kvp in idAndName[0])
      // {
      //   Console.Write("{0} ", kvp.Value);
      // }
      // Console.WriteLine("");
      
      // db.Close();
      // return View(customers);
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

