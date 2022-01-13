using System;
using System.Collections.Generic;
namespace DAM.Models
{
    public abstract class DBObject
    {
        protected static string tableName;
        protected static string primaryKey;
        protected static List<string> collumnNames {get ; set;}
        public void setTableName(string _tableName)
        {
            tableName = _tableName;
        }
        public string getTableName()
        {
            return tableName;
        }
        public void setPrimaryKey(string _primaryKey)
        {
            primaryKey = _primaryKey;
        }
        public string getPrimaryKey()
        {
            return primaryKey;
        }

        public bool save()
        {
            DBManager db = DBManagerSingleton.GetInstance();
            return db.Insert(this);
        }

        public bool update()
        {
            DBManager db = DBManagerSingleton.GetInstance();
            return db.Update(this);
        }

        public bool delete()
        {
            DBManager db = DBManagerSingleton.GetInstance();
            return db.Delete(this);
        }
    }
}
