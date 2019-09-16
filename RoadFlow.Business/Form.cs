using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using RoadFlow.Utility;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.IO;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{
    public class Form
    {
        private readonly Data.Form formData;
        public Form()
        {
            formData = new Data.Form();
        }
        /// <summary>
        /// 得到所有表单
        /// </summary>
        /// <returns></returns>
        public List<Model.Form> GetAll()
        {
            return formData.GetAll();
        }
        /// <summary>
        /// 查询一个表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Form Get(Guid id)
        {
            return formData.Get(id);
        }
        /// <summary>
        /// 添加一个表单
        /// </summary>
        /// <param name="form">表单实体</param>
        /// <returns></returns>
        public int Add(Model.Form form)
        {
            return formData.Add(form);
        }
        /// <summary>
        /// 更新表单
        /// </summary>
        /// <param name="form">表单实体</param>
        public int Update(Model.Form form)
        {
            return formData.Update(form);
        }
        /// <summary>
        /// 删除表单同时删除应用程序库中记录
        /// </summary>
        /// <param name="form">表单实体</param>
        /// <param name="delete">是否彻底删除 0不 1彻底删除</param>
        /// <returns></returns>
        public int DeleteAndApplibrary(Model.Form form, int delete = 0)
        {
            return formData.Delete(form, new AppLibrary().GetByCode(form.Id.ToString()), delete);
        }
        /// <summary>
        /// 查询一页数据
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="userId">当前人员ID</param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="order"></param>
        /// <param name="status">状态，默认为查询没有删除的表单</param>
        /// <returns></returns>
        public DataTable GetPagerList(out int count, int size, int number, Guid userId, string name, string type, string order, int status = -1)
        {
            return formData.GetPagerList(out count, size, number, userId, name, type, order, status);
        }

        /// <summary>
        /// 根据SQL得到下拉选项
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="sql"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetOptionsBySQL(string connId, string sql, string value)
        {
            if (!connId.IsGuid(out Guid cid) || sql.IsNullOrWhiteSpace())
            {
                return "";
            }
            DbConnection dbConnection = new DbConnection();
            DataTable dt;
            try
            {
                dt = dbConnection.GetDataTable(cid, Wildcard.Filter(sql.FilterSelectSql()));
            }
            catch
            {
                dt = new DataTable();
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {
                string value1 = string.Empty;
                string title = string.Empty;
                if (dt.Columns.Count > 0)
                {
                    value1 = dr[0].ToString();
                }
                if (dt.Columns.Count > 1)
                {
                    title = dr[1].ToString();
                }
                else
                {
                    title = value1;
                }
                if (value1.IsNullOrWhiteSpace() || title.IsNullOrWhiteSpace())
                {
                    continue;
                }
                stringBuilder.Append("<option value=\"" + value1 + "\"" + (value1.EqualsIgnoreCase(value) ? " select=\"select\"" : "") + ">" + title + "</option>");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 根据URL得到下拉选项
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetOptionsByUrl(string url)
        {
            url = Wildcard.Filter(url);
            if (!url.ContainsIgnoreCase("http")
                && !url.ContainsIgnoreCase("https"))
            {
                url = Tools.GetHttpHost() + url;
            }
            return HttpHelper.HttpGet(url);
        }

        /// <summary>
        /// 根据字符串表达式得到选项
        /// </summary>
        /// <param name="expression">选项文本1,选项值1;选项文本2,选项值2</param>
        /// <returns></returns>
        public string GetOptionsByStringExpression(string expression, string defaultValue = "")
        {
            if (expression.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
            string[] array = expression.Split(';');
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string arr in array)
            {
                if (arr.IsNullOrWhiteSpace())
                {
                    continue;
                }
                string[] array1 = arr.Split(',');
                string title = array1[0];
                string value = array1.Length > 1 ? array1[1] : title;
                stringBuilder.Append("<option value=\"" + value + "\"" + (value.Equals(defaultValue) ? " selected='selected'" : "") + ">"
                    + title + "</option>");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 得到radio或checkbox选项
        /// </summary>
        /// <param name="source">source 来源：0 数据字典,1 字符串,2 sql,3 url</param>
        /// <param name="connId">connId 来源为sql时的连接ID</param>
        /// <param name="dictId">dictId 来源为dict时的字典ID</param>
        /// <param name="dictValueField">dictValueField 来源为dict时的值字段</param>
        /// <param name="text">text 来源为sql或url时的sql语句或url地址</param>
        /// <param name="defaultValue">defaultValue 默认值otherAttr</param>
        /// <param name="type">type radio或checkbox</param>
        /// <param name="name">name radio或checkbox的名称</param>
        /// <param name="otherAttr">otherAttr 其它属性</param>
        /// <returns></returns>
        public string GetRadioOrCheckboxHtml(int source, string connId, string dictId, string dictValueField, string text, string defaultValue, string type, string name, string otherAttr)
        {
            if (3 == source)//url直接返回URL内容
            {
                return GetOptionsByUrl(Wildcard.Filter(text));
            }
            Dictionary dictionary = new Dictionary();
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            switch (source)
            {
                case 0://数据字典
                    var childs = new Dictionary().GetChilds(dictId.ToGuid());
                    foreach (var child in childs)
                    {
                        string value = dictionary.GetLanguageTitle(child);
                        string key = string.Empty;
                        switch (dictValueField)
                        {
                            case "id":
                                key = child.Id.ToString();
                                break;
                            case "code":
                                key = child.Code;
                                break;
                            case "value":
                                key = child.Value;
                                break;
                            case "title":
                                key = dictionary.GetLanguageTitle(child);
                                break;
                            case "note":
                                key = child.Note;
                                break;
                            case "other":
                                key = child.Other;
                                break;
                        }
                        dicts.Add(key, value);
                    }
                    break;
                case 1://字符串
                    foreach (string str in text.Split(';'))
                    {
                        if (str.IsNullOrWhiteSpace())
                        {
                            continue;
                        }
                        string[] str1 = str.Split(',');
                        string key = string.Empty, value = string.Empty;
                        if (str1.Length > 0)
                        {
                            value = str1[0];
                        }
                        if (str1.Length > 1)
                        {
                            key = str1[1];
                        }
                        else
                        {
                            key = value;
                        }
                        if (!key.IsNullOrWhiteSpace() && !value.IsNullOrWhiteSpace())
                        {
                            dicts.Add(key, value);
                        }
                    }
                    break;
                case 2://SQL
                    DbConnection dbConnection = new DbConnection();
                    if (connId.IsGuid(out Guid cid))
                    {
                        DataTable dt = dbConnection.GetDataTable(cid, Wildcard.Filter(text.FilterSelectSql()));
                        foreach (DataRow dr in dt.Rows)
                        {
                            string key = string.Empty, value = string.Empty;
                            if (dt.Columns.Count > 0)
                            {
                                key = dr[0].ToString();
                            }
                            if (dt.Columns.Count > 1)
                            {
                                value = dr[1].ToString();
                            }
                            else
                            {
                                value = key;
                            }
                            if (!key.IsNullOrWhiteSpace() && !value.IsNullOrWhiteSpace())
                            {
                                dicts.Add(key, value);
                            }
                        }
                    }
                    break;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var dict in dicts)
            {
                stringBuilder.Append("<input type=\"" + type + "\" style=\"vertical-align:middle;\" value=\"" + dict.Key + "\" name=\"" + name + "\" id=\"" + name + "_" + dict.Key + "\"");
                if (!defaultValue.IsNullOrWhiteSpace() && ("," + defaultValue + ",").ContainsIgnoreCase("," + dict.Key + ","))
                {
                    stringBuilder.Append(" checked=\"checked\"");
                }
                stringBuilder.Append(" " + otherAttr);
                stringBuilder.Append("/>");
                stringBuilder.Append("<label style=\"vertical-align:middle;\" for=\"" + name + "_" + dict.Key + "\">" + dict.Value + "</label>");
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 得到联动的下级选项
        /// </summary>
        /// <param name="source">来源：dict,sql,url</param>
        /// <param name="connId">来源为sql时的连接ID</param>
        /// <param name="text">来源为sql或url时的sql语句或url地址</param>
        /// <param name="parentValue">上级值</param>
        /// <param name="dictValueField">来源为dict时的值字段</param>
        /// <param name="dictId">来源为dict时的字典ID</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="dictIschild">数据字典是否加载下级</param>
        /// <returns></returns>
        public string GetChildOptions(string source, string connId, string text, string parentValue, string dictValueField, string dictId, string defaultValue, bool dictIschild)
        {
            if (parentValue.IsNullOrWhiteSpace())
            {
                return "";
            }
            switch (source)
            {
                case "dict":
                    Dictionary dictionary = new Dictionary();
                    Guid parentId = parentValue.ToGuid();
                    Dictionary.ValueField valueField = Dictionary.ValueField.Id;
                    switch (dictValueField)
                    {
                        case "code":
                            valueField = Dictionary.ValueField.Code;
                            if (!dictValueField.EqualsIgnoreCase("id"))
                            {
                                var child = dictionary.GetChilds(dictId.ToGuid()).Find(p => p.Code.EqualsIgnoreCase(parentValue));
                                if (null != child)
                                {
                                    parentId = child.Id;
                                }
                            }
                            break;
                        case "value":
                            valueField = Dictionary.ValueField.Value;
                            if (!dictValueField.EqualsIgnoreCase("id"))
                            {
                                var child = dictionary.GetChilds(dictId.ToGuid()).Find(p => p.Value.EqualsIgnoreCase(parentValue));
                                if (null != child)
                                {
                                    parentId = child.Id;
                                }
                            }
                            break;
                        case "title":
                            valueField = Dictionary.ValueField.Title;
                            if (!dictValueField.EqualsIgnoreCase("id"))
                            {
                                var child = dictionary.GetChilds(dictId.ToGuid()).Find(p => p.Title.EqualsIgnoreCase(parentValue));
                                if (null != child)
                                {
                                    parentId = child.Id;
                                }
                            }
                            break;
                        case "note":
                            valueField = Dictionary.ValueField.Note;
                            if (!dictValueField.EqualsIgnoreCase("id"))
                            {
                                var child = dictionary.GetChilds(dictId.ToGuid()).Find(p => p.Note.EqualsIgnoreCase(parentValue));
                                if (null != child)
                                {
                                    parentId = child.Id;
                                }
                            }
                            break;
                        case "other":
                            valueField = Dictionary.ValueField.Other;
                            if (!dictValueField.EqualsIgnoreCase("id"))
                            {
                                var child = dictionary.GetChilds(dictId.ToGuid()).Find(p => p.Other.EqualsIgnoreCase(parentValue));
                                if (null != child)
                                {
                                    parentId = child.Id;
                                }
                            }
                            break;
                    }
                    return dictionary.GetOptionsByID(parentId, valueField, defaultValue, dictIschild);
                case "sql":
                    return GetOptionsBySQL(connId, Wildcard.Filter(text.FilterSelectSql()), defaultValue);
                case "url":
                    return GetOptionsByUrl(Wildcard.Filter(text));
            }
            return "";
        }

        /// <summary>
        /// 查询表单数据
        /// </summary>
        /// <param name="dbConnectionModel"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public DataTable GetFormDataTable(Model.DbConnection dbConnectionModel, string tableName, string primaryKey, string instanceId)
        {
            return new DbConnection().GetDataTable(dbConnectionModel, tableName, primaryKey, instanceId);
        }

        /// <summary>
        /// 查询表单数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="taskId"></param>
        /// <param name="flowId"></param>
        /// <param name="formatJSON"></param>
        /// <param name="fieldStatusJSON"></param>
        /// <returns></returns>
        public string GetFormData(string connId, string tableName, string primaryKey, string instanceId, string stepId, string flowId, string formatJSON, out string fieldStatusJSON)
        {
            fieldStatusJSON = "[]";
            if (!connId.IsGuid(out Guid connGuid))
            {
                return "[]";
            }
            DbConnection dbConnection = new DbConnection();
            var dbconnectionModel = dbConnection.Get(connGuid);
            if (null == dbconnectionModel)
            {
                return "[]";
            }

            #region 查询字段状态
            List<Model.FlowRunModel.StepFieldStatus> stepFieldStatuses = new List<Model.FlowRunModel.StepFieldStatus>();
            if (flowId.IsGuid(out Guid flowGuid))
            {
                var flowrunModel = new Flow().GetFlowRunModel(flowGuid);
                if (null != flowrunModel)
                {
                    Guid stepGuid = stepId.IsGuid(out Guid sid) ? sid : flowrunModel.FirstStepId;

                    var stepModel = flowrunModel.Steps.Find(p => p.Id == stepGuid);
                    if (null != stepModel)
                    {
                        JArray fArray = new JArray();
                        stepFieldStatuses = stepModel.StepFieldStatuses;
                        foreach (var fieldStatus in stepFieldStatuses)
                        {
                            string[] fileds = fieldStatus.Field.Split('.');
                            if (fileds.Length != 3)
                            {
                                continue;
                            }
                            JObject fObject = new JObject
                            {
                                { "name", (fileds[1] + "-" + fileds[2]).ToUpper() },
                                { "status", fieldStatus.Status },
                                { "check", fieldStatus.Check }
                            };
                            fArray.Add(fObject);
                        }
                        fieldStatusJSON = fArray.ToString(Newtonsoft.Json.Formatting.None);
                    }
                }
            }
            else if (Tools.HttpContext.Request.Querys("programid").IsGuid(out Guid pId)) //生成程序获取字段状态
            {
                var programRunModel = new Program().GetRunModel(pId);
                if (null != programRunModel)
                {
                    var fieldStatusList = programRunModel.ProgramValidates;
                    JArray fArray = new JArray();
                    foreach (var fieldStatus in fieldStatusList)
                    {
                        JObject fObject = new JObject
                        {
                            { "name", (fieldStatus.TableName + "-" + fieldStatus.FieldName).ToUpper() },
                            { "status", fieldStatus.Status },
                            { "check", fieldStatus.Validate }
                        };
                        fArray.Add(fObject);
                    }
                    fieldStatusJSON = fArray.ToString(Newtonsoft.Json.Formatting.None);
                }
            }
            #endregion

            JArray formatArray = null;
            try
            {
                formatArray = JArray.Parse(formatJSON);
            }
            catch { }
            DataTable dataTable = GetFormDataTable(dbconnectionModel, tableName, primaryKey, instanceId);
            if (dataTable.Rows.Count == 0)
            {
                return "[]";
            }
            JArray jArray = new JArray();
            foreach (DataColumn column in dataTable.Columns)
            {
                //检查字段状态，如果是隐藏则不返回数据
                var fieldStatus = stepFieldStatuses.Find(p => p.Field.EqualsIgnoreCase(dbconnectionModel.Id + "." + tableName + "." + column.ColumnName));
                if (null != fieldStatus && fieldStatus.Status == 2)
                {
                    continue;
                }
                string name = (tableName + "-" + column.ColumnName).ToUpper();
                string value = dataTable.Rows[0][column.ColumnName].ToString();
                #region 格式化
                if (formatArray != null)
                {
                    foreach (JObject formatObject in formatArray)
                    {
                        if (name.Equals(formatObject.Value<string>("id")))
                        {
                            string type = formatObject.Value<string>("type");
                            string format = formatObject.Value<string>("format");
                            if (!format.IsNullOrWhiteSpace())
                            {
                                switch (type)
                                {
                                    case "datetime":
                                        value = value.IsDateTime(out DateTime dt) ? dt.ToString(format) : "";
                                        break;
                                }
                            }
                        }
                    }
                }
                #endregion
                JObject jObject = new JObject
                {
                    { "name",  name },
                    { "value", value }
                };
                jArray.Add(jObject);
            }
            return jArray.ToString(Newtonsoft.Json.Formatting.None);
        }

        /// <summary>
        /// 得到默认值JSON
        /// </summary>
        /// <param name="defaultJSON">[{'id':'RF_TEST-F1','value':'{<UserName>}'}]</param>
        /// <param name="fieldStatusJSON"></param>
        /// <returns></returns>
        public string GetDefaultValuesJSON(string defaultJSON, string fieldStatusJSON)
        {
            if (defaultJSON.IsNullOrWhiteSpace())
            {
                return "[]";
            }
            JArray defaultArray = null;
            try
            {
                defaultArray = JArray.Parse(defaultJSON);
            }
            catch
            {
                return "[]";
            }
            JArray statusArray = null;
            try
            {
                statusArray = JArray.Parse(fieldStatusJSON);
            }
            catch
            { }
            JArray valueArray = new JArray();
            foreach (JObject jObject in defaultArray)
            {
                string id = jObject.Value<string>("id");
                //检查字段状态，如果状态是2隐藏，则不显示值
                int status = 0;
                if (statusArray != null)
                {
                    foreach (JObject sObject in statusArray)
                    {
                        if (id.Equals(sObject.Value<string>("name")))
                        {
                            status = sObject.Value<string>("status").ToInt(0);
                        }
                    }
                }
                if (2 == status)
                {
                    continue;
                }
                JObject dObject = new JObject
                {
                    { "id", id },
                    { "value", Wildcard.Filter(jObject.Value<string>("value")) }
                };
                valueArray.Add(dObject);
            }

            return valueArray.ToString(Newtonsoft.Json.Formatting.None);
        }

        /// <summary>
        /// 替换标题表达式
        /// </summary>
        /// <param name="titleExpression"></param>
        /// <param name="tableName"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public string ReplaceTitleExpression(string titleExpression, string tableName, string instanceId, HttpRequest request)
        {
            titleExpression = Wildcard.Filter(titleExpression);
            List<string> keys = new List<string>();
            string[] array = titleExpression.Split('{');
            foreach (string key in array)
            {
                if (key.Contains("}"))
                {
                    keys.Add(key.Substring(0, key.IndexOf("}")));
                }
            }
            foreach (string key in keys)
            {
                string controlName = (tableName + "-" + key).ToUpper();
                string value = request.Forms(controlName);
                string serialnumber_config = request.Form["rf_serialnumber_config_" + controlName];
                //流水号要从数据表中查询
                if ((!serialnumber_config.IsNullOrWhiteSpace() || value.IsNullOrEmpty()) && !instanceId.IsNullOrWhiteSpace() && !tableName.IsNullOrWhiteSpace())
                {
                    string form_dbconnid = request.Form["form_dbconnid"];
                    string form_dbtableprimarykey = request.Form["form_dbtableprimarykey"];
                    var formData = GetFormDataTable(new DbConnection().Get(form_dbconnid.ToGuid()), tableName, form_dbtableprimarykey, instanceId);
                    if (formData.Rows.Count > 0)
                    {
                        value = formData.Rows[0][key].ToString();
                    }
                }         
                titleExpression = titleExpression.ReplaceIgnoreCase("{" + key + "}", value);
            }
            return titleExpression;
        }

        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <returns>业务表主键值, 错误信息</returns>
        /// <param name="request">当前请求</param>
        /// <param name="localizer">语言包</param>
        public (string, string) SaveData(HttpRequest request, IStringLocalizer localizer = null)
        {
            string form_dbconnid = request.Forms("form_dbconnid");
            string form_dbtable = request.Forms("form_dbtable");
            string form_dbtableprimarykey = request.Forms("form_dbtableprimarykey");
            string form_instanceid = request.Forms("form_instanceid");
            string form_fieldstatus = request.Forms("form_fieldstatus");

            //如果是自定义表单iframe加载的,不在这里保存
            if ("1".Equals(request.Forms("form_iscustomeform")))
            {
                return (form_instanceid, string.Empty);
            }

            if (form_instanceid.IsNullOrWhiteSpace())
            {
                form_instanceid = request.Querys("instanceid");
            }

            if (!form_dbconnid.IsGuid(out Guid connId))
            {
                return (string.Empty, localizer == null ? "连接ID为空" : localizer["Execute_SaveDataConnIdEmpty"]);
            }
            if (form_dbtable.IsNullOrWhiteSpace() || form_dbtableprimarykey.IsNullOrWhiteSpace())
            {
                return (string.Empty, localizer == null ? "表名为空" : localizer["Execute_SaveDataTableNameEmpty"]);
            }

            DbConnection dbConnection = new DbConnection();
            Model.DbConnection dbConnectionModel = dbConnection.Get(connId);
            if (null == dbConnectionModel)
            {
                return (string.Empty, localizer == null ? "未找到连接实体" : localizer["Execute_SaveDataConnModelNotFound"]);
            }
            var tableFields = dbConnection.GetTableFields(dbConnectionModel, form_dbtable);
            if (tableFields.Count == 0)
            {
                return (string.Empty, localizer == null ? "表没有字段" : localizer["Execute_SaveDataTableNotField"]);
            }
            JArray fieldStatusArray = null;
            try
            {
                fieldStatusArray = JArray.Parse(form_fieldstatus);
            }
            catch { }

            object newInstanceId = form_instanceid;//主键值
            bool isIdentity = false;
            string seqName = string.Empty;//数据库为oracle时的序列名称

            #region 保存主表 
            bool isAdd = form_instanceid.IsNullOrWhiteSpace();
            DataTable oldData = null;
            if (!isAdd)//如果不是新增，要查询旧数据，以便值为空时填充旧数据
            {
                oldData = GetFormDataTable(dbConnectionModel, form_dbtable, form_dbtableprimarykey, form_instanceid);
            }
            //dicts 字段名称，字段值字典，tableName 表名, primaryKey 主键, flag 0删除 1新增 2修改
            List<(Dictionary<string, object> dicts, string tableName, string primaryKey, int flag)> tuples = new List<(Dictionary<string, object> dicts, string tableName, string primaryKey, int flag)>();
            Dictionary<string, object> tableDict = new Dictionary<string, object>();
            foreach (var field in tableFields)
            {
                string controlName = (form_dbtable + "-" + field.FieldName).ToUpper();
                string value = request.Form[controlName];
                bool isPrimaryKey = form_dbtableprimarykey.EqualsIgnoreCase(field.FieldName);
                int fieldStatus = 0;//字段状态 0编辑 1只读 2隐藏
                if (fieldStatusArray != null)
                {
                    foreach (JObject jObject in fieldStatusArray)
                    {
                        if (jObject.Value<string>("name").Equals(controlName))
                        {
                            fieldStatus = jObject.Value<int>("status");
                            break;
                        }
                    }
                }
                if (isPrimaryKey && null == value)//如果是主键
                {
                    #region 主键
                    if (!isAdd)//如果是修改则直接添加主键值
                    {
                        tableDict.Add(field.FieldName, newInstanceId);
                        continue;
                    }

                    if (field.Type.EqualsIgnoreCase("uniqueidentifier")
                        || field.Type.ContainsIgnoreCase("char"))//如果是字符串，则自动生成GUID
                    {
                        newInstanceId = Guid.NewGuid();
                        tableDict.Add(field.FieldName, newInstanceId);
                    }
                    //由于oracle无法判断列是否是使用了序列，所以这里如果是主键并且数据类型是数字，则认为是自增类型
                    else if (field.IsIdentity || field.Type.ContainsIgnoreCase("number")
                        || field.Type.ContainsIgnoreCase("int")
                        || field.Type.ContainsIgnoreCase("long"))
                    {
                        isIdentity = true;
                        seqName = form_dbtable + "_SEQ";//oracle规定序列名称为表名_SEQ
                    }
                    continue;
                    #endregion
                }
                else
                {
                    #region 流水号
                    if (value.IsNullOrEmpty() && fieldStatus == 0 && ("," + request.Forms("rf_serialnumber") + ",").Contains("," + controlName + ","))
                    {
                        string serialnumber_config = request.Forms("rf_serialnumber_config_" + controlName);
                        if (!serialnumber_config.IsNullOrWhiteSpace())
                        {
                            var (searialNumber, maxField, maxValue) = GetSerialNumber(dbConnectionModel, form_dbtable, serialnumber_config);
                            tableDict.Add(field.FieldName, searialNumber);
                            if (!maxField.IsNullOrWhiteSpace())
                            {
                                tableDict.Add(maxField, maxValue);
                            }
                            continue;
                        }
                    }
                    #endregion

                    if (fieldStatus != 0)
                    {
                        continue;
                    }

                    if (tableDict.ContainsKey(field.FieldName))
                    {
                        continue;
                    }

                    if (field.Type.ContainsIgnoreCase("date"))//如果是日期时间要验证数据
                    {
                        if (value.IsDateTime(out DateTime dt))
                        {
                            tableDict.Add(field.FieldName, dt);
                        }
                        else
                        {
                            if (field.IsNull && !field.IsDefault)
                            {
                                tableDict.Add(field.FieldName, DBNull.Value);
                            }
                        }
                    }
                    else
                    {
                        if (null == value)
                        {
                            if (oldData != null && oldData.Rows.Count > 0)
                            {
                                tableDict.Add(field.FieldName, oldData.Rows[0][field.FieldName]);
                            }
                            else if (field.IsNull && !field.IsDefault)
                            {
                                tableDict.Add(field.FieldName, DBNull.Value);
                            }
                        }
                        else if (value.IsNullOrWhiteSpace() && field.IsNull && !field.IsDefault)//如果值是空字符串，数据表字段又可为空，则设置为NULL
                        {
                            tableDict.Add(field.FieldName, DBNull.Value);
                        }
                        else if (value.IsNullOrWhiteSpace() && !field.IsNull && field.IsDefault)//如果值是空字符串，数据表字段不能为空，又设置了默认值，则不添加，采用默认值
                        {
                            continue;
                        }
                        else
                        {
                            tableDict.Add(field.FieldName, value);
                        }
                    }

                }
            }
            if (tableDict.Any(p => !p.Key.EqualsIgnoreCase(form_dbtableprimarykey)))
            {
                tuples.Add((tableDict, form_dbtable, form_dbtableprimarykey, isAdd ? 1 : 2));//1新增 2修改
            }
            #endregion

            #region 保存子表
            string SUBTABLE_id = request.Forms("SUBTABLE_id");
            if (!SUBTABLE_id.IsNullOrEmpty())
            {
                if (isAdd && isIdentity)//如果主表是自增，这里要先得到值
                {
                    string identityValue = dbConnection.SaveData(dbConnectionModel, tuples, true, seqName);
                    if (identityValue.IsInt())
                    {
                        newInstanceId = identityValue;
                    }
                    else
                    {
                        return (string.Empty, identityValue);
                    }
                    tuples.Clear();
                }
                foreach (var subtableId in SUBTABLE_id.Split(','))
                {
                    string secondtable = request.Forms("SUBTABLE_" + subtableId + "_secondtable");
                    string primarytablefiled = request.Forms("SUBTABLE_" + subtableId + "_primarytablefiled");
                    string secondtableprimarykey = request.Forms("SUBTABLE_" + subtableId + "_secondtableprimarykey");
                    string secondtablerelationfield = request.Forms("SUBTABLE_" + subtableId + "_secondtablerelationfield");
                    if (secondtable.IsNullOrWhiteSpace() || primarytablefiled.IsNullOrWhiteSpace()
                        || secondtableprimarykey.IsNullOrWhiteSpace() || secondtablerelationfield.IsNullOrWhiteSpace())
                    {
                        continue;
                    }
                    DataTable subtableDt = dbConnection.GetDataTable(dbConnectionModel, secondtable, secondtablerelationfield, newInstanceId.ToString());
                    var subtableFields = dbConnection.GetTableFields(dbConnectionModel, secondtable);
                    string[] rowIndexs = request.Forms("SUBTABLE_" + subtableId + "_rowindex").Split(',');
                    foreach (string rowIndex in rowIndexs)
                    {
                        if (rowIndex.IsNullOrWhiteSpace())
                        {
                            continue;
                        }
                        bool subtableIsAdd = subtableDt.Rows.Count == 0;
                        if (!subtableIsAdd)
                        {
                            try
                            {
                                subtableIsAdd = subtableDt.Select(secondtableprimarykey + "='" + rowIndex + "'").Length == 0;
                            }
                            catch
                            {
                                subtableIsAdd = true;
                            }
                        }
                        Dictionary<string, object> subtableDict = new Dictionary<string, object>();
                        foreach (var subtableField in subtableFields)
                        {
                            bool subTableIsPrimaryKey = secondtableprimarykey.EqualsIgnoreCase(subtableField.FieldName);
                            string value = request.Form[("SUBTABLE_" + subtableId + "_" + subtableField.FieldName + "_" + rowIndex).ToUpper()];
                            int subfieldStatus = 0;//字段状态
                            if (fieldStatusArray != null)
                            {
                                foreach (JObject jObject in fieldStatusArray)
                                {
                                    if (jObject.Value<string>("name").Equals((secondtable + "-" + subtableField.FieldName).ToUpper()))
                                    {
                                        subfieldStatus = jObject.Value<int>("status");
                                        break;
                                    }
                                }
                            }
                            if (null == value && secondtablerelationfield.EqualsIgnoreCase(subtableField.FieldName))//关联字段
                            {
                                subtableDict.Add(subtableField.FieldName, newInstanceId);
                            }
                            else if (null == value && subTableIsPrimaryKey)//如果是主键
                            {
                                if (!subtableIsAdd)//如果是修改，直接赋值
                                {
                                    subtableDict.Add(subtableField.FieldName, rowIndex);
                                }
                                else if (subtableField.Type.EqualsIgnoreCase("uniqueidentifier")
                                    || subtableField.Type.ContainsIgnoreCase("char"))//如果是字符串，则自动生成GUID
                                {
                                    subtableDict.Add(subtableField.FieldName, Guid.NewGuid());
                                }
                            }
                            else
                            {
                                if (subfieldStatus != 0)
                                {
                                    continue;
                                }
                                if (subtableDict.ContainsKey(subtableField.FieldName))
                                {
                                    continue;
                                }
                                if (subtableField.Type.ContainsIgnoreCase("date"))//如果是日期时间要验证数据
                                {
                                    if (value.IsDateTime(out DateTime dt))
                                    {
                                        subtableDict.Add(subtableField.FieldName, dt);
                                    }
                                    else
                                    {
                                        if (subtableField.IsNull && !subtableField.IsDefault)
                                        {
                                            subtableDict.Add(subtableField.FieldName, DBNull.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (null == value)
                                    {
                                        if (subtableField.IsNull && !subtableField.IsDefault)
                                        {
                                            subtableDict.Add(subtableField.FieldName, DBNull.Value);
                                        }
                                    }
                                    else if (value.IsNullOrWhiteSpace() && subtableField.IsNull && !subtableField.IsDefault)//如果值是空字符串，数据表字段又可为空，则设置为NULL
                                    {
                                        subtableDict.Add(subtableField.FieldName, DBNull.Value);
                                    }
                                    else
                                    {
                                        subtableDict.Add(subtableField.FieldName, value);
                                    }
                                }

                            }
                        }
                        if (subtableDict.Any(p => !p.Key.EqualsIgnoreCase(secondtableprimarykey)))
                        {
                            tuples.Add((subtableDict, secondtable, secondtableprimarykey, subtableIsAdd ? 1 : 2));
                        }
                    }
                    //标记要删除的行
                    foreach (DataRow dr in subtableDt.Rows)
                    {
                        bool isIn = rowIndexs.Contains(dr[secondtableprimarykey].ToString(), StringComparer.CurrentCultureIgnoreCase);
                        if (!isIn)
                        {
                            Dictionary<string, object> subtableDict = new Dictionary<string, object>
                            {
                                { secondtableprimarykey, dr[secondtableprimarykey].ToString() }
                            };
                            tuples.Add((subtableDict, secondtable, secondtableprimarykey, 0));
                        }
                    }
                }
            }
            #endregion

            string returnValue = dbConnection.SaveData(dbConnectionModel, tuples, isIdentity, seqName);
            int returnId = returnValue.IsInt(out int id) ? id : -1;
            if ((newInstanceId == null || newInstanceId.ToString().IsNullOrWhiteSpace()) && returnId != -1)
            {
                newInstanceId = returnId.ToString();
            }
            string errMsg = returnId == -1 ? returnValue : string.Empty;
            return (newInstanceId.ToString().ToUpper(), errMsg);
        }

        /// <summary>
        /// 得到流水号
        /// </summary>
        /// <param name="dbConnectionModel">连接实体</param>
        /// <param name="tableName">表名</param>
        /// <param name="serialnumberConfig">流水号配置JSON字符串</param>
        /// <param name="maxSerialNumber">返回当前最大编号</param>
        /// <returns>searialNumber 当前流水号, maxField最大编号字段，maxValue最大编号</returns>
        public (string searialNumber, string maxField, int maxValue) GetSerialNumber(Model.DbConnection dbConnectionModel, string tableName, string serialnumberConfig)
        {
            JObject jObject = JObject.Parse(serialnumberConfig);
            string maxfiled = jObject.Value<string>("maxfiled");
            string length = jObject.Value<string>("length");
            string formatstring = jObject.Value<string>("formatstring");
            string sqlwhere = jObject.Value<string>("sqlwhere");
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT MAX(" + maxfiled + ") FROM " + tableName + " ");
            if (!sqlwhere.IsNullOrWhiteSpace())
            {
                string sqlwhere1 = Wildcard.Filter(sqlwhere.UrlDecode().Trim());
                if (sqlwhere1.StartsWith("where", StringComparison.CurrentCultureIgnoreCase))
                {
                    sql.Append(sqlwhere1);
                }
                else
                {
                    sql.Append("WHERE " + sqlwhere1);
                }
            }
            int max = 1;
            DataTable dt = new DbConnection().GetDataTable(dbConnectionModel, sql.ToString());
            if (dt.Rows.Count > 0)
            {
                max = dt.Rows[0][0].ToString().ToInt(0) + 1;
            }
            string number = max.ToString().PadLeft(length.ToInt(1), '0');
            number = Wildcard.Filter(formatstring).ReplaceIgnoreCase("{serialnumber}", number);
            return (number, maxfiled, max);
        }

        /// <summary>
        /// 得到一个表单所关联的表
        /// </summary>
        /// <param name="id"></param>
        /// <returns>(连接ID，表名，是否主表，主键，从表与主表关联字段)</returns>
        public List<(Guid, string, int, string, string)> GetFormTables(Guid id)
        {
            
            List<(Guid, string, int, string, string)> list = new List<(Guid, string, int, string, string)>();
            var formModel = Get(id);
            if (null == formModel)
            {
                return list;
            }
            JObject attrObject = null;
            try
            {
                attrObject = JObject.Parse(formModel.attribute);
            }
            catch
            {
               
            }
            Guid dbConnId = Guid.Empty;
            if (attrObject != null)
            {
                dbConnId = attrObject.Value<string>("dbConn").ToGuid();
                string dbTable = attrObject.Value<string>("dbTable");
                string dbTablePrimaryKey = attrObject.Value<string>("dbTablePrimaryKey");
                list.Add((dbConnId, dbTable, 1, dbTablePrimaryKey, string.Empty));
            }
            JArray subArray = null;
            try
            {
                subArray = JArray.Parse(formModel.SubtableJSON);
            }
            catch
            {
                
            }
            if (subArray != null)
            {
                foreach (JObject jObject in subArray)
                {
                    string secondtable = jObject.Value<string>("secondtable");
                    string secondtableprimarykey = jObject.Value<string>("secondtableprimarykey");
                    string secondtablerelationfield = jObject.Value<string>("secondtablerelationfield");
                    list.Add((dbConnId, secondtable, 0, secondtableprimarykey, secondtablerelationfield));
                }
            }
            
            return list;
        }

        /// <summary>
        /// 删除一个表单对应的数据
        /// </summary>
        /// <param name="formId">表单Id(RF_Form表Id)</param>
        /// <param name="instanceId">实例Id</param>
        /// <returns>返回1表单成功，其它为错误信息</returns>
        public string DeleteFormData(Guid formId, string instanceId)
        {
            List<(Guid, string, int, string, string)> tables = GetFormTables(formId);
            if (null == tables || tables.Count == 0)
            {
                return "表单未绑定表!";
            }
            List<(string, IEnumerable<object>)> sqlList = new List<(string, IEnumerable<object>)>();
            foreach (var (dbConnId, tableName, isPrimary, primaryKey, relField) in tables)
            {
                if (1 == isPrimary)
                {
                    string sql = "DELETE FROM " + tableName + " WHERE " + primaryKey + "={0}";
                    sqlList.Add((sql, new List<object>() { instanceId }));
                }
                else
                {
                    string sql = "DELETE FROM " + tableName + " WHERE " + relField + "={0}";
                    sqlList.Add((sql, new List<object>() { instanceId }));
                }
            }
            Guid connId = tables.First().Item1;
            string msg = new DbConnection().ExecuteSQL(connId, sqlList);
            return msg.IsInt() ? "1" : msg;
        }

        /// <summary>
        /// 得到列表显示数据字符串(从表显示子表列表的时候会用到)
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="showModel">显示类型</param>
        /// <param name="format">格式</param>
        /// <param name="sql">SQL（通过sql显示另一个表字段时）</param>
        /// <returns></returns>
        public string GetShowString(object value, string showModel, string format, string sql)
        {
            if (null == value)
            {
                return string.Empty;
            }
            string v = value.ToString();
            switch (showModel)
            {
                case "normal"://常规
                    return v;
                case "dict_id_title"://数据字典ID显示为标题
                    return v.IsGuid(out Guid dictionaryId) ? new Dictionary().GetTitle(dictionaryId) : string.Empty;
                case "dict_code_title"://数据字典代码显示为标题
                    return new Dictionary().GetTitle(v);
                case "dict_value_title"://数据字典值显示为标题
                case "dict_note_title"://数据字典备注显示为标题
                case "dict_other_title"://数据字典其它显示为标题
                    return format.IsGuid(out Guid parentId) ? new Dictionary().GetTitle(parentId, v) : new Dictionary().GetTitle(format, v);
                case "organize_id_name"://组织机构ID显示为名称
                    return new Organize().GetNames(v);
                case "files_link"://附件显示为连接
                    return v.ToFilesShowString(false);
                case "files_img"://附件显示为图片
                    return v.ToFilesImgString();
                case "datetime_format"://日期时间显示为指定格式
                    return v.IsDateTime(out DateTime dt) ? dt.ToString(format) : v;
                case "number_format"://数字显示为指定格式
                    return v.IsDecimal(out decimal d) ? d.ToString(format) : v;
                case "custom"://自定义
                    return Wildcard.Filter(v, null, value);
            }

            return v;
        }

        /// <summary>
        /// 得到表单导出的JSON字符串
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string GetExportFormString(string ids)
        {
            if (ids.IsNullOrWhiteSpace())
            {
                return "";
            }
            string[] formIds = ids.Split(',');
            JObject jObject = new JObject();
            JArray jArrayForm = new JArray();
            JArray jArrayApplibrary = new JArray();
            foreach (string formId in formIds)
            {
                if (!formId.IsGuid(out Guid fid))
                {
                    continue;
                }
                var formModel = Get(fid);
                if (null == formModel)
                {
                    continue;
                }
                jArrayForm.Add(JObject.FromObject(formModel));

                AppLibrary appLibrary = new AppLibrary();
                var appModel = appLibrary.GetByCode(formModel.Id.ToString());
                if (null == appModel)
                {
                    continue;
                }
                jArrayApplibrary.Add(JObject.FromObject(appModel));
            }
            jObject.Add("forms", jArrayForm);
            jObject.Add("applibrarys", jArrayApplibrary);
            return jObject.ToString();
        }

        /// <summary>
        /// 导入表单
        /// </summary>
        /// <param name="json"></param>
        /// <returns>返回1表示成功，其它为错误信息</returns>
        public string ImportForm(string json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return "要导入的JSON为空!";
            }
            JObject jObject = null;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch
            {
                return "json解析错误!";
            }
            var forms = jObject.Value<JArray>("forms");
            if (null != forms)
            {
                foreach (JObject formObject in forms)
                {
                    Model.Form formModel = formObject.ToObject<Model.Form>();
                    if (null == formModel)
                    {
                        continue;
                    }
                    if (Get(formModel.Id) != null)
                    {
                        Update(formModel);
                    }
                    else
                    {
                        Add(formModel);
                    }
                    //如果表单状态是发布，要发布表单
                    if (formModel.Status == 1)
                    {
                        #region 写入文件
                        string webRootPath = Tools.GetWebRootPath();
                        string path = webRootPath + "/RoadFlowResources/scripts/formDesigner/form/";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string file = path + formModel.Id + ".rfhtml";
                        Stream stream = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                        stream.SetLength(0);
                        StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
                        sw.Write(formModel.RunHtml);
                        sw.Close();
                        stream.Close();
                        #endregion
                    }
                }
            }

            //加入应用程序库
            var applibarys = jObject.Value<JArray>("applibrarys");
            AppLibrary appLibrary = new AppLibrary();
            if (null != applibarys)
            {
                foreach (JObject app in applibarys)
                {
                    Model.AppLibrary appLibraryModel = app.ToObject<Model.AppLibrary>();
                    if (null == appLibraryModel)
                    {
                        continue;
                    }
                    if (appLibrary.Get(appLibraryModel.Id) != null)
                    {
                        appLibrary.Update(appLibraryModel);
                    }
                    else
                    {
                        appLibrary.Add(appLibraryModel);
                    }
                }
            }
            return "1";
        }

        /// <summary>
        /// 得到表格HTML，表单设计控件的数据表格
        /// </summary>
        /// <param name="widht">表格宽度</param>
        /// <param name="height">表格高度</param>
        /// <param name="dataSource">数据来源 0 sql 1 url 2 方法</param>
        /// <param name="dataSourceText">数据来源字符串</param>
        /// <param name="connId">连接ID</param>
        /// <param name="formData">表单数据JSON</param>
        /// <returns></returns>
        public string GetDataTableHtml(string widht, string height, string dataSource, string dataSourceText, string connId, string formData)
        {
            if (!dataSource.IsInt(out int source) || source.NotIn(0, 1, 2) || dataSourceText.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }
           
            JArray jArray = null;
            try
            {
                jArray = JArray.Parse(formData);
            }
            catch
            {
                jArray = null;
            }

            dataSourceText = Wildcard.Filter(dataSourceText, null, jArray);
            #region URL
            if (source == 1)
            {
                string url = string.Empty;
                if (!dataSourceText.Trim1().StartsWith("http"))
                {
                    url = Tools.GetHttpHost() + dataSourceText;
                }
                else
                {
                    url = dataSourceText;
                }
                try
                {
                    return HttpHelper.HttpPost(url, "");
                }
                catch
                {
                    return string.Empty;
                }
            }
            #endregion

            #region 方法
            if (source == 2)
            {
                try
                {
                   (object, Exception) result = Tools.ExecuteMethod(dataSourceText, jArray);
                    if (result.Item2 == null && result.Item1 != null)
                    {
                        return result.Item1.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }
            #endregion

            #region SQL
            if (source == 0 && connId.IsGuid(out Guid connGuid))
            {
                StringBuilder tableHtml = new StringBuilder();
                DataTable dt;
                try
                {
                    dt = new DbConnection().GetDataTable(connGuid, dataSourceText);
                }
                catch
                {
                    return string.Empty;
                }
                tableHtml.Append("<table cellpadding=\"0\" cellspacing=\"1\" border=\"0\" style=\""
                    + (widht.IsNullOrWhiteSpace() ? "" : "width:" + widht + ";")
                    + (height.IsNullOrWhiteSpace() ? "" : "height:" + height + ";") + "\" class=\"formlisttable\">");
                tableHtml.Append("<thead><tr>");
                foreach (DataColumn col in dt.Columns)
                {
                    tableHtml.Append("<th>" + col.ColumnName + "</th>");
                }
                tableHtml.Append("</tr></thead>");
                tableHtml.Append("<tbody>");
                foreach (DataRow dr in dt.Rows)
                {
                    tableHtml.Append("<tr>");
                    foreach (DataColumn col in dt.Columns)
                    {
                        tableHtml.Append("<td>" + dr[col.ColumnName].ToString() + "</td>");
                    }
                    tableHtml.Append("</tr>");
                }
                tableHtml.Append("</tbody>");
                tableHtml.Append("</table>");
                return tableHtml.ToString();
            }
            #endregion
            return string.Empty;
        }
    }
}
