using System.Collections.Immutable;
namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also a TokenUnit (i.e. a child TokenUnit), 
/// or a TokenUnit x-Of PolyItemCapure. During compilation of a RegexTemplate, thie record creates an instance 
/// of the child TokenUnit type and gets its rendered Regex string to add it to the parent TokenUnit's rendered Regex.
/// </summary>
public record TokenUnitSegment : CaptureGroupSegmentBase
{
    public override Regex ManyMatchRegex => TokenTypeRegistry.Templates[TemplatePropInfo.UnderlyingType].Regex;
    protected ImmutableList<RegexSegmentBase> ChildSegments { get; init; }

    public TokenUnitSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
        ChildSegments = template.RegexSegments.ToImmutableList();
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo);
        ConcatenatingComposer.Instance.Compose(builder, ChildSegments.ToList());
        var groupIsOptional = TemplatePropInfo.Prop.IsDefined(typeof(OptionalComponentAttribute));
        GroupQuantifier? groupQuantifier = groupIsOptional ? GroupQuantifier.Optional : null;
        builder.CloseGroup(groupQuantifier);
    }

    public override object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, Capture scopedCapture)
    {
        MatchTraversalState typeMatch = new(TemplatePropInfo.UnderlyingType, parentTokenUnitMatch, TemplatePropInfo.Name);
        var tokenUnitInstance = TokenUnit.InstantiateFromMatch(typeMatch);

        return tokenUnitInstance;
    }

    public override string ToString() => base.ToString();
}