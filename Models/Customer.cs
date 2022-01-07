using System;
namespace DAM.Models
{
  public class Customer : DBObject
  {
    public int customer_id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public DateTime birth_date {get; set;}
    public string phone {get; set;}
    public string address {get; set;}
    public string state {get; set;}
    public string city { get; set; }
    public int points {get; set;}
  }
}
