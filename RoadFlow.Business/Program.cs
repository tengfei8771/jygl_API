using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Microsoft.AspNetCore.Http;
using RoadFlow.Utility;
using RoadFlow.Data;
using System.Linq;
using System.Data;
using Microsoft.Extensions.Localization;

namespace RoadFlow.Business
{
    public class Program
    {
        private readonly Data.Program ProgramData;
        public Program()
        {
            ProgramData = new Data.Program();
        }
        /// <summary>
        /// 查询一个程序设计
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.Program Get(Guid id)
        {
            return ProgramData.Get(id);
        }
        /// <summary>
        /// 添加一个程序设计
        /// </summary>
        /// <param name="program">程序设计实体</param>
        /// <returns></returns>
        public int Add(Model.Program program)
        {
            return ProgramData.Add(program);
        }
        /// <summary>
        /// 更新程序设计
        /// </summary>
        /// <param name="program">程序设计实体</param>
        public int Update(Model.Program program)
        {
            return ProgramData.Update(program);
        }
        /// <summary>
        /// 查询一页程序设计
        /// </summary>
        /// <param name="count"></param>
        /// <param name="size"></param>
        /// <param name="number"></param>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public System.Data.DataTable GetPagerData(out int count, int size, int number, string name, string types, string order)
        {
            return ProgramData.GetPagerData(out count, size, number, name, types, order);
        }
        /// <summary>
        /// 发布一个程序设计
        /// </summary>
        /// <param name="id"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public string Publish(Guid id, IStringLocalizer localizer = null)
        {
            string key = "ProgramRun_" + id.ToString("N");
            Cache.IO.Remove(key);
            Model.Program programModel = Get(id);
            if (null == programModel)
            {
                return localizer == null ? "没有找到要发布的程序!" : localizer["NotFoundModel"];
            }
            //更新状态
            programModel.Status = 1;
            Update(programModel);

            Model.ProgramRun programRunModel = GetRunModel(id);
            if (null == programRunModel)
            {
                return localizer == null ? "没有找到要发布的程序!" : localizer["NotFoundModel"];
            }

            #region 加入应用程序库
            AppLibrary appLibrary = new AppLibrary();
            var appModel = appLibrary.GetByCode(id.ToString());
            bool isAdd = false;
            if (null == appModel)
            {
                isAdd = true;
                appModel = new Model.AppLibrary
                {
                    Id = Guid.NewGuid(),
                    Code = id.ToString()
                };
            }
            appModel.Address = "/RoadFlowCore/ProgramDesigner/Run?programid=" + id.ToString();
            appModel.OpenMode = 0;
            appModel.Title = programRunModel.Name;
            appModel.Type = programRunModel.Type;

            //多语言要写入不同语言的标题，这里只写一种
            appModel.Title_en = programRunModel.Name;
            appModel.Title_zh = programRunModel.Name;

            if (isAdd)
            {
                appLibrary.Add(appModel);
            }
            else
            {
                appLibrary.Update(appModel);
            }
            #endregion

            #region 加按钮到应用程序库
            AppLibraryButton appLibraryButton = new AppLibraryButton();
            var buttons = appLibraryButton.GetListByApplibraryId(appModel.Id);
            var programButtons = new ProgramButton().GetAll(id);
            foreach (var button in programButtons)
            {
                var model = buttons.Find(p => p.Id == button.Id);
                bool isAddButton = false;
                if (null == model)
                {
                    isAddButton = true;
                    model = new Model.AppLibraryButton
                    {
                        Id = button.Id
                    };
                }
                model.AppLibraryId = appModel.Id;
                model.ButtonId = button.ButtonId;
                model.Events = button.ClientScript;
                model.Ico = button.Ico;
                model.IsValidateShow = button.IsValidateShow;
                model.Name = button.ButtonName;
                model.ShowType = button.ShowType;
                model.Sort = button.Sort;
                if (isAddButton)
                {
                    appLibraryButton.Add(model);
                }
                else
                {
                    appLibraryButton.Update(model);
                }
            }
            foreach (var button in buttons)
            {
                if (!programButtons.Exists(p => p.Id == button.Id))
                {
                    appLibraryButton.Delete(button.Id);
                }
            }
            #endregion
            return "1";
        }
        /// <summary>
        /// 得到运行时实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        public Model.ProgramRun GetRunModel(Guid id, IStringLocalizer localizer = null)
        {
            string key = "ProgramRun_" + id.ToString("N");
            var obj = Cache.IO.Get(key);
            if (obj != null)
            {
                return (Model.ProgramRun)obj;
            }
            else
            {
                var programModel = Get(id);
                if (null == programModel)
                {
                    return null;
                }
                Model.ProgramRun programRunModel = new Model.ProgramRun
                {
                    ButtonLocation = programModel.ButtonLocation,
                    ClientScript = programModel.ClientScript,
                    ConnId = programModel.ConnId,
                    CreateTime = programModel.CreateTime,
                    CreateUserId = programModel.CreateUserId,
                    EditModel = programModel.EditModel,
                    ExportFileName = programModel.ExportFileName,
                    ExportHeaderText = programModel.ExportHeaderText,
                    ExportTemplate = programModel.ExportTemplate,
                    FormId = programModel.FormId,
                    Height = programModel.Height,
                    Id = programModel.Id,
                    InDataNumberFiledName = programModel.InDataNumberFiledName,
                    IsAdd = programModel.IsAdd,
                    IsPager = programModel.IsPager,
                    RowNumber = programModel.RowNumber,
                    Name = programModel.Name,
                    ProgramButtons = new ProgramButton().GetAll(programModel.Id),
                    ProgramExports = new ProgramExport().GetAll(programModel.Id),
                    ProgramFields = new ProgramField().GetAll(programModel.Id),
                    ProgramQueries = new ProgramQuery().GetAll(programModel.Id),
                    ProgramValidates = new ProgramValidate().GetAll(programModel.Id),
                    PublishTime = programModel.PublishTime,
                    SelectColumn = programModel.SelectColumn,
                    SqlString = programModel.SqlString,
                    Status = programModel.Status,
                    TableHead = programModel.TableHead,
                    TableStyle = programModel.TableStyle,
                    Type = programModel.Type,
                    Width = programModel.Width,
                    GroupHeaders = programModel.GroupHeaders
                };
                //取消在发布时获取HTML，因为要根据运行时登录人员来判断按钮权限
                //List<string> buttonHtml = GetButtonHtml(programRunModel);
                //programRunModel.Button_Toolbar = buttonHtml[0];
                //programRunModel.Button_Normal = buttonHtml[1];
                //programRunModel.Button_List = buttonHtml[2];
                programRunModel.QueryHtml = GetQueryHtml(programRunModel.ProgramQueries, localizer);
                programRunModel.QueryData = GetQueryData(programRunModel.ProgramQueries);
                programRunModel.GridColNames = GetColNames(programRunModel.ProgramFields);
                programRunModel.GridColModels = GetColModels(programRunModel.ProgramFields);
                programRunModel.DefaultSort = programModel.DefaultSort.IsNullOrWhiteSpace() ? GetDefaultSort(programRunModel.ProgramFields) : programModel.DefaultSort;

                Cache.IO.Insert(key, programRunModel);
                return programRunModel;
            }
        }

