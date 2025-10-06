namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// The base class for all TokenUnit properties with a name Regex capture group whose pattern, is 
/// associated with some scalar property.
/// </summary>
public abstract class ScalarCapturePropBase : CaptureGroupPropBase
{
    public override Regex MatchRegex => TokenTypeRegistry.PropScalarAlternativeSets[RegexPropInfo].CollectiveRegex;

    public ScalarAlternateSet ScalarAlternativeSet { get; protected set; }

    public ScalarCapturePropBase(RegexPropInfo captureProp) : base(captureProp)
    {
        SetScalarAlternativeSet(captureProp);
    }

    protected virtual void SetScalarAlternativeSet(RegexPropInfo captureProp)
    {
        if (TokenTypeRegistry.PropScalarAlternativeSets.TryGetValue(captureProp, out var scalarAlternativeSet))
            ScalarAlternativeSet = scalarAlternativeSet;
        else
        {
            var captureAlternatives = (captureProp.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [captureProp.Name])
                .OrderByDescending(s => s.Length).ToList();

            ScalarAlternativeSet = new(captureAlternatives);
        }
    }

    public override string ToString() => base.ToString();

}
