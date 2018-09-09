using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using DbModel;
using IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using SqlSugar;
using WebApi.Utility;

namespace WebApi.Controllers
{
    /// <summary>
    /// 套餐
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("any")]
    public class MealController : Controller
    {
        private readonly Itb_mealService _tbMeal;
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public MealController(Itb_mealService tbMeal)
        {
            _tbMeal = tbMeal;
        }

        /// <summary>
        /// 获得套餐交易记录分页结果
        /// </summary>
        /// <param name="sEcho"></param>
        /// <param name="iDisplayStart"></param>
        /// <param name="iDisplayLength"></param>
        /// <param name="nameOrMobile">姓名或手机号</param>
        /// <param name="startName">开始时间</param>
        /// <param name="endName">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public JsonResult Meals(int sEcho,
            int iDisplayStart,
            int iDisplayLength,
            string nameOrMobile,
            string startName,
            string endName)
        {
            try
            {
                int pageStart = iDisplayStart;
                int pageSize = iDisplayLength;
                int pageIndex = (pageStart / pageSize) + 1;
                int totalRecordNum = default(int);
                var expression = CreateCondition(nameOrMobile, startName, endName);
                var pageRecords = _tbMeal.FindMeals(pageIndex, pageSize, ref totalRecordNum, expression);
                return Json(new
                {
                    code = JsonReturnMsg.SuccessCode,
                    sEcho,
                    iTotalRecords = totalRecordNum,
                    iTotalDisplayRecords = totalRecordNum,
                    aaData = pageRecords
                });
            }
            catch (Exception ex)
            {
                Log.Error("错误:" + ex);
                return Json(new
                {
                    code = JsonReturnMsg.FailCode,
                    msg = JsonReturnMsg.GetFail
                });
            }
        }

        /// <summary>
        /// 拼接查询条件
        /// </summary>
        /// <param name="nameOrMobile"></param>
        /// <param name="startName"></param>
        /// <param name="endName"></param>
        /// <returns></returns>
        private static Expression<Func<tb_meal, bool>> CreateCondition(string nameOrMobile, string startName, string endName)
        {
            //Sql Server 最小时间是1753-01-01 00:00:00
            var startTime = string.IsNullOrEmpty(startName)
                ? DateTime.Parse("1753-01-01 00:00:00")
                : DateTime.Parse(DateTime.Parse(startName).ToString("yyyy-MM-dd 00:00:00"));
            var endTime = string.IsNullOrEmpty(endName)
                ? DateTime.MaxValue
                : DateTime.Parse(DateTime.Parse(endName).ToString("yyyy-MM-dd 23:59:59"));

            Expression<Func<tb_meal, bool>> expression;
            if (!string.IsNullOrEmpty(nameOrMobile))
                expression = t =>
                    (t.mobile == nameOrMobile || t.realname == nameOrMobile) &&
                    (t.tradeTime >= startTime && t.tradeTime <= endTime);
            else
                expression = t => t.tradeTime >= startTime && t.tradeTime <= endTime;
            return expression;
        }

        /// <summary>
        /// 导出套餐Excel
        /// </summary>
        /// <param name="nameOrMobile">姓名或手机号</param>
        /// <param name="startName">开始时间</param>
        /// <param name="endName">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public FileContentResult ExportExcel(string nameOrMobile, string startName, string endName)
        {
            //查询套餐
            var list = GetMeals(nameOrMobile, startName, endName);
            //生成Excel WorkBook
            var lstTitle = new List<string> { "手机号", "姓名", "档次", "交易时间" };
            var fileName = @"交易记录.xls";
            var book = ExcelHelper.CreateWorkbook(lstTitle, list, FillCell);
            //导出Excel
            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(StreamToBytes(ms), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// 添加套餐
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddMeal([FromBody]JObject obj)
        {
            try
            {
                var insertObj = new tb_meal
                {
                    mobile = obj["mobile"] + "",
                    realname = obj["realname"] + "",
                    grade = obj["grade"] + "",
                    tradeTime = DateTime.Now
                };
                var inertReturn = _tbMeal.Insert(insertObj);
                return Json(new
                {
                    code = JsonReturnMsg.SuccessCode,
                    msg = JsonReturnMsg.AddSuccess,
                    data = inertReturn
                });
            }
            catch (Exception ex)
            {
                Log.Error("错误:" + ex);
                return Json(new
                {
                    code = JsonReturnMsg.FailCode,
                    msg = JsonReturnMsg.AddFail
                });
            }
        }

        private tb_meal[] GetMeals(string nameOrMobile, string startName, string endName)
        {
            var expression = CreateCondition(nameOrMobile, startName, endName);
            var list = _tbMeal.FindListByClause(expression, t => t.tradeTime, OrderByType.Desc).ToArray();
            return list;
        }

        private static void FillCell(IRow row, tb_meal[] list, int i)
        {
            row.CreateCell(0).SetCellValue(list[i].mobile);
            row.CreateCell(1).SetCellValue(list[i].realname);
            row.CreateCell(2).SetCellValue(list[i].grade);
            row.CreateCell(3).SetCellValue(DateTime.Parse(list[i].tradeTime + "").ToString("yyyy-MM-dd hh:mm:ss"));
            row.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellType(CellType.String);
        }

        private byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            return bytes;
        }
    }
}
