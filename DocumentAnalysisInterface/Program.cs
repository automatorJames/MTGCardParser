using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MTGGlyphs.Data;
using Glyphotype.Interfaces;
using Glyphotype.GlyphAnalysisDTOs;
using Glyphotype.Distiller.Workbench;

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
        // everything in the process - this container and GlyphGrammar.Default alike, the latter
        // built whenever something first touches it - reads one already-resolved instance.
        builder.Services.AddSingleton(GlobalSettings.Current);
        builder.Services.AddSingleton(_ => GlyphGrammar.Default);
        builder.Services.AddScoped<ProtectedLocalStorage>();
        builder.Services.AddScoped<RuntimeSettings>();
        builder.Services.AddSingleton<IDocumentRepository, CardDataGetter>();
        builder.Services.AddSingleton<CorpusAnalyzer>();

        // Where glyph sources are read from and written to - by the Glyph editor, and by Grammar Tools commits.
        var glyphSources = new GlyphSourceLocation(
            Directory: Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "MTGGlyphs", "GlyphDefinitions")),
            Namespace: "MTGGlyphs.GlyphDefinitions");

        builder.Services.AddSingleton(glyphSources);

        // The Grammar Tools tab's working definition: edits are scored against the corpus the analyzer already
        // tokenized, saved outside the repo until committed, and committed into the glyph sources.
        builder.Services.AddSingleton(services => new GrammarWorkbench(
            GlyphGrammar.Default,
            services.GetRequiredService<CorpusAnalyzer>().ProcessedDocuments,
            new WorkbenchOptions(
                WorkingDefinitionPath: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Glyphotype", "working-grammar.json"),
                SourceDirectory: glyphSources.Directory,
                SourceNamespace: glyphSources.Namespace,
                AllowPartialSegmentMatches: GlobalSettings.Current.AllowPartialSegmentMatches)));

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