        /// <summary>
        /// 得到按钮HTML
        /// </summary>
        /// <param name="programRunModel"></param>
        /// <param name="userId">当前登录人员ID，验证权限</param>
        /// <param name="menuId">菜单ID(URL中的appid)</param>
        /// <param name="localizer">语言包</param>
        /// <returns><returns>List[0]工具栏按钮 List[1]常规按钮 List[2]列表按钮</returns></returns>
        public List<string> GetButtonHtml(Model.ProgramRun programRunModel, Guid userId, Guid menuId, IStringLocalizer localizer = null)
        {
            StringBuilder button_toolbar = new StringBuilder();
            StringBuilder button_normal = new StringBuilder();
            StringBuilder button_list = new StringBuilder();
            if (null == programRunModel)
            {
                return new List<string>() { "", "", "" };
            }
            //如果有查询要添加查询按钮
            if (programRunModel.ProgramQueries.Any())
            {
                Guid queryButtonId = "A5678AAB-69D8-40C5-9523-B4882A234975".ToGuid();
                if (!programRunModel.ProgramButtons.Exists(p => p.Id == queryButtonId))
                {
                    programRunModel.ProgramButtons.Add(new Model.ProgramButton()
                    {
                        Id = queryButtonId,
                        ButtonName = localizer == null ? "查&nbsp;询&nbsp;" : localizer["Query"],
                        ProgramId = programRunModel.Id,
                        ClientScript = "query();",
                        IsValidateShow = 0,
                        ShowType = 1,
                        Sort = 0,
                        Ico = "fa-search"
                    });
                }
            }

            List<Model.MenuUser> menuusers = new MenuUser().GetAll();
            Menu menu = new Menu();
            SystemButton systemButton = new SystemButton();
            string language = Tools.GetCurrentLanguage();
            foreach (var button in programRunModel.ProgramButtons.OrderBy(p => p.Sort))
            {
                //检查权限
                if (1 == button.IsValidateShow)
                {
                    if (!menu.HasUseButton(menuId, button.Id, userId, menuusers))
                    {
                        continue;
                    }
                }

                string funName = "fun_" + Guid.NewGuid().ToString("N");
                string butName = button.ButtonName;
                //如果是选择的按钮要从按钮库中获取名称(多语言时)
                if (button.ButtonId.HasValue && !Config.Language_Default.IsNullOrWhiteSpace())
                {
                    var systemButtonModel = systemButton.Get(button.ButtonId.Value);
                    if(null != systemButtonModel)
                    {
                        butName = language.Equals("en-US") ? systemButtonModel.Name_en : language.Equals("zh") ? systemButtonModel.Name_zh : systemButtonModel.Name;
                    }
                }
                string ico = button.Ico;
                if (button.ShowType == 0)//工具栏按钮
                {
                    if (ico.IsFontIco())
                    {
                        button_toolbar.Append("<a href=\"javascript:void(0);\" onclick=\"" + funName + "();\">" +
                            "<i class='fa " + ico + "'></i><label>" + butName + "</label></a>");
                    }
                    else
                    {
                        if (!ico.IsNullOrWhiteSpace())//图片图标
                        {
                            button_toolbar.Append("<a href=\"javascript:void(0);\" onclick=\"" + funName + "();\">" +
                                    "<span style=\"background-image:url(" + ico + ");\">" + butName + "</span></a>");
                        }
                        else//没有设置图标
                        {
                            button_toolbar.Append("<a href=\"javascript:void(0);\" onclick=\"" + funName + "();\">" +
                                    "<label>" + butName + "</label></a>");
                        }
                    }
                    button_toolbar.Append("<script type='text/javascript'>function " + funName + "(){" + Wildcard.Filter(button.ClientScript) + "}</script>");
                }
                else if (button.ShowType == 1)//常规按钮
                {
                    button_normal.Append("<button type='button' class='mybutton' style='margin-right:8px;'");
                    button_normal.Append(" onclick=\"" + funName + "();\"");
                    button_normal.Append(">");
                    if (!ico.IsNullOrWhiteSpace())
                    {
                        if (ico.IsFontIco())//如果是字体图标
                        {
                            button_normal.Append("<i class='fa " + ico + "' style='margin-right:3px;'></i>" + butName);
                        }
                        else
                        {
                            button_normal.Append("<img src=\"" + ico + "\" style='margin-right:3px;vertical-align:middle;'/>" + butName);
                        }
                    }
                    else
                    {
                        button_normal.Append(butName);
                    }
                    button_normal.Append("</button>");
                    button_normal.Append("<script type='text/javascript'>function " + funName + "(){" + Wildcard.Filter(button.ClientScript) + "}</script>");
                }
                else if (button.ShowType == 2)//列表按钮 不替换通配符，列表按钮涉及运行时数据，在运行时替换。
                {
                    if (ico.IsFontIco())
                    {
                        button_list.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"" + button.ClientScript + "\">" +
                            "<i class='fa " + ico + "'></i>" + butName + "</a>");
                    }
                    else
                    {
                        if (!ico.IsNullOrWhiteSpace())//图片图标
                        {
                            button_list.Append("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"" + button.ClientScript + "\">" +
                                    "<span style=\"background-image:url(" + ico + ");\">" + butName + "</span></a>");
                        }
                        else//没有设置图标
                        {
                            button_list.Append("<a href=\"javascript:void(0);\" onclick=\"" + button.ClientScript + "\">" +
                                    "<label>" + butName + "</label></a>");
                        }
                    }
                }
            }
            return new List<string>() { button_toolbar.ToString(), button_normal.ToString(), button_list.ToString() };
        }

