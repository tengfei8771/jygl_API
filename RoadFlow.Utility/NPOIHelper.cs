using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace RoadFlow.Utility
{
    public class NPOIHelper
    {
        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="templateFile">模板文件</param>
        /// <param name="fileName">导出的文件名</param>
        public static IWorkbook Export(DataTable dtSource, string strHeaderText, string templateFile = "", string fileName = "")
        {
            IWorkbook workbook = null;
            ISheet sheet = null;
            bool isTemplateFile = false;//是否采用模板文件导出
            int rowIndex = 0;
            bool isExcel2007 = false;
            if (!templateFile.IsNullOrEmpty() && File.Exists(templateFile))
            {
                string templateFilePath = templateFile;
                isExcel2007 = templateFilePath.EndsWith("xlsx", StringComparison.CurrentCultureIgnoreCase);
                using (FileStream file = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read))
                {
                    if (isExcel2007)
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    else
                    {
                        workbook = new HSSFWorkbook(file);
                    }
                }
                if (workbook != null)
                {
                    sheet = workbook.GetSheetAt(0);
                    if (sheet != null)
                    {
                        isTemplateFile = true;
                        rowIndex = sheet.LastRowNum + 1;
                    }
                }
            }
            else
            {
                isExcel2007 = fileName.EndsWith("xlsx", StringComparison.CurrentCultureIgnoreCase);
            }
            if (workbook == null)
            {
                if (isExcel2007)
                {
                    workbook = new XSSFWorkbook();
                }
                else
                {
                    workbook = new HSSFWorkbook();
                }
            }
            if (sheet == null)
            {
                sheet = workbook.CreateSheet("Sheet1");
            }
          
            int[] arrColWidth = new int[] { };
            string[] arrColDataType = new string[] { };
            if (!isTemplateFile)
            {
                //取得列宽
                arrColWidth = new int[dtSource.Columns.Count];
                arrColDataType = new string[dtSource.Columns.Count];
                foreach (DataColumn item in dtSource.Columns)
                {
                    if (item.Caption.IsInt())
                    {
                        arrColWidth[item.Ordinal] = item.Caption.ToInt();
                    }
                    else
                    {
                        arrColWidth[item.Ordinal] = Encoding.Default.GetBytes(item.ColumnName.ToString()).Length;
                    }
                    arrColDataType[item.Ordinal] = item.DataType.ToString();
                }
                if (dtSource.Rows.Count > 0)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        if (!dtSource.Columns[j].Caption.IsInt())
                        {
                            int intTemp = Encoding.Default.GetBytes(dtSource.Rows[0][j].ToString()).Length;
                            if (intTemp > arrColWidth[j])
                            {
                                arrColWidth[j] = intTemp;
                            }
                        }
                    }
                }
            }
            bool hasHeader = !strHeaderText.IsNullOrEmpty();//是否有表头

            if (!isTemplateFile)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    #region 表头及样式
                    if (hasHeader)
                    {

                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headStyle.BorderLeft = BorderStyle.Thin;
                        headStyle.BorderRight = BorderStyle.Thin;
                        headStyle.BorderTop = BorderStyle.Thin;
                        headStyle.BorderBottom = BorderStyle.Thin;
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(hasHeader ? 1 : 0);
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.BorderLeft = BorderStyle.Thin;
                        headStyle.BorderRight = BorderStyle.Thin;
                        headStyle.BorderTop = BorderStyle.Thin;
                        headStyle.BorderBottom = BorderStyle.Thin;
                        headStyle.IsLocked = true;
                        headStyle.SetFont(font);
                        ICellStyle cellStyle = workbook.CreateCellStyle();
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            if (arrColWidth.Length > 0)
                            {
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            }
                        }
                        //sheet.CreateFreezePane(0, hasHeader ? 2 : 1, 0, dtSource.Columns.Count - 1);
                    }
                    #endregion
                    rowIndex = hasHeader ? 2 : 1;
                }
                #endregion
            }
            int columnsCount = dtSource.Columns.Count;
            ICellStyle cellStyle1 = workbook.CreateCellStyle();
            cellStyle1.BorderLeft = BorderStyle.Thin;
            cellStyle1.BorderRight = BorderStyle.Thin;
            cellStyle1.BorderTop = BorderStyle.Thin;
            cellStyle1.BorderBottom = BorderStyle.Thin;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                for (int i = 0; i < columnsCount; i++)
                {
                    ICell newCell = dataRow.CreateCell(i);
                    //IDataFormat format = workbook.CreateDataFormat();
                    //cellStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
                    newCell.CellStyle = cellStyle1;
                    string drValue = row[i].ToString();
                    string columnDataType = arrColDataType.Length > i ? arrColDataType[i] : "System.String";
                    switch (columnDataType)
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);

                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }

                #endregion
                rowIndex++;
            }
            return workbook;
        }

        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        /// <param name="templateFile">模板文件</param>
        public static void ExportToFile(DataTable dtSource, string strHeaderText, string strFileName, string templateFile = "")
        {
            using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = Export(dtSource, strHeaderText, templateFile, strFileName);
                workbook.Write(fs);
                workbook.Close();
                workbook = null;
                fs.Flush();
            }
        }

        /// <summary>
        /// 用于Web导出
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">文件名</param>
        /// <param name="teplateFile">模板文件</param>
        public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName, HttpResponse response = null, string templateFile = "")
        {
            string fileName = strFileName.UrlEncode();
            var Response = response ?? Tools.HttpContext.Response;
            Response.Clear();
            Response.Headers.Add("Server-FileName", fileName);
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", "attachment;filename=" + fileName);
            IWorkbook workbook = Export(dtSource, strHeaderText, templateFile, strFileName);
            //Response.Headers.Add("Content-Length", );
            workbook.Write(Response.Body);
            //Response.Headers.Add("Content-Length", stream.Length.ToString());
            workbook.Close();
            workbook = null;
            Response.Body.Flush();
            Response.Body.Close();
        }

        /// <summary>
        /// 读取excel 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <param name="header">前几行为表头，默认1</param>
        /// <returns></returns>
        public static DataTable ReadToDataTable(string strFileName, int headerRows = 1)
        {
            DataTable dt = new DataTable();
            IWorkbook hssfworkbook;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (Path.GetExtension(strFileName).Equals(".xlsx", StringComparison.CurrentCultureIgnoreCase))
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }
                else
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + headerRows); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                    {
                        dataRow[j] = row.GetCell(j).ToString();
                    }
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 将DataTable导出成CSV格式
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns>CSV字符串数据</returns>
        public static void ExportCSV(DataTable dt, string strFileName)
        {
            StringBuilder data = new StringBuilder();
            //写出列名
            for (int i = 0; i < dt.Columns.Count;i++ )
            {
                data.Append(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1)
                {
                    data.Append(",");
                }
            }
            data.Append("\n");
            //写出数据
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data.Append(row[i]);
                    if (i < dt.Columns.Count - 1)
                    {
                        data.Append(",");
                    }
                }
                data.Append("\n");
            }
            data.Append("\n");
            using (FileStream f = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
            {
                byte[] data1 = Encoding.Default.GetBytes(data.ToString());
                f.Write(data1, 0, data1.Length);
                f.Close();
            }
        }
    }
}