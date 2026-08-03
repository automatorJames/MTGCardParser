
namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>The opening brick of a named group, e.g. <c>(?&lt;name&gt;</c>.</summary>
public class RegexBrickGroupOpen : RegexBrickGroupBookend
{
    /// <summary>The group's fully qualified capture-group name, as used in the matching regex.</summary>
    public string GroupName { get; }

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