        /// <summary>
        /// 得到查询HTML
        /// </summary>
        /// <param name="programQueries"></param>
        /// <param name="localizer">语言包</param>
        /// <returns></returns>
        private string GetQueryHtml(List<Model.ProgramQuery> programQueries, IStringLocalizer localizer = null)
        {
            StringBuilder query_controls = new StringBuilder();
            if (null == programQueries)
            {
                return "";
            }
            foreach (var query in programQueries)
            {
                string title = query.ShowTitle.IsNullOrWhiteSpace() ? query.Field : query.ShowTitle;
                string controlName = query.ControlName.IsNullOrWhiteSpace() ? "ctl_" + query.Id.ToString("N") : query.ControlName;
                query_controls.Append("<span style=\"margin-right:8px;\">");
                query_controls.Append("<span>" + title + "：</span>");
                switch (query.InputType)
                {
                    case 0: //文本框
                        query_controls.Append("<input type='text' class='mytext'");
                        query_controls.Append(" id='" + controlName + "'");
                        query_controls.Append(" name='" + controlName + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        query_controls.Append("/>");
                        break;
                    case 1://日期
                        query_controls.Append("<input type='text' class='mycalendar'");
                        query_controls.Append(" id='" + controlName + "'");
                        query_controls.Append(" name='" + controlName + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        query_controls.Append("/>");
                        break;
                    case 2://日期范围
                        query_controls.Append("<input type='text' class='mycalendar'");
                        query_controls.Append(" id='" + controlName + "'");
                        query_controls.Append(" name='" + controlName + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        query_controls.Append("/>");
                        query_controls.Append(localizer == null ? (localizer == null ? " 至 " : localizer["To"]) : localizer["To"]);
                        query_controls.Append("<input type='text' class='mycalendar'");
                        query_controls.Append(" id='" + controlName + "1'");
                        query_controls.Append(" name='" + controlName + "1'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        query_controls.Append("/>");
                        break;
                    case 3://日期时间
                        query_controls.Append("<input type='text' class='mycalendar' istime='1'");
                        query_controls.Append(" id='" + controlName + "'");
                        query_controls.Append(" name='" + controlName + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        query_controls.Append("/>");
                        break;
                    case 4://日期时间范围
                        query_controls.Append("<input type='text' class='mycalendar' istime='1'");
                        query_controls.Append(" id='" + controlName + "'");
                        query_controls.Append(" name='" + controlName + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        query_controls.Append("/>");
                        query_controls.Append(localizer == null ? (localizer == null ? " 至 " : localizer["To"]) : localizer["To"]);
                        query_controls.Append("<input type='text' class='mycalendar' istime='1'");
                        query_controls.Append(" id='" + controlName + "1'");
                        query_controls.Append(" name='" + controlName + "1'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        query_controls.Append("/>");
                        break;
                    case 5://下拉选项
                        query_controls.Append("<select class='myselect'");
                        query_controls.Append(" id='" + controlName + "'");
                        query_controls.Append(" name='" + controlName + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        query_controls.Append(">");
                        query_controls.Append("<option value=\"\"></option>");
                        switch (query.DataSource.Value)
                        {
                            case 0://字符串表达式
                                query_controls.Append(new Form().GetOptionsByStringExpression(query.DataSourceString, ""));
                                break;
                            case 1://数据字典
                                Dictionary.ValueField valueField = Dictionary.ValueField.Id;
                                switch (query.OrgAttribute.ToInt(0))
                                {
                                    case 1:
                                        valueField = Dictionary.ValueField.Title;
                                        break;
                                    case 2:
                                        valueField = Dictionary.ValueField.Code;
                                        break;
                                    case 3:
                                        valueField = Dictionary.ValueField.Value;
                                        break;
                                    case 4:
                                        valueField = Dictionary.ValueField.Note;
                                        break;
                                    case 5:
                                        valueField = Dictionary.ValueField.Other;
                                        break;
                                    default:
                                        valueField = Dictionary.ValueField.Id;
                                        break;
                                }
                                query_controls.Append(new Dictionary().GetOptionsByID(query.DictValue.ToGuid(), valueField));
                                break;
                            case 2://SQL
                                query_controls.Append(new Form().GetOptionsBySQL(query.ConnId, query.DataSourceString, ""));
                                break;
                        }
                        query_controls.Append("</select>");
                        break;
                    case 6://组织机构选择
                        query_controls.Append("<input type='text' class='mymember'");
                        query_controls.Append(" id='" + controlName + "'");
                        query_controls.Append(" name='" + controlName + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        string orgOptions = new Organize().GetOrganizeAttrString(query.OrgAttribute);
                        query_controls.Append(orgOptions);
                        query_controls.Append("/>");
                        break;
                    case 7://数据字典选择
                        query_controls.Append("<input type='text' class='mydict'");
                        query_controls.Append(" id='" + controlName + "'");
                        query_controls.Append(" name='" + controlName + "'");
                        query_controls.Append(" rootid='" + query.DictValue + "'");
                        if (!query.ShowStyle.IsNullOrWhiteSpace())
                        {
                            query_controls.Append(" style=\"" + query.ShowStyle + "\"");
                        }
                        query_controls.Append("/>");
                        break;
                }
                query_controls.Append("</span>");
            }
            return query_controls.ToString();
        }

        /// <summary>
        /// 得到查询的JSON data
        /// </summary>
        /// <param name="programQueries"></param>
        /// <returns></returns>
        private string GetQueryData(List<Model.ProgramQuery> programQueries)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("\"pagesize\": size || curPageSize, \"pagenumber\": number || curPageNumber,");
            foreach (var query in programQueries)
            {
                string controlName = query.ControlName.IsNullOrWhiteSpace() ? "ctl_" + query.Id.ToString("N") : query.ControlName;
                stringBuilder.Append("\"" + controlName + "\":$(\"#" + controlName + "\").val(),");
                if (query.InputType.In(2, 4))//如果是日期时间范围要加上截止时间
                {
                    stringBuilder.Append("\"" + controlName + "1\":$(\"#" + controlName + "1\").val(),");
                }
            }
            return "{" + stringBuilder.ToString().TrimEnd(',') + "}";
        }

        /// <summary>
        /// 得到Grid列头
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        private string GetColNames(List<Model.ProgramField> programFields)
        {
            if (null == programFields)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            foreach (var field in programFields)
            {
                stringBuilder.Append("\"" + (field.ShowTitle.IsNullOrWhiteSpace() ? field.Field : field.ShowTitle) + "\",");
            }
            return stringBuilder.ToString().TrimEnd(',') + "]";
        }

        /// <summary>
        /// 得到GRID列JSON
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        private string GetColModels(List<Model.ProgramField> programFields)
        {
            if (null == programFields)
            {
                return string.Empty;
            }
            Newtonsoft.Json.Linq.JArray jArray = new Newtonsoft.Json.Linq.JArray();
            foreach (var field in programFields)
            {
                Newtonsoft.Json.Linq.JObject jObject = new Newtonsoft.Json.Linq.JObject();
                string name = field.Field;
                if (name.IsNullOrWhiteSpace())
                {
                    if(100 == field.ShowType)
                    {
                        name = "opation";
                    }
                    else if(1 == field.ShowType)
                    {
                        name = "rowserialnumber";
                    }
                }
                jObject.Add("name", name);
                if (!field.IsSort.IsNullOrWhiteSpace())
                {
                    jObject.Add("index", field.IsSort.Trim());
                }
                else
                {
                    jObject.Add("sortable", false);
                }
                if (field.ShowType == 100)
                {
                    jObject.Add("title", false);
                }
                if (!field.Width.IsNullOrWhiteSpace())
                {
                    jObject.Add("width", field.Width.GetNumber());
                }
                jObject.Add("align", field.Align);

                jArray.Add(jObject);
            }
            return jArray.ToString(Newtonsoft.Json.Formatting.None);
        }

        /// <summary>
        /// 得到默认排序列
        /// </summary>
        /// <param name="programFields"></param>
        /// <returns></returns>
        private string GetDefaultSort(List<Model.ProgramField> programFields)
        {
            if (null == programFields)
            {
                return string.Empty;
            }
            var filed = programFields.Find(p => !p.IsSort.IsNullOrWhiteSpace());
            if (null != filed)
            {
                return filed.IsSort;
            }
            return string.Empty;
        }

        /// <summary>
        /// 得到查询数据
        /// </summary>
        /// <param name="programRunModel"></param>
        /// <returns></returns>
        public DataTable GetData(Model.ProgramRun programRunModel, HttpRequest request, int size, int number, out int count)
        {
            DbConnection dbConnection = new DbConnection();
            var dbConnModel = dbConnection.Get(programRunModel.ConnId);
            if (null == dbConnModel)
            {
                count = 0;
                return new DataTable();
            }
            DbconnnectionSql dbconnnectionSql = new DbconnnectionSql(dbConnModel);
            string tempSql = Wildcard.Filter(programRunModel.SqlString).Trim1();
            bool isProc = tempSql.Trim().StartsWith("exec", StringComparison.CurrentCultureIgnoreCase);//是否是存储过程

            #region 组织查询条件
            StringBuilder whereBuilder = new StringBuilder();
            List<DbParameter> dbParameters = new List<DbParameter>();
            if (!isProc)
            {
                if (!programRunModel.SqlString.ContainsIgnoreCase(" where "))
                {
                    whereBuilder.Append(" WHERE 1=1");
                }
                foreach (var query in programRunModel.ProgramQueries)
                {
                    string fieldName = query.Field;
                    string operators = query.Operators;
                    string controlName = query.ControlName.IsNullOrWhiteSpace() ? "ctl_" + query.Id.ToString("N") : query.ControlName;
                    string queryValue = request.Form[controlName];
                    if (queryValue.IsNullOrEmpty())
                    {
                        continue;
                    }
                    string paramsChar = dbConnModel.ConnType.EqualsIgnoreCase("oracle") ? ":" : "@";
                    if (query.InputType.In(1, 2, 3, 4))
                    {
                        if (queryValue.IsDateTime(out DateTime d))
                        {
                            whereBuilder.Append(" AND " + fieldName + operators + paramsChar + fieldName);
                            dbParameters.Add(dbconnnectionSql.SqlInstance.GetDbParameter(paramsChar + fieldName,
                                query.InputType.In(1, 2) ? d.Date : d));
                        }
                        if (query.InputType.In(2, 4))//日期时间范围
                        {
                            string queryValue1 = request.Form[controlName + "1"];
                            if (queryValue1.IsDateTime(out DateTime d1))
                            {

                                string operators1 = string.Empty;
                                switch (operators)
                                {
                                    case ">":
                                        operators1 = "<";
                                        break;
                                    case "<":
                                        operators1 = ">";
                                        break;
                                    case ">=":
                                        operators1 = "<";//因为在查询时日期要加上一天，所以这里是<
                                        break;
                                    case "<=":
                                        operators1 = ">";
                                        break;
                                }
                                whereBuilder.Append(" AND " + fieldName + operators1 + paramsChar + fieldName + "1");
                                dbParameters.Add(dbconnnectionSql.SqlInstance.GetDbParameter(paramsChar + fieldName + "1",
                                    query.InputType.In(1, 2) ? d1.AddDays(1).Date : d1.AddDays(1)));
                            }
                        }
                    }
                    else
                    {
                        switch (operators)
                        {
                            case "IN":
                                whereBuilder.Append(" AND " + fieldName + " IN(" + queryValue.FilterSelectSql() + ")");
                                break;
                            case "NOT IN":
                                whereBuilder.Append(" AND " + fieldName + " NOT IN(" + queryValue.FilterSelectSql() + ")");
                                break;
                            case "%LIKE":
                                whereBuilder.Append(" AND " + fieldName + " LIKE '%" + queryValue.FilterSelectSql() + "'");
                                break;
                            case "LIKE%":
                                whereBuilder.Append(" AND " + fieldName + " LIKE '" + queryValue.FilterSelectSql() + "%'");
                                break;
                            case "%LIKE%":
                                whereBuilder.Append(" AND " + fieldName + " LIKE '%" + queryValue.FilterSelectSql() + "%'");
                                break;
                            default:
                                whereBuilder.Append(" AND " + fieldName + operators + paramsChar + fieldName);
                                dbParameters.Add(dbconnnectionSql.SqlInstance.GetDbParameter(paramsChar + fieldName, queryValue));
                                break;
                        }
                    }
                }
            }
            else
            {
                //存储过程直接把获取到的值作为参数
                foreach (var query in programRunModel.ProgramQueries)
                {
                    string controlName = query.ControlName.IsNullOrWhiteSpace() ? "ctl_" + query.Id.ToString("N") : query.ControlName;
                    string queryValue = request.Form[controlName];
                    whereBuilder.Append("'" + queryValue + "',");
                }
            }
            #endregion

            string sidx = request.Forms("sidx");
            string sord = request.Forms("sord");
            string order = (sidx.IsNullOrWhiteSpace() ? programRunModel.DefaultSort : sidx) + (sord.IsNullOrEmpty() ? "" : " " + sord);

            StringBuilder querySql = new StringBuilder();
            if (!isProc)
            {
                if (dbConnModel.ConnType.EqualsIgnoreCase("sqlserver") || dbConnModel.ConnType.EqualsIgnoreCase("oracle"))
                {
                    tempSql = tempSql.Insert(tempSql.IndexOfIgnoreCase("from") - 1, ",ROW_NUMBER() OVER(ORDER BY " + order + ") AS PagerAutoRowNumber ");
                }
                querySql.Append(tempSql);
                querySql.Append(whereBuilder.ToString());
                if (dbConnModel.ConnType.EqualsIgnoreCase("mysql") && !order.IsNullOrWhiteSpace())
                {
                    querySql.Append(" ORDER BY " + order);
                }
            }
            else
            {
                querySql.Append(tempSql);
                querySql.Append(tempSql.EndsWith("'") ? "," : " ");
                querySql.Append(whereBuilder.ToString().TrimEnd(','));//存储过程直接把查询参数加到后面
            }
            string querySqlString = querySql.ToString();
            //将sql写入SESSION，导出时用
            request.HttpContext.Session.SetString("program_querysql_" + programRunModel.Id.ToString("N"), querySqlString);
            if (dbParameters.Count > 0)//将SQL参数保存到缓存
            {
                Cache.IO.Insert("program_queryparameter_" + programRunModel.Id.ToString("N") + "_"
                    + request.HttpContext.Session.Id, dbParameters, DateExtensions.Now.AddHours(2));
            }
            count = 0;
            string pagerSql = string.Empty;
            if (programRunModel.IsPager == 1 && !isProc)
            {
                pagerSql = dbconnnectionSql.SqlInstance.GetPaerSql(querySqlString, size, number, out count, dbParameters.ToArray()); 
            }
            else if(programRunModel.IsPager == 1 && isProc)
            {
                //querySql.Append((querySql.ToString().EndsWith("'") ? "," : " ") + size.ToString() + "," + number.ToString());
                //querySqlString += (querySqlString.EndsWith("'") ? "," : " ") + size.ToString() + "," + number.ToString();
                pagerSql = querySqlString;
            }
            return dbConnection.GetDataTable(dbConnModel, programRunModel.IsPager == 1 ? pagerSql : querySqlString, dbParameters.ToArray());
        }

        /// <summary>
        /// 得到导出excel数据(不分页的数据)
        /// </summary>
        /// <param name="programRunModel"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public DataTable GetExportData(Model.ProgramRun programRunModel, HttpRequest request)
        {
            DataTable exportDt = new DataTable();
            if (null == programRunModel || programRunModel.ProgramExports.Count == 0)
            {
                return exportDt;
            }
            string sql = request.HttpContext.Session.GetString("program_querysql_" + programRunModel.Id.ToString("N"));
            if (sql.IsNullOrWhiteSpace())
            {
                return exportDt;
            }
            object obj = Cache.IO.Get("program_queryparameter_" + programRunModel.Id.ToString("N") + "_" + request.HttpContext.Session.Id);
            DbParameter[] parameters = null;
            if (null != obj)
            {
                parameters = ((List<DbParameter>)obj).ToArray();
            }
            DbConnection dbConnection = new DbConnection();
            DataTable dt = dbConnection.GetDataTable(programRunModel.ConnId, sql, parameters);
            if (dt.Rows.Count == 0)
            {
                return exportDt;
            }
            foreach (var field in programRunModel.ProgramExports)
            {
                exportDt.Columns.Add(field.ShowTitle.IsNullOrEmpty() ? field.Field : field.ShowTitle, Type.GetType("System.String"));
            }
            foreach (DataRow dr in dt.Rows)
            {
                DataRow dr1 = exportDt.NewRow();
                foreach (var field in programRunModel.ProgramExports)
                {
                    string value = dt.Columns.Contains(field.Field) ? dr[field.Field].ToString() : string.Empty;
                    if (!value.IsNullOrWhiteSpace())
                    {
                        switch (field.ShowType)
                        {
                            case 1://序号

                                break;
                            case 2://日期时间
                                if (value.IsDateTime(out DateTime d))
                                {
                                    value = d.ToString(field.ShowFormat.IsNullOrWhiteSpace() ? "yyyy-MM-dd HH:mm:ss" : field.ShowFormat);
                                }
                                break;
                            case 3://数字
                                if (!field.ShowFormat.IsNullOrWhiteSpace())
                                {
                                    value = value.ToDecimal().ToString(field.ShowFormat);
                                }
                                break;
                            case 4://数据字典ID显示为标题
                                value = new Dictionary().GetTitles(value);
                                break;
                            case 5://组织架构显示为名称
                                value = new Organize().GetNames(value);
                                break;
                            case 6://自定义
                                value = Wildcard.Filter(field.CustomString, null, dr);
                                break;
                            case 7://人员ID显示为姓名
                                value = new User().GetNames(value);
                                break;
                        }
                    }
                    dr1[field.ShowTitle.IsNullOrEmpty() ? field.Field : field.ShowTitle] = value;
                }
                exportDt.Rows.Add(dr1);
            }
            return exportDt;
        }

        /// <summary>
        /// 根据主键值得到标题
        /// </summary>
        /// <param name="values">值(多个逗号分开)</param>
        /// <param name="pkField">主键字段</param>
        /// <param name="titleField">标题字段</param>
        /// <param name="programId">程序ID</param>
        /// <returns></returns>
        public string GetTitles(string values, string pkField, string titleField, Guid programId)
        {
            var programModel = Get(programId);
            if (null == programModel)
            {
                return "";
            }
            string[] idArray = values.Split(',');
            if (idArray.Length == 0)
            {
                return "";
            }
            StringBuilder stringBuilder = new StringBuilder();
            DbConnection dbConnection = new DbConnection();
            foreach (string id in idArray)
            {
                string sql = "SELECT " + titleField + " FROM (" + programModel.SqlString + ") TEMPTABLE_" + titleField + " WHERE " + pkField + "={0}";
                string title = dbConnection.GetFieldValue(programModel.ConnId, sql, id);
                if (!title.IsNullOrEmpty())
                {
                    stringBuilder.Append(title);
                    stringBuilder.Append("、");
                }
            }
            return stringBuilder.ToString().TrimEnd('、');
        }
        
    }
}
