
namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValueEnum : AlternateValue, IMatchableAlternateEnum
{
    new public object CanonicalValue { get; }
    public Type EnumType { get; }
    public int EnumMemberCount { get; }
    public EnumScalarAlternate EnumScalar { get; }


    public AlternateValueEnum(Enclosure[] enclosures, EnumScalarAlternate enumScalar, int enumMemberCount)
        : base(
            enclosures,
            enumScalar.RegexString,
            enumScalar.DisplayName,
            enumScalar.Ordinal
        )
    {
        CanonicalValue = enumScalar.EnumValue;
        EnumType = enumScalar.EnumType;
        EnumMemberCount = enumMemberCount;
        EnumScalar = enumScalar;
    }

    public override string ToString() => base.ToString();
}