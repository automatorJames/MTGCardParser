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
        CommentPrefix: EnumScalar.FriendlyName.PadLeft(LongestSiblingName)
    )
{
    public override string ToString() => base.ToString();
}
