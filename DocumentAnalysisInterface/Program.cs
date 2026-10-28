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
        builder.Services.AddScoped<ProtectedLocalStorage>();
        builder.Services.AddScoped<RuntimeSettings>();
        builder.Services.AddSingleton<IDocumentRepository, CardDataGetter>();

        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Glyphotype");

        // Which of the corpus's volumes are read: those last chosen in the settings, kept here between runs.
        var corpusVolumesPath = Path.Combine(appData, "corpus-volumes.json");

        // The corpus as the compiled grammar tokenizes it. Pages don't read it directly: they read ActiveCorpus (below),
        // which starts from this and follows whichever workspace's grammar is active, and whichever volumes are chosen.
        builder.Services.AddSingleton(services => new CorpusAnalyzer(
            services.GetRequiredService<IDocumentRepository>(), GlyphGrammar.Default, ActiveCorpus.ReadVolumes(corpusVolumesPath)));

        // Where glyph sources are read from and written to - by Grammar Tools commits.
        var glyphSources = new GlyphSourceLocation(
            Directory: Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "MTGGlyphs", "GlyphDefinitions")),
            Namespace: "MTGGlyphs.GlyphDefinitions");

        builder.Services.AddSingleton(glyphSources);

        // The Grammar Tools tab's grammars: the one compiled from the glyph sources, plus any scratch grammars, each
        // scored against the corpus the analyzer already tokenized and kept outside the repo - the source one until
        // it's committed into the glyph sources, the scratch ones until they're exported.
        // Resolved by the startup below, once the corpus is read: the committed grammar's tokenization of it is made only when
        // something needs it (the corpus shown while nothing's changed, the committed score), so a working definition that
        // differs from the sources has the corpus tokenized once at startup, by itself, rather than twice.
        builder.Services.AddSingleton(services => new WorkspaceManager(
            new SourceWorkspace(
                Name: "MTGGlyphs",
                Grammar: GlyphGrammar.Default,
                Corpus: services.GetRequiredService<CorpusAnalyzer>().Documents ?? throw new InvalidOperationException("The corpus isn't read yet"),
                Documents: new(() =>
                {
                    var analyzer = services.GetRequiredService<CorpusAnalyzer>();
                    var startup = services.GetRequiredService<DocumentAnalysisInterface.Startup.AppStartup>();
                    analyzer.EnsureInitializedAsync(new Progress<(int Done, int Total)>(x => startup.Progress(x.Done, x.Total))).GetAwaiter().GetResult();
                    return analyzer.ProcessedDocuments;
                }),
                SourceDirectory: glyphSources.Directory,
                SourceNamespace: glyphSources.Namespace),
            root: Path.Combine(appData, "workspaces"),
            legacyWorkingDefinitionPath: Path.Combine(appData, "working-grammar.json"),
            configuredGuidance: WorkspaceGuidance(builder.Configuration.GetSection("WorkspaceGuidance"))));

        // What every tab shows: the corpus as the active workspace's grammar tokenizes it.
        builder.Services.AddSingleton(services => new ActiveCorpus(
            services.GetRequiredService<WorkspaceManager>(),
            services.GetRequiredService<CorpusAnalyzer>(),
            services.GetRequiredService<IDocumentRepository>(),
            corpusVolumesPath));

        // The grammar's lowering engine, if the corpus has one: what the Lowered view shows. Generic to the app - it only
        // reads Glyphotype.Lowering's contract - and composed here, where the grammar is.
        builder.Services.AddSingleton<Glyphotype.Lowering.ILoweringEngine>(new MTGRulesIR.Display.RulesLoweringEngine());
        builder.Services.AddSingleton(services => new LoweredCorpus(
            services.GetRequiredService<ActiveCorpus>(),
            services.GetService<Glyphotype.Lowering.ILoweringEngine>()));

        // An agent's tools over those same workspaces, served at /mcp: whatever an agent does there shows up live in
        // the Grammar Tools tab, and vice versa.
        // How agent sessions run: when to check in, and what a step must achieve. Optional - each setting has a default.
        var agentSettings = (builder.Configuration.GetSection("GrammarAgent").Get<AgentSessionSettings>() ?? new()) with
        {
            DocumentGlyphs = GlobalSettings.Current.AgentDocumentsGlyphs,
        };

        builder.Services.AddSingleton(services =>
        {
            var corpus = services.GetRequiredService<ActiveCorpus>();

            return new GrammarAgent(
                services.GetRequiredService<WorkspaceManager>(),
                settings: agentSettings,
                corpusDescription: () => "the text of cards in the card database"
                    + (corpus.Volumes.Kind == VolumeSelectionKind.All ? "" : $", from {corpus.Volumes.Describe()} (sets, by set sequence)"));
        });

        // The same tools, worked by an agent the app runs itself where there's a local one (the Claude Code CLI) to run:
        // the Grammar Tools tab's chat pane.
        builder.Services.AddSingleton(new LocalAgent(builder.Configuration.GetSection("LocalAgent").Get<LocalAgentSettings>() ?? new()));

        builder.Services.AddSingleton(services => new GrammarChat(
            services.GetRequiredService<LocalAgent>(),
            services.GetRequiredService<GrammarAgent>(),
            services.GetRequiredService<WorkspaceManager>(),
            services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>(),
            workingDirectory: Path.Combine(appData, "agent"),
            new SubscriptionCostEstimator(Path.Combine(appData, "agent", "cost-model.json"))));

        // Corpus Captures' chats about one line each: the agent writes the glyphs that capture it.
        builder.Services.AddSingleton(services => new LineChats((documentName, lineIndex, lineText) => new LineChat(
            services.GetRequiredService<LocalAgent>(),
            services.GetRequiredService<GrammarAgent>(),
            services.GetRequiredService<WorkspaceManager>(),
            services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>(),
            workingDirectory: Path.Combine(appData, "agent"),
            documentName, lineIndex, lineText)));

        builder.Services
            .AddMcpServer(options => options.ServerInstructions = GrammarAgent.Instructions)
            .WithHttpTransport()
            .WithTools<GrammarAgentTools>()
            .WithPrompts<GrammarAgentPrompts>();

        // What the app does before it can serve its pages, as it happens - shown by the one page there is until then (see App.razor).
        builder.Services.AddSingleton<DocumentAnalysisInterface.Startup.AppStartup>();

        var app = builder.Build();

        // The grammar, the corpus and the workspaces are app-wide data, not per-session state, so they're made once here
        // rather than by whichever page a user happens to land on first - but alongside serving, not before it: the web host
        // starts now, the browser gets a page showing the work as it goes, and everything that needs the work waits for it.
        var startup = app.Services.GetRequiredService<DocumentAnalysisInterface.Startup.AppStartup>();
        _ = Task.Run(() => StartUpAsync(app.Services, startup));

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

        // An agent that connects before the app is ready waits, rather than finding nothing to work on.
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/mcp"))
                await startup.WhenReady;

            await next(context);
        });

        app.MapMcp("/mcp");
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        await app.RunAsync();
    }

    /// <summary>
    /// Builds the grammar, reads the corpus, opens the workspaces and scores the working grammar, telling <paramref name="startup"/>
    /// how far it's got - then makes the agent's chat (which puts back the session settings last set there, which an agent in a
    /// terminal works under too) - and says it's ready. A failure anywhere is the startup's to show.
    /// </summary>
    static async Task StartUpAsync(IServiceProvider services, DocumentAnalysisInterface.Startup.AppStartup startup)
    {
        const string building = "Building the grammar";
        const string reading = "Reading the corpus";
        const string opening = "Opening the workspaces";
        const string scoring = "Scoring the working grammar";

        try
        {
            startup.Plan(building, reading, opening, scoring);

            startup.Begin(building);
            var analyzer = services.GetRequiredService<CorpusAnalyzer>();
            _ = GlyphGrammar.Default;

            startup.Begin(reading);
            await analyzer.LoadDocumentsAsync();

            startup.Begin(opening);
            var workspaces = services.GetRequiredService<WorkspaceManager>();

            // The active workspace's score, with its progress shown as it goes: the working grammar is built and the corpus
            // tokenized by it here, where the sources have uncommitted changes; else the committed grammar tokenizes it.
            startup.Begin(scoring);
            var workbench = workspaces.Active;
            void OnProgress() => startup.Progress(workbench.Progress?.Done ?? 0, workbench.Progress?.Total ?? 0, workbench.Progress?.Stage);
            workbench.ProgressChanged += OnProgress;

            try
            {
                await workbench.GetCurrentTrialAsync();
            }
            finally
            {
                workbench.ProgressChanged -= OnProgress;
            }

            services.GetRequiredService<ActiveCorpus>();
            services.GetRequiredService<GrammarChat>();
            startup.Ready();
        }
        catch (Exception exception)
        {
            startup.Fail(exception);
        }
    }

    /// <summary>
    /// Guidance by workspace name: each entry is the text itself, or (since JSON has no multi-line strings) an array of its lines.
    /// </summary>
    static Dictionary<string, string> WorkspaceGuidance(IConfigurationSection section) =>
        section.GetChildren().ToDictionary(
            x => x.Key,
            x => x.Value ?? string.Join(Environment.NewLine, x.GetChildren().OrderBy(line => int.Parse(line.Key)).Select(line => line.Value)));
}