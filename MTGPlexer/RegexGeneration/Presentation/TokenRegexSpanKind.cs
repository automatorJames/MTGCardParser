namespace MTGPlexer.RegexGeneration.Presentation;

/// <summary>
/// Which rendering role a <see cref="SmartSpan"/> plays within TypeRegexPage's formatted output, e.g.
/// <c>TokenRegexSpanKind.GroupOpenHeaderText</c>. Looked up in <see cref="SmartSpanColorPanel"/> to get that
/// role's color treatment. A span with no applicable kind (most bricks) keeps the older per-named-group
/// rainbow coloring instead of falling under any of these.
/// </summary>
public enum TokenRegexSpanKind
{
    /// <summary>A standalone (or synonym-group-header) enum member's regex-side pattern text.</summary>
    EnumMemberRegex,

    /// <summary>One row's regex-side pattern text within a synonym group (multiple represented patterns for the same member).</summary>
    EnumMemberSynonymRegex,

    /// <summary>The leading pipe/space that joins one enum member row's regex text to the row above it.</summary>
    EnumMemberJoiner,

    /// <summary>The member-name portion of a standalone enum member row's comment (e.g. "Tap" in "Tap : 12").</summary>
    EnumMemberName,

    /// <summary>The occurrence-count portion of any enum member row's comment, standalone or grouped (e.g. "12" in "Tap : 12").</summary>
    EnumMemberOccurrenceCount,

    /// <summary>The header row labeling a group of enum-member synonym rows (a <see cref="RegexBrickSynonymSectionHeader"/>).</summary>
    EnumMemberSynonymHeader,

    /// <summary>The divider line closing a group of enum-member synonym rows (a <see cref="RegexBrickSynonymSectionFooter"/>).</summary>
    EnumMemberSynonymFooter,

    /// <summary>The "N omitted" / "All N omitted" summary row for enum members that never occurred (a <see cref="RegexBrickOmittedCount"/>).</summary>
    OmittedCount,

    /// <summary>A run of whitespace that represents a connective space between words, whether it's a dedicated "[ ]" joiner brick or a plain space embedded in a multi-word literal match.</summary>
    ConnectiveSpace,

    /// <summary>The non-space text of a literal-match brick (a <see cref="RegexNode"/> produced by a <see cref="TextNode"/>).</summary>
    LiteralMatch,

    /// <summary>A structural joiner between sibling bricks in the regex body (e.g. "-", "_", "."), excluding the space joiner (see <see cref="ConnectiveSpace"/>) and enum-internal pipes (see <see cref="EnumMemberJoiner"/>).</summary>
    RegexJoiner,

    /// <summary>The "Type" (or "Type: UnderlyingType" base) label on a group's opening border.</summary>
    GroupOpenHeaderText,

    /// <summary>The ": UnderlyingType" suffix appended to a group's opening label when its name doesn't already say its type.</summary>
    GroupOpenHeaderDisambiguator,

    /// <summary>The group's own name on its closing border.</summary>
    GroupFooterText,

    /// <summary>The "(quantifier)" parenthetical appended to a group's closing label when it carries a non-default quantifier.</summary>
    GroupFooterQuantifierReminder,

    /// <summary>The border glyphs (corners, horizontal padding line, vertical wall bars) drawn around a group's comment box, optionally varied by <see cref="NamedGroupNode.NodeType"/>.</summary>
    GroupBorderWall,

    /// <summary>The "  #  "-style separator between a line's regex column and its comment column.</summary>
    RegexCommentSeparator,
}
