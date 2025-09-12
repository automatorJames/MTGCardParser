using MTGPlexer.CommonDTOs.StructuredMatches;

namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// The base class for all TokenUnit properties with a name Regex capture group whose pattern, is 
/// associated with some scalar property.
/// </summary>
public abstract class ScalarCapturePropBase : CaptureGroupPropBase
{
    public override Regex MatchRegex => TokenTypeRegistry.PropScalarAlternativeSets[RegexPropInfo].Regex;

    public ScalarAlternativeSet ScalarAlternativeSet { get; protected set; }

    public ScalarCapturePropBase(RegexPropInfo captureProp) : base(captureProp)
    {
        SetScalarAlternativeSet();
    }

    protected virtual void SetScalarAlternativeSet()
    {
        if (TokenTypeRegistry.PropScalarAlternativeSets.TryGetValue(RegexPropInfo, out var scalarAlternativeSet))
            ScalarAlternativeSet = scalarAlternativeSet;
        else
        {
            var captureAlternatives = (RegexPropInfo.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [RegexPropInfo.Name])
                .OrderByDescending(s => s.Length).ToList();

            ScalarAlternativeSet = new(captureAlternatives);
        }
    }

    public override string ToString() => base.ToString();

}
