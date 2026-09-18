using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MTGGlyphs.Data;
using Glyphotype.Interfaces;
using Glyphotype.GlyphAnalysisDTOs;

namespace DocumentAnalysisInterface;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        // Registered from GlobalSettings.Current rather than bound off builder.Configuration, so that
        // everything in the process - this container and Glyphotype's static registry alike, the latter
        // initializing whenever something first touches it - reads one already-resolved instance.
        builder.Services.AddSingleton(GlobalSettings.Current);
        builder.Services.AddScoped<ProtectedLocalStorage>();
        builder.Services.AddScoped<RuntimeSettings>();
        builder.Services.AddSingleton<IDocumentRepository, CardDataGetter>();
        builder.Services.AddSingleton<CorpusAnalyzer>();

        var app = builder.Build();

        // The corpus analyzer is app-wide data, not per-session state, so it's warmed
        // once here rather than being lazily triggered by whichever page a user happens
        // to land on first.
        await app.Services.GetRequiredService<CorpusAnalyzer>().EnsureInitializedAsync();

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