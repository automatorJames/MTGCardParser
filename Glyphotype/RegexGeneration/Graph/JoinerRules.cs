namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// Where a joiner goes relative to the node it precedes - see <see cref="JoinerRules.PlaceLeadingJoiner"/>.
/// </summary>
public enum JoinerPlacement
{
    /// <summary>No joiner in front of this node (it's first, or nothing before it is guaranteed to render).</summary>
    None,

    /// <summary>Unconditionally, just before this node's own bricks - owned by the enclosing group.</summary>
    BeforeNode,

    /// <summary>Inside this (nullable) node's own group, as its first content - so it renders only when the node does.</summary>
    InsideNodeLeading,
}

/// <summary>
/// The facts a joiner decision reads about two adjacent siblings, <c>before</c> and <c>after</c>, in one group.
/// Gathered from the node graph by <see cref="Of"/>; plain values, so the rules can be exercised directly.
/// </summary>
/// <param name="GroupJoiner">The enclosing group's own joiner (see <see cref="NamedGroupNode.EffectiveChildJoiner"/>).</param>
/// <param name="GroupIsOneOf">Whether the enclosing group is a one-of, whose joiner (the pipe) separates alternatives only.</param>
/// <param name="BeforeIsProperty">Whether <c>before</c> is a property's group, rather than literal text.</param>
/// <param name="AfterIsProperty">Whether <c>after</c> is a property's group, rather than literal text.</param>
/// <param name="AfterText">The regex text of <c>after</c>, when it's literal text; else null.</param>
/// <param name="AfterIsClauseBreak">Whether <c>after</c> is a bare-period nib (see <see cref="TextNode.IsClauseBreak"/>).</param>
/// <param name="AfterOpenings">The ways <c>after</c>'s match can open, when it's text (see <see cref="TextNode.Branches"/>): one regex per alternative, each decided as a nib of its own (see <see cref="JoinerRules.ForOpenings"/>); else null.</param>
/// <param name="AlreadySeparated">Whether the regex emitted so far already ends in a space (see <see cref="RegexCollector.AlreadySeparated"/>).</param>
/// <param name="BeforeClosings">The literal texts <c>before</c>'s match can end with, when it's literal text certain to render (see <see cref="Nib.Literals"/>); else null.</param>
/// <param name="AfterClosesEnclosure">Whether <c>after</c> is text starting with a delimiter that closes an enclosure (see <see cref="TextNode.ClosesEnclosure"/>), binding it to what precedes it.</param>
/// <param name="BeforeOpensEnclosure">Whether <c>before</c> is text ending with a delimiter that opens an enclosure (see <see cref="TextNode.OpensEnclosure"/>), binding what follows to it.</param>
public readonly record struct JoinSite(
    Joiner GroupJoiner,
    bool GroupIsOneOf = false,
    bool BeforeIsProperty = false,
    bool AfterIsProperty = false,
    string AfterText = null,
    bool AfterIsClauseBreak = false,
    bool AlreadySeparated = false,
    IReadOnlyList<string> AfterOpenings = null,
    IReadOnlyList<string> BeforeClosings = null,
    bool AfterClosesEnclosure = false,
    bool BeforeOpensEnclosure = false)
{
    /// <summary>The site between <paramref name="after"/> and its preceding sibling in <paramref name="group"/>, with what's been emitted so far read off <paramref name="collector"/> (none, when null).</summary>
    public static JoinSite Of(NamedGroupNode group, RegexNode after, RegexCollector collector = null)
    {
        var before = group.Children[group.Children.IndexOf(after) - 1];
        var afterText = after as TextNode;

        // A nested glyph is read through to the literal text it's certain to open or close with, so its punctuation
        // binds as it would written directly: [Prop(Effect), Prop(Where)] with Where opening "," reads "x, where",
        // not "x , where".
        var openingText = afterText ?? EdgeText(after, opening: true);
        var closingText = before is TextNode { IsNullable: false } beforeText ? beforeText : EdgeText(before, opening: false);

        return new(
            group.EffectiveChildJoiner,
            GroupIsOneOf: group is GlyphOneOfNode,
            BeforeIsProperty: before is NamedGroupNode,
            AfterIsProperty: after is NamedGroupNode,
            AfterText: openingText?.Text,
            AfterIsClauseBreak: openingText?.IsClauseBreak ?? false,
            AlreadySeparated: collector?.AlreadySeparated ?? false,
            AfterOpenings: afterText?.Branches,
            BeforeClosings: closingText?.Closings,
            AfterClosesEnclosure: openingText?.ClosesEnclosure ?? false,
            BeforeOpensEnclosure: closingText?.OpensEnclosure ?? false);
    }

    /// <summary>
    /// The literal text <paramref name="node"/>, a nested glyph, is certain to open (or close) with: its first (or last)
    /// child, read through further nested glyphs, when that's required text with a single way to open - else null.
    /// Never read through an alternation (a vocabulary, a bare one-of, a dynamic), whose edge isn't one text.
    /// </summary>
    static TextNode EdgeText(RegexNode node, bool opening)
    {
        if (node is not GlyphNode { Children: [_, ..] } glyph)
            return null;

        var edge = opening ? glyph.Children[0] : glyph.Children[^1];

        if (glyph.EffectiveChildJoiner == Joiner.Pipe && edge is not TextNode)
            return null;

        return edge switch
        {
            TextNode { IsNullable: false, Branches.Count: 1 } text => text,
            GlyphNode nested when !nested.IsNullable => EdgeText(nested, opening),
            _ => null,
        };
    }
}

