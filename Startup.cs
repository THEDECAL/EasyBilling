using EasyBilling.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace EasyBilling
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //���������� ����������� ��������
            services.AddAccessRightsManager();
            services.AddDbInitializer();
            services.AddTariffRegulator();
            services.AddEmailSender();

            //������� ����������� ��� ������ ������
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddRouting();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }
                app.UseHttpsRedirection(); //������������ ��������������� �� ���������� ��������
                //app.UseStatusCodePages(); //���������� ������ ���
                app.UseStaticFiles();

                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseCookiePolicy(); //����������� �������� �� �������� ����
                app.UseSession(); //������������ ������

                app.UseEndpoints(endpoints =>
                {

                    endpoints.MapControllerRoute(
                            name: "default",
                            pattern:
                                "{controller:alpha:length(0, 30)=home}/" +
                                "{action:alpha:length(0, 30)=index}/" +
                                "{id?}");
                    endpoints.MapRazorPages();
                });
        }
    }
}