using System.Reflection.Emit;

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
        new GlyphGrammar(LoadAllAssemblyTypes(), GlobalSettings.Current.AllowPartialSegmentMatches));

    /// <summary>
    /// The grammar of every Glyph type in the assemblies alongside the running one - Glyph types are defined by
    /// consumer assemblies (e.g. MTGGlyphs), not by this library - tokenized under
    /// <see cref="GlobalSettings.Current"/>. Built on first use.
    /// </summary>
    public static GlyphGrammar Default => _default.Value;

    // Everything the grammar was given, Glyph or not: CaptureUnit types (e.g. for GetAllCaptureUnitTypes) are
    // drawn from here too.
    readonly List<Type> _candidateTypes;
    readonly Dictionary<string, Type> _typesByName = [];

    /// <summary>Whether a top-level type may match only part of a segment - see <see cref="GlobalSettings.AllowPartialSegmentMatches"/>.</summary>
    public bool AllowPartialSegmentMatches { get; }

    /// <summary>
    /// Every Glyph type in play: each non-generic, non-abstract Glyph type given - dependents included - plus every
    /// type reachable by walking their property nibs, which is what surfaces a closed generic like
    /// <c>OneOf&lt;CardType, CreatureType&gt;</c> that only ever appears as a property type. When any are marked
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
    public GlyphGrammar(IEnumerable<Type> types, bool allowPartialSegmentMatches)
    {
        _candidateTypes = types.ToList();
        AllowPartialSegmentMatches = allowPartialSegmentMatches;
        Build();
    }

    /// <summary>A grammar of every type in <paramref name="assemblies"/>.</summary>
    public static GlyphGrammar FromAssemblies(IEnumerable<Assembly> assemblies, bool allowPartialSegmentMatches) =>
        new(assemblies.SelectMany(GetLoadableTypes), allowPartialSegmentMatches);

    public List<CaptureUnit> Tokenize(string sourceText) =>
        Tokenizer.Tokenize(sourceText);

    public bool TryGetType(string name, out Type type) =>
        _typesByName.TryGetValue(name, out type);

    /// <summary>
    /// Runs every validation rule over <see cref="Types"/> and returns each failure as a "TypeName: message" string
    /// instead of throwing. Construction runs the same checks but throws; this is for a diagnostic tool that wants
    /// the full list of everything currently broken as data rather than as an exception.
    /// </summary>
    public List<string> GetStructuralValidationErrors() =>
        GetStructuralValidationErrors(Types);

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
        ThrowIfAny(GetStructuralValidationErrors(types), "One or more Glyph types failed structural validation");

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
        // property dependency graph too, so a type like WheneverACardEntersTheBattlefield still finds every Glyph
        // type it depends on (e.g. OneOf<CardType, CreatureType>).
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
        Tokenizer = new(TopLevelTypes.ToList(), DependentTypes.ToList(), AllowPartialSegmentMatches);
    }

    static List<string> GetStructuralValidationErrors(IEnumerable<Type> types)
    {
        var errors = new List<string>();

        foreach (var type in types)
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
        }

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

    // ---- Types created at runtime by the Glyph editor ----

    const string _dynamicAssemblyName = "Glyphotype.DynamicGlyphs";
    static readonly ModuleBuilder _moduleBuilder = AssemblyBuilder
        .DefineDynamicAssembly(new AssemblyName(_dynamicAssemblyName), AssemblyBuilderAccess.Run)
        .DefineDynamicModule("MainModule");
    static readonly string _sourceCodeDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "MTGGlyphs"));

    /// <summary>Emits <paramref name="editorGlyph"/> as a new type, adds it to this grammar (rebuilding and revalidating it), and saves its source alongside MTGGlyphs' own.</summary>
    public void CreateAndRegisterNewTypeAndSaveToDisk(EditorGlyph editorGlyph)
    {
        var newType = CreateDynamicGlyphType(editorGlyph);
        _candidateTypes.Add(newType);

        try
        {
            Build();
        }
        catch
        {
            // An invalid new type is rejected whole: rebuild without it, leaving the grammar as it was.
            _candidateTypes.Remove(newType);
            Build();
            throw;
        }

        DeterministicPalette.RefreshTypePaletteSet();
        var outputPath = Path.Combine(_sourceCodeDir, editorGlyph.ClassName + ".cs");
        File.WriteAllText(outputPath, editorGlyph.ClassStringForSavingToFile);
    }

    static Type CreateDynamicGlyphType(EditorGlyph editorGlyph)
    {
        var baseType = typeof(Glyph);
        var nibType = typeof(Nib);

        var tb = _moduleBuilder.DefineType(
            editorGlyph.ClassName,
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
            baseType
        );

        // 1) Set TokenizationOrder Attribute
        var orderCtor = typeof(TokenizationOrderAttribute).GetConstructor([typeof(int)])!;
        var orderAttr = new CustomAttributeBuilder(orderCtor, [-1]);
        tb.SetCustomAttribute(orderAttr);

        // 2) Define Auto-Properties (Non-Virtual)
        foreach (var nib in editorGlyph.Nibs.OfType<EditorPropertyNib>())
        {
            DefineAutoProperty(tb, nib.PropertyNameRepresentation, nib.ResolvedType);
        }

        // 3) Override "protected virtual Nib[] Nibs" (This one MUST be virtual to override)
        var getNibsMethod = tb.DefineMethod(
            "get_Nibs",
            MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
            nibType.MakeArrayType(),
            Type.EmptyTypes);

        var il = getNibsMethod.GetILGenerator();
        var nibs = editorGlyph.Nibs;

        il.Emit(OpCodes.Ldc_I4, nibs.Count);
        il.Emit(OpCodes.Newarr, nibType);

        var nibFromString = nibType.GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, null, [typeof(string)], null)
            ?? throw new InvalidOperationException("Nib.op_Implicit(string) not found.");

        for (int i = 0; i < nibs.Count; i++)
        {
            var nib = nibs[i];
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldc_I4, i);

            if (nib is EditorPropertyNib propNib)
            {
                var propMethod = typeof(Glyph).GetMethod(nameof(Glyph.Prop))
                    ?? throw new Exception("Glyph.Prop not found.");

                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ldc_I4, (int)propNib.Proptions);
                il.Emit(OpCodes.Ldstr, propNib.PropertyNameRepresentation);

                il.Emit(OpCodes.Call, propMethod);
            }
            else if (nib is EditorMethodNib methodNib)
            {
                var method = methodNib.Method;
                var paras = method.GetParameters();

                for (int pIdx = 0; pIdx < paras.Length; pIdx++)
                {
                    var pType = paras[pIdx].ParameterType;
                    if (pType == typeof(string[]))
                    {
                        var args = methodNib.Args;
                        il.Emit(OpCodes.Ldc_I4, args.Length);
                        il.Emit(OpCodes.Newarr, typeof(string));
                        for (int j = 0; j < args.Length; j++)
                        {
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Ldc_I4, j);
                            il.Emit(OpCodes.Ldstr, args[j]);
                            il.Emit(OpCodes.Stelem_Ref);
                        }
                    }
                    else if (pType == typeof(string))
                    {
                        string val = methodNib.Args.Length > 0 ? methodNib.Args[0] : "";
                        il.Emit(OpCodes.Ldstr, val);
                    }
                    else if (pType == typeof(Nib))
                    {
                        // e.g. Opt(Nib): the editor's text argument, as a literal-text Nib
                        string val = methodNib.Args.Length > 0 ? methodNib.Args[0] : "";
                        il.Emit(OpCodes.Ldstr, val);
                        il.Emit(OpCodes.Call, nibFromString);
                    }
                    else il.Emit(OpCodes.Ldnull);
                }
                il.Emit(OpCodes.Call, method);
            }
            else if (nib is EditorTextNib textNib)
            {
                il.Emit(OpCodes.Ldstr, textNib.TrimmedText);
                il.Emit(OpCodes.Call, nibFromString);
            }

            il.Emit(OpCodes.Stelem_Ref);
        }

        il.Emit(OpCodes.Ret);

        var propNibs = tb.DefineProperty("Nibs", PropertyAttributes.None, nibType.MakeArrayType(), null);
        propNibs.SetGetMethod(getNibsMethod);

        // 4) Constructor
        var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
        var ctorIl = ctor.GetILGenerator();
        ctorIl.Emit(OpCodes.Ldarg_0);
        var baseDefaultCtor = baseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null)!;
        ctorIl.Emit(OpCodes.Call, baseDefaultCtor);
        ctorIl.Emit(OpCodes.Ret);

        return tb.CreateType()!;

        // Corrected helper: Removed MethodAttributes.Virtual
        void DefineAutoProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var fieldBuilder = typeBuilder.DefineField($"<{propertyName}>k__BackingField", propertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Standard Public, Non-Virtual Getter
            var getter = typeBuilder.DefineMethod($"get_{propertyName}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                propertyType, Type.EmptyTypes);
            var gIl = getter.GetILGenerator();
            gIl.Emit(OpCodes.Ldarg_0);
            gIl.Emit(OpCodes.Ldfld, fieldBuilder);
            gIl.Emit(OpCodes.Ret);

            // Standard Public, Non-Virtual Setter
            var setter = typeBuilder.DefineMethod($"set_{propertyName}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null, [propertyType]);
            var sIl = setter.GetILGenerator();
            sIl.Emit(OpCodes.Ldarg_0);
            sIl.Emit(OpCodes.Ldarg_1);
            sIl.Emit(OpCodes.Stfld, fieldBuilder);
            sIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getter);
            propertyBuilder.SetSetMethod(setter);
        }
    }
}
