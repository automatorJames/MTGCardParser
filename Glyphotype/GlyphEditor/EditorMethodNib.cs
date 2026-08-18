
using Glyphotype.RegexGeneration.Graph;

namespace Glyphotype.GlyphEditor;

public record EditorMethodNib : EditorBlockNib
{
    public ShortcutNibMethod MethodType { get; init; }
    public string[] Args { get; init; }
    public MethodInfo Method { get; init; }

    public EditorMethodNib(ShortcutNibMethod methodType, string[] args, string id)
        : base(
            editorRepresentation: $"@{methodType}({string.Join(", ", args)})",
            parameterRepresentation: args.Length == 0 ? $"{methodType}()" : $"{methodType}(\"{string.Join("\", \"", args)}\")",
            id: id)
    {
        MethodType = methodType;
        Args = args;
        Method = typeof(Glyph).GetMethod(methodType.ToString())
                 ?? throw new Exception($"Method {methodType} not found in NibShortcuts");
    }

    public override string GetParameterHtmlRepresentation()
    {
        var methodName = Span($"{MethodType}", SpanClass.method);
        if (Args.Length == 0)
            return $"{methodName}{Span("()", SpanClass.identifier)}";

        var joinedArgs = string.Join(
            Span("\"", SpanClass.stringliteral) + Span(", ", SpanClass.identifier) + Span("\"", SpanClass.stringliteral),
            Args);

        return $"{methodName}{Span("(", SpanClass.identifier)}{Span("\"", SpanClass.stringliteral)}" +
               $"{joinedArgs}{Span("\"", SpanClass.stringliteral)}{Span(")", SpanClass.identifier)}";
    }

    public override NamedGroupNode ToNamedGroupNode()
    {
        //var parameters = Method.GetParameters();
        //
        //object[] invokeArgs = parameters.Length switch
        //{
        //    0 => Array.Empty<object>(),
        //    1 when parameters[0].ParameterType == typeof(string[]) => [Args],
        //    1 => [Args],
        //    2 => [null!, Args],
        //    _ => null
        //};
        //
        //if (invokeArgs == null)
        //    throw new Exception($"Could not map parameters for '{Method.Name}'");
        //
        //var shortcutNib = (Nib)Method.Invoke(null, invokeArgs)!;
        //return new TextSegment(shortcutNib);

        throw new NotImplementedException();
    }
}