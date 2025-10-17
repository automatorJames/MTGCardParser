namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class SynonymValueEnum : AlternateValueEnum
{
    public AlternateValueEnum CanonicalParent { get; }
    public int ParentTotalInstanceCount { get; }
    new public object CanonicalValue { get; }

    public SynonymValueEnum(AlternateValueEnum canonicalParent, int parentTotalInstanceCount, string synonym)
        : base(
            canonicalParent.Enclosures,
            canonicalParent.EnumScalar
        )
    {
        CanonicalParent = canonicalParent;
        ParentTotalInstanceCount = parentTotalInstanceCount;
        CanonicalValue = synonym;
    }

    public override string ToString() => base.ToString();
}