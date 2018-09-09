using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using IService;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.ViewModels;
using ServiceStack.Redis;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("any")]
    public class AuthorizeController : Controller
    {
        private readonly JwtSettings _jwtSettings;
        private readonly Itb_userService _tb_user;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtSettingsAccesser"></param>
        public AuthorizeController(IOptions<JwtSettings> jwtSettingsAccesser, Itb_userService tb_user)
        {
            _jwtSettings = jwtSettingsAccesser.Value;
            _tb_user = tb_user;
        }

        /// <summary>
        /// 登录验证拿到Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Token([FromBody]LoginViewModel model)
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

            //验证码
            var validateCode = string.Empty;
            var headers = HttpContext.Request.Headers;
            if (headers.TryGetValue("validateKey", out var headerValues))
            {
                validateCode = headerValues.First();
            }
            using (RedisClient redisClient = RedisHelper.CreateClient())
            {
                var cacheValidateCode = redisClient.Get<string>(validateCode);
                if (cacheValidateCode != null)
                {
                    if (!String.Equals(model.ValidateCode.Trim(), cacheValidateCode.Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        return Json(new
                        {
                            code = JsonReturnMsg.FailCode,
                            msg = "验证码输入错误!"
                        });
                    }
                }
            }

            var claim = new[]{
                    new Claim(ClaimTypes.Name, model.UserName),
                    new Claim(ClaimTypes.Role,"admin")
                };

            //对称秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            //签名证书(秘钥，加密算法)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //生成token  [注意]需要nuget添加Microsoft.AspNetCore.Authentication.JwtBearer包，并引用System.IdentityModel.Tokens.Jwt命名空间
            var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claim, DateTime.Now, DateTime.Now.AddMinutes(30), creds);

            //return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });

            return Json(new
            {
                code = JsonReturnMsg.SuccessCode,
                msg = JsonReturnMsg.GetSuccess,
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}