namespace Glyphotype.RegexGeneration.Graph.Nodes;

public class GlyphCompoundOfNode : GlyphNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.CompoundOf;
    public GlyphCompoundOfNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    /// <summary>Adds one <see cref="EnumMemberNode"/> per (member, synonym pattern) pair — including plural variants when <see cref="OptionalPluralAttribute"/> is declared on the enum type.</summary>
    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        var enumType = Navigation.UnderlyingType;
        var enumMembers = Enum.GetValues(enumType).Cast<object>().ToList();

        for (int i = 0; i < enumMembers.Count; i++)
        {
            var enumMember = enumMembers[i];
            var enumAsString = enumMember.ToString();
            var field = enumType.GetField(enumAsString);

            List<string> patterns =
                field.GetCustomAttribute<RegexPatternAttribute>()?.Patterns.ToList()
                ?? [enumAsString.ToFriendlyCase(TitleDisplayOption.Lower)];

            if (enumType.IsDefined(typeof(OptionalPluralAttribute)))
            {
                patterns = patterns
                    .SelectMany(x => new[] { x, x.AddPluralization(makeOptional: false) })
                    .ToList();
            }

            for (int j = 0; j < patterns.Count; j++)
                children.Add(new EnumMemberNode(
                    parentNode: this,
                    name: enumAsString,
                    scalarValue: enumMember,
                    regexString: patterns[j],
                    positionAmongSiblings: i,
                    positionAmongSynonyms: enumMembers.Count > 1 ? j : null));
        }
    }

    protected override void AppendRegexBricksInnerContent(RegexCollector collector)
    {
        var firstItem = Children[0];
        var secondPlus = Children[1];

        child 

        // append all children and joiners
        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            child.AppendRegexBricks(collector);
            var joiner = Navigation.GlyphTypeConfiguration?.ChildJoiner ?? ChildJoiner ?? Joiner.None;

            bool shouldAddJoiner =
                i < Children.Count - 1
                && joiner != Joiner.None
                && collector.LastChar != ' '
                && !(Children[i + 1] is TextNode textNode && textNode.FirstChar == '\'');

            if (shouldAddJoiner)
                collector.Append(new RegexBrickJoiner(this, joiner));
        }
    }
}