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

    public PropertyNib Prop(object member, [CallerArgumentExpression("member")] string expression = "")
    {
        var resolvedProp = MemberExpressionToProp(expression);

        return new PropertyNib(resolvedProp.Name, resolvedProp);
    }

    /// <summary>Exactly one of several literal texts, e.g. <c>Alt("sleeps", "naps")</c>.</summary>
    public NibAlternatives Alt(params string[] alternatives) =>
        new NibAlternatives(alternatives);

    /// <summary>A nib that may be absent: literal text, e.g. <c>Opt("some")</c>, or a pattern, e.g. <c>Opt(Pattern("an?"))</c>.</summary>
    public OptionalNib Opt(Nib optional) =>
        new OptionalNib(optional);

    /// <summary>An optional plural suffix on the word before it: <c>"dog", Plural()</c> matches "dog" and "dogs".</summary>
    public OptionalPluralNib Plural() =>
        new OptionalPluralNib();

    /// <summary>
    /// A regex rather than literal text - the explicit opt-in, for what literal text and the other helpers can't
    /// express, e.g. <c>Pattern("an?")</c>. Every other nib is matched exactly as written.
    /// </summary>
    public PatternNib Pattern(string regex) =>
        new PatternNib(regex);

    /// <summary>
    /// Only intended to be called by <see cref="GlyphGrammar"/> while it validates. May be overridden by
    /// inheriting abstract classes who want to specify their own validation requirements.
    /// </summary>
    public virtual string ValidateStructure()
    {
        var regexGraph = GlyphTypeCache.GetRegexGraph(Type);

        if (string.IsNullOrEmpty(regexGraph.BuiltRegex.MinifiedRegex))
            return $"{nameof(regexGraph.BuiltRegex.MinifiedRegex)} is null or empty";

        // AllowPartialClauseMatch exists solely to exempt a top-level type from the Tokenizer's
        // whole-clause requirement. A dependent is never a top-level candidate in the first place, so on one
        // the attribute is dead weight that reads like it's doing something.
        if (Type.IsDefined(typeof(AllowPartialClauseMatchAttribute)) && Type.IsDefined(typeof(DependentAttribute)))
            return $"{Type.Name} cannot be both {nameof(AllowPartialClauseMatchAttribute)} and {nameof(DependentAttribute)} - a dependent is only ever matched as a subgraph of a parent, so it is never subject to the whole-clause requirement this opts out of";

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

        if (props.FirstOrDefault(x => x.IsDefined(typeof(AllowUnmatchedAttribute)) && x.PropertyType != typeof(DynamicGlyph)) is PropertyInfo misplacedAllowUnmatched)
            return $"{Type.Name}.{misplacedAllowUnmatched.Name} declares [AllowUnmatched] but isn't a {nameof(DynamicGlyph)} - only a dynamic resolves its text, so only a dynamic can leave it unresolved";

        // A pipe-joined Glyph is a one-of in all but name - its properties are alternatives, exactly one of which
        // matches - but only a OneOfBase is built, hydrated and validated as one.
        if (Joiner == Joiner.Pipe && this is not OneOfBase)
            return $"{Type.Name} overrides {nameof(Joiner)} to {nameof(Joiner)}.{nameof(Joiner.Pipe)}, which makes its properties alternatives - derive it from {nameof(GlyphOneOf)} instead, which is built, hydrated and validated as exactly that";

        if (GetUnanchoredDynamicError() is string unanchoredDynamicError)
            return unanchoredDynamicError;

        if (GetRefersToError(props) is string refersToError)
            return refersToError;

        if (Type.IsDefined(typeof(AgreementAttribute)) && this is not BackReference)
            return $"{Type.Name} declares [Agreement] but isn't a {nameof(BackReference)} - only a back-reference has a referent to agree with (a referent's own features go on [Introduces])";

        if (this is BackReference && !Type.IsDefined(typeof(DependentAttribute)))
            return $"{Type.Name} is a {nameof(BackReference)} but isn't [Dependent] - a back-reference only means anything inside the phrase around it, so it's never matched on its own";

        return null;
    }

    /// <summary>
    /// <see cref="RefersToAttribute"/> binds a back-reference to a sibling capture, so it's refused anywhere else: on a
    /// property that isn't a <see cref="BackReference"/> (nothing else refers to anything), or naming anything but
    /// another of this type's own nib-bound properties (there'd be nothing for it to bind to).
    /// </summary>
    string GetRefersToError(PropertyInfo[] props)
    {
        foreach (var prop in props.Where(x => x.IsDefined(typeof(RefersToAttribute))))
        {
            var targetName = prop.GetCustomAttribute<RefersToAttribute>().PropertyName;

            if (!typeof(BackReference).IsAssignableFrom(prop.PropertyType))
                return $"{Type.Name}.{prop.Name} declares [RefersTo] but isn't a {nameof(BackReference)} - only a back-reference refers back to anything";

            if (targetName == prop.Name || !NibBoundProps(Type).Any(x => x.Name == targetName))
                return $"{Type.Name}.{prop.Name} declares [RefersTo(\"{targetName}\")], which names no other property of {Type.Name} - it can only bind to a sibling capture";
        }

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
    /// Contrast the deliberate extension point <see cref="GlyphOneOf"/>, which exists to be subclassed and
    /// extended (with named alternatives), and so isn't listed.
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
    /// Refuses a subclass of a fixed-shape primitive (see <see cref="_fixedShapePrimitives"/>) that declares a
    /// settable property of its own, or overrides <see cref="Nibs"/>/<see cref="Joiner"/>. A get-only property
    /// of its own (e.g. a convenience computed from <c>Items</c>) is the author's business: it's never bound to
    /// the regex, so none of the below applies to it.
    /// <para>
    /// Subclassing one as a pure alias is fine, and is how a primitive becomes top-level: e.g.
    /// <c>class ShoppingList : CompoundOf&lt;Ingredient&gt;;</c> just gives a <see cref="CompoundOf{T}"/> a name,
    /// and room for class-level attributes. Extending one is valid C# but not a valid
    /// Glyph composition. With no <see cref="Nibs"/> override, the added properties are laid out in reflection
    /// order - unspecified, and in practice ahead of the inherited ones - so the regex silently expects them in
    /// the wrong place. With one, the subclass has to restate the primitive's internal layout (e.g.
    /// <see cref="CompoundOf{T}"/>'s <c>FirstItem</c>/<c>SecondPlus</c> split) and nothing checks it did so
    /// correctly. Either way the primitive stops being a black box.
    /// </para>
    /// <para>
    /// Static and Type-only, and run by <see cref="GlyphGrammar"/> before <see cref="ValidateStructure"/>,
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
            declaredProps.AddRange(current.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(x => x.CanWrite || x.GetMethod.GetBaseDefinition().DeclaringType != x.DeclaringType)
                .Select(x => x.Name));

        if (declaredProps.Count == 0)
            return null;

        var primitiveName = FormatTypeName(primitive);
        var primitiveKind = primitive.Name[..primitive.Name.IndexOf('`')];

        return $"{type.Name} subclasses {primitiveName} but declares its own members ({string.Join(", ", declaredProps)}). " +
            $"A {primitiveKind} subclass may only alias it - giving it a name and class-level attributes such as [Dependent] - because a {primitiveKind}'s regex layout is fixed by the primitive itself. " +
            $"Added properties would be laid out in reflection order (unspecified, and in practice ahead of the inherited ones), so the regex would silently expect them in the wrong place; " +
            $"overriding {nameof(Nibs)} instead would mean restating {primitiveKind}'s internal layout by hand, which nothing validates. " +
            $"Instead, derive {type.Name} from {nameof(Glyph)} and compose: declare a {primitiveName} property alongside the new ones, and order them all explicitly in {nameof(Nibs)}";
    }

    /// <summary>
    /// Refuses a <see cref="List{T}"/> property on any Glyph type other than Glyphotype's own internal
    /// primitives. Repetition is what the primitives exist to express - <see cref="CompoundOf{T}"/> for a
    /// joined run, <see cref="ManyOf{T}"/> for a list with a conjunction, made optional by
    /// <see cref="OptionalAttribute"/> or <see cref="OptionalOf{T}"/> - and a raw list bypasses all of it: it
    /// has no joiner between its items, no conjunction, and (for a list of enums) no regex support at all.
    /// Inherited primitive properties (e.g. <see cref="CompoundOf{T}"/>'s own <c>SecondPlus</c> on an alias
    /// subclass) are the primitive's, so they pass.
    /// </summary>
    public static string GetListPropertyError(Type type)
    {
        var listProps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.DeclaringType.Assembly != typeof(Glyph).Assembly && Navigation.IsListType(x.PropertyType))
            .Select(x => $"{x.Name} ({FormatTypeName(x.PropertyType)})")
            .ToList();

        if (listProps.Count == 0)
            return null;

        return $"{type.Name} declares List<> properties: {string.Join(", ", listProps)}. " +
            $"Only Glyphotype's internal primitives may hold a raw list - express repetition with one of them instead: " +
            $"CompoundOf<T> for a joined run (\"a, b, c\", or \"a b c\" with [JoinedBy(Joiner.Space)]), " +
            $"or ManyOf<T> for a list with a conjunction (\"a, b, and c\"). " +
            $"If the items may be absent altogether, mark the property [Optional] or wrap it in OptionalOf<T>";
    }

    /// <summary>
    /// Refuses a nib-bound (publicly settable) property whose type the engine can't capture. Every captured
    /// word is either grammatical scaffolding (literal nib text) or a terminal of interest, so a property must be
    /// a <see cref="Glyph"/> (nested structure), an enum (a closed vocabulary), a bool (a phrase's presence) or a
    /// supported primitive (an open-vocabulary value parsed from the text - see <see cref="PrimitiveTerminal"/>).
    /// Lists are <see cref="GetListPropertyError"/>'s concern.
    /// </summary>
    public static string GetPropertyTypeError(Type type)
    {
        var unsupported = NibBoundProps(type)
            .Where(x => !Navigation.IsListType(x.PropertyType) && !IsCapturableType(x.PropertyType))
            .Select(x => $"{x.Name} ({FormatTypeName(x.PropertyType)})")
            .ToList();

        if (unsupported.Count == 0)
            return null;

        return $"{FormatTypeName(type)} declares properties of types the engine can't capture: {string.Join(", ", unsupported)}. " +
            $"A Glyph property must be a Glyph, an enum, a bool, or a supported primitive ({string.Join(", ", PrimitiveTerminal.SupportedDisplayNames)})";
    }

    /// <summary>
    /// Refuses a non-nullable value-type property that can be absent after a successful match - one marked
    /// <see cref="OptionalAttribute"/>, or one of a <see cref="OneOfBase"/>'s alternatives, all but one of which
    /// are always absent. Left non-nullable, an absent value would be indistinguishable from a matched default
    /// (an int that "matched" 0, an enum that "matched" its first member). Where a property is required, either
    /// is fine: it's always set when its glyph matches. A bool is exempt - absent means false, by design.
    /// </summary>
    public static string GetNullabilityError(Type type)
    {
        var alternatives = typeof(OneOfBase).IsAssignableFrom(type) ? OneOfBase.GetAlternativeProps(type) : [];

        var offenders = NibBoundProps(type)
            .Where(x => x.IsDefined(typeof(OptionalAttribute)) || alternatives.Contains(x))
            .Where(x => x.PropertyType.IsValueType && x.PropertyType != typeof(bool) && Nullable.GetUnderlyingType(x.PropertyType) == null)
            .Select(x => $"{x.Name} ({FormatTypeName(x.PropertyType)} → {FormatTypeName(x.PropertyType)}?)")
            .ToList();

        if (offenders.Count == 0)
            return null;

        return $"{FormatTypeName(type)} has value-type properties that can be absent after a match (being [Optional], or one-of alternatives) but aren't nullable: {string.Join(", ", offenders)}. " +
            $"Make them nullable, so an absent value reads as null rather than as a matched default";
    }

    /// <summary>
    /// Refuses a generic <see cref="OneOf{T1,T2}"/>/<see cref="OneOf{T1,T2,T3}"/> that repeats a type argument
    /// (nullable-unwrapped), e.g. <c>OneOf&lt;int?, int?&gt;</c>. Its alternatives are distinguished only by type,
    /// so two of the same type are the same alternative twice: nothing could say which one matched. (It's also
    /// what names its capture groups - see <see cref="PropertyNib"/>.) Alternatives that genuinely differ in
    /// meaning but share a type want names of their own: a <see cref="GlyphOneOf"/>.
    /// </summary>
    public static string GetOneOfTypeArgumentError(Type type)
    {
        var oneOf = type;

        while (oneOf is not null && !(oneOf.IsGenericType && oneOf.GetGenericTypeDefinition() is var definition && (definition == typeof(OneOf<,>) || definition == typeof(OneOf<,,>))))
            oneOf = oneOf.BaseType;

        if (oneOf is null)
            return null;

        var repeated = oneOf.GetGenericArguments()
            .GroupBy(x => Nullable.GetUnderlyingType(x) ?? x)
            .Where(x => x.Count() > 1)
            .Select(x => FormatTypeName(x.Key))
            .ToList();

        if (repeated.Count == 0)
            return null;

        return $"{FormatTypeName(oneOf)} repeats {string.Join(", ", repeated)} among its alternatives, which a generic OneOf distinguishes only by type - so nothing could tell which of them matched. " +
            $"If they mean different things, derive from {nameof(GlyphOneOf)} and give each alternative its own name";
    }

    /// <summary>
    /// The Type-only rules - <see cref="GetPrimitiveExtensionError"/>, <see cref="GetListPropertyError"/>,
    /// <see cref="GetPropertyTypeError"/>, <see cref="GetNullabilityError"/> and
    /// <see cref="GetOneOfTypeArgumentError"/> - which
    /// <see cref="GlyphGrammar"/> checks before building any regex graph, since a violation can break
    /// graph building itself (an unsupported property type does) before <see cref="ValidateStructure"/> gets the
    /// chance to report it.
    /// </summary>
    public static string GetTypeShapeError(Type type) =>
        GetPrimitiveExtensionError(type)
        ?? GetListPropertyError(type)
        ?? GetPropertyTypeError(type)
        ?? GetNullabilityError(type)
        ?? GetOneOfTypeArgumentError(type);

    /// <summary>A regex matching a literal period: <c>\.</c> not itself preceded by an escaping backslash.</summary>
    static readonly Regex _escapedPeriod = new(@"(?<!\\)(?:\\\\)*\\\.");

    /// <summary>
    /// The period rules, which keep every period on a line a <see cref="ClauseBreak"/> the Tokenizer can see:
    /// <list type="bullet">
    /// <item>A plain literal nib may have a period inside it only if <paramref name="allowPeriodsInLiteralNibs"/>
    /// (see <see cref="GlobalSettings.AllowPeriodsInLiteralNibs"/>) - it's then split around it into a clause-break
    /// nib of its own (see <see cref="ClauseBreak.SplitAtPeriods"/>).</item>
    /// <item>Nothing else may match a literal period: not a pattern (<see cref="PatternNib"/>, or a
    /// <see cref="RegexPatternAttribute"/> on the type, a dynamic property or an enum member), an
    /// <see cref="Alt"/> or an <see cref="Opt"/> - none can be split around a break, so a period in one could
    /// only ever match by swallowing it.</item>
    /// <item>A type may end with a period (bare <c>"."</c> nib included) only where it's redundant, and so dropped
    /// (see <see cref="ClauseBreak.IsTrailingPeriodRedundant"/>): a clause's closing period is the Tokenizer's own
    /// <see cref="ClauseBreak"/>, never part of a Glyph. Not, then, on a dependent or partial-match type, nor on
    /// one <paramref name="isUsedAsProperty"/> by another glyph - in each, the period is a real constraint the
    /// type can't keep.</item>
    /// </list>
    /// Run by <see cref="GlyphGrammar"/> after every graph is built, since both the setting and which types nest
    /// which are the grammar's.
    /// </summary>
    public static string GetPeriodError(Type type, bool allowPeriodsInLiteralNibs, bool isUsedAsProperty)
    {
        const string useClauseBreakNib = "write the period as a bare \".\" nib of its own instead";
        var textNibs = GlyphTypeCache.GetConfiguration(type).Nibs.Where(x => x is not PropertyNib).ToList();

        foreach (var nib in textNibs)
        {
            var error = nib switch
            {
                PatternNib or OptionalNib { Inner: PatternNib } when _escapedPeriod.IsMatch(nib.Regex) =>
                    $"the pattern \"{nib.Regex}\" matches a literal period, which a pattern can't be split around - {useClauseBreakNib}",
                NibAlternatives alternatives when alternatives.Alternatives.Any(x => x.Contains(ClauseBreak.Period)) =>
                    $"Alt(\"{string.Join("\", \"", alternatives.Alternatives)}\") contains a period, which an alternation can't be split around - {useClauseBreakNib}",
                OptionalNib { Inner: not PatternNib } when nib.Text.Contains(ClauseBreak.Period) =>
                    $"Opt(\"{nib.Text}\") contains a period, and a clause break can't be optional - {useClauseBreakNib}",
                _ when !allowPeriodsInLiteralNibs && ClauseBreak.IsSplittable(nib) =>
                    $"the nib \"{nib.Text}\" contains a period, which this grammar doesn't allow in literal text ({nameof(GlobalSettings)}.{nameof(GlobalSettings.AllowPeriodsInLiteralNibs)} is off) - {useClauseBreakNib}",
                _ => null,
            };

            if (error is not null)
                return error;
        }

        var graph = GlyphTypeCache.GetRegexGraph(type);

        var propertyPatterns = graph.RootNode.Children.OfType<DynamicGlyphNode>()
            .SelectMany(x => x.Children.OfType<TextNode>().Select(y => (Owner: x.Navigation.Prop?.Name, Pattern: y.Text)))
            .Concat(graph.RootNode.Children.OfType<EnumNode>()
                .SelectMany(x => x.Children.OfType<EnumMemberNode>().Select(y => (Owner: $"{x.Navigation.UnderlyingType.Name}.{y.Name}", Pattern: y.RegexString))));

        if (propertyPatterns.FirstOrDefault(x => _escapedPeriod.IsMatch(x.Pattern)) is { Pattern: not null } offending)
            return $"{offending.Owner}'s pattern \"{offending.Pattern}\" matches a literal period, which a pattern can't be split around - {useClauseBreakNib}";

        // Only a plain literal nib is left to check: a pattern, Alt or Opt with a period in it was refused above.
        if (!ClauseBreak.EndsWithPeriod(type))
            return null;

        var where =
            type.IsDefined(typeof(DependentAttribute)) ? "it's [Dependent], so that period would fall inside its parent's match"
            : type.IsDefined(typeof(AllowPartialClauseMatchAttribute)) ? "it's [AllowPartialClauseMatch], so that period would be what stops it matching partway through a clause"
            : isUsedAsProperty ? "another glyph uses it as a property, so that period would fall inside that glyph's match"
            : null;

        return where is null ? null
            : $"{type.Name} ends with a period, but {where} - and a clause's closing period is a {nameof(ClauseBreak)} the Tokenizer emits between Glyphs, never part of one. " +
              $"Drop it, and if the period belongs inside a larger match, write it there as a bare \".\" nib.";
    }

    /// <summary>A Glyph type's publicly settable properties below <see cref="Glyph"/> itself (so not, e.g., <see cref="CaptureUnit.CaptureContext"/>) - the ones that are nib-bound.</summary>
    static IEnumerable<PropertyInfo> NibBoundProps(Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.GetSetMethod() != null && x.DeclaringType.IsSubclassOf(typeof(Glyph)));

    static bool IsCapturableType(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        return underlying.IsAssignableTo(typeof(Glyph))
            || underlying.IsEnum
            || underlying == typeof(bool)
            || PrimitiveTerminal.IsSupported(underlying);
    }

    static string FormatTypeName(Type type) =>
        Nullable.GetUnderlyingType(type) is Type underlying ? $"{FormatTypeName(underlying)}?"
        : !type.IsGenericType
            ? (PrimitiveTerminal.TryGet(type, out var primitive) ? primitive.DisplayName : type.Name)
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
        var nibs = GlyphTypeCache.GetConfiguration(Type).Nibs;

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
        // Literal text always matches itself (non-empty, as TextNode requires); Alt's literal alternatives do as
        // long as none is empty. An OptionalNib may match nothing, as may Plural()'s suffix, and a pattern is
        // opaque - it might (e.g. "(on)?") - so none of those can anchor.
        if (nib is not PropertyNib propertyNib)
            return nib switch
            {
                OptionalNib or OptionalPluralNib or PatternNib => false,
                NibAlternatives alternatives => alternatives.Alternatives.All(x => !string.IsNullOrEmpty(x)),
                _ => true,
            };

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

        return GlyphTypeCache.GetConfiguration(nodeType).Nibs.Any(x => AlwaysConsumesText(x, visitedTypes));
    }

    public string CheckForReferenceLoops() => CheckForReferenceLoops(GetType());

    /// <summary>
    /// Whether <paramref name="type"/>'s property graph contains a cycle - impossible for it to
    /// legitimately arise (a cyclic property graph could never produce a finite regex), so any
    /// hit here is an authoring mistake. Static and Type-only (no instantiation) so callers can run
    /// it before building anything for the type - notably before <see cref="GlyphTypeCache.GetRegexGraph"/>,
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