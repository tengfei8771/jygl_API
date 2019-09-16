using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Data;

namespace RoadFlow.Mapper
{
    public class Common
    {
        /// <summary>
        /// 得到表名(根据类注释)
        /// </summary>
        /// <param name="type"></param>
        /// <returns>如果类没有属性[Table("")],则返回类名作为表名</returns>
        public static string GetTableName(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(TableAttribute), false);
            return attrs.Length > 0 ? (attrs[0] as TableAttribute).Name : type.Name;
        }

        /// <summary>
        /// 得到主键
        /// </summary>
        /// <param name="type"></param>
        /// <returns>如果没有标记主键[Key]则查找名称为Id的字段</returns>
        public static List<string> GetPrimaryKeys(PropertyInfo[] properties)
        {
            List<string> list = new List<string>();
            foreach (var p in properties)
            {
                var attrs = p.GetCustomAttributes(typeof(KeyAttribute), false);
                if (attrs.Length > 0)
                {
                    list.Add(p.Name);
                }
            }
            if (list.Any())
            {
                return list;
            }
            //如果没有标记主键[Key]则查找名称为Id的字段
            if (properties.Any(p => p.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase)))
            {
                list.Add("Id");
            }
            return list;
        }

        /// <summary>
        /// 得到主键(包含字段类型)
        /// </summary>
        /// <param name="type"></param>
        /// <returns>(主键名称, 字段类型)</returns>
        public static List<(string, Type)> GetPrimaryKeyAndTypes(PropertyInfo[] properties)
        {
            List<(string, Type)> list = new List<(string, Type)>();
            foreach (var p in properties)
            {
                var attrs = p.GetCustomAttributes(typeof(KeyAttribute), false);
                if (attrs.Length > 0)
                {
                    list.Add((p.Name, p.PropertyType));
                }
            }
            if (list.Any())
            {
                return list;
            }
            //如果没有标记主键[Key]则查找名称为Id的字段
            foreach (var p in properties)
            {
                if (p.Name.Equals("id", StringComparison.CurrentCultureIgnoreCase))
                {
                    list.Add(("Id", p.PropertyType));
                    break;
                }
            }
            return list;
        }

        public static readonly Type _String = typeof(string);
        public static readonly Type _Guid = typeof(Guid);
        public static readonly Type _DateTime = typeof(DateTime);
        public static readonly Type _Short = typeof(short);
        public static readonly Type _Int = typeof(int);
        public static readonly Type _Long = typeof(long);
        public static readonly Type _Double = typeof(double);
        public static readonly Type _Float = typeof(float);
        public static readonly Type _Decimal = typeof(decimal);
        public static readonly Type _Bool = typeof(bool);
        public static readonly Type _GuidNull = typeof(Guid?);
        public static readonly Type _DateTimeNull = typeof(DateTime?);
        public static readonly Type _ShortNull = typeof(short?);
        public static readonly Type _IntNull = typeof(int?);
        public static readonly Type _LongNull = typeof(long?);
        public static readonly Type _DoubleNull = typeof(double?);
        public static readonly Type _FloatNull = typeof(float?);
        public static readonly Type _DecimalNull = typeof(decimal?);

        /// <summary>
        /// 空类型对应的类型
        /// </summary>
        public static readonly Dictionary<Type, Type> NullTypes = new Dictionary<Type, Type>() {
            { _GuidNull, _Guid},
            { _DateTimeNull, _DateTime},
            { _ShortNull, _Short},
            { _LongNull, _Long},
            { _DoubleNull, _Double},
            { _FloatNull, _Float},
            { _DecimalNull, _Decimal}
        };

        /// <summary>
        /// 得到字段属性对应的值
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetReaderValue(Type propertyType, object value)
        {
            if (null == value || DBNull.Value == value)
            {
                return DBNull.Value;
            }
            if (propertyType == _String)
            {
                return value.ToString();
            }
            if (propertyType == _Guid)
            {
                return Guid.Parse(value.ToString());
            }
            if (propertyType == _DateTime)
            {
                return DateTime.Parse(value.ToString());
            }
            if (propertyType == _Short)
            {
                return short.Parse(value.ToString());
            }
            if (propertyType == _Int)
            {
                return int.Parse(value.ToString());
            }
            if (propertyType == _Long)
            {
                return long.Parse(value.ToString());
            }
            if (propertyType == _Double)
            {
                return double.Parse(value.ToString());
            }
            if (propertyType == _Float)
            {
                return float.Parse(value.ToString());
            }
            if (propertyType == _Decimal)
            {
                return decimal.Parse(value.ToString());
            }
            if (propertyType == _Bool)
            {
                return bool.Parse(value.ToString());
            }

            #region Nullable
            if (propertyType == _GuidNull)
            {
                return Guid.TryParse(value.ToString(), out Guid guid) ? new Guid?(guid) : new Guid?();
            }
            if (propertyType == _DateTimeNull)
            {
                return DateTime.TryParse(value.ToString(), out DateTime dateTime) ? new DateTime?(dateTime) : new DateTime?();
            }
            if (propertyType == _ShortNull)
            {
                return short.TryParse(value.ToString(), out short i) ? new short?(i) : new short?();
            }
            if (propertyType == _IntNull)
            {
                return int.TryParse(value.ToString(), out int i) ? new int?(i) : new int?();
            }
            if (propertyType == _LongNull)
            {
                return long.TryParse(value.ToString(), out long i) ? new long?(i) : new long?();
            }
            if (propertyType == _DoubleNull)
            {
                return double.TryParse(value.ToString(), out double i) ? new double?(i) : new double?();
            }
            if (propertyType == _FloatNull)
            {
                return float.TryParse(value.ToString(), out float i) ? new float?(i) : new float?();
            }
            if (propertyType == _DecimalNull)
            {
                return decimal.TryParse(value.ToString(), out decimal i) ? new decimal?(i) : new decimal?();
            }
            #endregion

            return value;
        }

        /// <summary>
        /// 得到oracle参数类型
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static object GetParameterValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return DBNull.Value;
            }
            var type = value.GetType();
            if (_Guid == type)
            {
                return value.ToString().ToUpper();
            }
            else if (_DateTime == type)
            {
                return DateTime.Parse(value.ToString());
            }
            return value.ToString();
        }

        /// <summary>
        /// 得到DataTable的列类型(如：Guid?要返回Guid)
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static Type GetDataColumnType(Type propertyType)
        {
            if (NullTypes.ContainsKey(propertyType))
            {
                return NullTypes[propertyType];
            }
            else
            {
                return propertyType;
            }
        }

        /// <summary>
        /// 将列表转换为DATATABLE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IEnumerable<T> ts)
        {
            DataTable dataTable = new DataTable();
            if (!ts.Any())
            {
                return dataTable;
            }
            Type type = typeof(T);
            var properties = type.GetProperties();
            string tablename = Common.GetTableName(type);
            if (string.IsNullOrWhiteSpace(tablename))
            {
                return dataTable;
            }
            dataTable.TableName = tablename;
            foreach (var p in properties)
            {
                DataColumn dataColumn = new DataColumn(p.Name)
                {
                    DataType = GetDataColumnType(p.PropertyType)
                };
                dataTable.Columns.Add(dataColumn); ;
            }
            foreach (var t in ts)
            {
                List<object> list = new List<object>();
                DataRow dataRow = dataTable.NewRow();
                foreach (var p in properties)
                {
                    object value = p.GetValue(t);
                    dataRow[p.Name] = value ?? DBNull.Value;
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
    }
}
