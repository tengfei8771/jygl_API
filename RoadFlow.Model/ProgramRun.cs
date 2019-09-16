﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    /// <summary>
    /// 程序设计运行时实体，用于缓存
    /// </summary>
    public class ProgramRun
    {
        /// <summary>
		/// Id
		/// </summary>
        [DisplayName("Id")]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [DisplayName("应用名称")]
        [Required(ErrorMessage = "应用名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [Required]
        [DisplayName("分类")]
        public Guid Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [DisplayName("发布时间")]
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Required]
        [DisplayName("创建人")]
        public Guid CreateUserId { get; set; }

        /// <summary>
        /// 查询SQL
        /// </summary>
        [Required]
        [DisplayName("查询SQL")]
        public string SqlString { get; set; }

        /// <summary>
        /// 是否显示新增按钮
        /// </summary>
        [Required]
        [DisplayName("是否显示新增按钮")]
        public int IsAdd { get; set; }

        /// <summary>
        /// 数据连接ID
        /// </summary>
        [Required]
        [DisplayName("数据连接ID")]
        public Guid ConnId { get; set; }

        /// <summary>
        /// 状态 0设计中 1已发布 2已作废
        /// </summary>
        [Required]
        [DisplayName("状态 0设计中 1已发布 2已作废")]
        public int Status { get; set; }

        /// <summary>
        /// 表单ID
        /// </summary>
        [DisplayName("表单ID")]
        public string FormId { get; set; }

        /// <summary>
        /// 编辑模式 0，当前窗口 1，弹出层
        /// </summary>
        [DisplayName("编辑模式 0，当前窗口 1，弹出层")]
        public int? EditModel { get; set; }

        /// <summary>
        /// 弹出层宽度
        /// </summary>
        [DisplayName("弹出层宽度")]
        public string Width { get; set; }

        /// <summary>
        /// 弹出层高度
        /// </summary>
        [DisplayName("弹出层高度")]
        public string Height { get; set; }

        /// <summary>
        /// 按钮显示位置 0新行 1查询后面
        /// </summary>
        [DisplayName("按钮显示位置 0新行 1查询后面")]
        public int ButtonLocation { get; set; }

        /// <summary>
        /// 是否分页
        /// </summary>
        [DisplayName("是否分页")]
        public int IsPager { get; set; }

        /// <summary>
        /// 选择列 0无 1单选 2多选
        /// </summary>
        public int SelectColumn { get; set; }

        /// <summary>
        /// 是否显示序号列
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// 页面脚本
        /// </summary>
        [DisplayName("页面脚本")]
        public string ClientScript { get; set; }

        /// <summary>
        /// 导出EXCEL模板
        /// </summary>
        [DisplayName("导出EXCEL模板")]
        public string ExportTemplate { get; set; }

        /// <summary>
        /// 导出Excel表头
        /// </summary>
        [DisplayName("导出Excel表头")]
        public string ExportHeaderText { get; set; }

        /// <summary>
        /// 导出EXCLE的文件名
        /// </summary>
        [DisplayName("导出EXCLE的文件名")]
        public string ExportFileName { get; set; }

        /// <summary>
        /// 列表样式
        /// </summary>
        [DisplayName("列表样式")]
        public string TableStyle { get; set; }

        /// <summary>
        /// 列表表头HTML
        /// </summary>
        [DisplayName("列表表头HTML")]
        public string TableHead { get; set; }

        /// <summary>
        /// 导入EXCEL数据时的标识字段，每次导入生成一个编号区分
        /// </summary>
        [DisplayName("导入EXCEL数据时的标识字段，每次导入生成一个编号区分")]
        public string InDataNumberFiledName { get; set; }

        /// <summary>
        /// 表头合并
        /// </summary>
        public string GroupHeaders { get; set; }

        /// <summary>
        ///工具栏按钮HTML
        /// </summary>
        public string Button_Toolbar { get; set; }

        /// <summary>
        ///常规按钮HTML
        /// </summary>
        public string Button_Normal { get; set; }

        /// <summary>
        ///列表HTML
        /// </summary>
        public string Button_List { get; set; }

        /// <summary>
        /// 查询HTML
        /// </summary>
        public string QueryHtml { get; set; }

        /// <summary>
        /// 查询按钮时的JSON data
        /// function query(size, number) {var data = 这里的JSON;}
        /// </summary>
        public string QueryData { get; set; }

        /// <summary>
        /// 表头
        /// </summary>
        public string GridColNames { get; set; }

        /// <summary>
        /// 列设置
        /// </summary>
        public string GridColModels { get; set; }

        /// <summary>
        /// 默认排序列
        /// </summary>
        public string DefaultSort { get; set; }

        /// <summary>
        /// 查询SQL
        /// </summary>
        public string QuerySql { get; set; }

        /// <summary>
        /// 得到条数SQL
        /// </summary>
        public string CountSql { get; set; }

        /// <summary>
        /// 列表字段
        /// </summary>
        public List<ProgramField> ProgramFields { get; set; }
        /// <summary>
        /// 按钮
        /// </summary>
        public List<ProgramButton> ProgramButtons { get; set; }
        /// <summary>
        /// 查询
        /// </summary>
        public List<ProgramQuery> ProgramQueries { get; set; }
        /// <summary>
        /// 验证
        /// </summary>
        public List<ProgramValidate> ProgramValidates { get; set; }
        /// <summary>
        /// 导出
        /// </summary>
        public List<ProgramExport> ProgramExports { get; set; }
    }
}
