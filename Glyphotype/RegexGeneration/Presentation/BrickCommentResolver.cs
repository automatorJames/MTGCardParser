namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// The single place that decides what display comment text a <see cref="RegexBrick"/> gets. Nothing in
/// the Graph layer assigns comments, so every brick's comment either comes from here (for the ordinary,
/// structural brick kinds handled below) or from <see cref="EnumSectionBuilder"/> (for the enum member
/// rows and synonym/omitted-count bricks it synthesizes, which are already fully formatted by the time
/// they exist).
/// </summary>
internal static class BrickCommentResolver
{
    /// <summary>Resolves and assigns <see cref="RegexBrick.CommentFormatted"/> for a single brick.</summary>
    public static void Apply(RegexBrick brick) =>
        brick.CommentFormatted = brick switch
        {
            RegexBrickGroupOpen open => ResolveGroupOpenComment(open),
            RegexBrickGroupClose close => ResolveGroupCloseComment(close),
            RegexBrickJoiner joiner => $"joiner {joiner.Joiner.ToString().ToFriendlyCase(TitleDisplayOption.Lower)}",
            _ when brick.Parent is TextNode => "literal match",
            _ => ""
        };

    /// <summary>"Type" (or "Type: UnderlyingType" when the group's own name doesn't already say its type). Also splits the result into <see cref="RegexBrickGroupOpen.TypeLabel"/>/<see cref="RegexBrickGroupOpen.TypeDisambiguator"/> so the two can be colored separately.</summary>
    static string ResolveGroupOpenComment(RegexBrickGroupOpen open)
    {
        var group = open.NamedGroupParent;
        var typeLabel = ResolveTypeLabel(group);
        var disambiguator = group.Name == group.Navigation.UnderlyingType.Name ? "" : $": {FormatTypeNameFriendly(group.Navigation.UnderlyingType)}";

        open.TypeLabel = typeLabel;
        open.TypeDisambiguator = disambiguator;

        return typeLabel + disambiguator;
    }

    /// <summary>
    /// The friendly "kind" label shown in a group's open-comment - by node type for the cases with a real
    /// behavioral difference, else by <see cref="Navigation"/> data for a plain <see cref="GlyphNode"/> that
    /// merely wraps a <see cref="CompoundOfBase"/> or carries an <see cref="Glyphotype.Quantifier.Optional"/>
    /// quantifier - neither of which needs (or has) a node type of its own to tell apart from an ordinary
    /// nested token group.
    /// </summary>
    static string ResolveTypeLabel(NamedGroupNode group) =>
        group switch
        {
            EnumNode => "enum",
            BoolNode => "bool",
            IntNode => "int",
            GlyphOneOfNode => "one of",
            DynamicGlyphNode => "dynamic",
            CommaSeparatedItemNode => "internals",
            GlyphNode glyphNode when typeof(CompoundOfBase).IsAssignableFrom(glyphNode.Navigation.NodeType) => "compound of",
            GlyphNode glyphNode when glyphNode.Quantifier == Glyphotype.Quantifier.Optional => "optional",
            _ => "token",
        };

    /// <summary>
    /// A type's friendly display name, e.g. "Card Keyword" for <c>CardKeyword</c>. For a generic type (e.g.
    /// <c>CompoundOf&lt;CardType&gt;</c>, whose raw <see cref="Type.Name"/> is "CompoundOf`1"), renders the
    /// friendly-cased open generic name followed by its friendly-cased type argument(s) in parentheses, e.g.
    /// "Compound Of (Card Type)", instead of leaking the backtick-arity suffix.
    /// </summary>
    static string FormatTypeNameFriendly(Type type)
    {
        if (!type.IsGenericType)
            return type.Name.ToFriendlyCase();

        var baseName = type.Name[..type.Name.IndexOf('`')].ToFriendlyCase();
        var typeArgs = string.Join(", ", type.GetGenericArguments().Select(FormatTypeNameFriendly));

        return $"{baseName} ({typeArgs})";
    }

    /// <summary>"Name" (or "Name (quantifier)" when the group carries a non-default quantifier). Also splits the result into <see cref="RegexBrickGroupClose.NameText"/>/<see cref="RegexBrickGroupClose.QuantifierReminder"/> so the two can be colored separately.</summary>
    static string ResolveGroupCloseComment(RegexBrickGroupClose close)
    {
        var group = close.NamedGroupParent;
        var quantifierComment = group.Navigation.Quantifier?.ToString().ToFriendlyCase(TitleDisplayOption.Lower);
        var quantifierReminder = quantifierComment == null ? "" : $" ({quantifierComment})";

        close.NameText = group.Name.ToFriendlyCase();
        close.QuantifierReminder = quantifierReminder;

        return close.NameText + quantifierReminder;
    }
}
