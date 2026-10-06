namespace Glyphotype.StaticRegistry;

/// <summary>
/// A set of Glyph types and the settings they're tokenized under: discovery (including
/// <see cref="IsolateForTestingAttribute"/> scoping), validation, tokenization order, and the
/// <see cref="Tokenizers.Tokenizer"/> itself. Several can coexist in one process - each is built from the types it's
/// given, validated on construction (throwing if anything is invalid), and independent of every other.
/// <para>
/// What's derived from a single type's own definition (its configuration and regex graph) doesn't vary between
/// grammars, and is shared through <see cref="GlyphTypeCache"/>.
/// </para>
/// <para>
/// <see cref="Default"/> is the app's grammar: every type in the assemblies alongside the running one, under
/// <see cref="GlobalSettings.Current"/>.
/// </para>
/// </summary>
public sealed class GlyphGrammar
{
    static readonly Lazy<GlyphGrammar> _default = new(() =>
        new GlyphGrammar(LoadAllAssemblyTypes(), GlobalSettings.Current.AllowPartialClauseMatches, GlobalSettings.Current.AllowPeriodsInLiteralNibs));

    /// <summary>
    /// The grammar of every Glyph type in the assemblies alongside the running one - Glyph types are defined by
    /// consumer assemblies (a domain's own glyph library), not by this library - tokenized under
    /// <see cref="GlobalSettings.Current"/>. Built on first use.
    /// </summary>
    public static GlyphGrammar Default => _default.Value;

    // Everything the grammar was given, Glyph or not: CaptureUnit types (e.g. for GetAllCaptureUnitTypes) are
    // drawn from here too.
    readonly List<Type> _candidateTypes;
    readonly Dictionary<string, Type> _typesByName = [];

    /// <summary>Whether a top-level type may match only part of a clause - see <see cref="GlobalSettings.AllowPartialClauseMatches"/>.</summary>
    public bool AllowPartialClauseMatches { get; }

    /// <summary>Whether a literal nib may have a period inside it - see <see cref="GlobalSettings.AllowPeriodsInLiteralNibs"/>.</summary>
    public bool AllowPeriodsInLiteralNibs { get; }

    /// <summary>
    /// Every Glyph type in play: each non-generic, non-abstract Glyph type given - dependents included - plus every
    /// type reachable by walking their property nibs, which is what surfaces a closed generic like
    /// <c>OneOf&lt;Animal, Person&gt;</c> that only ever appears as a property type. When any are marked
    /// <see cref="IsolateForTestingAttribute"/>, just their dependency closures.
    /// </summary>
    public IReadOnlyList<Type> Types { get; private set; }

    /// <summary>The top-level types, in the order the Tokenizer tries them (see <see cref="TokenizationOrderAttribute"/>).</summary>
    public IReadOnlyList<Type> TopLevelTypes { get; private set; }

    /// <summary>
    /// The dependent types a <see cref="DynamicGlyph"/> may resolve to, beyond the top-level ones.
    /// <para>
    /// Scoped to <see cref="Types"/> for the same reason - and by the same isolation rule - that
    /// <see cref="TopLevelTypes"/> is. Consequence worth knowing while testing: under isolation a dynamic can only
    /// resolve to types in the isolated closure. A payload type that's only ever reached *through* a dynamic has no
    /// static property reference to pull it in, so isolating a dynamic-bearing type means marking its intended
    /// payload types with <see cref="IsolateForTestingAttribute"/> too - they're roots of the closure in their own
    /// right, dependent or not.
    /// </para>
    /// </summary>
    public IReadOnlyList<Type> DependentTypes { get; private set; }

    public Tokenizer Tokenizer { get; private set; }

    /// <summary>Builds and validates a grammar from <paramref name="types"/> (any types; the Glyph types among them are used). Throws if any Glyph type in play is invalid.</summary>
    public GlyphGrammar(IEnumerable<Type> types, bool allowPartialClauseMatches, bool allowPeriodsInLiteralNibs = true)
    {
        _candidateTypes = types.ToList();
        AllowPartialClauseMatches = allowPartialClauseMatches;
        AllowPeriodsInLiteralNibs = allowPeriodsInLiteralNibs;
        Build();
    }

    /// <summary>A grammar of every type in <paramref name="assemblies"/>.</summary>
    public static GlyphGrammar FromAssemblies(IEnumerable<Assembly> assemblies, bool allowPartialClauseMatches, bool allowPeriodsInLiteralNibs = true) =>
        new(assemblies.SelectMany(GetLoadableTypes), allowPartialClauseMatches, allowPeriodsInLiteralNibs);

    public List<CaptureUnit> Tokenize(string sourceText) =>
        Tokenizer.Tokenize(sourceText);

    public bool TryGetType(string name, out Type type) =>
        _typesByName.TryGetValue(name, out type);

    /// <summary>This grammar's glyph types - and the vocabularies and markers they refer to - as a <see cref="GrammarDefinition"/>.</summary>
    public GrammarDefinition ToDefinition() =>
        GrammarDefinition.FromTypes(Types);

