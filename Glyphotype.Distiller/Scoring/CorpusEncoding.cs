namespace Glyphotype.Distiller.Scoring;

/// <summary>Where the bits of a corpus's encoding go.</summary>
public enum DataComponent
{
    /// <summary>What each line holds next: which top-level glyph, unmatched text, a clause break, or the line's end.</summary>
    Tokens,

    /// <summary>
    /// Variation a match lets through without capturing it: which literal realization (see
    /// <see cref="CorpusEncoding.GetFrame"/>) each match took, and which synonym spelled each captured value.
    /// </summary>
    Frames,

    /// <summary>What matches captured: enum members, numbers, and the types dynamic properties resolved to.</summary>
    Values,

    /// <summary>Unmatched text, as words from a residual lexicon - which is itself spelled out once.</summary>
    Residual,
}

/// <summary>
/// The encoding of a tokenized corpus given its grammar, as a probabilistic grammar over its parse trees: every
/// choice the text makes is a symbol drawn in some context, and each context is coded adaptively (see
/// <see cref="CodeLength.Adaptive"/>) - so a context always making the same choice costs nothing, and the
/// cost of learning each context's distribution is paid for inside it.
/// <para>
/// The contexts:
/// <list type="bullet">
/// <item>per line, each token's kind: a top-level glyph type, unmatched text, a clause break, or the line's end;</item>
/// <item>per glyph type, its frame - the text it matched with every child capture replaced by a placeholder -
/// which absorbs every choice a match makes that no property captures: Alt/Opt/Plural/Pattern nibs, which
/// optional parts are present, how many items a list holds, which one-of alternative matched;</item>
/// <item>per property slot, its value: the enum member (among the members the corpus uses - unused ones would be
/// culled from a finished grammar, see <see cref="VocabularyUsage"/>), the number (a universal integer code), or the
/// type a dynamic resolved to - and per enum member, the synonym that spelled it;</item>
/// <item>unmatched text: its word count, then each word from a residual lexicon, whose distinct words are
/// spelled once - including text a dynamic left unresolved (see <see cref="AllowUnmatchedAttribute"/>), which is
/// unmatched text held in place;</item>
/// <item>text matched by an open-ended regex (see <see cref="RegexOpenness"/>) - a pattern nib's, or a terminal's
/// own pattern: masked out of its frame as <c>{~}</c>, drawn from a context of its own, and spelled out the first
/// time each distinct text appears. The grammar paid for a closed regex's every text by writing it down, but an
/// open-ended one matches text the grammar never wrote, which would otherwise come free.</item>
/// </list>
/// Whitespace between tokens is taken as given. What's left is lossless: <see cref="Reconstruct"/> rebuilds
/// every match's text from exactly these choices.
/// </para>
/// </summary>
public sealed class CorpusEncoding
{
    const string _unmatchedSymbol = "«unmatched»";
    const string _clauseBreakSymbol = "«.»";
    const string _endOfLineSymbol = "«¶»";
    const string _unresolvedSymbol = "«unresolved»";
    const string _patternPlaceholder = "{~}";

    readonly Dictionary<string, Context> _contexts = [];
    readonly Dictionary<string, int> _residualWordCounts = [];
    readonly double _charBits;
    readonly long _tokenAlphabetSize;
    readonly List<TokenEncoding> _tokens = [];
    readonly Dictionary<DataComponent, double> _fixedBits = Enum.GetValues<DataComponent>().ToDictionary(x => x, _ => 0.0);

    CorpusEncoding(double charBits, int topLevelTypeCount)
    {
        _charBits = charBits;
        _tokenAlphabetSize = topLevelTypeCount + 3; // plus unmatched text, clause break and end of line
    }

    /// <summary>Every top-level token of the corpus and the choices it was encoded as.</summary>
    public IReadOnlyList<TokenEncoding> Tokens => _tokens;

    /// <summary>The vocabulary members the corpus used, and how each was spelled - what the grammar's cost counts (see <see cref="GrammarCost.Of"/>).</summary>
    public VocabularyUsage VocabularyUsage { get; } = new();

    /// <summary>
    /// Matches whose open-ended pattern text couldn't be located (see <see cref="BuiltRegex.FindPatternCaptures"/>),
    /// so went uncharged. Expected to be zero; anything else is a gap in the score.
    /// </summary>
    public int UnlocatedPatternMatches { get; private set; }

