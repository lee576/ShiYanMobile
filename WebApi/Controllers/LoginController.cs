using System;
using System.Linq;
using IService;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using ServiceStack.Redis;
using WebApi.Utility;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("any")]
    public class LoginController : Controller
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
        private Itb_areainfoService _tb_areainfo;
        private readonly Itb_userService _tb_user;

        /// <inheritdoc />
        public LoginController(Itb_areainfoService tb_areainfo, Itb_userService tb_user)
        {
            _tb_areainfo = tb_areainfo;
            _tb_user = tb_user;
        }

        /// <summary>
        /// 生成登录验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CreateValidateCode")]
        public JsonResult CreateValidateCode()
        {
            //首先实例化验证码的类
            ValidateCodeHelper validateCode = new ValidateCodeHelper();
            //生成验证码指定的长度
            string code = validateCode.GetRandomString(4);
            //创建验证码的图片
            var base64Str = validateCode.CreateImage(code);
            var token = Guid.NewGuid().ToString("N");
            using (RedisClient redisClient = RedisHelper.CreateClient())
            {
                redisClient.Set(token, code, TimeSpan.FromMinutes(5));
            }
            return Json(new
            {
                base64Str,
                token
            });
        }

        /// <summary>
        /// 检验用户身份及登录验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("CheckLogin")]
        public JsonResult CheckLogin([FromBody]LoginViewModel model)
        {
            try
            {
                var findUser = _tb_user.FindByClause(t => t.username == model.UserName && t.password == model.Password);
                if (findUser == null)
                {
                    return Json(new
                    {
                        code = JsonReturnMsg.FailCode,
                        msg = "用户名或密码错误!"
                    });
                }

                var token = string.Empty;
                var headers = HttpContext.Request.Headers;
                if (headers.TryGetValue("validateKey", out var headerValues))
                {
                    token = headerValues.First();
                }
                using (RedisClient redisClient = RedisHelper.CreateClient())
                {
                    var cacheValidateCode = redisClient.Get<string>(token);
                    if (cacheValidateCode != null)
                    {
                        if (String.Equals(model.ValidateCode.Trim(), cacheValidateCode.Trim(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            return Json(new
                            {
                                code = JsonReturnMsg.SuccessCode,
                                msg = JsonReturnMsg.GetSuccess
                            });
                        }
                        return Json(new
                        {
                            code = JsonReturnMsg.FailCode,
                            msg = "验证码输入错误!"
                        });
                    }
                    return Json(new
                    {
                        code = JsonReturnMsg.FailCode,
                        msg = "验证码已过期,请重新刷新!"
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error("错误:" + ex);
                return Json(new
                {
                    code = JsonReturnMsg.FailCode,
                    msg = "登录错误,系统异常,请联系管理员!"
                });
            }
        }
    }
}