using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DAM.Models;
using System.Text.RegularExpressions;
using System.ComponentModel;


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
      //Console.WriteLine(dbModel.connectionString + "Pwd=" + dbModel.password);
      DBManager db = DBManagerSingleton.InitDBManager(dbModel.databaseType);
      db.Connect(dbModel.connectionString + "Pwd=" + dbModel.password);

      List<string> tableList = new List<string>();
      var tables = db.Query("SHOW TABLES");

      foreach (var table in tables)
      {
          foreach (KeyValuePair<string, dynamic>  item in table)
          {
            tableList.Add(item.Value);
          }
      }
       
      return View(tableList);
    }

    public IActionResult Handle()
    {
      string tableName = this.RouteData.Values["id"].ToString();

      // Get class name from table name. Ex: customers => Customer
      string className = Regex.Replace(tableName, @"(?<!\w)\w", m => m.Value.ToUpper()).Remove(tableName.Length - 1, 1);

      // Get all column name of table
      DBManager db = MySqLManager.GetInstance();
      var cols = db.Query("DESCRIBE " + tableName);
      List<string> colNames = new List<string>();

      foreach (var col in cols)
      {
        foreach (KeyValuePair<string, dynamic> colName  in col)
        {
          colNames.Add(colName.Value);
          break;
        }
      }
      
      if (Request.HasFormContentType) {
        switch (Request.Form["handle"])
        {
          case "Update": 
            Console.WriteLine("Update");
            break;
          case "Insert": 
            Console.WriteLine("Insert");

            Type t = Type.GetType("DAM.Models." + className);
            object obj = Activator.CreateInstance(t);
          
            foreach (var colName in colNames)
            {  
              TypeConverter typeConverter = TypeDescriptor.GetConverter(obj.GetType().GetProperty(colName).PropertyType);

              if (Request.Form[colName] != "") {
                obj.GetType().GetProperty(colName).SetValue(obj, typeConverter.ConvertFromString(Request.Form[colName]));
              }
            }

            var type = obj.GetType();
            // Set tableName
            var methodInfo = type.GetMethod("setTableName");
            methodInfo.Invoke(obj, new object[]{tableName});

            // Insert with savve method
            methodInfo = type.GetMethod("save");
            methodInfo.Invoke(obj, null);
            break;
          default:   
            Console.WriteLine("Invalid");
            break;
        }
      }

      var tableData = db.Query("SELECT * FROM " + tableName);

      ViewBag.tableName = tableName;
      ViewBag.colNames = colNames;
      ViewBag.Data = tableData;

      return View("TableInfo", ViewBag);
    }
  }
}