    /// <summary>
    /// Encodes <paramref name="lines"/>, tokenized by a grammar with <paramref name="topLevelTypeCount"/> top-level
    /// types, spelling residual words at <paramref name="charBits"/> per character.
    /// </summary>
    public static CorpusEncoding Encode(IEnumerable<ProcessedLine> lines, int topLevelTypeCount, double charBits)
    {
        var encoding = new CorpusEncoding(charBits, topLevelTypeCount);

        foreach (var line in lines)
            encoding.EncodeLine(line);

        return encoding;
    }

    /// <summary>The bits spent on each <see cref="DataComponent"/>.</summary>
    public IReadOnlyDictionary<DataComponent, double> GetComponentBits()
    {
        var bits = new Dictionary<DataComponent, double>(_fixedBits);

        foreach (var context in _contexts.Values)
            bits[context.Component] += context.TotalBits;

        bits[DataComponent.Residual] += _residualWordCounts.Keys.Sum(SpellingBits);

        return bits;
    }

    /// <summary>The bits <paramref name="token"/>'s own choices cost, each at its share of its context's code (see <see cref="CodeLength.AdaptiveShare"/>).</summary>
    public double GetBits(TokenEncoding token) =>
        token.Choices.Sum(x => x.Context.ShareOf(x.Symbol)) + token.FixedBits;

    /// <summary>
    /// What <paramref name="text"/> would cost as unmatched text under this encoding's residual lexicon: its word
    /// count, then each word at its share of the residual code - or, for a word the lexicon lacks, an escape plus
    /// its spelling.
    /// </summary>
    public double GetResidualBits(string text)
    {
        var words = SplitWords(text);
        var residual = _contexts.GetValueOrDefault(ResidualContextName);
        var bits = CodeLength.EliasGamma(Math.Max(1, words.Length));

        foreach (var word in words)
            bits += residual is not null && residual.Counts.ContainsKey(word)
                ? residual.ShareOf(word)
                : CodeLength.Uniform((residual?.Total ?? 0) + (residual?.AlphabetSize ?? 0) + 1) + SpellingBits(word);

        return bits;
    }

    void EncodeLine(ProcessedLine line)
    {
        foreach (var unit in line.Glyphs)
        {
            var token = new TokenEncoding(unit);
            var root = unit.CaptureContext.RootCaptureTrace;

            if (root.IsUnmatchedString)
            {
                Choose(token, TokenContext(), _unmatchedSymbol);
                EncodeResidual(token, unit.CaptureValue);
            }
            else if (root.IsClauseBreak)
            {
                Choose(token, TokenContext(), _clauseBreakSymbol);
            }
            else
            {
                Choose(token, TokenContext(), unit.Type.Name);
                EncodeTrace(token, root, FindOpenPatterns(unit.Type, root));
            }

            _tokens.Add(token);
        }

        TokenContext().Add(_endOfLineSymbol);
    }

    /// <param name="openPatterns">What open-ended patterns matched within the glyph <paramref name="trace"/> belongs to (see <see cref="FindOpenPatterns"/>).</param>
    void EncodeTrace(TokenEncoding token, CaptureTrace trace, IReadOnlyList<PatternCapture> openPatterns)
    {
        var node = trace.SourceNode;

        if (node is EnumNode)
        {
            var enumType = node.Navigation.UnderlyingType;
            var member = trace.ClrValue?.ToString() ?? throw new InvalidOperationException($"Enum capture '{trace.CaptureValue}' ({trace.FullyQualifiedName}) has no hydrated value");
            var memberPatterns = enumType.GetField(member)?.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;

            VocabularyUsage.Add(enumType.Name, member, trace.CaptureValue);

            // Among the members seen, not all the enum's: a member that never matches would be culled from the grammar.
            Choose(token, GetContext($"value {Slot(node)}", DataComponent.Values, alphabetSize: null), member);
            ChooseSpelling(token, GetContext($"spelling {enumType.Name}.{member}", DataComponent.Frames, alphabetSize: null), trace.CaptureValue, memberPatterns);
            return;
        }

        if (node is BoolNode)
        {
            ChooseSpelling(token, GetContext($"spelling {Slot(node)}", DataComponent.Frames, alphabetSize: null), trace.CaptureValue, node.Navigation.Patterns);
            return;
        }

        if (node is PrimitiveNode)
        {
            token.AddFixed(DataComponent.Values, CodeLength.SignedInteger(Convert.ToInt64(trace.ClrValue ?? 0)), _fixedBits);
            ChooseSpelling(token, GetContext($"spelling {Slot(node)}", DataComponent.Frames, alphabetSize: null), NormalizeDigits(trace.CaptureValue), node.Navigation.Patterns);
            return;
        }

        var frameType = node.Navigation.NodeType;

        if (node is DynamicGlyphNode)
        {
            frameType = trace.ResolvedNodeType ?? throw new InvalidOperationException($"Dynamic capture '{trace.CaptureValue}' ({trace.FullyQualifiedName}) has no resolved type");

            if (frameType == typeof(UnmatchedString))
            {
                // Left unresolved: unmatched text, coded as unmatched text is - it merely sits inside a match.
                Choose(token, GetContext($"resolution {Slot(node)}", DataComponent.Values, alphabetSize: null), _unresolvedSymbol);
                EncodeResidual(token, trace.CaptureValue);
                return;
            }

            Choose(token, GetContext($"resolution {Slot(node)}", DataComponent.Values, alphabetSize: null), frameType.Name);

            // The resolved glyph was matched by its own regex, so its patterns are found by that one.
            openPatterns = FindOpenPatterns(frameType, trace);
        }

        var frameName = Navigation.GetRegexSafeTypeName(frameType);
        var frame = GetFrame(trace, openPatterns);
        Choose(token, GetContext($"frame {frameName}", DataComponent.Frames, alphabetSize: null), frame.Text);

        foreach (var pattern in frame.Patterns)
            ChooseSpelling(token, GetContext($"pattern {frameName} /{pattern.Pattern}/", DataComponent.Frames, alphabetSize: null), pattern.Text, [pattern.Pattern]);

        foreach (var child in frame.Children)
            EncodeTrace(token, child, openPatterns);
    }

