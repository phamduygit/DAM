using System;
using System.Collections.Generic;
using System.Reflection;

namespace DAM.Models
{
    public abstract class DBManager
    {
        private string getValueProp(PropertyInfo prop, Object obj)
        {
            string result = "";
            if (prop.PropertyType.Name == "String")
            {
                result += "'" + prop.GetValue(obj, null) + "'";
            }
            else if (prop.PropertyType.Name == "DateTime")
            {
                result += "'" + ((DateTime)prop.GetValue(obj, null)).ToString("yyyy-MM-dd H:mm:ss") + "'";
            }
            else
            {
                result += prop.GetValue(obj, null);
            }
            return result;
        }
        private string getUpdateString(DBObject obj)
        {
            string tableName = obj.getTableName();
            string key_value = "";
            string condition = "";
            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                if (condition == "")
                {
                    condition = prop.Name + "=" + getValueProp(prop, obj) + ", ";
                }
                else
                {
                    key_value += prop.Name + "=" + getValueProp(prop, obj) + ", ";
                }
            }
            key_value = key_value.Remove(key_value.Length - 2, 2);
            condition = condition.Remove(condition.Length - 2, 2);
            return $"update {tableName} set {key_value} where {condition}";
        }
        private string getInsertString(DBObject obj)
        {
            string tableName = obj.getTableName();
            string keys = "";
            string values = "";
            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                Console.WriteLine(prop.PropertyType.Name);
                keys += prop.Name + ", ";
                values += getValueProp(prop, obj) + ", ";
            }
            keys = keys.Remove(keys.Length - 2, 2);
            values = values.Remove(values.Length - 2, 2);
            // return insert String  
            return $"INSERT INTO {tableName}({keys}) VALUES({values})";
        }

        public static string dbType;
        protected DBManager() { }
        protected static DBManager instance;

        public static DBManager GetInstance()
        {
            return instance;
        }
        public static DBManager Init(string _dbType)
        {
            if (instance != null)
            {
                instance.Close();
                instance = null;
            }
            if (_dbType == "mysql")
            {
                instance = MySqLManager.CreateInstance();
                dbType = _dbType;
            }
            else if (_dbType == "postgresql")
            {
                instance = PostgreSqlManager.CreateInstance();
                dbType = _dbType;
            }
            return instance;
        }
        protected static DBManager _instance;
        //public abstract static MySQLManager getInstance();
        public abstract bool Connect(String connectStr);

        public bool Update(DBObject obj)
        {
            string updateString = getUpdateString(obj);
            return ExecuteUpdate(updateString);
        }
        public abstract bool ExecuteUpdate(string updateString);

        public bool Insert(DBObject obj)
        {
            string insertString = getInsertString(obj);
            Console.WriteLine(insertString);
            return ExecuteInsert(insertString);
        }
        public abstract bool ExecuteInsert(string insertString);


        private string getDeleteString(DBObject obj)
        {
            string tableName = obj.getTableName();
            string key = "";
            string value = "";
            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                key = prop.Name;
                value = prop.GetValue(obj, null).ToString();
                break;
            }
            return $"DELETE FROM {tableName} WHERE {key} = {value}";
        }
        public bool Delete(DBObject obj)
        {
            string deleteString = getDeleteString(obj);
            return ExecuteDelete(deleteString);
        }
        public abstract bool ExecuteDelete(string deleteString);


        public abstract List<Dictionary<string, dynamic>> Query(String queryStr);
        public abstract bool Close();
    }

}
