using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MTGPlexer.Data;
using MTGPlexer.Interfaces;
using MTGPlexer.TokenAnalysisDTOs;

namespace CardAnalysisInterface;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddScoped<ProtectedLocalStorage>();
        builder.Services.AddScoped<RuntimeSettings>();

        // Load the corpus up front with a genuine await (not a blocking .Result), so the
        // rest of app startup stays on the normal async host-building path.
        CardDataGetter cardDataGetter = new(builder.Configuration["SqlConnString"], 1);
        List<IDocument> documents = await cardDataGetter.GetCardsAsync();
        builder.Services.AddSingleton(documents);
        builder.Services.AddSingleton<CorpusAnalyzer>();

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

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        await app.RunAsync();
    }
}