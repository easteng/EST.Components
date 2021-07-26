/**********************************************************************
*******命名空间： ESTTool.Excel
*******类 名 称： ExcelExtension
*******类 说 明： 扩展方法
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/26/2021 5:49:04 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ESTTool.Excel
{
    /// <summary>
    ///  excel 扩展方法
    /// </summary>
    public static class ExcelExtension
    {
        /// <summary>
        /// 将表格<seealso cref="IWorkbook"/>转为<seealso cref="Byte[]"/>流 
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(this IWorkbook workbook)
        {
            try
            {
                var stream = new MemoryStream();
                workbook.Write(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                return new byte[0] { };
            }
        }

        /// <summary>
        /// 将对象转化为字节数字
        /// </summary>
        /// <param name="obj">需要转化对象</param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(this object obj) => obj.ConvertToStream().GetBuffer();

        /// <summary>
        /// 对象转为数据流
        /// </summary>
        /// <param name="obj">需要转化的对象</param>
        /// <returns></returns>
        public static MemoryStream ConvertToStream(this object obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                formatter.Serialize(stream, obj);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                return stream;
            }
        }

        /// <summary>
        /// 字节数组转数据流
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static MemoryStream BytesToStream(this byte[] datas)
        {
            return new MemoryStream(datas);
        }

        /// <summary>
        /// 获取工作表的了类型
        /// </summary>
        public static ExcelFileType GetWorkbookType(this ISheet sheet)
        {
            var workType = sheet.Workbook.GetType().Name;
            switch (workType)
            {
                case "XSSFWorkbook":
                    return ExcelFileType.Xlsx;
                case "HSSFWorkbook":
                    return ExcelFileType.Xls;
                default: return default;
            }
        }
        public static ExcelFileType GetWorkbookType(this IRow row) => row.Sheet.GetWorkbookType();

        /// <summary>
        /// 检查需要导入的工作表是和模板实体对应
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet">工作表</param>
        /// <param name="validDataIndex">有效数据索引</param>
        /// <returns></returns>
        public static ISheet CheckColumnAccordTempleModel<T>(this ISheet sheet, int validDataIndex = 0)
        {
            if (sheet == null) throw new ArgumentNullException(paramName: nameof(CheckColumnAccordTempleModel), "工作表为空");
            var cellCount = sheet.GetRow(validDataIndex).PhysicalNumberOfCells;
            var properties = typeof(T).GetProperties();

            // 逻辑优化,如果实体包含扩展内容，则不需要再判断
            if (properties.Any(a => a.Name == "Properties"))
            {
                return sheet;
            }
            else
            {
                var modelPropsCount = typeof(T).GetProperties().Length;
                return cellCount == modelPropsCount ? sheet : throw new Exception("导入文件与对应的模板不符，请检查");
            };
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet"></param>
        /// <param name="datas"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static ISheet WriteData<T>(this ISheet sheet, List<T> datas, int rowIndex = 0)
        {
            // 判断T中是否有扩展属性
            var props = typeof(T).GetProperties();
            if (props.Count(a => a.Name != "Properties") != sheet.GetRow(rowIndex).PhysicalNumberOfCells)
            {
                throw new ArgumentException("模板与数据格式不符");
            }

            bool isGetStyle = false;
            ICellStyle cellStyle = null;
            var rowNumber = 1;

            ICell theCell;
            //cellStyle = ExcelHelper.ExcelHelper.GetCopyCellStyle(sheet.Workbook, theCell.CellStyle);
            cellStyle = sheet.Workbook.CreateCellStyle();

            datas?.ForEach(a =>
            {
                var colIndex = 0;

                var row = sheet
                    .CreateRow(rowIndex);
                foreach (var prop in props)
                {
                    row
                    .CreateCell(colIndex)
                    .SetCellValue(prop.GetValue(a)?.ToString() == "0" && colIndex == 0 ? rowNumber.ToString() : prop.GetValue(a)?.ToString());

                    theCell = row.GetCell(colIndex);
                    if (isGetStyle == false)
                    {
                        cellStyle.BorderTop = BorderStyle.Thin;
                        cellStyle.BorderRight = BorderStyle.Thin;
                        cellStyle.BorderBottom = BorderStyle.Thin;
                        cellStyle.BorderLeft = BorderStyle.Thin;
                        isGetStyle = true;
                    }
                    theCell.CellStyle = cellStyle;

                    colIndex++;
                }
                isGetStyle = false;
                rowNumber++;
                rowIndex++;
            });
            return sheet;
        }

        /// <summary>
        /// 设置单元格样式
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="cellStyle"></param>
        /// <returns></returns>
        public static ICell SetCellStyle(this ICell cell, ICellStyle cellStyle)
        {
            cell.CellStyle = cellStyle;
            return cell;
        }

        /// <summary>
        /// 读取指定导入dto中的文件流信息
        /// </summary>
        /// <param name="fileDto"></param>
        /// <returns></returns>
        //  public static Stream ReadFileStream(this FileUploadDto fileDto) => fileDto.File?.OpenReadStream();

        /// <summary>
        /// 检查文件流是否为空
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Stream CheckNull(this Stream stream) =>
            stream ?? throw new ArgumentNullException(paramName: nameof(stream), "文件流为空");

        /// <summary>
        /// 检查转换后集合是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> CheckNull<T>(this List<T> list) =>
            list ?? throw new ArgumentNullException(paramName: nameof(CheckNull), "未将对象实例化");

        /// <summary>
        ///  将数据表转为泛型对象
        ///  <see cref="ExcelHelper.ExcelHelper.SheetToList"/>
        /// </summary>
        /// <typeparam name="T">与工作表列对应数量的泛型实体类</typeparam>
        /// <param name="sheet">休要转换工作表</param>
        /// <param name="rowIndex">工作表中有效数据的起始索引</param>
        /// <returns></returns>
        public static List<T> TryTransToList<T>(this ISheet sheet, int rowIndex = 0) where T : class, new()
        {
            try
            {
                return SheetToList<T>(sheet, rowIndex);
            }
            catch (Exception e)
            {
                throw new ArgumentNullException(paramName: nameof(TryTransToList), "数据转换异常，请检查源文件");
            }
        }

        public static int FindIndex<T>(this List<T> list, object item)
        {
            return list.FindIndex(a => a.Equals(item));
        }

        public static List<string> GetParentOrganizationCodeArray(this string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            if (!code.Contains(".")) return null;
            var codes = code.Split('.');
            return codes.Select((a, b) => string.Join(".", codes.Take(b + 1))).ToList();
        }

        /// <summary>
        /// 获取工作表空间
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static IWorkbook GetWorkbookByStream(Stream stream, string fileName)
        {
            // 判断是不是文件
            if (fileName.EndsWith(".xlsx"))
            {
                return new XSSFWorkbook(stream);
            }
            else if (fileName.EndsWith(".xls"))
            {
                return new HSSFWorkbook(stream);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 工作表转List
        /// </summary>
        /// <typeparam name="T">需要转换的泛型类</typeparam>
        /// <param name="sheet">工作表</param>
        /// <param name="rowStartIndex">起始索引</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        public static List<T> SheetToList<T>(ISheet sheet, int rowStartIndex = 1) where T : class, new()
        {
            var list = new List<T>();
            var workType = sheet.GetWorkbookType();
            var rows = sheet.GetRowEnumerator();
            if (rows == null) return null;
            IRow row = null;
            IRow firstRow = null;
            while (rows.MoveNext())
            {
                switch (workType)
                {
                    case ExcelFileType.Xlsx:
                        row = (XSSFRow)rows.Current;
                        break;
                    case ExcelFileType.Xls:
                        row = (HSSFRow)rows.Current;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (row == null) continue;
                if (row.RowNum == 0)
                {
                    firstRow = row;
                    continue;
                }
                if (row.RowNum < rowStartIndex)
                {
                    continue;
                }
                var t = Activator.CreateInstance(typeof(T)) as T;
                var props = t.GetType().GetProperties().ToArray();
                // 逻辑修改，根据实体的属性顺序读取表格行中的内容

                var cells = row.Cells;// 获取所有的列
                var pCount = Math.Min(props.Length, cells.Count);
                for (int i = 0; i < pCount; i++)
                {
                    var cell = cells[i];
                    if (cell == null || string.IsNullOrEmpty(cell.ToString())) continue;
                    var type = props[i].PropertyType;
                    var name = props[i].Name;
                    if (name == "Properties") continue;
                    var value = GetCellValue(type, cell.ToString());
                    if (name == "Index")
                    {
                        value = rowStartIndex++;
                    }
                    props[i].SetValue(t, value);
                }
                // 获取是否有扩展的内容
                var properties = props.FirstOrDefault(a => a.Name == "Properties");
                if (properties != null)
                {
                    // 存在扩展属性
                    var propertyCells = cells.Skip(pCount - 1).ToList();
                    if (propertyCells != null && propertyCells.Any())
                    {
                        var dic = new Dictionary<string, string>();
                        propertyCells.ForEach(a =>
                        {
                            // 获取第一行的数据
                            var cindex = propertyCells.FindIndex(a);
                            var key = firstRow.GetCell(pCount + cindex - 1).ToString(); // 获取表头的名称
                            var value = a.ToString();
                            dic.Add(key, value);
                        });
                        properties.SetValue(t, dic);
                    }
                }
                list.Add(t);
            }
            return list;
        }
        /// <summary>
        /// 获取单元格的值
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object GetCellValue(Type valueType, string value)
        {
            if (valueType == typeof(int))
            {
                return int.Parse(value);
            }
            else if (valueType == typeof(float))
            {
                return float.Parse(value);
            }
            else if (valueType == typeof(DateTime))
            {
                return DateTime.Parse(value);
            }
            else if (valueType == typeof(decimal))
            {
                return decimal.Parse(value);
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 给指定工作表添加信息列，默认信息列背景色为黄
        /// </summary>
        /// <param name="sheet">信息表</param>
        /// <param name="infos">错误信息</param>
        /// <param name="colTitle">列标题</param>
        /// <param name="titleRowIndex">列标题行起始索引</param>
        /// <returns></returns>
        public static void AddInfoColumn(
            ISheet sheet,
          //  List<WrongInfo> infos,
            string colTitle,
            int titleRowIndex = 0
            )
        {
            if (sheet == null) return;
            try
            {
                ICellStyle cellStyle = sheet.Workbook.CreateCellStyle();
                bool isAddTitle = false;
                var font = sheet.Workbook.CreateFont();
                font.Color = HSSFColor.Red.Index;
                cellStyle.WrapText = true;
                cellStyle.FillForegroundColor = HSSFColor.Orange.Index;
                cellStyle.SetFont(font);
                // 根据错误信息中行索引进行添加数据
                //foreach (var item in infos)
                //{
                //    var row = sheet.GetRow(item.RowIndex);
                //    //根据数据行的列位置创建 异常信息列 列头
                //    if (!isAddTitle)
                //    {
                //        isAddTitle = true;
                //        var titleRow = sheet.GetRow(titleRowIndex);
                //        var titleCell = titleRow.CreateCell(row.LastCellNum);
                //        titleCell.SetCellValue(colTitle);
                //        sheet.SetColumnWidth(row.LastCellNum, 30 * 256);
                //    }
                //    var cell = row.CreateCell(row.LastCellNum);
                //    cell.SetCellValue(item.WrongMsg);
                //    cell.CellStyle = cellStyle;
                //}
            }
            catch (Exception e)
            {
                return;
            }
        }
        /// <summary>
        /// 设置单元格样式
        /// </summary>
        /// <param name="row"></param>
        private static void SetCellStyle(
            IRow row)
        {
            if (row == null) return;
            for (int i = row.FirstCellNum; i < row.LastCellNum; i++)
            {
                //row.GetCell(i).CellStyle.FillBackgroundColor = 8;
                row.GetCell(i).CellStyle.FillForegroundColor = 10; // 设置前景色
                row.GetCell(i).CellStyle.FillPattern = FillPattern.SolidForeground;
            }
        }

        /// <summary>
        /// 导出excel 文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList"></param>
        /// <param name="templateName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="imageConfig"></param>
        /// <returns></returns>
        public static Stream ExcelExportStream<T>(List<T> dataList, string templateName, int rowIndex = 0, FileImageConfig imageConfig = null)
        {
            // 判断实体T 的结构与模板的结构是否相同
            //获取文件模板
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(),
                path2: $"import_templates\\import_templates_{templateName}.xls");
            MemoryStream fileMemoryStream;
            if (File.Exists(templatePath))
            {
                var fileStream = File.OpenRead(templatePath);
                var sheet = GetWorkbookByStream(
                             fileStream,
                        $"import_templates_{templateName}.xls")
                    .GetSheetAt(0)                             // 获取工作表
                    .CheckColumnAccordTempleModel<T>(rowIndex) // 校验工作表
                    .WriteData(dataList, rowIndex);            // 写入数据
                if (imageConfig != null)
                {
                    // 添加图片
                    var drawing = sheet.CreateDrawingPatriarch();
                    var pic = sheet.Workbook.AddPicture(imageConfig.ImageBytes, PictureType.PNG);
                    var anchor = new HSSFClientAnchor();
                    anchor.SetAnchor(
                        imageConfig.Col1,
                        imageConfig.Row1,
                        imageConfig.X1,
                        imageConfig.Y1,
                        imageConfig.Col2,
                        imageConfig.Row2,
                        imageConfig.X2,
                        imageConfig.Y2);
                    anchor.AnchorType = AnchorType.MoveAndResize;
                    drawing.CreatePicture(anchor, pic);
                }
                // 写入数据
                fileMemoryStream = sheet.Workbook
                    .ConvertToBytes()
                    .BytesToStream();
            }
            else
            {
                throw new ArgumentException($"import_templates_{templateName}.xls文件模板不存在");
            }
            return fileMemoryStream;
        }
    }
    public class FileImageConfig
    {
        public byte[] ImageBytes { get; set; }
        public short Col1 { get; set; }
        public int Row1 { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public short Col2 { get; set; }
        public int Row2 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
    }
}
