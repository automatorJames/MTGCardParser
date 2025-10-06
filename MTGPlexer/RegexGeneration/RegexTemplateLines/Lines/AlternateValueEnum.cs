namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class AlternateValueEnum : AlternateValue, IMatchableAlternate
{
    new public object CanonicalValue { get; }
    new public string CanonicalValueDisplay { get; }
    new public Regex AlternateRegex { get; }

    public AlternateValueEnum(Enclosure[] enclosures, EnumScalarAlternate enumScalar)
        : base(
            enclosures,
            enumScalar.RegexString,
            enumScalar.DisplayName,
            enumScalar.Ordinal == 0
        )
    {
        CanonicalValue = enumScalar.EnumValue;
        CanonicalValueDisplay = ToFriendlyStringOrPattern(enumScalar.EnumValue);
        AlternateRegex = enumScalar.ItemRegex;
    }

    public override string ToString() => base.ToString();
}