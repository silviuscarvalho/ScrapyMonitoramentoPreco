using MonitoramentoRede.Web.Extensions;
using MonitoramentoRede.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.AdicionarServicosMonitoramento();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<DashboardHub>("/hubs/dashboard");
app.MapRazorPages();

app.Run();

public partial class Program
{
}
