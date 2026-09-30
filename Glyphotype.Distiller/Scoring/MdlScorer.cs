namespace Glyphotype.Distiller.Scoring;

/// <summary>
/// Scores a grammar against a corpus by minimum description length: the bits to describe the grammar
/// (<see cref="GrammarCost"/>) plus the bits to describe the corpus given it (<see cref="CorpusEncoding"/>).
/// Lower is better. Both extremes pay: a grammar that matches little leaves the corpus to the residual code,
/// and a grammar that spells the corpus out in literals pays for every character in its own description.
/// </summary>
public static class MdlScorer
{
    /// <summary>Scores <paramref name="grammar"/> against <paramref name="documents"/>, which it tokenized.</summary>
    public static MdlScore Score(GlyphGrammar grammar, IReadOnlyList<ProcessedDocument> documents)
    {
        var lines = documents.SelectMany(x => x.Lines).ToList();

        // Every character literal text could need, plus a terminator.
        var charBits = CodeLength.Uniform(lines.SelectMany(x => x.SourceText.FormattedText).Distinct().Count() + 1);

        var definition = grammar.ToDefinition();
        var grammarCost = GrammarCost.Of(definition, charBits);
        var encoding = CorpusEncoding.Encode(lines, grammar.TopLevelTypes.Count, charBits);
        var componentBits = encoding.GetComponentBits();

        // The baseline: the same corpus with no grammar at all, every segment unmatched.
        var emptyGrammar = new GlyphGrammar([], grammar.AllowPartialSegmentMatches);
        var baselineLines = documents.SelectMany(x => ProcessedLine.GetAll(x.Document, emptyGrammar));
        var baselineBits = GrammarCost.Of(new GrammarDefinition(), charBits).TotalBits
            + CorpusEncoding.Encode(baselineLines, topLevelTypeCount: 0, charBits).GetComponentBits().Values.Sum();

        var topLevelNames = grammar.TopLevelTypes.Select(x => x.Name).ToHashSet();
        var tokensByGlyph = encoding.Tokens
            .Where(x => x.Unit is Glyph)
            .ToLookup(x => x.Unit.Type.Name);

        var glyphs = definition.Glyphs
            .Select(glyph =>
            {
                var tokens = tokensByGlyph[glyph.Name].ToList();

                return new GlyphContribution(
                    Name: glyph.Name,
                    IsTopLevel: topLevelNames.Contains(glyph.Name),
                    DefinitionBits: grammarCost.GlyphBits[glyph.Name],
                    Occurrences: tokens.Count,
                    Words: tokens.Sum(x => CountWords(x.Unit.CaptureValue) - x.Unit.UnresolvedTraces.Sum(y => CountWords(y.CaptureValue))),
                    DataBits: tokens.Sum(encoding.GetBits),
                    ResidualEquivalentBits: tokens.Sum(x => encoding.GetResidualBits(x.Unit.CaptureValue)));
            })
            .ToList();

        // Unmatched tokens, and text matches hold unresolved (see AllowUnmatchedAttribute) - priced as unmatched text alone would be.
        var residuals = encoding.Tokens
            .Where(x => x.Unit is UnmatchedString)
            .Select(x => (Text: x.Unit.CaptureValue.Trim(), Bits: encoding.GetBits(x)))
            .Concat(encoding.Tokens
                .Where(x => x.Unit is Glyph)
                .SelectMany(x => x.Unit.UnresolvedTraces)
                .Select(x => (Text: x.CaptureValue.Trim(), Bits: encoding.GetResidualBits(x.CaptureValue))))
            .GroupBy(x => x.Text)
            .Select(x => new ResidualSpan(x.Key, x.Count(), CountWords(x.Key), x.Sum(y => y.Bits)))
            .ToList();

        return new MdlScore
        {
            GrammarBits = grammarCost.TotalBits,
            ComponentBits = componentBits,
            BaselineBits = baselineBits,
            CharBits = charBits,
            Documents = documents.Count,
            Lines = lines.Count,
            Words = documents.Sum(x => x.WordCount),
            CapturedWords = documents.Sum(x => x.CapturedWordCount),
            Glyphs = glyphs,
            Vocabularies = grammarCost.VocabularyBits,
            Residuals = residuals,
            UnlocatedPatternMatches = encoding.UnlocatedPatternMatches,
        };
    }

    static int CountWords(string text) => text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
}

/// <summary>What one glyph costs and what it saves.</summary>
/// <param name="Occurrences">Top-level matches - zero for a glyph only ever matched nested in another.</param>
/// <param name="DataBits">What its top-level matches cost to encode, nested captures included.</param>
/// <param name="ResidualEquivalentBits">What the same text would cost as unmatched text instead (approximate: another glyph might claim some of it, if this one went).</param>
public sealed record GlyphContribution(
    string Name,
    bool IsTopLevel,
    double DefinitionBits,
    int Occurrences,
    int Words,
    double DataBits,
    double ResidualEquivalentBits)
{
    /// <summary>Bits saved net of its own definition: positive when the glyph pays for itself. Nested-only glyphs are paid for by the glyphs using them, so theirs is just the negated definition cost.</summary>
    public double NetBits => ResidualEquivalentBits - DataBits - DefinitionBits;
}

