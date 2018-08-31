using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrastructure.Service
{
    public abstract class GenericService<T> : IServiceBase<T>, IDependency where T : class, new()
    {
        #region Implementation of IService<T>

        public IEnumerable<T> Top(int topNum)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var entity = db.Queryable<T>().Take(topNum).ToList();
                return entity;
            }
        }

        public IEnumerable<T> Top(int topNum, Expression<Func<T, bool>> predicate, string orderBy = "")
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var query = db.Queryable<T>().Take(topNum).Where(predicate);
                if (!string.IsNullOrEmpty(orderBy))
                {
                    query = query.OrderBy(orderBy);
                }
                var entities = query.ToList();
                return entities;
            }
        }

        /// <summary>
        /// 根据主值查询单条数据
        /// </summary>
        /// <param name="pkValue">主键值</param>
        /// <returns>泛型实体</returns>
        public T FindById(object pkValue)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var entity = db.Queryable<T>().InSingle(pkValue);
                return entity;
            }
        }

        /// <summary>
        /// 查询所有数据(无分页,请慎用)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> FindAll()
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var list = db.Queryable<T>().ToList();
                return list;
            }
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <param name="orderBy">排序</param>
        /// <returns>泛型实体集合</returns>
        public IEnumerable<T> FindListByClause(Expression<Func<T, bool>> whereLambda,
            Expression<Func<T, object>> orderLambda,
            OrderByType orderType = OrderByType.Asc)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var query = db.Queryable<T>().Where(whereLambda).OrderBy(orderLambda, orderType);
                var entities = query.ToList();
                return entities;
            }
        }

        public IEnumerable<T> FindListByOrder(Expression<Func<T, object>> orderLambda,
            OrderByType orderType = OrderByType.Asc)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var query = db.Queryable<T>();
                query = query.OrderBy(orderLambda, orderType);
                var entities = query.ToList();
                return entities;
            }
        }

        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="predicate">条件表达式树</param>
        /// <returns></returns>
        public T FindByClause(Expression<Func<T, bool>> predicate)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var entity = db.Queryable<T>().First(predicate);
                return entity;
            }
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public long Insert(T entity)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                //返回插入数据的标识字段值
                var i = db.Insertable(entity).ExecuteReturnBigIdentity();
                return i;
            }
        }

        /// <summary>
        /// 批量写入实体数据
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public long Insert(List<T> entities)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var i = db.Insertable(entities).ExecuteReturnBigIdentity();
                return i;
            }
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                //这种方式会以主键为条件
                var i = db.Updateable(entity).ExecuteCommand();
                return i > 0;
            }
        }

        public bool Update(Expression<Func<T, bool>> @where)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var i = db.Updateable<T>(@where).ExecuteCommand();
                return i > 0;
            }
        }

        /// <summary>
        /// 根据主键更新指定的列
        /// </summary>
        /// <param name="entity"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool UpdateColumnsById(T entity, Expression<Func<T, object>> columns)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var i = db.Updateable(entity).UpdateColumns(columns).ExecuteCommand();
                return i > 0;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var i = db.Deleteable(entity).ExecuteCommand();
                return i > 0;
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where">过滤条件</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> @where)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var i = db.Deleteable<T>(@where).ExecuteCommand();
                return i > 0;
            }
        }


        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool DeleteByIds(object[] ids)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var i = db.Deleteable<T>().In(ids).ExecuteCommand();
                return i > 0;
            }
        }

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex">当前页面索引</param>
        /// <param name="pageSize">分布大小</param>
        /// <returns></returns>
        public IPagedList<T> FindPagedList(Expression<Func<T, bool>> predicate, string orderBy = "", int pageIndex = 1, int pageSize = 20)
        {
            using (var db = DbFactory.GetSqlSugarClient())
            {
                var totalCount = 0;
                var page = db.Queryable<T>().OrderBy(orderBy).ToPageList(pageIndex, pageSize, ref totalCount);
                var list = new PagedList<T>(page, pageIndex, pageSize, totalCount);
                return list;
            }
        }
        #endregion
    }
}
