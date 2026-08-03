
namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>The opening brick of a named group, e.g. <c>(?&lt;name&gt;</c>.</summary>
public class RegexBrickGroupOpen : RegexBrickGroupBookend
{
    /// <summary>The group's fully qualified capture-group name, as used in the matching regex.</summary>
    public string GroupName { get; }

    /// <summary>Display-only: the "Type" portion of this group's opening comment (e.g. "Enum"). Populated by <c>BrickCommentResolver</c>.</summary>
    public string TypeLabel { get; set; } = "";

    /// <summary>Display-only: the ": UnderlyingType" suffix appended when the group's name doesn't already say its type, or "" when not needed. Populated by <c>BrickCommentResolver</c>.</summary>
    public string TypeDisambiguator { get; set; } = "";

    public RegexBrickGroupOpen(RegexNode parentNode, string groupName)
        : base(parentNode, GetRegex(groupName))
    {
        GroupName = groupName;
    }

    static string GetRegex(string groupName) =>
        $"(?<{groupName}>";

    /// <summary>Overrides <see cref="RegexBrick.RegexFormatted"/> to display a shorter, disambiguated group name instead of the fully qualified one.</summary>
    public void SetFormattedGroupName(string formattedName)
    {
        RegexFormatted = GetRegex(formattedName);
    }
}

