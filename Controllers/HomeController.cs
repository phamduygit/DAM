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
        Type t = Type.GetType("DAM.Models." + className);
        object obj = Activator.CreateInstance(t);
        var methodInfo = t.GetMethod("setTableName");
        methodInfo.Invoke(obj, new object[]{tableName});

        switch (Request.Form["handle"])
        {
          case "Update": 
            Console.WriteLine("Update");

            foreach (var colName in colNames)
            {  
              TypeConverter typeConverter = TypeDescriptor.GetConverter(obj.GetType().GetProperty(colName).PropertyType);

              if (Request.Form[colName] != "") {
                obj.GetType().GetProperty(colName).SetValue(obj, typeConverter.ConvertFromString(Request.Form[colName]));
              }
            }

            // Insert with savve method
            methodInfo = t.GetMethod("update");
            methodInfo.Invoke(obj, null);
            break;
          case "Insert": 
            Console.WriteLine("Insert");
          
            foreach (var colName in colNames)
            {  
              TypeConverter typeConverter = TypeDescriptor.GetConverter(obj.GetType().GetProperty(colName).PropertyType);

              if (Request.Form[colName] != "") {
                obj.GetType().GetProperty(colName).SetValue(obj, typeConverter.ConvertFromString(Request.Form[colName]));
              }
            }

            methodInfo = t.GetMethod("save");
            methodInfo.Invoke(obj, null);
            break;
          case "Delete":
            Console.WriteLine("Delete");

            foreach (var colName in colNames)
            {  
              TypeConverter typeConverter = TypeDescriptor.GetConverter(obj.GetType().GetProperty(colName).PropertyType);

              if (Request.Form[colName] != "") {
                obj.GetType().GetProperty(colName).SetValue(obj, typeConverter.ConvertFromString(Request.Form[colName]));
              }
            }

            methodInfo = t.GetMethod("delete");
            methodInfo.Invoke(obj, null);
            break;

          case "Query":
            Console.WriteLine("Query");
            var result = db.Query(Request.Form["queryStr"]);

            ViewBag.tableName = tableName;
            ViewBag.colNames = colNames;
            ViewBag.Data = result;

            return View("TableInfo", ViewBag);

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

    public IActionResult CloseConnection()
    {
      DBManager db = MySqLManager.GetInstance();
      db.Close();
       
      return View("Index");
    }
  }
}

