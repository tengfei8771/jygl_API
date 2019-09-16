using System;
using System.Collections.Generic;
using System.Text;

namespace RoadFlow.Business
{
    public class WorkDate
    {
        private readonly Data.WorkDate workDateData;
        public WorkDate()
        {
            workDateData = new Data.WorkDate();
        }
        /// <summary>
        /// 得到一年所有工作日
        /// </summary>
        /// <returns></returns>
        public List<Model.WorkDate> GetYearList(int year)
        {
            return workDateData.GetYearList(year);
        }

        /// <summary>
        /// 添加一年工作日(先删除再添加)
        /// </summary>
        /// <param name="userShortcuts"></param>
        /// <param name="year">年</param>
        /// <returns></returns>
        public int Add(Model.WorkDate[] workDates, int year)
        {
            return workDateData.Add(workDates, year);
        }

        /// <summary>
        /// 删除一年的工作日
        /// </summary>
        /// <param name="year">年</param>
        /// <returns></returns>
        public int Delete(int year)
        {
            return workDateData.Delete(year);
        }

        /// <summary>
        /// 得到所有设置中最小年份
        /// </summary>
        /// <returns></returns>
        public int GetMinYear()
        {
            return workDateData.GetMinYear();
        }

        /// <summary>
        /// 得到一个时间加上几天之后的工作时间
        /// </summary>
        /// <param name="days">天数</param>
        /// <param name="dt">时间</param>
        /// <returns></returns>
        public DateTime GetWorkDateTime(double days, DateTime? dt = null)
        {
            DateTime dateTime = dt != null && dt.HasValue ? dt.Value : Utility.DateExtensions.Now;
            var yearList = GetYearList(dateTime.Year);
            int max = (int)Math.Floor(days);
            for (int i = 0; i < max; i++)
            {
                if (yearList.Exists(p => p.WorkDay == dateTime.AddDays(i).Date && p.IsWork == 0))
                {
                    max++;
                }
            }
            return dateTime.AddDays(max + (days - Math.Floor(days)));
        }

    }
}
