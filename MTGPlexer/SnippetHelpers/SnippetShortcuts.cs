using System.Runtime.CompilerServices;

namespace MTGPlexer.SnippetHelpers;

public static class SnippetShortcuts
{
    public static PropertySnippet Prop(object member, Proptions proptions = Proptions.None, [CallerArgumentExpression("member")] string expression = "")
    {
        // 1. Get the property name from the expression string
        var lastDot = expression.LastIndexOf('.');
        var name = lastDot == -1 ? expression : expression[(lastDot + 1)..];

        // 2. Get the Calling Type using StackFrame
        // We use NoInlining on this method to ensure index 1 is ALWAYS the caller.
        var callerFrame = new StackFrame(1);
        var callerMethod = callerFrame.GetMethod();
        var callerType = callerMethod?.DeclaringType;

        // 3. Find the PropertyInfo on that type
        PropertyInfo propInfo = callerType?.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        // 4. Add relevant attribute metadata
        if (propInfo.IsDefined(typeof(OneOrMoreAttribute)))
            proptions |= Proptions.OneOrMore;

        return new PropertySnippet(name, propInfo, proptions)
        {
            IsPlural = proptions.HasFlag(Proptions.Plural),
            IsOptional = proptions.HasFlag(Proptions.Optional),
        };
    }

    public static SnippetAlternatives Alt(params string[] alternatives) =>
        new SnippetAlternatives(alternatives);

    public static SnippetOptional Opt(string optionalText) =>
        new SnippetOptional(optionalText);

    public static SnippetOptionalPlural Plural() =>
        new SnippetOptionalPlural();

    public static IReadOnlyList<string> GetPublicStaticMethodNames()
    {
        return typeof(SnippetShortcuts)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .ToList();
    }
}