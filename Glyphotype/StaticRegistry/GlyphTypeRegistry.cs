using Glyphotype.NibHelpers;
using System.Reflection.Emit;

namespace Glyphotype.StaticRegistry;

public static partial class GlyphTypeRegistry
{
    const string _dynamicAssemblyName = "Glyphotype.DynamicGlyphs";
    static AssemblyBuilder _asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(_dynamicAssemblyName), AssemblyBuilderAccess.Run);
    static ModuleBuilder _moduleBuilder = _asmBuilder.DefineDynamicModule("MainModule");
    static Type[] _staticAssemblyTypes = LoadAllAssemblyTypes();
    static List<Type> _dynamicAssemblyTypes = [];
    static string _sourceCodeDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "MTGGlyphs"));

    public static Dictionary<Type, RegexGraph> RegexGraphs { get; set; } = [];
    public static Dictionary<Type, Regex> TypeRegexes { get; set; } = [];
    public static Dictionary<Type, GlyphTypeConfiguration> TypeConfigurations { get; set; } = [];
    public static Dictionary<string, Type> NameToType { get; set; } = [];
    public static List<Type> AppliedOrderTypes { get; set; } = [];
    public static HashSet<Type> ReferencedEnumTypes { get; set; } = [];
    public static Tokenizer ClassTokenizer { get; set; }

    static GlyphTypeRegistry()
    {
        // Add a default configuration for type Glyph, since it's abstract and can't be instantiated directly to check its Nibs
        TypeConfigurations[typeof(Glyph)] = new GlyphTypeConfiguration(typeof(Glyph), [], Joiner.Space);

        var topLevelTypes = GetAllTopLevelGlyphTypes();

        foreach (var type in topLevelTypes)
            SetRootNode(type);

        // Only the top-level types are validated automatically at startup; the exhaustive sweep over
        // every type discoverable via property nibs (GetAllTypesForValidation) is deliberately left
        // for callers to opt into via GetStructuralValidationErrors(), since some of what it finds today
        // (e.g. OneOf<CardType, CreatureType>'s non-nullable enums) is a known, not-yet-fixed issue that
        // would otherwise prevent the registry - and everything built on it - from initializing at all.
        ValidateAllStructures(topLevelTypes);

        InitializeClassTokenizer();
    }

    /// <summary>
    /// Loads every non-dynamic assembly sitting alongside this one and returns every type across
    /// the loaded AppDomain. Glyph types are defined by consumer assemblies (e.g. MTGGlyphs),
    /// not by this library itself, so discovery has to reach beyond GetExecutingAssembly().
    /// </summary>
    static Type[] LoadAllAssemblyTypes()
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
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    return e.Types.Where(t => t != null);
                }
            })
            .ToArray();
    }

    public static RegexGraph GetRegexGraph(Type type) =>
        RegexGraphs.TryGetValue(type, out var regexGraph)
            ? regexGraph : SetRootNode(type);

    static RegexGraph SetRootNode(Type type)
    {
        // Ensure TypeConfiguration is set (not strictly necessary, but convenient)
        _ = GetGlyphTypeConfiguration(type);

        NameToType[type.Name] = type;
        var regexGraph = RegexGraph.Create(type);
        RegexGraphs[type] = regexGraph;

        return regexGraph;
    }

    /// <summary>
    /// Runs Glyph.ValidateStructure() over every registered type in a single pass, after the
    /// entire registry has already been populated via SetRootNode. Running it as a separate pass
    /// (rather than inline within GetGlyphTypeConfiguration) avoids reentering the registry
    /// mid-construction, which previously caused stack overflows from recursive validation calls.
    /// </summary>
    static void ValidateAllStructures(List<Type> types)
    {
        var errors = GetStructuralValidationErrors(types);

        if (errors.Count > 0)
            throw new AggregateException("One or more Glyph types failed structural validation:\n" + string.Join("\n", errors));
    }

    /// <summary>
    /// Runs Glyph.ValidateStructure() over every type reachable via GetAllTypesForValidation() -
    /// every scanned Glyph type plus everything discoverable by walking property nibs, such as a
    /// closed generic OneOf&lt;T1,T2&gt; that never appears in the assembly scan on its own - and returns
    /// each failure as a "TypeName: message" string instead of throwing. Unlike the automatic startup
    /// check (which only covers top-level types and stops the registry from initializing on failure),
    /// this is meant to be called deliberately - e.g. by a diagnostic tool that wants the full list of
    /// everything currently broken, without needing every issue fixed first just to run it.
    /// </summary>
    public static List<string> GetStructuralValidationErrors() =>
        GetStructuralValidationErrors(GetAllTypesForValidation());

    static List<string> GetStructuralValidationErrors(List<Type> types)
    {
        var errors = new List<string>();

        foreach (var type in types)
        {
            if (!typeof(Glyph).IsAssignableFrom(type) || type.IsAssignableTo(typeof(DynamicGlyph)))
                continue;

            // Some ValidateStructure overrides (e.g. OneOfBase) index RegexGraphs directly rather than
            // going through the lazy GetRegexGraph accessor, so a graph must already exist here - which
            // matters for types (like a closed OneOf<T1,T2>) only ever discovered as a property.
            GetRegexGraph(type);

            var instance = (Glyph)Activator.CreateInstance(type);

            if (instance.ValidateStructure() is string error)
                errors.Add($"{type.Name}: {error}");
        }

        return errors;
    }

    public static GlyphTypeConfiguration GetGlyphTypeConfiguration(Type glyphType)
    {
        if (TypeConfigurations.TryGetValue(glyphType, out var configuration))
            return configuration;

        // DynamicGlyphs have no nibs b/c it contains an Item object that will be resolved via the Tokenizer at runtime
        if (glyphType.IsAssignableTo(typeof(DynamicGlyph)))
            return new(glyphType, [], Joiner.None)    ;

        var instance = (Glyph)Activator.CreateInstance(glyphType);
        var nibs = instance.Nibs.ToArray();

        if (nibs.Length == 0)
        {
            var propertyNibs = PropertyNib.GetPropertyNibs(glyphType);

            if (propertyNibs.Length > 0)
                nibs = propertyNibs;
            else if (glyphType.GetCustomAttribute<RegexPatternAttribute>() is RegexPatternAttribute attr)
                nibs = attr.Patterns.Select(x => new Nib(x)).ToArray();
            else
                nibs = [new Nib(glyphType.Name.ToFriendlyCase(TitleDisplayOption.Lower))];
        }

        configuration = new(glyphType, nibs, instance.Joiner);
        TypeConfigurations[glyphType] = configuration;

        return configuration;
    }

    public static List<CaptureUnit> Tokenize(string sourceText) =>
        ClassTokenizer.Tokenize(sourceText);

    public static List<Type> GetAllTopLevelGlyphTypes()
    {
        var allTypes = _staticAssemblyTypes
            .Where(x =>
                x.IsClass && !x.IsAbstract
                && typeof(Glyph).IsAssignableFrom(x)
                && !x.ContainsGenericParameters
                && !x.IsDefined(typeof(DependentAttribute)))
            .Concat(_dynamicAssemblyTypes)
            .ToList();

        var isolatedTypes = allTypes.Where(x => x.IsDefined(typeof(IsolateForTestingAttribute))).ToList();

        // When one or more types opt into isolated testing, don't just keep those exact types - pull in
        // their whole property dependency graph too, so a type like WheneverACardEntersTheBattlefield
        // still finds every Glyph type it depends on (e.g. OneOf<CardType, CreatureType>).
        if (isolatedTypes.Count > 0)
        {
            var isolatedClosure = GetTransitiveGlyphTypeClosure(isolatedTypes);
            allTypes = allTypes.Where(x => isolatedClosure.Contains(x)).ToList();
        }

        return allTypes;
    }

    /// <summary>
    /// Every type ValidateStructure() should run against: every non-generic, non-abstract Glyph
    /// type found by the assembly scan - including Dependent-only types, which GetAllTopLevelGlyphTypes
    /// deliberately excludes - plus every type reachable by walking each of those types' own property
    /// nibs. The property walk is what surfaces a closed generic like OneOf&lt;CardType, CreatureType&gt;,
    /// which never appears in the assembly scan on its own since it's only ever referenced as a property.
    /// When one or more types are marked IsolateForTesting, this scopes down to just their dependency
    /// closures, mirroring GetAllTopLevelGlyphTypes.
    /// </summary>
    public static List<Type> GetAllTypesForValidation()
    {
        var scannedTypes = _staticAssemblyTypes
            .Where(x =>
                x.IsClass && !x.IsAbstract
                && typeof(Glyph).IsAssignableFrom(x)
                && !x.ContainsGenericParameters)
            .Concat(_dynamicAssemblyTypes)
            .ToList();

        var isolatedTypes = scannedTypes.Where(x => x.IsDefined(typeof(IsolateForTestingAttribute))).ToList();
        var roots = isolatedTypes.Count > 0 ? isolatedTypes : scannedTypes;

        return GetTransitiveGlyphTypeClosure(roots).ToList();
    }

    /// <summary>The direct property-typed dependencies of <paramref name="type"/>: the underlying Glyph
    /// type behind each of its PropertyNib-backed nibs, unwrapping Nullable
    /// and List&lt;T&gt; the same way Navigation does when building the actual regex graph.</summary>
    static IEnumerable<Type> GetDirectDependentGlyphTypes(Type type) =>
        GetGlyphTypeConfiguration(type).Nibs
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

    public static List<Type> GetAllTypesExhaustive()
    {
        return _staticAssemblyTypes
            .Where(x =>
                x.IsClass
                && !x.IsAbstract
                && typeof(CaptureUnit).IsAssignableFrom(x))
            .Concat(_dynamicAssemblyTypes)
            .Concat(ReferencedEnumTypes)
            .Distinct()
            .OrderBy(x => x.Name)
            .ToList();
    }

    static void InitializeClassTokenizer()
    {
        // Reset applied orders, since the order may change during runtime
        AppliedOrderTypes = [];

        var allGlyphTypes = GetAllTopLevelGlyphTypes();

        // Since it's possible for multiple types to define the same order via TokenizationOrder,
        // each dictionary entry is a List, though each List should ideally only have one item.
        // An entry List may have multiple items if they each declare the same position.
        Dictionary<int, List<Type>> orderedTypes =
            allGlyphTypes.Where(x => x.IsDefined(typeof(TokenizationOrderAttribute)))
            .GroupBy(x => x.GetCustomAttribute<TokenizationOrderAttribute>().Order)
            .ToDictionary(x => x.Key, x => x.ToList());
        
        var typeOrderedItems = TypeOrderList
            .Select((type, idx) => (type, idx))
            .ToList();

        // Add types defined in the static TypeOrderList next.
        // Here, "idx" refers to the 0-based order of appearance in TypeOrderList,
        // which of course might be the same value as a defined order type above.
        // This means defined order type position takes precedence over TypeOrderList position.
        typeOrderedItems.ForEach(x => { if (!orderedTypes.TryAdd(x.idx, [x.type])) orderedTypes[x.idx].Add(x.type); });

        // Add all remaining types (i.e. those the user didn't bother to define anywhere).
        // Order by descending length, which is a rough approximate of complexity/match length (not exact)
        var unorderedRemainingTypes = allGlyphTypes
            .Except(orderedTypes.SelectMany(x => x.Value))
            .OrderByDescending(x => RegexGraphs[x].BuiltRegex.MinifiedRegex.Length)
            .ToList();

        var nextKey = orderedTypes.Keys.Any() ? orderedTypes.Keys.Max() + 1 : 0;
        orderedTypes[nextKey] = unorderedRemainingTypes;

        orderedTypes
            .Where(x => x.Key >= 0)
            .OrderBy(x => x.Key)
            .SelectMany(x => x.Value)
            .Concat(orderedTypes
                .Where(x => x.Key < 0)
                .SelectMany(x => x.Value))
            .Distinct()
            .ToList()
            .ForEach(AddClassGlyphType);

        TypeRegexes = RegexGraphs.Where(x => typeof(Glyph).IsAssignableFrom(x.Key)).ToDictionary(x => x.Key, x => x.Value.BuiltRegex.Regex);
        ClassTokenizer = new(AppliedOrderTypes);
    }

    static void AddClassGlyphType(Type glyphType)
    {
        if (AppliedOrderTypes.Contains(glyphType) || glyphType.IsDefined(typeof(DependentAttribute)))
            return;

        AppliedOrderTypes.Add(glyphType);
    }

    public static void CreateAndRegisterNewTypeAndSaveToDisk(EditorGlyph dynamicGlyphType)
    {
        var newType = CreateDynamicGlyphType(dynamicGlyphType);
        SetRootNode(newType);
        DeterministicPalette.RefreshTypePaletteSet();
        var outputPath = Path.Combine(_sourceCodeDir, dynamicGlyphType.ClassName + ".cs");
        File.WriteAllText(outputPath, dynamicGlyphType.ClassStringForSavingToFile);
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

        // 5) Finalize
        var type = tb.CreateType()!;
        SetRootNode(type);
        ValidateAllStructures([type]);
        _dynamicAssemblyTypes.Add(type);
        InitializeClassTokenizer();

        return type;

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

    static List<Type> TypeOrderList =
    [
    
    ];
}