/// <summary>
/// Every rule about joiners - the separators (<see cref="Joiner"/>) rendered between parts of a pattern - in one
/// place: which one a group calls for between two neighbours, when it's suppressed, where it's placed, and how
/// repeated items are separated. The node graph gathers facts (<see cref="JoinSite"/>, nullability) and emits
/// bricks; every decision is made here, as a pure function of those facts.
/// </summary>
public static class JoinerRules
{
    /// <summary>
    /// Punctuation that binds to the token before it, so no space is written in front of it: "dog, ball", "dog's",
    /// "(dog)", "50%". The joiner after a nib ending in one also belongs with that nib when displayed (see
    /// <see cref="AdheresToPrecedingText"/>) - so a nib written <c>","</c> reads as <c>,[ ]</c>, as if written <c>", "</c>.
    /// </summary>
    static readonly HashSet<char> _bindsBackward = ['\'', ',', '.', Enclosures.EnclosedPeriod, ';', ':', '!', '?', ')', ']', '}', '%', '-', '/'];

    /// <summary>Punctuation that binds to the token after it, so no space is written after it: "(dog)", "[x]", "$3", "#1".</summary>
    static readonly HashSet<char> _bindsForward = ['(', '[', '{', '$', '#', '-', '/'];

    // A hyphen or slash binds both ways - "dog-fish", "+1/+1" - and an author wanting spaces around one writes them: " - ".

    /// <summary>
    /// What separates two adjacent siblings - <see cref="Joiner.None"/> for nothing:
    /// <list type="number">
    /// <item>The group's own joiner - except in a one-of, whose pipe only ever sits between two alternatives
    /// (never between an alternative and the literal text around them - see <see cref="GlyphOneOfNode"/>).</item>
    /// <item>Suppressed when the regex so far already ends in a space, which a joiner would double.</item>
    /// <item>Suppressed when <c>after</c> is text that binds to the token before it - opening with backward-binding
    /// punctuation (see <see cref="_bindsBackward"/>), or a bare period - or that supplies its own leading space.</item>
    /// <item>Suppressed when <c>before</c> is text that binds to the token after it - every way it can end does so
    /// with forward-binding punctuation (see <see cref="_bindsForward"/>).</item>
    /// <item>Text that can open several ways (see <see cref="JoinSite.AfterOpenings"/>) is decided per opening (see
    /// <see cref="ForOpenings"/>). Where they all agree, that's the answer; where they don't, this is
    /// <see cref="Joiner.None"/> - the text carries each opening's own separator inside its alternatives instead.</item>
    /// </list>
    /// </summary>
    public static Joiner Between(JoinSite site)
    {
        if (site.AfterOpenings is { Count: > 1 })
            return ForOpenings(site).Distinct().ToList() is [var shared] ? shared : Joiner.None;

        var joiner = Intended(site);

        if (joiner == Joiner.None || site.AlreadySeparated || BindsToPrecedingToken(site) || BindsToFollowingToken(site))
            return Joiner.None;

        return joiner;
    }

    /// <summary>
    /// What separates the preceding sibling from each of the ways <c>after</c> can open (see
    /// <see cref="JoinSite.AfterOpenings"/>) - each decided by <see cref="Between"/> exactly as if it were a nib
    /// of its own: so <c>Alt(",", "and")</c> after a word is nothing before the comma, a space before "and".
    /// </summary>
    public static IReadOnlyList<Joiner> ForOpenings(JoinSite site) =>
        site.AfterOpenings is { Count: > 0 } openings
            ? openings.Select(x => Between(site with { AfterText = x, AfterOpenings = null })).ToList()
            : [Between(site)];

