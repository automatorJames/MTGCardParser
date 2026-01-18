namespace MTGPlexer.CommonDTOs;

public record DynamicSnippet
(
    DynamicSnippetType SnippetType, 
    string Text, Type Type = null, 
    MethodInfo Method = null,
    bool IsEnum = false
);

public enum DynamicSnippetType
{
    Text,
    Type,
    Method
}