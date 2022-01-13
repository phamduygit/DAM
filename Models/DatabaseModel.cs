using System;
using System.Collections.Generic;
namespace DAM.Models
{
    public class DatabaseModel
    {
        public string databaseType { get; set; }

        public string connectionString { get; set; }

        public string password { get; set; }
    }
}
