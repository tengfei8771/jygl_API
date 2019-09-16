using System;
using System.Collections.Generic;
using System.Text;
using RoadFlow.Utility;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{
    public class SystemButton
    {
        private static readonly string CacheKey = "Cache_SystemButton";
        private readonly Data.SystemButton systemButtonData;
        public SystemButton()
        {
            systemButtonData = new Data.SystemButton();
        }

        /// <summary>
        /// 得到所有按钮
        /// </summary>
        /// <returns></returns>
        public List<Model.SystemButton> GetAll()
        {
            var obj = Cache.IO.Get(CacheKey);
            if(null != obj)
            {
                return (List<Model.SystemButton>)obj;
            }
            var list = systemButtonData.GetAll();
            Cache.IO.Insert(CacheKey, list);
            return list;
        }
        /// <summary>
        /// 查询一个按钮
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.SystemButton Get(Guid id)
        {
            var list = GetAll();
            var model = list?.Find(p => p.Id == id);
            return model ?? systemButtonData.Get(id);
        }
        /// <summary>
        /// 添加一个按钮
        /// </summary>
        /// <param name="systemButton">字典实体</param>
        /// <returns></returns>
        public int Add(Model.SystemButton systemButton)
        {
            ClearCache();
            return systemButtonData.Add(systemButton);
        }
        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="systemButton">字典实体</param>
        public int Update(Model.SystemButton systemButton)
        {
            ClearCache();
            return systemButtonData.Update(systemButton);
        }
        /// <summary>
        /// 删除一个按钮
        /// </summary>
        /// <param name="systemButton">字典实体</param>
        /// <returns></returns>
        public int Delete(Model.SystemButton systemButton)
        {
            ClearCache();
            return systemButtonData.Delete(systemButton);
        }

        /// <summary>
        /// 删除一批按钮
        /// </summary>
        /// <param name="ids">id字符串，多个逗号分开</param>
        /// <returns></returns>
        public List<Model.SystemButton> Delete(string ids)
        {
            List<Model.SystemButton> systemButtons = new List<Model.SystemButton>();
            foreach (string id in ids.Split(','))
            {
                var but = Get(id.ToGuid());
                if (null != but)
                {
                    systemButtons.Add(but);
                }
            }
            systemButtonData.Delete(systemButtons.ToArray());
            ClearCache();
            return systemButtons;
        }
        /// <summary>
        /// 得到最大排序
        /// </summary>
        /// <returns></returns>
        public int GetMaxSort()
        {
            var allButtons = GetAll();
            return allButtons.Count == 0 ? 5 : allButtons.Max(p => p.Sort) + 5;
        }
        /// <summary>
        /// 得到所有按钮下拉选项
        /// </summary>
        /// <param name="value">默认值</param>
        /// <param name="language">语言</param>
        /// <returns></returns>
        public string GetOptions(string value = "", string language = "zh-CN")
        {
            var buttons = GetAll();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var button in buttons)
            {
                string name = language.Equals("en-US") ? button.Name_en : language.Equals("zh") ? button.Name_zh : button.Name;
                string note = button.Note.IsNullOrWhiteSpace() ? string.Empty : " (" + button.Note + ")";
                stringBuilder.Append("<option value=\"" + button.Id + "\"" + (button.Id.ToString().EqualsIgnoreCase(value) ? " selected=\"selected\"" : "") + ">" + name + note + "</option>");
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 得到所有按钮JSON
        /// </summary>
        /// <returns></returns>
        public Newtonsoft.Json.Linq.JArray GetAllJson()
        {
            var buttons = GetAll();
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (var button in buttons)
            {
                jArray.Add(Newtonsoft.Json.Linq.JObject.Parse(button.ToString()));
            }
            return jArray;
        }
        /// <summary>
        /// 得到按钮类型下拉选择
        /// </summary>
        /// <param name="value"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public string GetButtonTypeOptions(string value = "", IStringLocalizer localizer = null)
        {
            return "<option value=\"0\"" + ("0".EqualsIgnoreCase(value) ? " selected=\"selected\"" : "") + ">" + (localizer == null ? "普通按钮" : localizer["ButtonType_Normal"]) + "</option>"
                 + "<option value=\"1\"" + ("1".EqualsIgnoreCase(value) ? " selected=\"selected\"" : "") + ">" + (localizer == null ? "列表按钮" : localizer["ButtonType_List"]) + "</option>"
                 + "<option value=\"2\"" + ("2".EqualsIgnoreCase(value) ? " selected=\"selected\"" : "") + ">" + (localizer == null ? "工具栏按钮" : localizer["ButtonType_Toolbar"]) + "</option>";
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCache()
        {
            Cache.IO.Remove(CacheKey);
        }
    }
}
