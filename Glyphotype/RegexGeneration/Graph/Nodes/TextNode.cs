namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any Glyph property, but rather must be matched as part of the Glyph's overall Regex.
/// <para>
/// It's spaced by the same rules as a property (see <see cref="JoinerRules"/>): an optional nib is nullable, so it
/// carries its separator inside its own optional group and renders it exactly when it renders its text; and text
/// that can open several ways (see <see cref="Branches"/>) is separated from what precedes it per opening.
/// </para>
/// </summary>
public class TextNode : RegexNode
{
    /// <summary>The nib's regex (see <see cref="Nib.Regex"/> - escaped literal text, or a pattern as written), with every literal space escaped to <see cref="BuiltRegex.EscapedSpace"/>: its <see cref="Branches"/>, as one alternation if there are several. Never includes the separators around it, nor an optional nib's own <c>?</c>.</summary>
    public string Text { get; }

    /// <summary>
    /// The ways this nib's match can open (see <see cref="Nib.Branches"/>), spaces escaped - one for plain text or a
    /// pattern, one per alternative of an <see cref="NibAlternatives"/>, one per possible first item of a
    /// <see cref="SomeNib"/>. Each is separated from what precedes it as a nib of its own would be (see
    /// <see cref="JoinerRules.ForOpenings"/>).
    /// </summary>
    public IReadOnlyList<string> Branches { get; }

    /// <summary>
    /// Whether this nib was authored as a bare literal period - the one way a Glyph type declares that it
    /// deliberately spans a clause boundary (see <see cref="RegexGraph.SpansClauses"/>). Checked against
    /// the authored nib rather than <see cref="Text"/>, where the period is already escaped to <c>\.</c>, and
    /// never true of a <see cref="PatternNib"/> (for which <c>"."</c> means any character).
    /// </summary>
    public bool IsClauseBreak { get; }

    /// <summary>Whether this text ends with a delimiter opening an enclosure in its glyph's text, so what follows binds to it: <c>"has \"", Prop(Ability)</c> reads <c>has "it flies</c>.</summary>
    public bool OpensEnclosure { get; }

    /// <summary>Whether this text starts with a delimiter closing an enclosure in its glyph's text, so it binds to what precedes it: <c>Prop(Ability), "\""</c> reads <c>it flies"</c>.</summary>
    public bool ClosesEnclosure { get; }

    /// <summary>
    /// The regex an author wrote, when this nib is one - a <see cref="PatternNib"/> (optional or not), or a
    /// class-level <see cref="RegexPatternAttribute"/> pattern - else null. Null for a <see cref="DynamicGlyphNode"/>'s
    /// own placeholder pattern: what that captures is resolved into a glyph of its own, not left as regex-matched text.
    /// See <see cref="BuiltRegex.FindPatternCaptures"/>.
    /// </summary>
    public string AuthoredPattern { get; }

    readonly bool _isOptional;

    /// <summary>An optional nib (see <see cref="Glyph.Opt"/>) can match nothing, so it's spaced as any nullable node is (see <see cref="JoinerRules.PlaceLeadingJoiner"/>).</summary>
    public override bool IsNullable => _isOptional;

    /// <summary>The literal texts this nib's match can end with (see <see cref="Nib.Literals"/>), or null for a pattern - whether it binds to what follows it (see <see cref="JoinSite.BeforeClosings"/>).</summary>
    public IReadOnlyList<string> Closings { get; }

    /// <summary>What this node's brick wraps around <see cref="Text"/> - see <see cref="WithPatternGroup"/>.</summary>
    string _renderedPrefix = "", _renderedSuffix = "";

