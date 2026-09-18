using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glyphotype;

/// <summary>
/// The application-wide configuration Glyphotype itself reads, bound from the <c>GlobalSettings</c>
/// section of the host's appsettings files. Resolved once, on first use, by <see cref="Current"/> - see
/// there for why this loads itself rather than being handed in by a host.
/// </summary>
public class GlobalSettings
{
    const string _sectionName = nameof(GlobalSettings);
    const string _baseFileName = "appsettings.json";

    public string SqlConnString { get; init; }
    public int? MaxSetSequence { get; init; }
    public bool IncludeEmptyDocuments { get; init; }

    /// <summary>
    /// Whether a top-level token may match only part of a segment - a segment being everything from the
    /// start of the tokenization scope up to but not including the next period, or through the end of the
    /// line, whichever comes first (a line may hold several).
    /// <para>
    /// When true (the Tokenizer's historical behavior, which was implicit rather than chosen), a top-level
    /// token that ends at any word boundary is accepted, so "The dog runs" can be tokenized out of "The dog
    /// runs very fast" and the remainder left to whatever matches next. When false, that match is rejected
    /// and the segment is only tokenized if a single top-level type consumes all of it - which is what
    /// surfaces an incompletely-modeled line as unmatched text instead of hiding it behind a partial match.
    /// </para>
    /// <para>
    /// Individual top-level types opt back out of the requirement with
    /// <see cref="AllowPartialSegmentMatchAttribute"/>.
    /// </para>
    /// </summary>
    public bool AllowPartialSegmentMatches { get; init; }

    /// <summary>
    /// The settings this process is running under, resolved on first access and fixed thereafter.
    /// <para>
    /// Self-loading rather than host-injected because the things that need it are reached statically -
    /// <see cref="GlyphTypeRegistry"/> and the <see cref="Tokenizers.Tokenizer"/> it owns are built by a
    /// static constructor that runs at whatever moment some caller first touches the registry. Nothing can
    /// reliably run *before* that to hand settings in, so the alternative is building with defaults and
    /// having a host apply the real values afterwards - which leaves a window where the registry is
    /// initialized but misconfigured, and silently wrong if a host ever forgets to close it. Reading the
    /// same appsettings the host reads is the same move the registry already makes for Glyph types: go
    /// look at what shipped next to the executing assembly and work it out.
    /// </para>
    /// </summary>
    public static GlobalSettings Current { get; } = Load();

    /// <summary>
    /// Binds <see cref="Current"/> from the <c>GlobalSettings</c> section of <c>appsettings.json</c>
    /// overlaid with <c>appsettings.{environment}.json</c>, both looked for next to the executing
    /// assembly (where the build drops them). Mirrors the layering a generic host would apply, so a
    /// host binding the same section through <c>IConfiguration</c> would land on the same values.
    /// </summary>
    static GlobalSettings Load()
    {
        JObject merged = null;

        foreach (var fileName in GetFileNamesInPrecedenceOrder())
        {
            var section = ReadSection(Path.Combine(AppContext.BaseDirectory, fileName));

            if (section == null)
                continue;

            if (merged == null)
                merged = section;
            else
                // Later files win key by key, exactly as a layered IConfiguration would resolve them -
                // an environment overlay that only sets one key leaves the rest of the base file intact.
                merged.Merge(section, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace });
        }

        if (merged == null)
            throw new InvalidOperationException(
                $"No '{_sectionName}' section was found in any of [{string.Join(", ", GetFileNamesInPrecedenceOrder())}] " +
                $"under {AppContext.BaseDirectory}.");

        return merged.ToObject<GlobalSettings>();
    }

    /// <summary>
    /// The base file, then the environment-specific overlay if one is named - lowest precedence first,
    /// since <see cref="Load"/> merges in the order this yields.
    /// </summary>
    static IEnumerable<string> GetFileNamesInPrecedenceOrder()
    {
        yield return _baseFileName;

        if (GetEnvironmentName() is string environmentName)
            yield return $"appsettings.{environmentName}.json";
    }

    /// <summary>
    /// The host environment name, or null if unset. Checks both variables the generic host does, so a web
    /// host (ASPNETCORE_) and a console host (DOTNET_) each resolve the overlay they'd expect.
    /// </summary>
    static string GetEnvironmentName()
    {
        var environmentName =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.IsNullOrWhiteSpace(environmentName) ? null : environmentName;
    }

    /// <summary>
    /// The <c>GlobalSettings</c> object out of <paramref name="filePath"/>, or null if the file doesn't
    /// exist or carries no such section. A file that exists but can't be parsed is an error rather than a
    /// silent miss: falling back to defaults there would quietly change how the corpus tokenizes.
    /// </summary>
    static JObject ReadSection(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        JObject root;

        try
        {
            root = JObject.Parse(File.ReadAllText(filePath));
        }
        catch (JsonException e)
        {
            throw new InvalidOperationException($"Could not parse {filePath} while loading {_sectionName}.", e);
        }

        return root[_sectionName] as JObject;
    }
}
