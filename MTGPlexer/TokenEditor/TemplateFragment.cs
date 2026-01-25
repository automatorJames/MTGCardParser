namespace MTGPlexer.TokenEditor;

public record TemplateFragment(
    string Text,
    string Id = null,
    bool IsPill = false,
    string TypeName = null,
    string MethodName = null,
    string[] Args = null);