    /// <summary>What <see cref="Between"/> starts from: the separator the grammar calls for between the two neighbours, before any suppression.</summary>
    public static Joiner Intended(JoinSite site) =>
        site.GroupIsOneOf && !(site.BeforeIsProperty && site.AfterIsProperty) ? Joiner.None : site.GroupJoiner;

    static bool BindsToPrecedingToken(JoinSite site) =>
        site.AfterText is { Length: > 0 } text
        && (site.AfterIsClauseBreak
            || site.AfterClosesEnclosure
            || _bindsBackward.Contains(FirstMatchedChar(text))
            || text.StartsWith(BuiltRegex.EscapedSpace)
            || text[0] == ' ');

    static bool BindsToFollowingToken(JoinSite site) =>
        site.BeforeOpensEnclosure ||
        site.BeforeClosings is { Count: > 0 } closings
        && closings.All(x => x.Length > 0 && _bindsForward.Contains(x[^1]));

    /// <summary>The first character <paramref name="regexText"/> matches literally - reading through a leading escape, since literal nib text arrives escaped (e.g. a <c>"?"</c> nib as <c>\?</c>; see <see cref="Nib.EscapeLiteral"/>).</summary>
    static char FirstMatchedChar(string regexText) =>
        regexText.Length > 1 && regexText[0] == '\\' ? regexText[1] : regexText[0];

    /// <summary>
    /// Where the joiner separating a node from its preceding sibling goes:
    /// <list type="bullet">
    /// <item>A first child has nothing to be separated from: <see cref="JoinerPlacement.None"/>.</item>
    /// <item>A node with nothing guaranteed to render before it (every earlier sibling is nullable) might be the
    /// very start of the match, so it can't lead with a joiner. Its nullable predecessor carries the joiner as its
    /// own trailing content instead - see <see cref="OwnsTrailingJoiner"/>.</item>
    /// <item>A nullable node carries it inside its own group, so it renders exactly when the node does:
    /// <see cref="JoinerPlacement.InsideNodeLeading"/>.</item>
    /// <item>A required node takes it unconditionally, just in front: <see cref="JoinerPlacement.BeforeNode"/>.</item>
    /// </list>
    /// So an optional's joiner always sits on the side facing the guaranteed text: <c>the( very)?( big)? dog</c>,
    /// but <c>(very )?(big )?dog</c>.
    /// </summary>
    public static JoinerPlacement PlaceLeadingJoiner(bool isFirstChild, bool isNullable, bool hasAnchorBefore) =>
        isFirstChild || !hasAnchorBefore ? JoinerPlacement.None
        : isNullable ? JoinerPlacement.InsideNodeLeading
        : JoinerPlacement.BeforeNode;

    /// <summary>
    /// Whether a nullable node carries, as its own trailing content, the joiner in front of the sibling after it -
    /// the case <see cref="PlaceLeadingJoiner"/> leaves unplaced: with nothing guaranteed to render before this
    /// node, the next one can't lead with a joiner, so this node renders it exactly when it renders itself. Every
    /// nullable in an all-nullable prefix does this, so each is followed by its joiner just when it's present.
    /// </summary>
    public static bool OwnsTrailingJoiner(bool isNullable, bool hasNext, bool hasAnchorBefore) =>
        isNullable && hasNext && !hasAnchorBefore;

    /// <summary>
    /// The separator in front of each repeated item of an X-Of (e.g. <see cref="CompoundOf{T}"/>'s <c>SecondPlus</c>):
    /// a <see cref="ManyOf{T}"/>'s is always <see cref="Joiner.CommaSpace"/> ("a, b, and c" is fixed English list
    /// grammar); a <see cref="CompoundOf{T}"/>'s is its <see cref="JoinedByAttribute"/> - the property's (the more
    /// specific site), else its own type's - else <see cref="Joiner.CommaSpace"/>.
    /// </summary>
    public static Joiner ForRepetition(bool ownerIsCompoundOf, Joiner? propertyJoinedBy, Joiner? typeJoinedBy) =>
        ownerIsCompoundOf ? propertyJoinedBy ?? typeJoinedBy ?? Joiner.CommaSpace : Joiner.CommaSpace;

    /// <summary>Whether the joiner following text <paramref name="precedingText"/> belongs with it when formatted for display - true when it ends in tight punctuation (see <see cref="_bindsBackward"/>).</summary>
    public static bool AdheresToPrecedingText(string precedingText) =>
        !string.IsNullOrEmpty(precedingText) && _bindsBackward.Contains(precedingText[^1]);
}
