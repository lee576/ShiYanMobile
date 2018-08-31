using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //app.UseMvc();

            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                StaticFileOptions = {ServeUnknownFileTypes = true},
                DefaultFilesOptions = {DefaultFileNames = new[] { "login.html" } },
            };
            app.UseFileServer(options);
        }
    }
}