    /// <summary>
    /// Builds and validates a grammar from <paramref name="definition"/>, emitting its types first (see
    /// <see cref="GrammarEmitter.Emit"/>, which also describes <paramref name="knownTypes"/>).
    /// </summary>
    public static GlyphGrammar FromDefinition(GrammarDefinition definition, bool allowPartialClauseMatches, IEnumerable<Type> knownTypes = null, bool allowPeriodsInLiteralNibs = true) =>
        new(GrammarEmitter.Emit(definition, knownTypes), allowPartialClauseMatches, allowPeriodsInLiteralNibs);

    /// <summary>
    /// Runs every validation rule over <see cref="Types"/> and returns each failure as a "TypeName: message" string
    /// instead of throwing. Construction runs the same checks but throws; this is for a diagnostic tool that wants
    /// the full list of everything currently broken as data rather than as an exception.
    /// </summary>
    public List<string> GetStructuralValidationErrors() =>
        GetStructuralValidationErrors(Types, AllowPeriodsInLiteralNibs);

    /// <summary>Every concrete <see cref="CaptureUnit"/> type given to this grammar, plus Glyphotype's own (e.g. <see cref="UnmatchedString"/>), by name.</summary>
    public List<Type> GetAllCaptureUnitTypes() =>
        _candidateTypes
            .Concat(typeof(CaptureUnit).Assembly.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && typeof(CaptureUnit).IsAssignableFrom(x))
            .Distinct()
            .OrderBy(x => x.Name)
            .ToList();

    void Build()
    {
        // 1) Discover every Glyph type: the given types plus everything only reachable by walking property nibs
        // (e.g. a closed generic OneOf<T1,T2>).
        var types = DiscoverTypes();

        // 2) Validate: rule out reference loops across the *entire* discovered set before building anything.
        // CheckForReferenceLoops is pure Type reflection with its own proper cycle guard (tracks the current DFS
        // path), so unlike a graph build's tree-walk (no guard of its own) it can never itself recurse forever -
        // it's what makes it safe to eagerly build everything next.
        ThrowIfAny(types.Select(Glyph.CheckForReferenceLoops), "One or more Glyph types have a circular property reference");

        // Likewise the Type-only shape rules (e.g. no List<> properties), since some violations - an unsupported
        // property type - would otherwise crash graph building below before step 4 could report them.
        ThrowIfAny(types.Select(t => Glyph.GetTypeShapeError(t) is string error ? $"{t.Name}: {error}" : null), "One or more Glyph types failed structural validation");

        // 3) Build every type's graph up front, not just top-level ones, so nothing downstream needs to worry
        // about when/whether a given type's graph has been built yet.
        _typesByName.Clear();

        foreach (var type in types)
        {
            GlyphTypeCache.GetRegexGraph(type);
            _typesByName[type.Name] = type;
        }

        // 4) Validate structure across the same full set - not just top-level types, but dependents and
        // everything only reachable via property nibs - so an authoring mistake anywhere in the graph stops
        // construction rather than surfacing later as a bad match. Runs as its own pass, after every graph is
        // built, since some rules read other types' graphs.
        ThrowIfAny(GetStructuralValidationErrors(types, AllowPeriodsInLiteralNibs),"One or more Glyph types failed structural validation");

        Types = types;
        BuildTokenizer();
    }

    List<Type> DiscoverTypes()
    {
        var givenTypes = _candidateTypes
            .Where(x => x.IsClass && !x.IsAbstract && typeof(Glyph).IsAssignableFrom(x) && !x.ContainsGenericParameters)
            .ToList();

        var isolatedTypes = givenTypes.Where(x => x.IsDefined(typeof(IsolateForTestingAttribute))).ToList();

        // When one or more types opt into isolated testing, don't just keep those exact types - pull in their whole
        // property dependency graph too, so a type like GreetsUs still finds every Glyph type it depends on
        // (e.g. OneOf<Animal, Person>).
        return GetTransitiveGlyphTypeClosure(isolatedTypes.Count > 0 ? isolatedTypes : givenTypes)
            .Where(t => !t.IsAssignableTo(typeof(DynamicGlyph)))
            .ToList();
    }

