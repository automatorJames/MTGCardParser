namespace MTGPlexer.CommonDTOs;

public record DynamicSnippet
(
    DynamicSnippetType SnippetType, 
    string Text, Type Type = null, 
    MethodInfo Method = null,
    bool IsEnum = false
)
{
    public override string ToString()
    {
        return SnippetType switch
        {
            DynamicSnippetType.Type => $"@{Type.Name}",
            DynamicSnippetType.Method => $"{Method.Name}({Text})",
            _ => $"\"{Text}\""
        };
    }
}

public enum DynamicSnippetType
{
    Text,
    Type,
    Method
}