namespace MTGPlexer.TokenEditor;

public record EditorMethodSnippet(ShortcutSnippetMethod MethodType, params string[] Args) 
    : EditorSnippet(
        EditorRepresentation: $"@{MethodType}({string.Join(", ", Args)})",
        ParameterRepresentation: GetParameterRepresentation(MethodType, Args),
        DisplayAsBlockInEditor: true
        )
{
    public MethodInfo Method { get; } = typeof(SnippetShortcuts).GetMethod(MethodType.ToString());

    static string GetParameterRepresentation(ShortcutSnippetMethod method, params string[] args)
    {
        if (args.Length == 0)
            return $"{method}()";
        else
            return $"{method}(\"{string.Join("\", \"", args)}\")";
    }

    public override string GetParameterHtmlRepresentation()
    {
        if (Args.Length == 0)
            return $"{Span($"{Method}", SpanClass.method)}{Span("()", SpanClass.identifier)}";

        var methodWithArgs = $"{Span($"{Method}", SpanClass.method)}{Span("(", SpanClass.identifier)}{Span("\"", SpanClass.stringliteral)}";
        methodWithArgs += string.Join(Span("\"", SpanClass.stringliteral) + Span(", ", SpanClass.identifier) + Span("\"", SpanClass.stringliteral), Args);
        methodWithArgs += Span("\"", SpanClass.stringliteral);
        methodWithArgs += Span(")", SpanClass.identifier);

        return methodWithArgs;
    }

    public override RegexSegmentBase GetRegexSegment()
    {
        var parameters = Method.GetParameters();

        object[] invokeArgs = parameters.Length switch
        {
            // 0 parameters: Return empty array so invokeArgs is NOT null
            0 => Array.Empty<object>(),

            // 1 parameter (string array)
            1 when parameters[0].ParameterType == typeof(string[])
                => new object[] { Args },

            // 1 parameter (default string)
            1 => new object[] { Args },

            // 2 parameters
            2 => new object[] { null, Args },

            // Default: No matching signature found
            _ => null
        };

        if (invokeArgs != null)
        {
            // If invokeArgs is Array.Empty<object>(), Invoke will be called with no parameters.
            var shortcutSnippet = (Snippet)Method.Invoke(null, invokeArgs);
            return new TextSegment(shortcutSnippet);
        }
        else
            throw new Exception($"Could not invoke mehtod '{Method.Name}' with args {string.Join(", " , Args)}");
    }
}

public enum ShortcutSnippetMethod
{
    Alt,
    Opt,
    NoSpace,
    Plural
}
