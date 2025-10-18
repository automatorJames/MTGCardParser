namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines.Alternates;

public class AlternateValueEnum : AlternateValue
{
    new public object CanonicalValue { get; }
    public EnumScalarAlternate EnumScalar { get; }
    public string DisplayOverrideName { get; set; }


    public AlternateValueEnum(Enclosure[] enclosures, EnumScalarAlternate enumScalar)
        : base(
            enclosures,
            enumScalar.RegexString,
            enumScalar.DisplayName
        )
    {
        CanonicalValue = enumScalar.EnumValue;
        EnumScalar = enumScalar;
    }

    public override string ToString() => base.ToString();
}