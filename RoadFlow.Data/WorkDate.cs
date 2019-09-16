using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadFlow.Mapper;
using RoadFlow.Utility;

namespace RoadFlow.Data
{
    public class WorkDate
    {
        /// <summary>
        /// 缓存KEY
        /// </summary>
        private const string CACHEKEY = "roadflow_cache_workdate";
        /// <summary>
        /// 得到一年所有工作日
        /// </summary>
        /// <returns></returns>
        public List<Model.WorkDate> GetYearList(int year)
        {
            string cacheKey = CACHEKEY + "_" + year.ToString();
            object obj = Cache.IO.Get(cacheKey);
            if (null == obj)
            {
                using (var db = new DataContext())
                {
                    string sql = db.IsOracle ? "SELECT * FROM RF_WorkDate WHERE TO_CHAR(WorkDay,'yyyy')=" + year
                        : "SELECT * FROM RF_WorkDate WHERE YEAR(WorkDay)=" + year;
                    var workDates = db.Query<Model.WorkDate>(sql);
                    Cache.IO.Insert(CACHEKEY, workDates);
                    return workDates;
                }
            }
            else
            {
                return (List<Model.WorkDate>)obj;
            }
        }

        /// <summary>
        /// 添加一年工作日(先删除再添加)
        /// </summary>
        /// <param name="userShortcuts"></param>
        /// <param name="year">年</param>
        /// <returns></returns>
        public int Add(Model.WorkDate[] workDates, int year)
        {
            ClearCache(year);
            using (var db = new DataContext())
            {
                string sql = db.IsOracle ? "DELETE FROM RF_WorkDate WHERE TO_CHAR(WorkDay,'yyyy')=" + year
                    : "DELETE FROM RF_WorkDate WHERE YEAR(WorkDay)=" + year;
                //先删除
                db.Execute(sql);
                if (workDates.Length > 0)
                {
                    db.AddRange(workDates);
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除一的工作日
        /// </summary>
        /// <param name="year">年</param>
        /// <returns></returns>
        public int Delete(int year)
        {
            ClearCache(year);
            using (var db = new DataContext())
            {
                var yearList = GetYearList(year);
                db.RemoveRange(yearList);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache(int year)
        {
            Cache.IO.Remove(CACHEKEY + "_" + year.ToString());
        }

        /// <summary>
        /// 得到所有设置中最小年份
        /// </summary>
        /// <returns></returns>
        public int GetMinYear()
        {
            using (var db = new DataContext())
            {
                string sql = db.IsOracle ? "SELECT MIN(TO_CHAR(WorkDay,'yyyy')) FROM RF_WorkDate"
                    : "SELECT MIN(YEAR(WorkDay)) FROM RF_WorkDate";
                string year = db.ExecuteScalarString(sql);
                return year.IsInt(out int y) ? y : Utility.DateExtensions.Now.Year;
            }
        }
    }
}
