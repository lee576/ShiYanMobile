using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DbModel;
using Infrastructure;
using IService;
using Infrastructure.Service;
using SqlSugar;

namespace Service
{
    public class tb_mealService : GenericService<tb_meal>,Itb_mealService
    {
        public IEnumerable<tb_meal> FindMeals(int pageIndex, int pageSize, ref int totalCount,
            Expression<Func<tb_meal, bool>> expression)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var page = db.Queryable<tb_meal>().Where(expression).OrderBy(t => t.tradeTime, OrderByType.Desc).ToPageList(pageIndex, pageSize, ref totalCount);
                return page;
            }
        }
    }
}