
namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickGroupOpen : RegexBrickGroupBookend
{
    public string GroupName { get; }

    public RegexBrickGroupOpen(RegexNode parentNode, string groupName, string comment) 
        : base(parentNode, GetRegex(groupName), comment)
    {
        GroupName = groupName;
    }

    static string GetRegex(string groupName) =>
        $"(?<{groupName}>";

    public void SetFormattedGroupName(string formattedName)
    {
        RegexFormatted = GetRegex(formattedName);
    }
}