    /// <summary>Unmatched text: its word count, then each word from the residual lexicon.</summary>
    void EncodeResidual(TokenEncoding token, string text)
    {
        var words = SplitWords(text);
        token.AddFixed(DataComponent.Residual, CodeLength.EliasGamma(Math.Max(1, words.Length)), _fixedBits);

        foreach (var word in words)
        {
            Choose(token, GetContext(ResidualContextName, DataComponent.Residual, alphabetSize: null), word);
            _residualWordCounts[word] = _residualWordCounts.GetValueOrDefault(word) + 1;
        }
    }

    /// <summary>
    /// What the open-ended patterns (see <see cref="RegexOpenness"/>) in <paramref name="glyphType"/>'s regex matched,
    /// in its match spanning <paramref name="span"/> - the text a frame has to mask and spell (see <see cref="GetFrame"/>).
    /// </summary>
    IReadOnlyList<PatternCapture> FindOpenPatterns(Type glyphType, CaptureTrace span)
    {
        var regex = GlyphTypeCache.GetRegexGraph(glyphType).BuiltRegex;

        if (!regex.HasAuthoredPatterns)
            return [];

        var captures = regex.FindPatternCaptures(span.CaptureContext.SourceText, span.Index, span.Length);

        if (captures is null)
        {
            UnlocatedPatternMatches++;
            return [];
        }

        return captures.Where(x => RegexOpenness.IsOpenEnded(x.Pattern)).ToList();
    }

    /// <summary>
    /// <paramref name="trace"/>'s frame: the text it matched with each child capture replaced by a <c>{Name}</c>
    /// placeholder, and those children in order - and, of <paramref name="openPatterns"/>, those matched in this
    /// trace's own text (rather than a child's), each replaced by <c>{~}</c>.
    /// </summary>
    public static Frame GetFrame(CaptureTrace trace, IReadOnlyList<PatternCapture> openPatterns = null)
    {
        var text = new StringBuilder();
        var children = new List<CaptureTrace>();
        var patterns = new List<(string Pattern, string Text)>();
        var position = trace.Index;

        foreach (var segment in CaptureTraceWalker.GetSegments(trace))
        {
            if (segment.Child is not null)
            {
                text.Append('{').Append(segment.Child.Name).Append('}');
                children.Add(segment.Child);
                position = segment.Child.End;
                continue;
            }

            var start = position;
            var cursor = 0;

            foreach (var pattern in openPatterns ?? [])
            {
                if (pattern.Index < start || pattern.Index + pattern.Length > start + segment.Text.Length)
                    continue;

                var offset = pattern.Index - start;

                if (offset < cursor)
                    continue;

                text.Append(segment.Text, cursor, offset - cursor).Append(_patternPlaceholder);
                patterns.Add((pattern.Pattern, segment.Text.Substring(offset, pattern.Length)));
                cursor = offset + pattern.Length;
            }

            text.Append(segment.Text, cursor, segment.Text.Length - cursor);
            position = start + segment.Text.Length;
        }

        return new(text.ToString(), children) { Patterns = patterns };
    }

