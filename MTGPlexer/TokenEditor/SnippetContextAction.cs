using System.ComponentModel;

namespace MTGPlexer.TokenEditor;

public record SnippetContextAction
{
    public EditorPropertySnippet Snippet { get; }
    public ContextActionType ActionType { get; }
    public string Html { get; }
    public bool AddSectionBreak { get; }

    public SnippetContextAction(EditorPropertySnippet snippet, ContextActionType type)
    {
        Snippet = snippet;
        ActionType = type;

        var appearanceAttribute = typeof(ContextActionType).GetField(type.ToString()).GetCustomAttribute<ContextActionAppearanceAttribute>();
        Html = appearanceAttribute.GetHtml();
        AddSectionBreak = appearanceAttribute.AddSectionBreak;
    }

    public static List<SnippetContextAction> GetSnippetContextOptions(EditorPropertySnippet snippet)
    {
        List<ContextActionType> actionList = [];

        var xOfType = snippet.XOfType;
        var proptions = snippet.Proptions;

        if (xOfType == XOfType.OneOf)
            actionList.Add(ContextActionType.RemoveOneOf);
        else
            actionList.Add(ContextActionType.ConvertToOneOf);

        if (xOfType == XOfType.ManyOf)
            actionList.Add(ContextActionType.RemoveManyOf);
        else
            actionList.Add(ContextActionType.ConvertToManyOf);

        if (xOfType == XOfType.CompoundOf)
            actionList.Add(ContextActionType.RemoveCompoundOf);
        else
            actionList.Add(ContextActionType.ConvertToCompoundOf);

        if (proptions.HasFlag(Proptions.Plural))
            actionList.Add(ContextActionType.RemovePlural);
        else
            actionList.Add(ContextActionType.MakePlural);

        if (proptions.HasFlag(Proptions.Optional))
            actionList.Add(ContextActionType.RemoveOptional);
        else
            actionList.Add(ContextActionType.MakeOptional);

        actionList.Add(ContextActionType.Delete);

        return actionList
            .Select(x => new SnippetContextAction(snippet, x))
            .ToList();
    }
}

public enum ContextActionType
{
    [ContextActionAppearance("Delete", MaterialIcon.delete, ContextMenuColor.Red, addSectionBreak: true)]
    Delete,

    [ContextActionAppearance("Convert to One-Of", MaterialIcon.code, ContextMenuColor.Magenta)]
    ConvertToOneOf,

    [ContextActionAppearance("Remove One-Of", MaterialIcon.code_off, ContextMenuColor.Magenta)]
    RemoveOneOf,

    [ContextActionAppearance("Convert to Many-Of", MaterialIcon.code, ContextMenuColor.Blue)]
    ConvertToManyOf,

    [ContextActionAppearance("Remove Many-Of", MaterialIcon.code_off, ContextMenuColor.Blue)]
    RemoveManyOf,

    [ContextActionAppearance("Convert to Compound-Of", MaterialIcon.code, ContextMenuColor.Cyan)]
    ConvertToCompoundOf,

    [ContextActionAppearance("Remove Compound-Of", MaterialIcon.code_off, ContextMenuColor.Cyan)]
    RemoveCompoundOf,

    [ContextActionAppearance("Make Plural", MaterialIcon.stacks, ContextMenuColor.Green)]
    MakePlural,

    [ContextActionAppearance("Remove Plural", MaterialIcon.remove, ContextMenuColor.Green)]
    RemovePlural,

    [ContextActionAppearance("Make Optional", MaterialIcon.question_mark, ContextMenuColor.Yellow)]
    MakeOptional,

    [ContextActionAppearance("Remove Plural", MaterialIcon.remove, ContextMenuColor.Yellow)]
    RemoveOptional
}

public enum ContextMenuColor
{
    [Description("#B07C6E")]
    Red,

    [Description("#B0B06E")]
    Yellow,

    [Description("#8AB06E")]
    YellowGreen,

    [Description("#6EB08F")]
    Green,

    [Description("#6E9DB0")]
    Cyan,

    [Description("#6E70B0")]
    Blue,

    [Description("#9D6EB0")]
    Magenta,

    [Description("#8F8F8F")]
    Grey
}

public enum MaterialIcon
{
    add,
    remove,
    delete,
    code,
    code_off,
    question_mark,
    stacks
}