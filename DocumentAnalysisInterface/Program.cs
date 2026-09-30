using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MTGGlyphs.Data;
using Glyphotype.Interfaces;
using Glyphotype.GlyphAnalysisDTOs;
using Glyphotype.Distiller.Workbench;
using Glyphotype.Distiller.Agent;
using Glyphotype.Distiller.Workspaces;
using DocumentAnalysisInterface.Agent;

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

        // The Grammar Tools tab's grammars: the one compiled from the glyph sources, plus any scratch grammars, each
        // scored against the corpus the analyzer already tokenized and kept outside the repo - the source one until
        // it's committed into the glyph sources, the scratch ones until they're exported.
        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Glyphotype");

        builder.Services.AddSingleton(services => new WorkspaceManager(
            new SourceWorkspace(
                Name: "MTGGlyphs",
                Grammar: GlyphGrammar.Default,
                Documents: services.GetRequiredService<CorpusAnalyzer>().ProcessedDocuments,
                SourceDirectory: glyphSources.Directory,
                SourceNamespace: glyphSources.Namespace),
            root: Path.Combine(appData, "workspaces"),
            allowPartialSegmentMatches: GlobalSettings.Current.AllowPartialSegmentMatches,
            legacyWorkingDefinitionPath: Path.Combine(appData, "working-grammar.json")));

        // An agent's tools over those same workspaces, served at /mcp: whatever an agent does there shows up live in
        // the Grammar Tools tab, and vice versa.
        var maxSetSequence = GlobalSettings.Current.MaxSetSequence;
        // How agent sessions run: when to check in, and what a step must achieve. Optional - each setting has a default.
        var agentSettings = builder.Configuration.GetSection("GrammarAgent").Get<AgentSessionSettings>() ?? new();

        builder.Services.AddSingleton(services => new GrammarAgent(
            services.GetRequiredService<WorkspaceManager>(),
            settings: agentSettings,
            corpusDescription: "the text of cards in the card database" + (maxSetSequence is int sets ? (sets == 1 ? ", from the first set" : $", from the first {sets} sets") : "")));

        builder.Services
            .AddMcpServer(options => options.ServerInstructions = GrammarAgent.Instructions)
            .WithHttpTransport()
            .WithTools<GrammarAgentTools>()
            .WithPrompts<GrammarAgentPrompts>();

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

        // Local agent clients connect to /mcp over plain http, and don't follow a redirect to the dev certificate.
        app.UseWhen(context => !context.Request.Path.StartsWithSegments("/mcp"), branch => branch.UseHttpsRedirection());

        app.UseStaticFiles();

        app.UseRouting();

        app.MapMcp("/mcp");
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        await app.RunAsync();
    }
}