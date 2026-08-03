namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>A synthetic brick labeling a group of enum-member rows that all represent synonyms of the same member.</summary>
public class RegexBrickSynonymSectionHeader : RegexBrick
{
    public RegexBrickSynonymSectionHeader(RegexNode parentNode, string comment)
        : base(parentNode, null)
    {
        CommentFormatted = comment;
    }
}
