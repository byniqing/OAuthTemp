using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace OIDCClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            services.AddAuthentication(option =>
            {
                /*
                 要想使用认证系统，必要先注册Scheme
                 而每一个Scheme必须指定一个Handler
                 AuthenticationHandler 负责对用户凭证的验证
                 这里指定的默认认证是cookie认证
                 Scheme可以翻译为方案，即默认的认证方案
                 */
                option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //option.DefaultChallengeScheme = OAuthDefaults.DisplayName;

                /*
                 因为这里用到了多个中间件，（AddAuthentication，AddCookie，AddOAuth）
                 所以要设置一个默认中间件OAuthDefaults.DisplayName 的默认值是OAuth
                 指定AddOAuth是默认中间件，在AddOAuth配置了很多选项

                如果只用了一个中间件，则可以不写，是否还记得cookie认证
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    ///Account/Login?ReturnUrl=%2Fadmin
                    option.LoginPath = "/login/index";
                    //option.ReturnUrlParameter = "params"; //指定参数名称
                    //option.Cookie.Domain
                    option.AccessDeniedPath = "/login/noAccess";
                    option.Cookie.Expiration = TimeSpan.FromSeconds(4);
                    option.Events = new CookieAuthenticationEvents
                    {
                        OnValidatePrincipal = LastChangedValidator.ValidateAsync
                    };
                });
                 */
                option.DefaultChallengeScheme = OAuthDefaults.DisplayName;
            })
            .AddCookie()
            .AddOAuth(OAuthDefaults.DisplayName,
            option => {
                option.ClientId = "oauth";
                option.ClientSecret = "secret";
                option.AuthorizationEndpoint = "http://localhost:5000/connect/authorize";
                option.TokenEndpoint = "http://localhost:5000/connect/token";
                option.CallbackPath = "/signin-oauth";
                //option.Scope.Clear();
                option.Scope.Add("api");
                // 事件执行顺序 ：
                // 1.创建Ticket之前触发
                //option.Events.OnCreatingTicket = context => Task.CompletedTask;
                // 2.创建Ticket失败时触发
                //option.Events.OnRemoteFailure = context => Task.CompletedTask;
                // 3.Ticket接收完成之后触发
                //option.Events.OnTicketReceived = context => Task.CompletedTask;
                // 4.Challenge时触发，默认跳转到OAuth服务器
                // options.Events.OnRedirectToAuthorizationEndpoint = context => context.Response.Redirect(context.RedirectUri);
            });
            //services.AddAuthentication(option =>
            //{
            //    option.DefaultScheme = "Cookie";
            //    option.DefaultChallengeScheme = "oidc";
            //})
            //.AddCookie("Cookie")
            //.AddOpenIdConnect("oidc", option =>
            //{
            //    option.ClientId = "clientid";
            //    option.ClientSecret = "secert";
            //    option.SignInScheme = "Cookie";
            //    option.Authority = "http://localhost:5000";
            //    option.SaveTokens = true;
            //    option.RequireHttpsMetadata = false;
            //    //option.Scope.Clear();
            //    option.Scope.Add("email");
            //    //option.Scope.Add("api");
            //});
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            //app.UseAuthorize();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
