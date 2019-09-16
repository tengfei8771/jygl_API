using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using RoadFlow.Utility;

namespace RoadFlow.Business
{
    public class Dictionary
    {
        /// <summary>
        /// 值字段
        /// </summary>
        public enum ValueField
        {
            Id,
            Title,
            Code,
            Value,
            Other,
            Note
        }
        private readonly Data.Dictionary dictionaryData;
        public Dictionary()
        {
            dictionaryData = new Data.Dictionary();
        }
        /// <summary>
        /// 得到所有字典
        /// </summary>
        /// <returns></returns>
        public List<Model.Dictionary> GetAll()
        {
            return dictionaryData.GetAll();
        }
        /// <summary>
        /// 更新一个字典
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public int Update(Model.Dictionary dictionary)
        {
            return dictionaryData.Update(dictionary);
        }
        /// <summary>
        /// 更新一批字典
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public int Update(Model.Dictionary[] dictionarys)
        {
            return dictionaryData.Update(dictionarys);
        }
        /// <summary>
        /// 添加字典
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public int Add(Model.Dictionary dictionary)
        {
            return dictionaryData.Add(dictionary);
        }
        /// <summary>
        /// 删除一个字典及其所有下级
        /// </summary>
        /// <param name="id"></param>
        /// <returns>删除的字典集合</returns>
        public List<Model.Dictionary> Delete(Guid id)
        {
            var dict = Get(id);
            if (null == dict)
            {
                return new List<Model.Dictionary>();
            }
            var allChilds = GetAllChilds(id);
            allChilds.Add(dict);
            dictionaryData.Delete(allChilds.ToArray());
            return allChilds;
        }
        /// <summary>
        /// 得到字典根ID
        /// </summary>
        /// <returns></returns>
        public Guid GetRootId()
        {
            var root = GetAll().Find(p => p.ParentId == Guid.Empty);
            return null == root ? Guid.Empty : root.Id;
        }
        /// <summary>
        /// 根据ID查询一个字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Dictionary Get(Guid id)
        {
            return GetAll().Find(p => p.Id == id);
        }
        /// <summary>
        /// 根据代码查询一个字典
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Model.Dictionary Get(string code)
        {
            return GetAll().Find(p => p.Code.EqualsIgnoreCase(code));
        }
        
        /// <summary>
        /// 根据ID得到下级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.Dictionary> GetChilds(Guid id)
        {
            return GetAll().FindAll(p => p.ParentId == id).OrderBy(p => p.Sort).ToList();
        }
        /// <summary>
        /// 根据代码得到下级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Model.Dictionary> GetChilds(string code)
        {
            var dict = Get(code);
            return null == dict ? new List<Model.Dictionary>() : GetAll().FindAll(p => p.ParentId == dict.Id).OrderBy(p => p.Sort).ToList();
        }
        /// <summary>
        /// 根据ID得到所有下级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">是否包含自己</param>
        /// <returns></returns>
        public List<Model.Dictionary> GetAllChilds(Guid id, bool isMe = false)
        {
            List<Model.Dictionary> dictionaries = new List<Model.Dictionary>();
            var dict = Get(id);
            if (null == dict)
            {
                return dictionaries;
            }
            if (isMe)
            {
                dictionaries.Add(dict);
            }
            AddChilds(dict, dictionaries);
            return dictionaries;
        }
        /// <summary>
        /// 根据代码得到所有下级
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">是否包含自己</param>
        /// <returns></returns>
        public List<Model.Dictionary> GetAllChilds(string code, bool isMe = false)
        {
            var dict = Get(code);
            return null == dict ? new List<Model.Dictionary>() : GetAllChilds(dict.Id, isMe);
        }
        /// <summary>
        /// 递归添加下级
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="dictionaries"></param>
        private void AddChilds(Model.Dictionary dictionary, List<Model.Dictionary> dictionaries)
        {
            var childs = GetChilds(dictionary.Id);
            if (childs.Count == 0)
            {
                return;
            }
            foreach (Model.Dictionary child in childs)
            {
                dictionaries.Add(child);
                AddChilds(child, dictionaries);
            }
        }
        /// <summary>
        /// 得到字段所有上级
        /// </summary>
        /// <param name="id">字典ID</param>
        /// <param name="isMe">是否包含自己</param>
        /// <param name="rootId">根id</param>
        /// <returns></returns>
        public List<Model.Dictionary> GetAllParent(Guid id, bool isMe = true, string rootId = "")
        {
            List<Model.Dictionary> dictionaries = new List<Model.Dictionary>();
            var dict = Get(id);
            if (null != dict)
            {
                Guid rId = rootId.IsGuid(out Guid rootGuid) ? rootGuid : GetRootId();
                if (isMe && dict.Id != rId)
                {
                    dictionaries.Add(dict);
                }
                AddParents(dict, dictionaries, rId);
            }
            return dictionaries;
        }
        /// <summary>
        /// 递归添加上级
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="dictionaries"></param>
        private void AddParents(Model.Dictionary dictionary, List<Model.Dictionary> dictionaries, Guid rootId)
        {
            var parent = Get(dictionary.ParentId);
            if (null != parent && parent.Id != rootId)
            {
                dictionaries.Add(parent);
                AddParents(parent, dictionaries, rootId);
            }
        }
        /// <summary>
        /// 得到字典所有上级标题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">是否显示自己</param>
        /// <param name="isRoot">是否显示根</param>
        /// <param name="rootId">显示到上级的ID</param>
        /// <param name="reverse">是否倒置</param>
        /// <returns></returns>
        public string GetAllParentTitle(Guid id, bool isMe = true, bool isRoot = true, string rootId = "", bool reverse = true)
        {
            var parents = GetAllParent(id, isMe, rootId);
            StringBuilder stringBuilder = new StringBuilder();
            Guid rId = rootId.IsGuid(out Guid rootGuid) ? rootGuid : Guid.Empty;
            if (!reverse)
            {
                parents.Reverse();
            }
            foreach (var parent in parents)
            {
                if (!isRoot && parent.Id == rId)
                {
                    continue;
                }
                stringBuilder.Append(GetLanguageTitle(parent));
                stringBuilder.Append(reverse ? " / " : " \\ ");
            }
            return stringBuilder.ToString().TrimEnd(' ', reverse ? '/' : '\\', ' ');
        }
        
