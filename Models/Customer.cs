using System;
namespace DAM.Models
{
    public class Customer : DBObject
    {
        public int customer_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public DateTime birth_date { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public int points { get; set; }
        public Customer(int customer_id, string first_name, string last_name, DateTime birth_date, string phone, string address, string state, string city, int points)
        {
            this.customer_id = customer_id;
            this.first_name = first_name;
            this.last_name = last_name;
            this.birth_date = birth_date;
            this.phone = phone;
            this.address = address;
            this.state = state;
            this.city = city;
            this.points = points;
        }

        public Customer() { }
    }
}
