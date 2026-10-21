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
/// spelled once - unless the grammar already spells them (see <see cref="UseGrammarWords"/>);</item>
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

        // What each line's tokens choose is a fact about the line alone (see LinePlan), found once per line - in
        // parallel, the first time - and kept with it: a corpus mostly made of lines scored before is replayed, not encoded again.
        var plans = (lines as IReadOnlyList<ProcessedLine> ?? lines.ToList())
            .AsParallel()
            .AsOrdered()
            .Select(x => _plans.GetValue(x, Plan))
            .ToList();

        foreach (var plan in plans)
            encoding.Replay(plan);

        return encoding;
    }

    /// <summary>Each line's plan, for as long as the line lives: a line kept from one tokenization to the next is scored again without being encoded again.</summary>
    static readonly System.Runtime.CompilerServices.ConditionalWeakTable<ProcessedLine, LinePlan> _plans = new();

    /// <summary>
    /// What encoding one line comes to, apart from any corpus: the steps each token takes - the context it chooses
    /// in and the symbol it chooses, a spelling that's paid for the first time the corpus sees it, bits fixed by the
    /// token alone, a vocabulary member used, a residual word - and how many pattern matches couldn't be located.
    /// Everything that depends on the rest of the corpus (how often a context saw a symbol, whether a spelling is new,
    /// the character cost) is left to <see cref="Replay"/>.
    /// </summary>
    sealed class LinePlan
    {
        public List<TokenPlan> Tokens { get; } = [];
        public int UnlocatedPatternMatches { get; set; }
    }

    sealed class TokenPlan(CaptureUnit unit)
    {
        public CaptureUnit Unit { get; } = unit;
        public List<Step> Steps { get; } = [];
    }

    enum StepKind { Choose, Spelling, Fixed, Vocabulary, ResidualWord }

    /// <param name="Context">The context's name - or, for a vocabulary step, the vocabulary's.</param>
    /// <param name="Symbol">The symbol chosen - or the member, for a vocabulary step; the word, for a residual word.</param>
    /// <param name="Bits">Fixed bits, for a fixed step.</param>
    /// <param name="PaysSpelling">For a spelling step: whether the text is spelled out when first seen (some pattern that matched it is open-ended).</param>
    /// <param name="Text">For a vocabulary step: how the member was spelled.</param>
    readonly record struct Step(StepKind Kind, string Context, DataComponent Component, string Symbol, double Bits = 0, bool PaysSpelling = false, string Text = null);

    /// <summary>The plan for <paramref name="line"/>: what <see cref="Replay"/> needs to encode it in any corpus.</summary>
    static LinePlan Plan(ProcessedLine line)
    {
        var plan = new LinePlan();

        foreach (var unit in line.Glyphs)
        {
            var token = new TokenPlan(unit);
            var root = unit.CaptureContext.RootCaptureTrace;

            if (root.IsUnmatchedString)
            {
                token.Steps.Add(new(StepKind.Choose, _tokenContextName, DataComponent.Tokens, _unmatchedSymbol));
                PlanResidual(token, unit.CaptureValue);
            }
            else if (root.IsClauseBreak)
            {
                token.Steps.Add(new(StepKind.Choose, _tokenContextName, DataComponent.Tokens, _clauseBreakSymbol));
            }
            else
            {
                token.Steps.Add(new(StepKind.Choose, _tokenContextName, DataComponent.Tokens, unit.Type.Name));
                PlanTrace(token, root, FindOpenPatterns(unit.Type, root, plan), plan);
            }

            plan.Tokens.Add(token);
        }

        return plan;
    }

    /// <summary>Encodes a line from its plan, in this corpus's contexts.</summary>
    void Replay(LinePlan plan)
    {
        foreach (var tokenPlan in plan.Tokens)
        {
            var token = new TokenEncoding(tokenPlan.Unit);

            foreach (var step in tokenPlan.Steps)
            {
                switch (step.Kind)
                {
                    case StepKind.Choose:
                        Choose(token, GetContext(step.Context, step.Component), step.Symbol);
                        break;

                    case StepKind.Spelling:
                        var context = GetContext(step.Context, step.Component);
                        var isNew = !context.Counts.ContainsKey(step.Symbol);
                        Choose(token, context, step.Symbol);

                        if (isNew && step.PaysSpelling)
                            token.AddFixed(DataComponent.Frames, SpellingBits(step.Symbol), _fixedBits);

                        break;

                    case StepKind.Fixed:
                        token.AddFixed(step.Component, step.Bits, _fixedBits);
                        break;

                    case StepKind.Vocabulary:
                        VocabularyUsage.Add(step.Context, step.Symbol, step.Text);
                        break;

                    case StepKind.ResidualWord:
                        _residualWordCounts[step.Symbol] = _residualWordCounts.GetValueOrDefault(step.Symbol) + 1;
                        break;
                }
            }

            _tokens.Add(token);
        }

        UnlocatedPatternMatches += plan.UnlocatedPatternMatches;
        TokenContext().Add(_endOfLineSymbol);
    }

    /// <summary>The bits spent on each <see cref="DataComponent"/>.</summary>
    public IReadOnlyDictionary<DataComponent, double> GetComponentBits()
    {
        var bits = new Dictionary<DataComponent, double>(_fixedBits);

        foreach (var context in _contexts.Values)
            bits[context.Component] += context.TotalBits;

        bits[DataComponent.Residual] += _residualWordCounts.Keys.Where(x => !_grammarWords.Contains(x)).Sum(SpellingBits);

        return bits;
    }

    IReadOnlySet<string> _grammarWords = new HashSet<string>();

    /// <summary>
    /// Words the grammar already spells (see <see cref="GrammarCost.SpelledWords"/>), which the residual lexicon can
    /// refer to instead of spelling again. Without this, a word defined in a vocabulary but still unmatched somewhere
    /// else is paid for twice - once in the grammar, once in the lexicon - until its last unmatched occurrence is
    /// covered, which makes every new vocabulary look like a loss while most of the corpus is still unmatched.
    /// </summary>
    public void UseGrammarWords(IReadOnlySet<string> words) => _grammarWords = words;

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
                : CodeLength.Uniform((residual?.Total ?? 0) + (residual?.AlphabetSize ?? 0) + 1) + (_grammarWords.Contains(word) ? 0 : SpellingBits(word));

        return bits;
    }

    /// <param name="openPatterns">What open-ended patterns matched within the glyph <paramref name="trace"/> belongs to (see <see cref="FindOpenPatterns"/>).</param>
    static void PlanTrace(TokenPlan token, CaptureTrace trace, IReadOnlyList<PatternCapture> openPatterns, LinePlan plan)
    {
        var node = trace.SourceNode;

        if (node is EnumNode)
        {
            var enumType = node.Navigation.UnderlyingType;
            var member = trace.ClrValue?.ToString() ?? throw new InvalidOperationException($"Enum capture '{trace.CaptureValue}' ({trace.FullyQualifiedName}) has no hydrated value");
            var memberPatterns = enumType.GetField(member)?.GetCustomAttribute<RegexPatternAttribute>()?.Patterns;

            token.Steps.Add(new(StepKind.Vocabulary, enumType.Name, DataComponent.Values, member, Text: trace.CaptureValue));

            // Among the members seen, not all the enum's: a member that never matches would be culled from the grammar.
            token.Steps.Add(new(StepKind.Choose, $"value {Slot(node)}", DataComponent.Values, member));
            PlanSpelling(token, $"spelling {enumType.Name}.{member}", trace.CaptureValue, memberPatterns);
            return;
        }

        if (node is BoolNode)
        {
            PlanSpelling(token, $"spelling {Slot(node)}", trace.CaptureValue, node.Navigation.Patterns);
            return;
        }

        if (node is PrimitiveNode)
        {
            token.Steps.Add(new(StepKind.Fixed, null, DataComponent.Values, null, Bits: CodeLength.SignedInteger(Convert.ToInt64(trace.ClrValue ?? 0))));
            PlanSpelling(token, $"spelling {Slot(node)}", NormalizeDigits(trace.CaptureValue), node.Navigation.Patterns);
            return;
        }

        var frameType = node.Navigation.NodeType;

        if (node is DynamicGlyphNode)
        {
            frameType = trace.ResolvedNodeType ?? throw new InvalidOperationException($"Dynamic capture '{trace.CaptureValue}' ({trace.FullyQualifiedName}) has no resolved type");

            token.Steps.Add(new(StepKind.Choose, $"resolution {Slot(node)}", DataComponent.Values, frameType.Name));

            // The resolved glyph was matched by its own regex, so its patterns are found by that one.
            openPatterns = FindOpenPatterns(frameType, trace, plan);
        }

        var frameName = Navigation.GetRegexSafeTypeName(frameType);
        var frame = GetFrame(trace, openPatterns);
        token.Steps.Add(new(StepKind.Choose, $"frame {frameName}", DataComponent.Frames, frame.Text));

        foreach (var pattern in frame.Patterns)
            PlanSpelling(token, $"pattern {frameName} /{pattern.Pattern}/", pattern.Text, [pattern.Pattern]);

        foreach (var child in frame.Children)
            PlanTrace(token, child, openPatterns, plan);
    }

    /// <summary>Unmatched text: its word count, then each word from the residual lexicon.</summary>
    static void PlanResidual(TokenPlan token, string text)
    {
        var words = SplitWords(text);
        token.Steps.Add(new(StepKind.Fixed, null, DataComponent.Residual, null, Bits: CodeLength.EliasGamma(Math.Max(1, words.Length))));

        foreach (var word in words)
        {
            token.Steps.Add(new(StepKind.Choose, ResidualContextName, DataComponent.Residual, word));
            token.Steps.Add(new(StepKind.ResidualWord, null, DataComponent.Residual, word));
        }
    }

    /// <summary>
    /// Chooses a text some regex among <paramref name="patterns"/> matched - and if one of them is open-ended, spells
    /// the text out the first time it's seen: the grammar never wrote it down.
    /// </summary>
    static void PlanSpelling(TokenPlan token, string context, string text, IEnumerable<string> patterns) =>
        token.Steps.Add(new(StepKind.Spelling, context, DataComponent.Frames, text, PaysSpelling: patterns is not null && patterns.Any(RegexOpenness.IsOpenEnded)));

    /// <summary>
    /// What the open-ended patterns (see <see cref="RegexOpenness"/>) in <paramref name="glyphType"/>'s regex matched,
    /// in its match spanning <paramref name="span"/> - the text a frame has to mask and spell (see <see cref="GetFrame"/>).
    /// </summary>
    /// <param name="plan">The line's plan, where a match that can't be located is counted.</param>
    static IReadOnlyList<PatternCapture> FindOpenPatterns(Type glyphType, CaptureTrace span, LinePlan plan)
    {
        // A match made by a plan recorded them as it went; one made by a regex is matched again to find them.
        if (span.CaptureContext.PatternCaptures is { } recorded)
            return recorded.Where(x => x.Index >= span.Index && x.Index + x.Length <= span.End && RegexOpenness.IsOpenEnded(x.Pattern)).ToList();

        var regex = GlyphTypeCache.GetRegexGraph(glyphType).BuiltRegex;

        if (!regex.HasAuthoredPatterns)
            return [];

        var captures = regex.FindPatternCaptures(span.CaptureContext.SourceText, span.Index, span.Length);

        if (captures is null)
        {
            plan.UnlocatedPatternMatches++;
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

    const string _tokenContextName = "tokens";

    Context TokenContext() => GetContext(_tokenContextName, DataComponent.Tokens);

    /// <summary>The context named <paramref name="name"/> - open (its alphabet the symbols seen), but for the token context, whose alphabet is the grammar's top-level types and the three other kinds of token.</summary>
    Context GetContext(string name, DataComponent component)
    {
        if (!_contexts.TryGetValue(name, out var context))
            _contexts[name] = context = new Context(name, component, name == _tokenContextName ? _tokenAlphabetSize : null);

        return context;
    }

    static void Choose(TokenEncoding token, Context context, string symbol)
    {
        context.Add(symbol);
        token.Choices.Add((context, symbol));
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
