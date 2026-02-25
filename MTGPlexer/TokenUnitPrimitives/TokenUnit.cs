using System.Runtime.CompilerServices;

namespace MTGPlexer.TokenUnitPrimitives;

public abstract class TokenUnit
{
    public virtual Snippet[] Snippets { get; } = [];
    public virtual Joiner Joiner => Joiner.Space;
    public virtual GroupQuantifier? Quantifier => null;

    public PropertySnippet Prop(object member, Proptions proptions = Proptions.None, [CallerArgumentExpression("member")] string expression = "")
    {
        var lastDot = expression.LastIndexOf('.');
        var name = lastDot == -1 ? expression : expression;

        // 1. Get the actual, fully resolved runtime type
        var actualType = this.GetType();

        // 2. Fetch the PropertyInfo from the closed type. 
        // This automatically resolves `T` to the concrete type.
        PropertyInfo propInfo = actualType.GetProperty(name,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        return new PropertySnippet(name, propInfo, proptions)
        {
            IsPlural = proptions.HasFlag(Proptions.Plural),
            IsOptional = proptions.HasFlag(Proptions.Optional),
        };
    }

    public SnippetAlternatives Alt(params string[] alternatives) =>
    new SnippetAlternatives(alternatives);

    public SnippetOptional Opt(string optionalText) =>
        new SnippetOptional(optionalText);

    public SnippetOptionalPlural Plural() =>
        new SnippetOptionalPlural();

    public CaptureContext CaptureContext { get; set; }
    public string Value => CaptureContext.FullMatch;

    Type _type;
    public Type Type
    {
        get
        {
            if (_type is null)
                _type = GetType();

            return _type;
        }
    }

    /// <summary>
    /// Only intended to be called by TokenTypeRegistry once upon startup. May be overridden by
    /// inheriting abstract classes who want to specify their own validation requirements.
    /// </summary>
    public virtual string ValidateStructure()
    {
        var regexGraph = TokenTypeRegistry.GetRegexGraph(Type);

        if (string.IsNullOrEmpty(regexGraph.BuiltRegex.MinifiedRegex))
            return $"{nameof(regexGraph.BuiltRegex.MinifiedRegex)} is null or empty";

        var expectedProps = Type.GetPublicPropNames();

        var missingProps = expectedProps
            .Except(regexGraph.RootNode.Children
            .OfType<NamedGroupNode>()
            .Select(x => x.Name))
            .ToList();

        if (missingProps.Any())
            return $"the following properties are not represented among template snippets: {string.Join(", ", missingProps)}";

        if (CheckForReferenceLoops() is string referenceLoopException)
            return referenceLoopException;

        return null;
    }

    public string CheckForReferenceLoops()
    {
        return FindLoop(GetType(), new Stack<Type>());

        string FindLoop(Type current, Stack<Type> path)
        {
            if (path.Contains(current))
            {
                var chain = string.Join(" -> ", path.Reverse().Select(t => t.Name));
                return $"Circular reference detected: {chain} -> {current.Name}";
            }

            path.Push(current);

            var dependencies = current.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany(p => GetUnderlyingTokenUnits(p.PropertyType))
                .Distinct();

            foreach (var dep in dependencies)
            {
                var error = FindLoop(dep, path);
                if (error != null) return error;
            }

            path.Pop();
            return null;
        }

        IEnumerable<Type> GetUnderlyingTokenUnits(Type type)
        {
            // 1. Explicitly ignore DynamicOf branches (as per requirements)
            if (typeof(DynamicOf<>).IsAssignableFrom(type))
                yield break;

            // 2. If it is a TokenUnit, that is a direct dependency
            if (typeof(TokenUnit).IsAssignableFrom(type))
                yield return type;

            // 3. If it is an XOf generic (ManyOf<T>, OneOf<T1, T2>, etc), 
            // recurse into the generic arguments to find the TokenUnits inside.
            else if (type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
                    foreach (var nested in GetUnderlyingTokenUnits(arg))
                        yield return nested;
            }
        }
    }
}