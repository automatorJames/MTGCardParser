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
/// <param name="AfterIsPluralSuffix">Whether <c>after</c> is an optional plural suffix (see <see cref="OptionalPluralNib"/>).</param>
/// <param name="AfterIsClauseBreak">Whether <c>after</c> is a bare-period nib (see <see cref="TextNode.IsClauseBreak"/>).</param>
/// <param name="AlreadySeparated">Whether the regex emitted so far already ends in a space (see <see cref="RegexCollector.AlreadySeparated"/>).</param>
public readonly record struct JoinSite(
    Joiner GroupJoiner,
    bool GroupIsOneOf = false,
    bool BeforeIsProperty = false,
    bool AfterIsProperty = false,
    string AfterText = null,
    bool AfterIsPluralSuffix = false,
    bool AfterIsClauseBreak = false,
    bool AlreadySeparated = false)
{
    /// <summary>The site between <paramref name="after"/> and its preceding sibling in <paramref name="group"/>, with what's been emitted so far read off <paramref name="collector"/> (none, when null).</summary>
    public static JoinSite Of(NamedGroupNode group, RegexNode after, RegexCollector collector = null)
    {
        var before = group.Children[group.Children.IndexOf(after) - 1];
        var afterText = after as TextNode;

        return new(
            group.EffectiveChildJoiner,
            GroupIsOneOf: group is GlyphOneOfNode,
            BeforeIsProperty: before is NamedGroupNode,
            AfterIsProperty: after is NamedGroupNode,
            AfterText: afterText?.Text,
            AfterIsPluralSuffix: afterText?.IsPluralSuffix ?? false,
            AfterIsClauseBreak: afterText?.IsClauseBreak ?? false,
            AlreadySeparated: collector?.AlreadySeparated ?? false);
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
    /// Punctuation that binds tight to whatever token sits next to it, so no space is ever written on that side
    /// of it: nothing separates a token from the comma or <c>'s</c> that follows it (see
    /// <see cref="Between"/>), and the joiner after a nib ending in one belongs with that nib (see
    /// <see cref="AdheresToPrecedingText"/>) - so a nib written <c>","</c> renders and reads as <c>,[ ]</c>,
    /// exactly as if it had been written <c>", "</c>.
    /// </summary>
    static readonly HashSet<char> _tightPunctuation = ['\'', ',', '.', ';', ':', '!', '?'];

    /// <summary>
    /// What separates two adjacent siblings - <see cref="Joiner.None"/> for nothing:
    /// <list type="number">
    /// <item>The group's own joiner - except in a one-of, whose pipe only ever sits between two alternatives
    /// (never between an alternative and the literal text around them - see <see cref="GlyphOneOfNode"/>).</item>
    /// <item>Suppressed when the regex so far already ends in a space, which a joiner would double.</item>
    /// <item>Suppressed when <c>after</c> is text that binds to the token before it - opening with tight
    /// punctuation, being a plural suffix (the "s" of "dogs"), or a bare period - or that supplies its own
    /// leading space.</item>
    /// </list>
    /// </summary>
    public static Joiner Between(JoinSite site)
    {
        var joiner = Intended(site);

        if (joiner == Joiner.None || site.AlreadySeparated || BindsToPrecedingToken(site))
            return Joiner.None;

        return joiner;
    }

    /// <summary>What <see cref="Between"/> starts from: the separator the grammar calls for between the two neighbours, before any suppression.</summary>
    public static Joiner Intended(JoinSite site) =>
        site.GroupIsOneOf && !(site.BeforeIsProperty && site.AfterIsProperty) ? Joiner.None : site.GroupJoiner;

    static bool BindsToPrecedingToken(JoinSite site) =>
        site.AfterText is string text
        && (site.AfterIsClauseBreak
            || site.AfterIsPluralSuffix
            || _tightPunctuation.Contains(FirstMatchedChar(text))
            || text.StartsWith(BuiltRegex.EscapedSpace)
            || text[0] == ' ');

    /// <summary>The first character <paramref name="regexText"/> matches literally - reading through a leading escape, since literal nib text arrives escaped (e.g. a <c>"?"</c> nib as <c>\?</c>; see <see cref="Nib.EscapeLiteral"/>).</summary>
    static char FirstMatchedChar(string regexText) =>
        regexText.Length > 1 && regexText[0] == '\\' ? regexText[1] : regexText[0];

    /// <summary>
    /// Where the joiner separating a node from its preceding sibling goes:
    /// <list type="bullet">
    /// <item>A first child has nothing to be separated from: <see cref="JoinerPlacement.None"/>.</item>
    /// <item>A nullable node carries it inside its own group, so it renders exactly when the node does:
    /// <see cref="JoinerPlacement.InsideNodeLeading"/>.</item>
    /// <item>A required node with something guaranteed to render before it takes it unconditionally, just in
    /// front: <see cref="JoinerPlacement.BeforeNode"/>.</item>
    /// <item>A required node with nothing guaranteed before it (every earlier sibling is nullable) can't: it
    /// might be the very start of the match. Its nullable predecessor carries the joiner instead - see
    /// <see cref="OwnsTrailingJoiner"/>.</item>
    /// </list>
    /// </summary>
    public static JoinerPlacement PlaceLeadingJoiner(bool isFirstChild, bool isNullable, bool hasAnchorBefore) =>
        isFirstChild ? JoinerPlacement.None
        : isNullable ? JoinerPlacement.InsideNodeLeading
        : hasAnchorBefore ? JoinerPlacement.BeforeNode
        : JoinerPlacement.None;

    /// <summary>
    /// Whether a nullable node carries, as its own trailing content, the joiner in front of the required node
    /// after it - the case <see cref="PlaceLeadingJoiner"/> leaves unplaced: that node has nothing guaranteed to
    /// render before it, so this node, the last of an all-nullable prefix, is the one place left that renders the
    /// joiner exactly when something precedes it. (Only the node immediately before takes this on, which covers
    /// every shape in use.)
    /// </summary>
    public static bool OwnsTrailingJoiner(bool isNullable, bool hasNext, bool nextIsNullable, bool nextHasAnchorBefore) =>
        isNullable && hasNext && !nextIsNullable && !nextHasAnchorBefore;

    /// <summary>
    /// The separator in front of each repeated item of an X-Of (e.g. <see cref="CompoundOf{T}"/>'s <c>SecondPlus</c>):
    /// a <see cref="ManyOf{T}"/>'s is always <see cref="Joiner.CommaSpace"/> ("a, b, and c" is fixed English list
    /// grammar); a <see cref="CompoundOf{T}"/>'s is its <see cref="JoinedByAttribute"/> - the property's (the more
    /// specific site), else its own type's - else <see cref="Joiner.CommaSpace"/>.
    /// </summary>
    public static Joiner ForRepetition(bool ownerIsCompoundOf, Joiner? propertyJoinedBy, Joiner? typeJoinedBy) =>
        ownerIsCompoundOf ? propertyJoinedBy ?? typeJoinedBy ?? Joiner.CommaSpace : Joiner.CommaSpace;

    /// <summary>Whether the joiner following text <paramref name="precedingText"/> belongs with it when formatted for display - true when it ends in tight punctuation (see <see cref="_tightPunctuation"/>).</summary>
    public static bool AdheresToPrecedingText(string precedingText) =>
        !string.IsNullOrEmpty(precedingText) && _tightPunctuation.Contains(precedingText[^1]);
}
