using System;
using System.Collections.Generic;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace WebApi.Utility
{
    /// <summary>
    /// Excel 帮助类
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// 创建WorkBook
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstTitle"></param>
        /// <param name="list"></param>
        /// <param name="fillCell"></param>
        /// <returns></returns>
        public static IWorkbook CreateWorkbook<T>(List<string> lstTitle, T[] list, Action<IRow, T[], int> fillCell)
        {
            IWorkbook book = new HSSFWorkbook();
            ISheet sheet = book.CreateSheet("Sheet1");
            IRow rowTitle = sheet.CreateRow(0);
            ICellStyle style = book.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center; //垂直居中  
            for (int i = 0; i < lstTitle.Count; i++)
            {
                rowTitle.CreateCell(i).SetCellValue(lstTitle[i]);
            }

            for (int i = 0; i < list.Length; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
                fillCell(row, list, i);
            }

            for (int i = 0; i < lstTitle.Count; i++)
            {
                sheet.AutoSizeColumn(i); //i：根据标题的个数设置自动列宽
            }

            return book;
        }
    }
}
