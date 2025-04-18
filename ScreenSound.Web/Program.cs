using MudBlazor.Services;
using ScreenSound.Web.Components;
using ScreenSound.Web.Services;

namespace ScreenSound.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddTransient<ArtistasApi>();
        builder.Services.AddTransient<MusicaApi>();

        builder.Services.AddHttpClient("API", client => {
            client.BaseAddress = new Uri(builder.Configuration["APIServer:Url"]!);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        builder.Services.AddMudServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
