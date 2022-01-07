using System;
using System.Collections.Generic;
namespace DAM.Models
{
    public abstract class DBManager
    {
        protected static DBManager _instance;
        //public abstract static MySQLManager getInstance();
        public abstract bool Connect(String connectStr);
        public abstract bool Update(DBObject obj);
        public abstract bool Insert(DBObject obj);
        public abstract bool Delete(DBObject obj);
        public abstract List<Dictionary<string, dynamic>> Query(String queryStr);
        public abstract bool Close();
    }

    public class DBManagerSingleton
    {
        private static DBManager _instance;
        public static DBManager InitDBManager(String type)
        {
            if (type == "mysql")
            {
                _instance = MySqLManager.GetInstance();
            }
            //else if(type == 'postgreSql'
            //    {
            //        _instance =  PostGreSqlManager.GetInstance();
            //    }
            return _instance;
        }
        public static DBManager GetInstance()
        {
            return _instance;
        }
    }

}
