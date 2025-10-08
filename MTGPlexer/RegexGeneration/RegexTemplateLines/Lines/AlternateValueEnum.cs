
namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValueEnum : AlternateValue, IMatchableAlternateEnum
{
    new public object CanonicalValue { get; }
    new public string CanonicalValueDisplay { get; }
    new public Regex AlternateRegex { get; }
    public Type EnumType { get; }
    public int EnumMemberCount { get; }

    public AlternateValueEnum(Enclosure[] enclosures, EnumScalarAlternate enumScalar, int enumMemberCount)
        : base(
            enclosures,
            enumScalar.RegexString,
            enumScalar.DisplayName,
            enumScalar.Ordinal
        )
    {
        CanonicalValue = enumScalar.EnumValue;
        CanonicalValueDisplay = ToFriendlyStringOrPattern(enumScalar.EnumValue);
        AlternateRegex = enumScalar.ItemRegex;
        EnumType = enumScalar.EnumType;
        EnumMemberCount = enumMemberCount;
    }

    public override string ToString() => base.ToString();
}