namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class SynonymSetHeader : AlternateValueEnum
{
    public AlternateValueEnum CanonicalParent { get; }

    public SynonymSetHeader(AlternateValueEnum canonicalParent)
        : base(
            canonicalParent.Enclosures,
            canonicalParent.EnumScalar
        )
    {
        CanonicalParent = canonicalParent;
    }

    public override string ToString() => base.ToString();
}