    void BuildTokenizer()
    {
        var topLevelTypes = Types.Where(x => !x.IsDefined(typeof(DependentAttribute)) && _candidateTypes.Contains(x)).ToList();

        // Since it's possible for multiple types to define the same order via TokenizationOrder,
        // each dictionary entry is a List, though each List should ideally only have one item.
        // An entry List may have multiple items if they each declare the same position.
        Dictionary<int, List<Type>> orderedTypes =
            topLevelTypes.Where(x => x.IsDefined(typeof(TokenizationOrderAttribute)))
            .GroupBy(x => x.GetCustomAttribute<TokenizationOrderAttribute>().Order)
            .ToDictionary(x => x.Key, x => x.ToList());

        // Add all remaining types (i.e. those the user didn't bother to define anywhere).
        // Order by descending length, which is a rough approximate of complexity/match length (not exact)
        var unorderedRemainingTypes = topLevelTypes
            .Except(orderedTypes.SelectMany(x => x.Value))
            .OrderByDescending(x => GlyphTypeCache.GetRegexGraph(x).BuiltRegex.MinifiedRegex.Length)
            .ToList();

        var nextKey = orderedTypes.Keys.Any() ? orderedTypes.Keys.Max() + 1 : 0;
        orderedTypes[nextKey] = unorderedRemainingTypes;

        TopLevelTypes = orderedTypes
            .Where(x => x.Key >= 0)
            .OrderBy(x => x.Key)
            .SelectMany(x => x.Value)
            .Concat(orderedTypes
                .Where(x => x.Key < 0)
                .SelectMany(x => x.Value))
            .Distinct()
            .ToList();

        DependentTypes = Types.Where(x => x.IsDefined(typeof(DependentAttribute)) && _candidateTypes.Contains(x)).ToList();
        Tokenizer = new(TopLevelTypes.ToList(), DependentTypes.ToList(), AllowPartialClauseMatches);
    }

    static List<string> GetStructuralValidationErrors(IEnumerable<Type> types, bool allowPeriodsInLiteralNibs)
    {
        var errors = new List<string>();
        var typeList = types.ToList();

        // Types some other glyph nests as a property - whose closing period, if any, would fall inside that glyph's
        // match (see Glyph.GetPeriodError).
        var propertyTypes = typeList.Where(x => Glyph.GetTypeShapeError(x) is null).SelectMany(GetDirectDependentGlyphTypes).ToHashSet();

        foreach (var type in typeList)
        {
            // Checked first, and without a graph: a type-shape violation can break graph building itself.
            if (Glyph.GetTypeShapeError(type) is string typeShapeError)
            {
                errors.Add($"{type.Name}: {typeShapeError}");
                continue;
            }

            // Some ValidateStructure overrides (e.g. OneOfBase) read a graph, so one must exist here - which
            // matters for types (like a closed OneOf<T1,T2>) only ever discovered as a property.
            GlyphTypeCache.GetRegexGraph(type);

            var instance = (Glyph)Activator.CreateInstance(type);

            if (instance.ValidateStructure() is string error)
                errors.Add($"{type.Name}: {error}");
            else if (Glyph.GetPeriodError(type, allowPeriodsInLiteralNibs, propertyTypes.Contains(type)) is string periodError)
                errors.Add($"{type.Name}: {periodError}");
        }

        // Across the whole grammar, since a back-reference's referents are declared on other types.
        if (errors.Count == 0)
            errors.AddRange(BackReference.GetUnreferableErrors(typeList));

        return errors;
    }

    static void ThrowIfAny(IEnumerable<string> errors, string summary)
    {
        var errorList = errors.Where(x => x != null).ToList();

        if (errorList.Count > 0)
            throw new AggregateException($"{summary}:\n" + string.Join("\n", errorList));
    }

    /// <summary>The direct property-typed dependencies of <paramref name="type"/>: the underlying Glyph type behind each of its PropertyNib-backed nibs, unwrapping Nullable and List&lt;T&gt; the same way Navigation does when building the actual regex graph.</summary>
    static IEnumerable<Type> GetDirectDependentGlyphTypes(Type type) =>
        GlyphTypeCache.GetConfiguration(type).Nibs
            .OfType<PropertyNib>()
            .Select(x => Nullable.GetUnderlyingType(x.Type) ?? x.Type)
            .Select(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(List<>) ? x.GetGenericArguments()[0] : x)
            .Where(x => x.IsAssignableTo(typeof(Glyph)));

    /// <summary>Walks the property dependency graph reachable from <paramref name="roots"/> (inclusive) and returns every Glyph type found.</summary>
    static HashSet<Type> GetTransitiveGlyphTypeClosure(IEnumerable<Type> roots)
    {
        HashSet<Type> visited = [];
        Stack<Type> pending = new(roots);

        while (pending.Count > 0)
        {
            var type = pending.Pop();

            if (!visited.Add(type))
                continue;

            foreach (var dependent in GetDirectDependentGlyphTypes(type))
                if (!visited.Contains(dependent))
                    pending.Push(dependent);
        }

        return visited;
    }

    /// <summary>
    /// Loads every non-dynamic assembly sitting alongside the running one and returns every type across the loaded
    /// AppDomain - how <see cref="Default"/> finds the consumer assemblies that define Glyph types.
    /// </summary>
    static IEnumerable<Type> LoadAllAssemblyTypes()
    {
        var loadedPaths = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => a.Location)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var dllPath in Directory.GetFiles(AppContext.BaseDirectory, "*.dll"))
        {
            if (loadedPaths.Contains(dllPath))
                continue;

            try
            {
                Assembly.LoadFrom(dllPath);
            }
            catch
            {
                // Not every DLL in the output directory is a managed assembly we can load; skip those.
            }
        }

        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(GetLoadableTypes)
            .ToList();
    }

    static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null);
        }
    }
}
