namespace MTGPlexer.TokenUnitGraphComponents;

public abstract class TokenUnit
{
    public HydratedNodeGraph NodeGraph { get; set; }
    protected virtual Snippet[] Snippets { get; } = [];
    public Snippet[] GetSnippets() => Snippets;

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
        var rootNode = TokenTypeRegistry.RootNodes[Type];

        if (string.IsNullOrEmpty(rootNode.BuiltRegex.MinifiedRegexString))
            return $"{nameof(RootNode.BuiltRegex.MinifiedRegexString)} is null or empty";

        var expectedProps = Type.GetPublicPropNames();
        var missingProps = expectedProps.Except(rootNode.CaptureChildren.Select(x => x.ConcreteProperty.Name)).ToList();

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
            if (typeof(DynamicOf).IsAssignableFrom(type))
                yield break;

            // 2. If it is a TokenUnit, that is a direct dependency
            if (typeof(TokenUnit).IsAssignableFrom(type))
                yield return type;

            // 3. If it is an XOf generic (ManyOf<T>, OneOf<T1, T2>, etc), 
            // recurse into the generic arguments to find the TokenUnits inside.
            else if (typeof(XOf).IsAssignableFrom(type) && type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
                    foreach (var nested in GetUnderlyingTokenUnits(arg))
                        yield return nested;
            }
        }
    }
}