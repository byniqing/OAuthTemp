using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthorizationServer.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Test;

namespace AuthorizationServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly TestUserStore _userStore;

        public AccountController(TestUserStore userStore)
        {
            _userStore = userStore;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("index");
        }
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            //ViewBag.returnUrl11 = returnUrl;
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var user = _userStore.FindByUsername(login.UserName);
                if (user != null)
                {
                    #region 刚开始这样登录是不行的
                    var claims = new List<Claim> {
                        new Claim("name",login.UserName)
                     };
                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

                    //这种方式是cookie认证，这样登录无效
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        claimsPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = true, //
                            ExpiresUtc = DateTime.Now.AddDays(5)
                        });
                    #endregion

                    #region 这样登录才正确
                    var p = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.Now.AddDays(5)
                    };

                    //identityserver 是身份验证 ，identity 所以要用该方法
                    /*
                     * 
                     */
                    //Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(HttpContext, user.SubjectId, user.Username, p);

                    //或者
                    HttpContext.SignInAsync(user.SubjectId, user.Username, p);
                    //登录成功，跳转到同意授权页面
                    return Redirect(login.ReturnUrl);
                    #endregion
                }
            }
            //else
                return View("login");
        }

        [HttpPost]
        public IActionResult Show()
        {
            return View();
        }
    }
}