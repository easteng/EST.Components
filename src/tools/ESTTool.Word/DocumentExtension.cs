/**********************************************************************
*******命名空间： ESTTool.Word
*******类 名 称： DocumentExtension
*******类 说 明： 文本操作扩展类
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/26/2021 6:06:49 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTTool.Word
{
    /// <summary>
    ///  文本操作扩展类
    /// </summary>
    public static class DocumentExtension
    {
        /// <summary>
        /// 设置行的高度
        /// </summary>
        /// <param name="row"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static XWPFTableRow SetRowHeight(this XWPFTableRow row, int height)
        {
            row.Height = height;
            return row;
        }
        /// <summary>
        /// 设置表格宽度
        /// </summary>
        /// <param name="table"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static XWPFTable SetTableWidth(this XWPFTable table, int width)
        {
            table.Width = width;
            return table;
        }
        /// <summary>
        /// 创建表格，并设置列宽
        /// </summary>
        /// <param name="document"></param>
        /// <param name="rows"></param>
        /// <param name="columnWidths"></param>
        /// <returns></returns>
        public static XWPFTable CreateTableAndSetColumnWidth(this XWPFDocument document,
            int rows, params ulong[] columnWidths)
        {
            var table = document.CreateTable(rows, columnWidths.Length);
            for (int i = 0; i < columnWidths.Length; i++)
            {
                table.SetColumnWidth(i, columnWidths[i]);
            }
            return table;
        }

        /// <summary>
        /// 合并指定行的指定单元格
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellStartIndex"></param>
        /// <param name="cellEndIndex"></param>
        /// <returns></returns>
        public static XWPFTable MergeRowCells(this XWPFTable table, int rowIndex, int cellStartIndex, int cellEndIndex)
        {
            var row = table.GetRow(rowIndex);
            if (row == null) throw new ArgumentException("行索引超过了已有的行");
            row.MergeCells(cellStartIndex, cellEndIndex);
            return table;
        }
        /// <summary>
        /// 设置表格的表头内容
        /// </summary>
        /// <param name="table"></param>
        /// <param name="paragraphAlign"></param>
        /// <param name="textPosition"></param>
        /// <param name="isBold"></param>
        /// <param name="fontSize"></param>
        /// <param name="titles"></param>
        public static XWPFTable SetTableHeadTitle(this XWPFTable table,
            ParagraphAlignment paragraphAlign,
            int textPosition = 24,
            bool isBold = false,
            int fontSize = 10,
            params string[] titles
            )
        {
            if (titles.Any())
            {
                for (int i = 0; i < titles.Length; i++)
                {
                    table.SetTableParagraph(0, i, titles[i], paragraphAlign, textPosition, isBold, fontSize);
                }
            }
            return table;
        }


        /// <summary>
        /// 设置指定行的段落文本
        /// </summary>
        /// <param name="document"></param>
        /// <param name="docPoint"></param>
        /// <param name="fillContent"></param>
        /// <param name="isBold"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontFamily"></param>
        /// <param name="paragraphAlign"></param>
        /// <param name="isStatement"></param>
        /// <param name="secondFillContent"></param>
        /// <param name="fontColor"></param>
        /// <param name="isItalic"></param>
        /// <param name="position"></param>
        public static void SetDocumentParagraph(this XWPFDocument document,
            int docPoint,
            string fillContent,
            bool isBold,
            int fontSize,
            string fontFamily,
            ParagraphAlignment paragraphAlign,
            bool isStatement = false,
            string secondFillContent = "",
            string fontColor = "000000",
            bool isItalic = false,
            int position = 35
            )
        {
            XWPFParagraph paragraph = document.CreateParagraph();//创建段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）


            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.IsBold = isBold;//文字加粗
            xwpfRun.SetText(fillContent);//填充内容
            xwpfRun.FontSize = fontSize;//设置文字大小
            xwpfRun.IsItalic = isItalic;//是否设置斜体（字体倾斜）
            xwpfRun.SetColor(fontColor);//设置字体颜色--十六进制
            xwpfRun.SetFontFamily(fontFamily, FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需求而定
            //xwpfRun.SetTextPosition(position);// 设置行间距
            if (isStatement)
            {
                XWPFRun secondXwpfRun = paragraph.CreateRun();//创建段落文本对象
                secondXwpfRun.IsBold = isBold;//文字加粗
                secondXwpfRun.SetText(secondFillContent);//填充内容
                secondXwpfRun.FontSize = fontSize;//设置文字大小
                secondXwpfRun.IsItalic = isItalic;//是否设置斜体（字体倾斜）
                secondXwpfRun.SetColor(fontColor);//设置字体颜色--十六进制
                //secondXwpfRun.SetTextPosition(position);// 设置行间距
                secondXwpfRun.SetFontFamily(fontFamily, FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需
            }
            document.SetParagraph(paragraph, docPoint);
        }
        /// <summary>
        /// 设置table 单元格的段落
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="fillContent"></param>
        /// <param name="paragraphAlign"></param>
        /// <param name="textPosition"></param>
        /// <param name="isBold"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontColor"></param>
        /// <param name="isItalic"></param>
        public static XWPFTable SetTableParagraph(
            this XWPFTable table,
            int rowIndex,
            int cellIndex,
            string fillContent,
            ParagraphAlignment paragraphAlign,
            int textPosition = 24,
            bool isBold = false,
            int fontSize = 10,
            string fontColor = "000000",
            bool isItalic = false
            )
        {
            var para = new CT_P();
            //设置单元格文本对齐
            para.AddNewPPr().AddNewTextAlignment();
            XWPFParagraph paragraph = new XWPFParagraph(para, table.Body);//创建表格中的段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）
            paragraph.FontAlignment = Convert.ToInt32(ParagraphAlignment.CENTER);//字体在单元格内显示位置与 paragraph.Alignment效果相似
            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.SetText(fillContent);
            xwpfRun.FontSize = fontSize;//字体大小
            xwpfRun.SetColor(fontColor);//设置字体颜色--十六进制
            xwpfRun.IsItalic = isItalic;//是否设置斜体（字体倾斜）
            xwpfRun.IsBold = isBold;//是否加粗
            xwpfRun.SetFontFamily("宋体", FontCharRange.None);//设置字体（如：微软雅黑,华文楷体,宋体）
            //xwpfRun.SetTextPosition(textPosition);//设置文本位置（设置两行之间的行间），从而实现table的高度设置效果 
            table.GetRow(rowIndex).GetCell(cellIndex)
                .SetParagraph(paragraph);
            return table;
        }


        /// <summary>
        /// 设置页边距
        /// </summary>
        /// <param name="document"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public static XWPFDocument SetDocumentMargin(this XWPFDocument document,
            string left,//左边距
            string top,// 上边距
            string right,
            string bottom
            )
        {
            CT_PageMar mar = new CT_PageMar();
            CT_SectPr sectPr = new CT_SectPr();
            sectPr.pgMar = mar;
            mar.bottom = bottom;
            mar.top = top;
            mar.left = ulong.Parse(left);
            mar.right = ulong.Parse(right);
            document.Document.body.sectPr = sectPr;
            return document;
        }
    }
}