        /// <summary>
        /// 判断一个字典是否有下级字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasChilds(Guid id)
        {
            return GetAll().Exists(p => p.ParentId == id);
        }
        /// <summary>
        /// 检查唯一代码是否重复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns>true 没有重复 false 重复</returns>
        public bool CheckCode(Guid id, string code)
        {
            var dict = Get(code);
            return null == dict ? true : dict.Id == id;
        }
        /// <summary>
        /// 得到一个字典下级的最大排序号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetMaxSort(Guid id)
        {
            var childs = GetChilds(id);
            return childs.Count == 0 ? 5 : childs.Max(p => p.Sort) + 5;
        }
        /// <summary>
        /// 根据唯一代码得到ID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Guid GetIdByCode(string code)
        {
            var dict = Get(code);
            return null == dict ? Guid.Empty : dict.Id;
        }
        /// <summary>
        /// 根据ID查询字典标题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="language">语言</param>
        /// <returns></returns>
        public string GetTitle(Guid id, string language = "")
        {
            var dict = Get(id);
            return null == dict ? "" : GetLanguageTitle(dict, language);
        }
        /// <summary>
        /// 根据Code查询字典标题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTitle(string code, string language = "")
        {
            var dict = Get(code);
            return null == dict ? "" : GetLanguageTitle(dict, language);
        }
        /// <summary>
        /// 根据ID字符串查询字典标题
        /// </summary>
        /// <param name="idString">逗号分开的id字符串</param>
        /// <returns></returns>
        public string GetTitles(string idString, string language = "")
        {
            if (idString.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string id in idString.Split(','))
            {
                if (id.IsGuid(out Guid guid))
                {
                    var dict = Get(guid);
                    if (null != dict)
                    {
                        stringBuilder.Append(GetLanguageTitle(dict, language));
                        stringBuilder.Append("、");
                    }
                }
            }
            return stringBuilder.ToString().TrimEnd('、');
        }
        /// <summary>
        /// 根据唯一代码和值查询标题
        /// </summary>
        /// <param name="code">唯一代码</param>
        /// <param name="value">值</param>
        /// <param name="language">语言</param>
        /// <returns></returns>
        public string GetTitle(string code, string value, string language = "")
        {
            var dicts = GetAllChilds(code);
            foreach (var dict in dicts)
            {
                if (dict.Value.Equals(value))
                {
                    return GetLanguageTitle(dict, language);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 根据id和值查询标题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value">值</param>
        /// <param name="language">语言</param>
        /// <returns></returns>
        public string GetTitle(Guid id, string value, string language = "")
        {
            var dicts = GetAllChilds(id);
            foreach (var dict in dicts)
            {
                if (dict.Value.Equals(value))
                {
                    return GetLanguageTitle(dict, language);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 得到所有下级ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isMe">是包含自己</param>
        /// <returns></returns>
        public List<Guid> GetAllChildsId(Guid id, bool isMe = true)
        {
            List<Guid> guids = new List<Guid>();
            if (isMe)
            {
                guids.Add(id);
            }
            var allChilds = GetAllChilds(id);
            foreach (var child in allChilds)
            {
                guids.Add(child.Id);
            }
            return guids;
        }
        /// <summary>
        /// 根据ID得到选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="valueField">值字段，默认id</param>
        /// <param name="value">默认值</param>
        /// <param name="isAllChild">是否显示所有下级</param>
        /// <param name="existsFlowType">是否包含流程分类（流程设计时选表单不能选择流程分类）</param>
        /// <returns></returns>
        public string GetOptionsByID(Guid id, ValueField valueField = ValueField.Id, string value = "", bool isAllChild = true, bool existsFlowType = true)
        {
            if (id.IsEmptyGuid())
            {
                return "";
            }
            var childs = isAllChild ? GetAllChilds(id) : GetChilds(id);
            StringBuilder options = new StringBuilder(childs.Count * 60);
            StringBuilder space = new StringBuilder();
            var flowChilds = existsFlowType ? new List<Model.Dictionary>() : GetAllChilds("system_applibrarytype_flow", true);//取出流程分类的下级，为了如果不包含流程分类时要排除
            foreach (var child in childs)
            {
                if (child.Status == 1)//标记为已删除的不作为选项
                {
                    continue;
                }
                if (!existsFlowType && flowChilds.Exists(p => p.Id == child.Id))//如果不包含流程分类要排除
                {
                    continue;
                }
                space.Clear();
                int parentCount = GetParentCount(childs, child);
                for (int i = 0; i < parentCount; i++)
                {
                    space.Append("&nbsp;&nbsp;");
                }
                if (parentCount > 0)
                {
                    space.Append("├");
                }
                string value1 = GetOptionValue(valueField, child);
                options.AppendFormat("<option value=\"{0}\"{1}>{2}{3}</option>", value1, value1.Equals(value) ? " selected=\"selected\"" : "", space.ToString(), GetLanguageTitle(child));
            }
            return options.ToString();
        }
        /// <summary>
        /// 根据唯一代码得到选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="valueField">值字段，默认id</param>
        /// <param name="value">默认值</param>
        /// <param name="isAllChild">是否显示所有下级</param>
        /// <param name="existsFlowType">>是否包含流程分类（流程设计时选表单不能选择流程分类）</param>
        /// <returns></returns>
        public string GetOptionsByCode(string code, ValueField valueField = ValueField.Id, string value = "", bool isAllChild = true, bool existsFlowType = true)
        {
            if (code.IsNullOrWhiteSpace())
            {
                return "";
            }
            var dict = Get(code);
            if (null == dict)
            {
                return "";
            }
            return GetOptionsByID(dict.Id, valueField, value, isAllChild, existsFlowType);
        }
        /// <summary>
        /// 得到一个字典项的上级节点数
        /// </summary>
        /// <param name="dictList"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        private int GetParentCount(List<Model.Dictionary> dictList, Model.Dictionary dict)
        {
            int parent = 0;
            Model.Dictionary parentDict = dictList.Find(p => p.Id == dict.ParentId);
            while (parentDict != null)
            {
                parentDict = dictList.Find(p => p.Id == parentDict.ParentId);
                parent++;
            }
            return parent;
        }
        /// <summary>
        /// 得到选项值
        /// </summary>
        /// <param name="valueField"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private string GetOptionValue(ValueField valueField, Model.Dictionary dictionary)
        {
            string value = string.Empty;
            switch (valueField)
            {
                case ValueField.Id:
                    value = dictionary.Id.ToString();
                    break;
                case ValueField.Code:
                    value = dictionary.Code;
                    break;
                case ValueField.Note:
                    value = dictionary.Note;
                    break;
                case ValueField.Other:
                    value = dictionary.Other;
                    break;
                case ValueField.Title:
                    value = GetLanguageTitle(dictionary, Tools.GetCurrentLanguage());
                    break;
                case ValueField.Value:
                    value = dictionary.Value;
                    break;
            }
            return value ?? string.Empty;
        }
        /// <summary>
        /// 得到radio选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="isAllChild"></param>
        /// <returns></returns>
        public string GetRadios(Guid id, string name, ValueField valueField = ValueField.Id, string value = "", string attr = "", bool isAllChild = false)
        {
            return GetRadioOrCheckBox(id, name, 0, valueField, value, attr, isAllChild);
        }
        /// <summary>
        /// 得到radio选项
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr"></param>
        /// <param name="isAllChild"></param>
        /// <returns></returns>
        public string GetRadiosByCode(string code, string name, ValueField valueField = ValueField.Id, string value = "", string attr = "", bool isAllChild = false)
        {
            var dict = Get(code);
            return dict == null ? string.Empty : GetRadioOrCheckBox(dict.Id, name, 0, valueField, value, attr, isAllChild);
        }
        /// <summary>
        /// 得到checkbox选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="isAllChild"></param>
        /// <returns></returns>
        public string GetCheckboxs(Guid id, string name, ValueField valueField = ValueField.Id, string value = "", string attr = "", bool isAllChild = false)
        {
            return GetRadioOrCheckBox(id, name, 1, valueField, value, attr, isAllChild);
        }
        /// <summary>
        /// 得到checkbox选项
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr"></param>
        /// <param name="isAllChild"></param>
        /// <returns></returns>
        public string GetCheckboxs(string code, string name, ValueField valueField = ValueField.Id, string value = "", string attr = "", bool isAllChild = false)
        {
            var dict = Get(code);
            return dict == null ? string.Empty : GetRadioOrCheckBox(dict.Id, name, 1, valueField, value, attr, isAllChild);
        }
        /// <summary>
        /// 得到radio或checkbox选项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type">0 radio 1 checkbox</param>
        /// <param name="valueField"></param>
        /// <param name="value"></param>
        /// <param name="attr"></param>
        /// <param name="isAllChild"></param>
        /// <returns></returns>
        private string GetRadioOrCheckBox(Guid id, string name, int type, ValueField valueField = ValueField.Id, string value = "", string attr = "", bool isAllChild = false)
        {
            if (id.IsEmptyGuid())
            {
                return "";
            }
            var childs = isAllChild ? GetAllChilds(id) : GetChilds(id);
            StringBuilder options = new StringBuilder(childs.Count * 60);
            foreach (var child in childs)
            {
                string value1 = GetOptionValue(valueField, child);
                options.Append("<input type=\"" + (0 == type ? "radio" : "checkbox") + "\" value=\"" + value1 + "\"");
                if (value1.Equals(value))
                {
                    options.Append(" checked=\"checked\"");
                }
                options.Append(" id=\"" + name + "_" + child.Id.ToString("N") + "\" name=\"" + name + "\"");
                if (!attr.IsNullOrWhiteSpace())
                {
                    options.Append(" " + attr);
                }
                options.Append(" style=\"vertical-align:middle\"/>");
                options.Append("<label style=\"vertical-align:middle\" for=\"" + name + "_" + child.Id.ToString("N") + "\">" + GetLanguageTitle(child) + "</label>");
            }
            return options.ToString();
        }

        /// <summary>
        /// 得到多语言标题
        /// </summary>
        /// <param name="dictionary">字典实体</param>
        /// <param name="language">语言</param>
        /// <returns></returns>
        public string GetLanguageTitle(Model.Dictionary dictionary, string language = "")
        {
            string title = string.Empty;
            if(null == dictionary)
            {
                return title;
            }
            if (language.IsNullOrWhiteSpace())
            {
                language = Tools.GetCurrentLanguage();
            }
            switch (language)
            {
                case "en-US":
                    title = dictionary.Title_en;
                    break;
                case "zh":
                    title = dictionary.Title_zh;
                    break;
                default:
                    title = dictionary.Title;
                    break;
            }
            if (title.IsNullOrWhiteSpace())
            {
                title = dictionary.Title;
            }
            return title;
        }

        /// <summary>
        /// 得到导出的JSON字符串
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetExportString(string id)
        {
            if (!id.IsGuid(out Guid dictId))
            {
                return string.Empty;
            }
            var dicts = GetAllChilds(dictId, true);
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (var dict in dicts)
            {
                jArray.Add(Newtonsoft.Json.Linq.JObject.FromObject(dict));
            }
            return jArray.ToString();
        }

        /// <summary>
        /// 导入数据字典
        /// </summary>
        /// <param name="json"></param>
        public string Import(string json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return "要导入的json为空!";
            }
            Newtonsoft.Json.Linq.JArray jArray = null;
            try
            {
                jArray = Newtonsoft.Json.Linq.JArray.Parse(json);
            }
            catch
            {
                jArray = null;
            }
            if (null == jArray)
            {
                return "json解析错误!";
            }
            foreach (Newtonsoft.Json.Linq.JObject jObject in jArray)
            {
                Model.Dictionary dictionaryModel = jObject.ToObject<Model.Dictionary>();
                if (null == dictionaryModel)
                {
                    continue;
                }
                if (Get(dictionaryModel.Id) != null)
                {
                    Update(dictionaryModel);
                }
                else
                {
                    Add(dictionaryModel);
                }
            }
            return "1";
        }
    }
}
