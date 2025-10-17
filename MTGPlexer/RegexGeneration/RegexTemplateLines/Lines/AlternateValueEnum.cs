
namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValueEnum : AlternateValue, IMatchableAlternate
{
    new public object CanonicalValue { get; }
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
        EnumScalar = enumScalar;
    }

    public override string ToString() => base.ToString();
}