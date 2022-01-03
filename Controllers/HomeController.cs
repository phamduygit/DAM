using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
      List<Customer> customers = new List<Customer>();

      Customer onePerson = new Customer();
      onePerson.first_name = "a";
      onePerson.last_name = "b";
      onePerson.city = "c";
      onePerson.customer_id = 15;
      onePerson.birth_date = new DateTime(2015, 12, 25); 
      onePerson.points = 2;
      onePerson.state = "NA";
      onePerson.address = "d";
      onePerson.phone = "123";
      
      MySQLManager db = new MySQLManager();
      db.connect("Server=localhost;Uid=root;Pwd=minhduy999*;Database=sql_store;");
      var maps = db.getAll("customers");
      foreach (Dictionary<string, dynamic> row in maps)
      {
        var customer = new Customer();
        customer.customer_id = row["customer_id"];
        customer.first_name = row["first_name"];
        customer.last_name = row["last_name"];
        customer.city = row["city"];
        customer.birth_date = row["birth_date"];
        customers.Add(customer);
      }
      // db.insert("customers", onePerson);
      db.delete("customers", onePerson);
      db.close();
      return View(customers);
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
  }
}
