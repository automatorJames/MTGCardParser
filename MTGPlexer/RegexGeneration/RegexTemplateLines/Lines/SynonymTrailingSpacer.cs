namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class SynonymTrailingSpacer : AlternateValueEnum
{

    public SynonymTrailingSpacer(AlternateValueEnum canonicalParent)
        : base(
            canonicalParent.Enclosures,
            canonicalParent.EnumScalar
        )
    {
    }

    public override string ToString() => base.ToString();
}