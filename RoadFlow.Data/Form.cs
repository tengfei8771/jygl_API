using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Mapper;

namespace RoadFlow.Data
{
    public class Form
    {
        /// <summary>
        /// 得到所有表单
        /// </summary>
        /// <returns></returns>
        public List<Model.Form> GetAll()
        {
            using (var db = new DataContext())
            {
                return db.QueryAll<Model.Form>();
            }
        }
        /// <summary>
        /// 查询一个表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Form Get(Guid id)
        {
            using (var db = new DataContext())
            {
                return db.Find<Model.Form>(id);
            }
        }
        /// <summary>
        /// 添加一个表单
        /// </summary>
        /// <param name="form">表单实体</param>
        /// <returns></returns>
        public int Add(Model.Form form)
        {
            using (var db = new DataContext())
            {
                db.Add(form);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 更新表单
        /// </summary>
        /// <param name="form">表单实体</param>
        public int Update(Model.Form form)
        {
            using (var db = new DataContext())
            {
                db.Update(form);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public int Delete(Model.Form form)
        {
            using (var db = new DataContext())
            {
                db.Remove(form);
                return db.SaveChanges();
            }
        }
        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="form">表单实体</param>
        /// <param name="appLibrary">应用程序库实体</param>
        /// <param name="delete">是否彻底删除 0不 1彻底删除</param>
        /// <returns></returns>
        public int Delete(Model.Form form, Model.AppLibrary appLibrary, int delete = 0)
        {
            using (var db = new DataContext())
            {
                if (null != form)
                {
                    if (delete == 0)
                    {
                        form.Status = 2;
                        db.Update(form);//只作删除标记，不物理删除
                    }
                    else
                    {
                        db.Remove(form);//物理删除
                    }
                }
                if (null != appLibrary)
                {
                    db.Remove(appLibrary);
                    new AppLibrary().ClearCache();
                }
                return db.SaveChanges();
            }
        }

        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="userId">当前人员ID</param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerList(out int count, int size, int number, Guid userId, string name, string type, string order, int status = -1)
        {
            using (var db = new DataContext())
            {
                DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(Utility.Config.DatabaseType);
                var (sql, parameter) = dbconnnectionSql.SqlInstance.GetFormSql(userId, name, type, order, status);
                string pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(sql, size, number, out count, parameter, order);
                return db.GetDataTable(pagerSql, parameter);
            }
        }
    }
}