    /// <summary>
    /// Rebuilds <paramref name="unit"/>'s text from the choices it's encoded as - frames filled in with their
    /// children's reconstructions, terminals with their spellings. Equal to the unit's own text exactly when
    /// the encoding accounts for every character of the match, once.
    /// </summary>
    public static string Reconstruct(CaptureUnit unit)
    {
        var root = unit.CaptureContext.RootCaptureTrace;

        return root.IsSynthesized ? unit.CaptureValue : Reconstruct(root);

        static string Reconstruct(CaptureTrace trace)
        {
            if (trace.IsTerminal)
                return trace.CaptureValue;

            return string.Concat(CaptureTraceWalker.GetSegments(trace).Select(x => x.Child is null ? x.Text : Reconstruct(x.Child)));
        }
    }

    public const string ResidualContextName = "residual words";

    Context TokenContext() => GetContext("tokens", DataComponent.Tokens, _tokenAlphabetSize);

    Context GetContext(string name, DataComponent component, long? alphabetSize)
    {
        if (!_contexts.TryGetValue(name, out var context))
            _contexts[name] = context = new Context(name, component, alphabetSize);

        return context;
    }

    static void Choose(TokenEncoding token, Context context, string symbol)
    {
        context.Add(symbol);
        token.Choices.Add((context, symbol));
    }

    /// <summary>
    /// Chooses a text some regex among <paramref name="patterns"/> matched - and if one of them is open-ended, spells
    /// the text out the first time it's seen: the grammar never wrote it down.
    /// </summary>
    void ChooseSpelling(TokenEncoding token, Context context, string text, IEnumerable<string> patterns)
    {
        var isNew = !context.Counts.ContainsKey(text);
        Choose(token, context, text);

        if (isNew && patterns is not null && patterns.Any(RegexOpenness.IsOpenEnded))
            token.AddFixed(DataComponent.Frames, SpellingBits(text), _fixedBits);
    }

    double SpellingBits(string word) => (word.Length + 1) * _charBits;

    static string Slot(NamedGroupNode node) =>
        node.Navigation.Prop is PropertyInfo prop
            ? $"{Navigation.GetRegexSafeTypeName(prop.DeclaringType)}.{prop.Name}"
            : node.FullyQualifiedName;

    /// <summary>A number's spelling with its digits abstracted away, so "3" and "12" are the same spelling: the value itself is coded separately.</summary>
    static string NormalizeDigits(string text) => System.Text.RegularExpressions.Regex.Replace(text, @"\d+", "#");

    static string[] SplitWords(string text) => text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    /// <summary>One coding context: a distribution over the symbols chosen in it.</summary>
    public sealed class Context(string name, DataComponent component, long? alphabetSize)
    {
        public string Name { get; } = name;
        public DataComponent Component { get; } = component;
        public Dictionary<string, int> Counts { get; } = [];
        public int Total { get; private set; }

        /// <summary>The declared alphabet size (e.g. an enum's member count), or for an open context, the number of distinct symbols seen.</summary>
        public long AlphabetSize => alphabetSize ?? Math.Max(1, Counts.Count);

        public double TotalBits => CodeLength.Adaptive(Counts.Values, AlphabetSize);

        public double ShareOf(string symbol) => CodeLength.AdaptiveShare(Counts.GetValueOrDefault(symbol), Total, AlphabetSize);

        public void Add(string symbol)
        {
            Counts[symbol] = Counts.GetValueOrDefault(symbol) + 1;
            Total++;
        }
    }
}

/// <summary>A match's text with its child captures replaced by placeholders, and those children in order (see <see cref="CorpusEncoding.GetFrame"/>).</summary>
public sealed record Frame(string Text, IReadOnlyList<CaptureTrace> Children)
{
    /// <summary>The open-ended patterns masked out of <see cref="Text"/> as <c>{~}</c>, in order: each regex, and what it matched.</summary>
    public IReadOnlyList<(string Pattern, string Text)> Patterns { get; init; } = [];
}

/// <summary>One top-level token and the choices it was encoded as.</summary>
public sealed class TokenEncoding(CaptureUnit unit)
{
    public CaptureUnit Unit { get; } = unit;
    public List<(CorpusEncoding.Context Context, string Symbol)> Choices { get; } = [];
    public double FixedBits { get; private set; }

    internal void AddFixed(DataComponent component, double bits, Dictionary<DataComponent, double> totals)
    {
        FixedBits += bits;
        totals[component] += bits;
    }
}
