using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Glyphotype.GlyphPrimitives;

public abstract class Glyph : CaptureUnit
{
    public virtual Nib[] Nibs { get; } = [];
    public virtual Joiner Joiner => Joiner.Space;

    PropertyInfo MemberExpressionToProp (string memberExpression)
    {
        var lastDot = memberExpression.LastIndexOf('.');
        var name = lastDot == -1 ? memberExpression : memberExpression;

        // 1. Get the actual, fully resolved runtime type
        var actualType = this.GetType();

        // 2. Fetch the PropertyInfo from the closed type. 
        // This automatically resolves `T` to the concrete type.
        PropertyInfo propInfo = actualType.GetProperty(name,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        return propInfo;
    }

    public PropertyNib Prop(object member, Proptions proptions = Proptions.None, Quantifier? quantifier = null, [CallerArgumentExpression("member")] string expression = "")
    {
        var resolvedProp = MemberExpressionToProp(expression);

        return new PropertyNib(resolvedProp.Name, resolvedProp, proptions, quantifier)
        {
            IsPlural = proptions.HasFlag(Proptions.Plural),
            IsOptional = proptions.HasFlag(Proptions.Optional),
        };
    }

    public NibAlternatives Alt(params string[] alternatives) =>
        new NibAlternatives(alternatives);

    public OptionalNib Opt(string optionalText) =>
        new OptionalNib(optionalText);

    public OptionalPluralNib Plural() =>
        new OptionalPluralNib();

    /// <summary>
    /// Only intended to be called by GlyphTypeRegistry once upon startup. May be overridden by
    /// inheriting abstract classes who want to specify their own validation requirements.
    /// </summary>
    public virtual string ValidateStructure()
    {
        var regexGraph = GlyphTypeRegistry.GetRegexGraph(Type);

        if (string.IsNullOrEmpty(regexGraph.BuiltRegex.MinifiedRegex))
            return $"{nameof(regexGraph.BuiltRegex.MinifiedRegex)} is null or empty";

        // A dependent only ever matches as a subgraph nested inside some parent's own pattern - its
        // parent must match first for the dependent to even be reached. MustMatchWholeLine, on the other
        // hand, means the type is only ever a candidate when its match consumes an entire tokenization
        // pass by itself (see Tokenizer/RegexGraph.TryMatch). A type can't be both: by the time a
        // dependent is reached, it's already partway through its parent's own line-spanning match, so it
        // can never independently be "the whole line" itself.
        if (Type.IsDefined(typeof(DependentAttribute)) && Type.IsDefined(typeof(MustMatchWholeLineAttribute)))
            return $"{Type.Name} cannot be both {nameof(DependentAttribute)} and {nameof(MustMatchWholeLineAttribute)} - a dependent is always matched as a subgraph of a parent, so it can never independently match a whole line";

        // DeclaredOnly still includes properties that override a base virtual member (e.g. Nibs,
        // Joiner), since C# generates a PropertyInfo on the derived type for those too. Excluding
        // anything whose base definition lives on a different type leaves only genuinely new,
        // capture-data properties like the derived type's own nib-bound properties.
        var props = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(x => x.GetMethod.GetBaseDefinition().DeclaringType == x.DeclaringType)
            .Where(x => x.CanWrite)
            .ToArray();

        var missingProps = props
            .Except(regexGraph.RootNode.Children.OfType<NamedGroupNode>().Select(y => y.Navigation.Prop))
            .ToList();

        if (missingProps.Any())
            return $"the following properties are not represented among template nibs: {string.Join(", ", missingProps)}";

        var misplacedQuantifierAttributeProps = props
            .Where(x => x.IsDefined(typeof(OneOrMoreAttribute)) || x.IsDefined(typeof(AnyNumberAttribute)))
            .Where(x => !Navigation.IsListType(x.PropertyType))
            .Select(x => x.Name)
            .ToList();

        if (misplacedQuantifierAttributeProps.Any())
            return $"{nameof(OneOrMoreAttribute)}/{nameof(AnyNumberAttribute)} may only appear on List<> properties, but found on: {string.Join(", ", misplacedQuantifierAttributeProps)}";

        return null;
    }

    public string CheckForReferenceLoops() => CheckForReferenceLoops(GetType());

    /// <summary>
    /// Whether <paramref name="type"/>'s property graph contains a cycle - impossible for it to
    /// legitimately arise (a cyclic property graph could never produce a finite regex), so any
    /// hit here is an authoring mistake. Static and Type-only (no instantiation) so callers can run
    /// it before building anything for the type - notably before <see cref="GlyphTypeRegistry.GetRegexGraph"/>,
    /// whose own tree-walk has no cycle guard and would recurse forever on a genuine loop.
    /// </summary>
    public static string CheckForReferenceLoops(Type type)
    {
        return FindLoop(type, new Stack<Type>());

        static string FindLoop(Type current, Stack<Type> path)
        {
            if (path.Contains(current))
            {
                var chain = string.Join(" -> ", path.Reverse().Select(t => t.Name));
                return $"Circular reference detected: {chain} -> {current.Name}";
            }

            path.Push(current);

            var dependencies = current.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany(p => GetUnderlyingGlyphs(p.PropertyType))
                .Distinct();

            foreach (var dep in dependencies)
            {
                var error = FindLoop(dep, path);
                if (error != null) return error;
            }

            path.Pop();
            return null;
        }

        static IEnumerable<Type> GetUnderlyingGlyphs(Type type)
        {
            // If it is a Glyph, that is a direct dependency
            if (typeof(Glyph).IsAssignableFrom(type))
                yield return type;

            // If it is an XOf generic (ManyOf<T>, OneOf<T1, T2>, etc), 
            // recurse into the generic arguments to find the Glyphs inside.
            else if (type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
                    foreach (var nested in GetUnderlyingGlyphs(arg))
                        yield return nested;
            }
        }
    }
}