/// <summary>One distinct span of unmatched text, and what it costs across the corpus.</summary>
public sealed record ResidualSpan(string Text, int Occurrences, int Words, double Bits);

/// <summary>A grammar's MDL score against a corpus (see <see cref="MdlScorer"/>), with coverage and residual detail.</summary>
public sealed record MdlScore
{
    public double GrammarBits { get; init; }
    public IReadOnlyDictionary<DataComponent, double> ComponentBits { get; init; }
    public double DataBits => ComponentBits.Values.Sum();
    public double TotalBits => GrammarBits + DataBits;

    /// <summary>The score of the empty grammar against the same corpus.</summary>
    public double BaselineBits { get; init; }

    /// <summary><see cref="TotalBits"/> as a fraction of <see cref="BaselineBits"/>: below 1 means the grammar compresses the corpus.</summary>
    public double CompressionRatio => TotalBits / BaselineBits;

    /// <summary>The cost of one spelled character.</summary>
    public double CharBits { get; init; }

    public int Documents { get; init; }
    public int Lines { get; init; }
    public int Words { get; init; }
    public int CapturedWords { get; init; }
    public double Coverage => Words == 0 ? 1 : (double)CapturedWords / Words;

    public IReadOnlyList<GlyphContribution> Glyphs { get; init; }
    public IReadOnlyDictionary<string, double> Vocabularies { get; init; }
    public IReadOnlyList<ResidualSpan> Residuals { get; init; }

    /// <summary>Matches whose open-ended pattern text went uncharged because it couldn't be located (see <see cref="CorpusEncoding.UnlocatedPatternMatches"/>) - expected to be zero.</summary>
    public int UnlocatedPatternMatches { get; init; }

    public string ToReport(int rows = 25)
    {
        var report = new StringBuilder();

        report.AppendLine($"MDL score: {Bits(TotalBits)} (grammar {Bits(GrammarBits)} + data {Bits(DataBits)}) = {CompressionRatio:P1} of the empty grammar's {Bits(BaselineBits)}");
        report.AppendLine($"  data: {string.Join(" · ", ComponentBits.Select(x => $"{x.Key.ToString().ToLowerInvariant()} {Bits(x.Value)}"))}");
        report.AppendLine($"  grammar: {Bits(Glyphs.Sum(x => x.DefinitionBits))} in {Glyphs.Count} glyphs, {Bits(Vocabularies.Values.Sum())} in {Vocabularies.Count} vocabularies ({CharBits:F2} bits/char)");
        report.AppendLine($"Coverage: {CapturedWords:N0} of {Words:N0} words ({Coverage:P1}), {Documents:N0} documents, {Lines:N0} lines");

        if (UnlocatedPatternMatches > 0)
            report.AppendLine($"Warning: {UnlocatedPatternMatches:N0} matches had open-ended pattern text the scorer couldn't locate, so didn't charge");

        report.AppendLine();

        var topLevel = Glyphs.Where(x => x.IsTopLevel).OrderByDescending(x => x.NetBits).ToList();

        report.AppendLine($"Top-level glyphs by net savings ({topLevel.Count(x => x.NetBits < 0)} of {topLevel.Count} cost more than they save):");
        report.AppendLine($"  {"net",10} {"saves",10} {"data",9} {"def",7} {"matches",8} {"words",7}  glyph");

        foreach (var glyph in TopAndBottom(topLevel, rows))
            report.AppendLine(glyph is null
                ? "  ..."
                : $"  {glyph.NetBits,10:N0} {glyph.ResidualEquivalentBits,10:N0} {glyph.DataBits,9:N0} {glyph.DefinitionBits,7:N0} {glyph.Occurrences,8:N0} {glyph.Words,7:N0}  {glyph.Name}");

        report.AppendLine();
        report.AppendLine("Vocabularies by definition cost:");

        foreach (var (name, bits) in Vocabularies.OrderByDescending(x => x.Value).Take(rows))
            report.AppendLine($"  {bits,10:N0}  {name}");

        report.AppendLine();
        report.AppendLine($"Residual hot spots ({Residuals.Count:N0} distinct unmatched spans, {Bits(ComponentBits[DataComponent.Residual])}):");
        report.AppendLine($"  {"bits",10} {"count",6} {"words",6}  text");

        foreach (var span in Residuals.OrderByDescending(x => x.Bits).Take(rows))
            report.AppendLine($"  {span.Bits,10:N0} {span.Occurrences,6:N0} {span.Words,6:N0}  {Truncate(span.Text, 100)}");

        return report.ToString();
    }

    /// <summary>The first and last <paramref name="rows"/>/2 items, with a null standing for the elided middle.</summary>
    static IEnumerable<T> TopAndBottom<T>(List<T> items, int rows) where T : class =>
        items.Count <= rows
            ? items
            : items.Take(rows - rows / 2).Append(null).Concat(items.Skip(items.Count - rows / 2));

    static string Bits(double bits) => $"{bits:N0} bits";

    static string Truncate(string text, int length) => text.Length <= length ? text : text[..(length - 1)] + "…";
}
