namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Which rendering role a <see cref="SmartSpan"/> plays within TypeRegexPage's formatted output, e.g.
/// <c>TokenRegexSpanKind.CommentGroupOpenHeaderText</c>. Looked up in <see cref="SmartSpanControlPanel"/> to
/// get that role's color treatment. A span with no applicable kind (most bricks) keeps the older
/// per-named-group rainbow coloring instead of falling under any of these. Named by which column of a
/// rendered line they belong to — <c>Regex...</c> for the regex column, <c>Comment...</c> for the comment
/// column — except <see cref="RegexCommentSeparator"/>, the "  #  " glue between the two columns.
/// </summary>
public enum TokenRegexSpanKind
{
    // --- Regex column ---

    /// <summary>One enum member row's regex-side pattern text — a standalone row or one row within a synonym group; the two aren't visually distinguished.</summary>
    RegexEnumMember,

    /// <summary>The leading pipe/space that joins one enum member row's regex text to the row above it.</summary>
    RegexEnumMemberJoiner,

    /// <summary>A run of whitespace that represents a connective space between words, whether it's a dedicated "[ ]" joiner brick or a plain space embedded in a multi-word literal match.</summary>
    RegexConnectiveSpace,

    /// <summary>The non-space text of a literal-match brick (a <see cref="RegexNode"/> produced by a <see cref="TextNode"/>).</summary>
    RegexLiteralMatch,

    /// <summary>A structural joiner between sibling bricks in the regex body (e.g. "-", "_", "."), excluding the space joiner (see <see cref="RegexConnectiveSpace"/>) and enum-internal pipes (see <see cref="RegexEnumMemberJoiner"/>).</summary>
    RegexJoiner,

    // --- Regex/comment separator ---

    /// <summary>The "  #  "-style separator between a line's regex column and its comment column.</summary>
    RegexCommentSeparator,

    // --- Comment column ---

    /// <summary>The member-name portion of an enum member row's comment (e.g. "Tap" in "Tap : 12"), whether a standalone row or a synonym-group header — the two aren't visually distinguished.</summary>
    CommentEnumMemberName,

    /// <summary>The occurrence-count portion of any enum member row's comment, standalone or grouped (e.g. "12" in "Tap : 12").</summary>
    CommentEnumMemberOccurrenceCount,

    /// <summary>The divider line closing a group of enum-member synonym rows (a <see cref="RegexBrickSynonymSectionFooter"/>). Has no standalone-row equivalent, so it keeps its own role.</summary>
    CommentEnumMemberSynonymFooter,

    /// <summary>The "N omitted" / "All N omitted" summary row for enum members that never occurred (a <see cref="RegexBrickOmittedCount"/>).</summary>
    CommentOmittedCount,

    /// <summary>The comment-column text ("literal match") of a literal-match brick, as opposed to its regex-column text (see <see cref="RegexLiteralMatch"/>).</summary>
    CommentLiteralMatch,

    /// <summary>The comment-column text (e.g. "joiner -") of a <see cref="RegexBrickJoiner"/>, as opposed to its regex-column text (see <see cref="RegexJoiner"/>).</summary>
    CommentJoiner,

    /// <summary>The "Type" (or "Type: UnderlyingType" base) label on a group's opening border.</summary>
    CommentGroupOpenHeaderText,

    /// <summary>The ": UnderlyingType" suffix appended to a group's opening label when its name doesn't already say its type.</summary>
    CommentGroupOpenHeaderDisambiguator,

    /// <summary>The group's own name on its closing border.</summary>
    CommentGroupFooterText,

    /// <summary>The "(quantifier)" parenthetical appended to a group's closing label when it carries a non-default quantifier.</summary>
    CommentGroupFooterQuantifierReminder,

    /// <summary>The border glyphs (corners, horizontal padding line, vertical wall bars) drawn around a group's comment box.</summary>
    CommentGroupBorderWall,
}
