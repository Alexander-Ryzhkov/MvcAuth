using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MvcAuth.Models;
using MvcAuth.Services;
using Microsoft.AspNet.Authentication.Facebook;
using Microsoft.AspNet.Authentication.Twitter;
//using Extensions.AspNet.Authentication.Vkontakte;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.DataProtection;
//using Microsoft.AspNet.Http.Abstractions;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.Dnx.Runtime;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Authentication;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Newtonsoft.Json.Linq;


namespace MvcAuth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseCookieAuthentication(options =>
            {

                options.AutomaticAuthenticate = true;
                options.LoginPath = new PathString("/login");
            });

            ConfigureVk(app);

            app.UseFacebookAuthentication(options =>
            {
                options.AppId = "1007890662636921";
                options.AppSecret = "e1e9360c600a54e1076d18e454d379ef";
            });

            app.UseTwitterAuthentication(options =>
            {
                options.ConsumerKey = "e0pMlTBhtrjUzjOG5hArv9uXk";
                options.ConsumerSecret = "t5hVRHQXMNpPgpELogNqMWZjo8QXV4WAbEGp2nkeqTYJ65yJ4w";
            });

            //app.Map("/login", sign =>
            //{
            //    sign.Run(async context =>
            //    {
            //        var authType = context.Request.Query["authscheme"];
            //        if (!string.IsNullOrEmpty(authType))
            //        {
            //            // By default the client will be redirect back to the URL that issued the challenge (/login?authtype=foo),
            //            // send them to the home page instead (/).
            //            await context.Authentication.ChallengeAsync(authType, new AuthenticationProperties() { RedirectUri = "/" });
            //            return;
            //        }
            //    });
            //});

            //app.UseVkontakteAuthentication(options =>
            //{
            //    options.ClientId = "5422538";
            //    options.ClientSecret = "mG6g5eNAcErMireSei8k";
            //    options.SaveTokensAsClaims = true;
            //});


            //VkontakteAppBuilderExtensions.UseVkontakteAuthentication(this, options =>
            //{
            //    options.ClientId = "5422538";
            //    options.ClientSecret = "mG6g5eNAcErMireSei8k";
            //    options.SaveTokensAsClaims = true;
            //});
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            
        }
        
        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        private static void ConfigureVk(IApplicationBuilder app)
        {
            app.UseOAuthAuthentication(new OAuthOptions
            {
                AuthenticationScheme = "Vk",
                DisplayName = "Vk",
                ClientId = "5422538",
                ClientSecret = "mG6g5eNAcErMireSei8k",
                CallbackPath = new PathString("/SignIn/vkontakte/"),
                AuthorizationEndpoint = "https://oauth.vk.com/authorize",
                TokenEndpoint = "https://oauth.vk.com/access_token",
                Scope = { "profile" },
                Events = new OAuthEvents()
                {
                    OnCreatingTicket = async (context) =>
                    {
                        var userId = context.TokenResponse.Response["user_id"].ToString();

                        string userInfoLink = "https://api.vk.com/method/" + "users.get.json" +
                                      "?user_ids=" + Uri.EscapeDataString(userId) +
                                      "&fields=" + Uri.EscapeDataString("photo_medium"); //nickname,screen_name

                        var request = new HttpRequestMessage(HttpMethod.Get, userInfoLink);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var user = JObject.Parse(await response.Content.ReadAsStringAsync()).Property("response").First().First();

                        context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Value<string>("uid")));

                        var firstName = user.Value<string>("first_name");
                        var lastName = user.Value<string>("last_name");

                        context.Identity.AddClaim(new Claim(ClaimTypes.Name, firstName + " " + lastName));

                        context.Identity.AddClaim(new Claim("Photo", user.Value<string>("photo_medium")));
                    }
                }
            });

        }


    }
}
