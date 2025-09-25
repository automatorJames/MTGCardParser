namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record AlternateValueEnum
(
    Enclosure[] Enclosures,
    int TotalOptions,
    EnumScalarAlternative EnumScalar,
    int LongestSiblingName
)
    : AlternateValue
    (
        Enclosures: Enclosures,
        Value: EnumScalar.RegexString,
        Ordinal: EnumScalar.Ordinal,
        TotalOptions: TotalOptions,
        CommentPrefix: EnumScalar.DisplayName.PadLeft(LongestSiblingName)
    )
    , IMatchableAlternate
{
    new public object CanonicalValue { get; } = EnumScalar.EnumValue;
    new public string CanonicalValueDisplay { get; } = ToFriendlyStringOrPattern(EnumScalar.EnumValue);
    new public Regex AlternateRegex { get; } = EnumScalar.ItemRegex;
    public override string ToString() => base.ToString();
}
