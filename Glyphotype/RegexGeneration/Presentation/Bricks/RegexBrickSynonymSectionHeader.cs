namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// A synthetic brick labeling a group of enum-member rows that all represent synonyms of the same member.
/// Its comment is a "Name : count" pair split the same way a standalone <see cref="RegexBrickValue"/> row's
/// is, so both render with the same <see cref="RegexSpanKind.CommentEnumMemberName"/>/<see cref="RegexSpanKind.CommentEnumMemberOccurrenceCount"/>
/// treatment rather than a distinct one.
/// </summary>
public class RegexBrickSynonymSectionHeader : RegexBrick
{
    /// <summary>Display-only: the member-name field of this header's comment.</summary>
    public string NameCommentFormatted { get; }

    /// <summary>Display-only: the ": count" field of this header's comment.</summary>
    public string CountCommentFormatted { get; }

    public RegexBrickSynonymSectionHeader(RegexNode parentNode, string nameCommentFormatted, string countCommentFormatted)
        : base(parentNode, null)
    {
        NameCommentFormatted = nameCommentFormatted;
        CountCommentFormatted = countCommentFormatted;
        CommentFormatted = nameCommentFormatted + countCommentFormatted;
    }
}
