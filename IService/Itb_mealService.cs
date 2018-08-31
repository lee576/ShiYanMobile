using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DbModel;
using Infrastructure.Service;
namespace IService
{
    public interface Itb_mealService : IServiceBase<tb_meal>
    {
        IEnumerable<tb_meal> FindMeals(int pageIndex, int pageSize, ref int total,
            Expression<Func<tb_meal, bool>> expression);
    }
}