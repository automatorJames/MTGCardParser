namespace MTGPlexer.RegexGeneration.Presentation;

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
            RegexBrickJoiner joiner => $"joiner {joiner.Joiner}",
            _ when brick.Parent is TextNode => "literal match",
            _ => ""
        };

    /// <summary>"Type" (or "Type: UnderlyingType" when the group's own name doesn't already say its type).</summary>
    static string ResolveGroupOpenComment(RegexBrickGroupOpen open)
    {
        var group = open.NamedGroupParent;
        var typeLabel = group.NodeType.ToString().ToFriendlyCase();

        return group.Name == group.Navigation.UnderlyingType.Name
            ? typeLabel
            : $"{typeLabel}: {group.Navigation.UnderlyingType.Name}";
    }

    /// <summary>"Name" (or "Name (quantifier)" when the group carries a non-default quantifier).</summary>
    static string ResolveGroupCloseComment(RegexBrickGroupClose close)
    {
        var group = close.NamedGroupParent;
        var quantifierComment = group.Navigation.Quantifier?.ToString().ToFriendlyCase(TitleDisplayOption.Lower);

        return quantifierComment == null ? group.Name : $"{group.Name} ({quantifierComment})";
    }
}
