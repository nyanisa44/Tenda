using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using TendaAdvisors.Models;
using TendaAdvisors.Providers;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;
using System.Net.Http;
using System.Web.Hosting;
using System.IO;
using Microsoft.AspNet.Identity.Owin;

[assembly: OwinStartup(typeof(TendaAdvisors.Startup))]

namespace TendaAdvisors
{
    public partial class Startup
    {
        static Lazy<byte[]> _indexFileContents = new Lazy<byte[]>(ReadIndexFileContents);
        static FileSystemWatcher _indexFileWatcher = null;

        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.EnsureInitialized();

            UseAuthentication(app);

            app.Map("/api", api =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                api.UseWebApi(config);
            });

            UseSpaRouting(app);
        }

        public void UseAuthentication(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/token"),
                Provider = new ApplicationOAuthProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            });
        }

        private void UseSpaRouting(IAppBuilder app)
        {
            app.Use(async (ctx, next) =>
            {
                //return the index page for pretty much all get requests
                if (ctx.Request.Method == "GET" && IsSpaPath(ctx.Request.Path))
                {
                    var bytes = _indexFileContents.Value;
                    ctx.Response.ContentType = "text/html; charset=UTF-8";
                    ctx.Response.ContentLength = bytes.Length;
                    await ctx.Response.WriteAsync(bytes);
                }
                else
                {
                    await next();
                }
            });
        }

        private static bool IsSpaPath(PathString path)
        {
            //matches all paths except api paths and those with periods in them.
            if (path.Value.StartsWith("api")) return false;
            if (path.Value.Contains('.')) return false;
            return true;
        }

        private static byte[] ReadIndexFileContents()
        {
            string file = "Index.html";
            var filePaths = new List<string> { Path.Combine(Environment.CurrentDirectory, file) };

            if (HostingEnvironment.IsHosted)
            {
                filePaths.Insert(0, HostingEnvironment.MapPath("~/" + file));
            }

            foreach (var path in filePaths)
            {
                if (File.Exists(path))
                {
                    var bytes = File.ReadAllBytes(path);

                    if (_indexFileWatcher == null)
                    {
                        _indexFileWatcher = new FileSystemWatcher(Path.GetDirectoryName(path), Path.GetFileName(path));
                        FileSystemEventHandler fseh = (s, e) => _indexFileContents = new Lazy<byte[]>(() => File.ReadAllBytes(path));
                        RenamedEventHandler fsrh = (s, e) => _indexFileContents = new Lazy<byte[]>(() => File.ReadAllBytes(path));
                        _indexFileWatcher.Changed += fseh;
                        _indexFileWatcher.Created += fseh;
                        _indexFileWatcher.Renamed += fsrh;
                        _indexFileWatcher.EnableRaisingEvents = true;
                    }

                    return bytes;
                }
            }

            throw new Exception($"Could not find {file}. Looked here:\r\n    " + string.Join("\r\n    ", filePaths));
        }
    }
}