    /// <param name="enclosed">
    /// Whether the nib sits inside an enclosure its glyph writes itself (see <see cref="Enclosures.Inside(IReadOnlyList{Nib})"/>),
    /// where a period ends a clause nested in the glyph's own text: each period it matches is then an enclosed one
    /// (see <see cref="Enclosures.EnclosedPeriod"/>), and a bare "." is no clause break.
    /// </param>
    /// <param name="opensEnclosure">Whether the nib ends with a delimiter opening an enclosure in its glyph's text (see <see cref="Enclosures.Delimiters"/>).</param>
    /// <param name="closesEnclosure">Whether the nib starts with a delimiter closing an enclosure in its glyph's text.</param>
    public TextNode(RegexNode parentNode, Nib nib, bool enclosed = false, bool opensEnclosure = false, bool closesEnclosure = false)
        : base(parentNode, nib.Text)
    {
        if (string.IsNullOrEmpty(nib.Text))
            throw new Exception($"{nameof(TextNode)} text can't be null or empty");

        // A bare literal "." nib is how a Glyph declares a clause break. Its regex is already the escaped
        // "\." - "." alone would be regex for "any character" - since literal nib text is escaped (see Nib).
        IsClauseBreak = !enclosed && nib is not PatternNib && nib.Text == ".";
        OpensEnclosure = opensEnclosure;
        ClosesEnclosure = closesEnclosure;
        Closings = nib.Literals;
        _isOptional = nib.IsOptional;

        if (parentNode is not DynamicGlyphNode && nib is PatternNib)
            AuthoredPattern = enclosed ? ToEnclosedPeriods(nib.Regex) : nib.Regex;

        // A nib with parts of its own (a Some()) separates them as the nibs around it are: by this group's own joiner.
        var joiner = parentNode is NamedGroupNode group and not GlyphOneOfNode ? group.EffectiveChildJoiner : Joiner.None;

        // Escaped here rather than left as a raw space so that "is there already a space here?" (see
        // RegexCollector.AlreadySeparated) is one check against one token, whether the space came from a nib
        // or from a joiner - and so that a nib's own spaces survive IgnorePatternWhitespace if it's ever
        // enabled. BuiltRegex unescapes the whole pattern before compiling, so matching is unaffected.
        // EscapeSpaces is idempotent, so a nib that already spells its space out as "[ ]" (the same token
        // this produces, and the same thing that token means as a regex) lands on exactly the text it would
        // have if it had been written with a plain space.
        // A pattern is one unit: a top-level "|" in it alternates within the pattern, not across the whole glyph.
        Branches = nib.Branches(joiner)
            .Select(x => nib is PatternNib && StartCharSet.HasTopLevelAlternation(x) ? $"({x})" : x)
            .Select(BuiltRegex.EscapeSpaces)
            .Select(x => enclosed ? ToEnclosedPeriods(x) : x)
            .ToList();
        Text = Branches.Count == 1 ? Branches[0] : "(" + string.Join('|', Branches) + ")";
    }

    /// <summary>A regex matching a literal period: <c>\.</c> not itself preceded by an escaping backslash.</summary>
    static readonly Regex _escapedPeriod = new(@"(?<!\\)((?:\\\\)*)\\\.");

    /// <summary><paramref name="regex"/> with each literal period it matches replaced by an enclosed one (see <see cref="Enclosures.EnclosedPeriod"/>).</summary>
    static string ToEnclosedPeriods(string regex) =>
        _escapedPeriod.Replace(regex, "${1}" + Enclosures.EnclosedPeriod);

    /// <summary>This node's brick with <see cref="AuthoredPattern"/> inside a named group, so a match records what the pattern itself matched.</summary>
    internal string WithPatternGroup(string groupName) =>
        _renderedPrefix + BuiltRegex.EscapeSpaces($"(?<{groupName}>{AuthoredPattern})") + _renderedSuffix;

    /// <summary>Whether <paramref name="brick"/> is the one this node rendered <see cref="Text"/> in, so <see cref="WithPatternGroup"/> can stand in for it.</summary>
    internal bool IsTextBrick(RegexBrick brick) =>
        brick.Parent == this && brick.Regex == _renderedPrefix + Text + _renderedSuffix;

    /// <summary>
    /// One brick: <see cref="Text"/> with the separators this node carries itself, wrapped as <c>(...)?</c> together
    /// with them when it's optional. A required node's shared leading joiner was already placed in front of it (see
    /// <see cref="RegexNode.AppendRegexBricks"/>), so it carries one only per opening, when they differ:
    /// <c>(,|[ ]and)</c>. An optional one carries its leading separator, <c>([ ]some)?</c> - unless its text supplies
    /// its own trailing space, which then faces what follows, leaving the separator in front unconditional:
    /// <c>[ ](some[ ])?</c> - or, in an all-optional run at the start, the separator after it: <c>(the[ ])?</c>.
    /// </summary>
    protected override void AppendOwnRegexBricks(RegexCollector collector)
    {
        var placement = LeadingJoinerPlacement;
        var openings = placement == JoinerPlacement.None ? null : JoinerRules.ForOpenings(JoinSite.Of((NamedGroupNode)ParentNode, this, collector));
        var content = Text;
        var leading = "";

        if (openings is not null && openings.Distinct().Count() > 1)
            content = "(" + string.Join('|', Branches.Select((x, i) => Separator(openings[i]) + x)) + ")";
        else if (placement == JoinerPlacement.InsideNodeLeading && openings[0] != Joiner.None)
        {
            if (RegexCollector.EndsSeparated(Text))
                collector.Append(new RegexBrickJoiner(ParentNode, openings[0]));
            else
                leading = Separator(openings[0]);
        }

        var trailing = TrailingJoinerSuccessor is RegexNode successor
            ? Separator(JoinerRules.Between(JoinSite.Of((NamedGroupNode)ParentNode, successor) with { AlreadySeparated = RegexCollector.EndsSeparated(content) }))
            : "";

        _renderedPrefix = (_isOptional ? "(" : "") + leading;
        _renderedSuffix = trailing + (_isOptional ? ")?" : "");

        collector.Append(new RegexBrick(this, _renderedPrefix + content + _renderedSuffix));
    }

    static string Separator(Joiner joiner) =>
        joiner == Joiner.None ? "" : joiner.GetDescription();
}
