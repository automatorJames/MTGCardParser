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

        // AllowPartialSegmentMatch exists solely to exempt a top-level type from the Tokenizer's
        // whole-segment requirement. A dependent is never a top-level candidate in the first place, and a
        // MustMatchWholeLine type is held to a rule strictly stricter than the one being opted out of -
        // so in either pairing the attribute is dead weight that reads like it's doing something.
        if (Type.IsDefined(typeof(AllowPartialSegmentMatchAttribute)) && Type.IsDefined(typeof(DependentAttribute)))
            return $"{Type.Name} cannot be both {nameof(AllowPartialSegmentMatchAttribute)} and {nameof(DependentAttribute)} - a dependent is only ever matched as a subgraph of a parent, so it is never subject to the whole-segment requirement this opts out of";

        if (Type.IsDefined(typeof(AllowPartialSegmentMatchAttribute)) && Type.IsDefined(typeof(MustMatchWholeLineAttribute)))
            return $"{Type.Name} cannot be both {nameof(AllowPartialSegmentMatchAttribute)} and {nameof(MustMatchWholeLineAttribute)} - requiring a whole line is strictly stricter than requiring a whole segment, so opting out of the latter would have no effect";

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

        if (GetJoinedByError(props) is string joinedByError)
            return joinedByError;

        if (GetUnanchoredDynamicError() is string unanchoredDynamicError)
            return unanchoredDynamicError;

        return null;
    }

    /// <summary>
    /// <see cref="JoinedByAttribute"/> only means anything to a <see cref="CompoundOf{T}"/> (the only thing whose
    /// items it separates), so it's refused anywhere else rather than silently ignored - on this type itself, or
    /// on any of <paramref name="props"/> (this type's own nib-bound properties). <see cref="Joiner.Pipe"/> is
    /// refused outright: as a separator inside the repeated item group it would turn "item, then more items" into
    /// an alternation.
    /// </summary>
    string GetJoinedByError(PropertyInfo[] props)
    {
        static bool IsCompoundOf(Type type) => typeof(CompoundOfBase).IsAssignableFrom(type);

        if (Type.GetCustomAttribute<JoinedByAttribute>() is JoinedByAttribute typeJoinedBy)
        {
            if (!IsCompoundOf(Type))
                return $"{Type.Name} declares [JoinedBy] but isn't a {nameof(CompoundOf<object>)}<T> subclass - the attribute only sets the separator between a {nameof(CompoundOf<object>)}'s items, so it would have no effect here";

            if (typeJoinedBy.Joiner == Joiner.Pipe)
                return $"{Type.Name} declares [JoinedBy({nameof(Joiner)}.{nameof(Joiner.Pipe)})], which would turn the item repetition into an alternation - use a {nameof(OneOf<object, object>)} for alternatives";
        }

        foreach (var prop in props)
        {
            if (prop.GetCustomAttribute<JoinedByAttribute>() is not JoinedByAttribute propJoinedBy)
                continue;

            var propType = Navigation.IsListType(prop.PropertyType) ? prop.PropertyType.GetUnderlyingType().GenericTypeArguments[0] : prop.PropertyType;

            if (!IsCompoundOf(propType))
                return $"{Type.Name}.{prop.Name} declares [JoinedBy] but isn't a {nameof(CompoundOf<object>)}<T> property - the attribute only sets the separator between a {nameof(CompoundOf<object>)}'s items, so it would have no effect here";

            if (propJoinedBy.Joiner == Joiner.Pipe)
                return $"{Type.Name}.{prop.Name} declares [JoinedBy({nameof(Joiner)}.{nameof(Joiner.Pipe)})], which would turn the item repetition into an alternation - use a {nameof(OneOf<object, object>)} for alternatives";
        }

        return null;
    }

    /// <summary>
    /// The fixed-shape primitives: generic building blocks whose regex layout is defined entirely by the
    /// primitive itself (or by the framework's handling of it), with no seam for a subclass to add to it.
    /// Contrast the deliberate extension points - <see cref="GlyphFused{T}"/> (via
    /// <see cref="GlyphFused{T}.BeforeContent"/>/<see cref="GlyphFused{T}.AfterContent"/>) and
    /// <see cref="GlyphOneOf"/> - which exist to be subclassed and extended, and so aren't listed.
    /// </summary>
    static readonly Type[] _fixedShapePrimitives =
    [
        typeof(CompoundOf<>),
        typeof(ManyOf<>),
        typeof(OptionalOf<>),
        typeof(OneOf<,>),
        typeof(OneOf<,,>),
        typeof(CompoundOfSecondItem<>),
        typeof(ManyOfSecondItem<>),
    ];

    /// <summary>
    /// Refuses a subclass of a fixed-shape primitive (see <see cref="_fixedShapePrimitives"/>) that declares any
    /// property of its own - a new nib-bound property, or an override of <see cref="Nibs"/>/<see cref="Joiner"/>.
    /// <para>
    /// Subclassing one as a pure alias is fine, and is how a primitive becomes top-level: e.g.
    /// <c>[MustMatchWholeLine] class CardAbilityLine : CompoundOf&lt;Keyword&gt;</c> just gives a
    /// <see cref="CompoundOf{T}"/> a name and class-level attributes. Extending one is valid C# but not a valid
    /// Glyph composition. With no <see cref="Nibs"/> override, the added properties are laid out in reflection
    /// order - unspecified, and in practice ahead of the inherited ones - so the regex silently expects them in
    /// the wrong place. With one, the subclass has to restate the primitive's internal layout (e.g.
    /// <see cref="CompoundOf{T}"/>'s <c>FirstItem</c>/<c>SecondPlus</c> split) and nothing checks it did so
    /// correctly. Either way the primitive stops being a black box.
    /// </para>
    /// <para>
    /// Static and Type-only, and run by <see cref="GlyphTypeRegistry"/> before <see cref="ValidateStructure"/>,
    /// since an override's own checks can otherwise misread the violation first - e.g.
    /// <see cref="OneOfBase.ValidateStructure"/> would happily count a property added to a
    /// <see cref="OneOf{T1,T2}"/> subclass as a third alternative (see <see cref="OneOfBase.GetAlternativeProps"/>).
    /// </para>
    /// </summary>
    public static string GetPrimitiveExtensionError(Type type)
    {
        var primitive = type.BaseType;

        while (primitive is not null && !(primitive.IsGenericType && _fixedShapePrimitives.Contains(primitive.GetGenericTypeDefinition())))
            primitive = primitive.BaseType;

        if (primitive is null)
            return null;

        var declaredProps = new List<string>();

        for (var current = type; current != primitive; current = current.BaseType)
            declaredProps.AddRange(current.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Select(x => x.Name));

        if (declaredProps.Count == 0)
            return null;

        var primitiveName = FormatTypeName(primitive);
        var primitiveKind = primitive.Name[..primitive.Name.IndexOf('`')];

        return $"{type.Name} subclasses {primitiveName} but declares its own members ({string.Join(", ", declaredProps)}). " +
            $"A {primitiveKind} subclass may only alias it - giving it a name and class-level attributes such as [MustMatchWholeLine] - because a {primitiveKind}'s regex layout is fixed by the primitive itself. " +
            $"Added properties would be laid out in reflection order (unspecified, and in practice ahead of the inherited ones), so the regex would silently expect them in the wrong place; " +
            $"overriding {nameof(Nibs)} instead would mean restating {primitiveKind}'s internal layout by hand, which nothing validates. " +
            $"Instead, derive {type.Name} from {nameof(Glyph)} and compose: declare a {primitiveName} property alongside the new ones, and order them all explicitly in {nameof(Nibs)}";
    }

    static string FormatTypeName(Type type) =>
        !type.IsGenericType
            ? type.Name
            : $"{type.Name[..type.Name.IndexOf('`')]}<{string.Join(", ", type.GetGenericArguments().Select(FormatTypeName))}>";

    /// <summary>
    /// Guards the one shape of <see cref="DynamicGlyph"/> authoring that can't terminate: a type whose
    /// dynamic capture is able to span the type's own entire match.
    /// <para>
    /// Resolving a dynamic re-enters the Tokenizer on the captured text (see
    /// <see cref="Nodes.DynamicGlyphNode.TryHydrate"/>), and that resolution only counts if a single Glyph
    /// consumes the capture end to end - so the recursion is "tokenize this text, which may pick this very
    /// type again". What normally makes that terminate is that every level is strictly shorter than the
    /// last: the type's other nibs eat at least one character before its dynamic gets the remainder. Take
    /// those away - as in a type whose nibs are nothing but <c>[Prop(SomeDynamic)]</c> - and the capture
    /// equals the whole match, the next level is handed the identical string, and it recurses until the
    /// stack dies. That's an uncatchable process kill, not an exception, so it has to be refused up front
    /// rather than caught later.
    /// </para>
    /// <para>
    /// So: a type carrying a dynamic nib must also carry at least one nib that is guaranteed to contribute
    /// at least one character - see <see cref="AlwaysConsumesText"/>. Note this bounds the recursion, it
    /// doesn't forbid it: a dynamic resolving to a type that itself has a dynamic is perfectly fine, and
    /// works today, precisely because each such level is anchored and so strictly shrinks.
    /// </para>
    /// </summary>
    string GetUnanchoredDynamicError()
    {
        var nibs = GlyphTypeRegistry.GetGlyphTypeConfiguration(Type).Nibs;

        var dynamicNibNames = nibs
            .OfType<PropertyNib>()
            .Where(x => x.Navigation.NodeType.IsAssignableTo(typeof(DynamicGlyph)))
            .Select(x => x.Name)
            .ToList();

        if (dynamicNibNames.Count == 0 || nibs.Any(x => AlwaysConsumesText(x)))
            return null;

        return $"{Type.Name} declares a {nameof(DynamicGlyph)} nib ({string.Join(", ", dynamicNibNames)}) but nothing that is guaranteed to match at least one character alongside it, so the dynamic's capture can span the type's entire match - resolving it would re-tokenize the identical text, re-pick this type, and recurse until the stack overflows. Add a non-optional literal nib (or another non-optional, non-dynamic property) so every level of the resolution consumes something";
    }

    /// <summary>
    /// Whether <paramref name="nib"/> is guaranteed to contribute at least one character to any match of
    /// the type that declares it - i.e. whether it can serve as the anchor
    /// <see cref="GetUnanchoredDynamicError"/> requires. Conservative by design: anything that might match
    /// nothing answers false, so a doubtful case is refused rather than allowed to recurse.
    /// </summary>
    static bool AlwaysConsumesText(Nib nib, HashSet<Type> visitedTypes = null)
    {
        // A plain literal text nib always emits its text; an OptionalNib wraps it in "( )?" and may not.
        if (nib is not PropertyNib propertyNib)
            return !nib.IsOptional;

        // "?" or "*" - permits zero occurrences by construction.
        if (propertyNib.Navigation.IsOptional)
            return false;

        var nodeType = propertyNib.Navigation.NodeType;

        // The thing being anchored against, so never itself the anchor.
        if (nodeType.IsAssignableTo(typeof(DynamicGlyph)))
            return false;

        // BoolNode hardcodes its own Optional quantifier rather than deriving it from Navigation, so
        // Navigation.IsOptional above reads false for a bool even though it matches nothing when absent.
        if (nodeType == typeof(bool))
            return false;

        // Enum and int terminals always emit one of their alternatives.
        if (!nodeType.IsAssignableTo(typeof(Glyph)))
            return true;

        // A nested Glyph only anchors if it has an anchor of its own - it contributes nothing on its own
        // account, only whatever its own nibs guarantee. Startup rules out reference loops before any of
        // this runs (see CheckForReferenceLoops), but a dynamically emitted type is validated on its own
        // without that sweep, so the visited set keeps this walk safe regardless.
        visitedTypes ??= [];

        if (!visitedTypes.Add(nodeType))
            return false;

        return GlyphTypeRegistry.GetGlyphTypeConfiguration(nodeType).Nibs.Any(x => AlwaysConsumesText(x, visitedTypes));
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