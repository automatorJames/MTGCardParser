using System.Reflection.Emit;

namespace MTGPlexer.StaticRegistry;

public static partial class TokenTypeRegistry
{
    static AssemblyBuilder _asmBuilder =AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicTokenUnits"), AssemblyBuilderAccess.Run);
    static ModuleBuilder _moduleBuilder =_asmBuilder.DefineDynamicModule("MainModule");
    static Type[] _staticAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
    static List<Type> _dynamicAssemblyTypes = [];
    static string _sourceCodeDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", nameof(MTGPlexer), nameof(TokenUnits)));
    static string _tokenizerIgnorePattern = @"\s+";

    public static Dictionary<Type, RegexTemplate> Templates { get; set; } = [];
    public static Dictionary<Type, Regex> TypeRegexes { get; set; } = [];
    public static Dictionary<string, Type> NameToType { get; set; } = [];
    public static Dictionary<Type, Dictionary<object, Regex>> EnumMemberRegexes { get; set; } = [];
    public static Dictionary<Type, string> EnumRegexStrings { get; set; } = [];
    public static Dictionary<Type, ScalarAlternativeSet> EnumScalarAlternativeSets { get; set; } = [];
    public static Dictionary<RegexPropInfo, ScalarAlternativeSet> PropScalarAlternativeSets { get; set; } = [];
    public static Dictionary<Type, Regex> ManyOfRegexes { get; set; } = [];
    public static Dictionary<Type, Dictionary<RegexPropInfo, List<RegexPropInfo>>> PropDistillationMaps { get; set; } = [];
    public static Dictionary<Type, Palette> Palettes { get; set; } = [];
    public static Dictionary<Type, Type> EmitedOptionalManyTypes { get; set; } = [];
    public static List<Type> AppliedOrderTypes { get; set; } = [];
    public static HashSet<Type> ReferencedEnumTypes { get; set; } = [];
    public static Tokenizer OriginalTextTokenizer { get; set; }
    public static CardTokenizer CardTokenizer { get; set; }

    static TokenTypeRegistry()
    {
        InitializeEmitedManyTypes();

        foreach (var type in GetAllTokenTypes())
            SetTypeTemplate(type);

        InitializeCardTokenizer();
        OriginalTextTokenizer = new([typeof(DefaultUnmatchedString)]);
    }

    public static RegexTemplate GetTypeTemplate(Type type)
    {
        if (!Templates.ContainsKey(type))
            SetTypeTemplate(type);

        return Templates[type];
    }

    static void SetTypeTemplate(Type type)
    {
        Palettes[type] = new DeterministicPalette(type).Palette;
        NameToType[type.Name] = type;
        var instance = (TokenUnit)Activator.CreateInstance(type);

        if (instance.ValidateStructure() is string errorString)
            throw new Exception($"Type '{type.Name}' failed validation: {errorString}");

        Templates[type] = instance.Template;
        var propCaptureSegments = instance.Template.CaptureGroupProps;

        // Register all newly encountered enums (we use the EnumRegexProp instance for this,
        // but steps taken during registration only care about the enum type itself)
        propCaptureSegments
            .OfType<EnumRegexProp>()
            .Where(x => !EnumMemberRegexes.ContainsKey(x.RegexPropInfo.UnderlyingType))
            .ToList()
            .ForEach(RegisterEnum);

        // Register all newly encountered scalar capture props that aren't enums (i.e. bools & placeholders)
        propCaptureSegments
            .OfType<ScalarCapturePropBase>()
            .Where(x => x.RegexPropInfo.RegexPropType != RegexPropType.Enum)
            .ToList()
            .ForEach(x => PropScalarAlternativeSets.TryAdd(x.RegexPropInfo, x.ScalarAlternativeSet));

        // Register all newly encountered ManyProps (we use BaseType as the key, not UnderlyingType which is List<T>)
        propCaptureSegments
            .OfType<TokenRegexManyProp>()
            .ToList()
            .ForEach(x => ManyOfRegexes.TryAdd(x.RegexPropInfo.BaseType, instance.Template.Collector.ExtractGroupRegex(x.RegexPropInfo)));
    }

    static void RegisterEnum(EnumRegexProp newEnumType)
    {
        var enumType = newEnumType.RegexPropInfo.UnderlyingType;
        EnumMemberRegexes[enumType] = newEnumType.EnumMemberRegexes;
        EnumRegexStrings[enumType] = newEnumType.RegexString;
        ReferencedEnumTypes.Add(enumType);
        Palettes[enumType] = new DeterministicPalette(enumType, baseSaturation: .4, baseLightness: .4).Palette;
        NameToType[enumType.Name] = enumType;
        EnumScalarAlternativeSets[enumType] = newEnumType.ScalarAlternativeSet;
    }

    public static List<TokenUnit> Tokenize(string text, bool originalTextOnly)
    {
        var tokens = originalTextOnly ? OriginalTextTokenizer.Tokenize(text) : CardTokenizer.Tokenize(text);
        return tokens;
    }

    /// <summary>
    /// Return all TokenUnit derived types except for DefaultUnmatchedString
    /// </summary>
    static List<Type> GetAllTokenTypes()
    {
        var allTypes = _staticAssemblyTypes
        .Where(x =>
            x.IsClass && !x.IsAbstract
            && typeof(TokenUnit).IsAssignableFrom(x)
            && !x.IsDefined(typeof(TokenUnitPropertyAttribute)))
        .Concat(_dynamicAssemblyTypes);

        if (allTypes.Any(x => x.IsDefined(typeof(IsolateForTestingAttribute))))
            allTypes = allTypes.Where(x => x.IsDefined(typeof(IsolateForTestingAttribute)) || x == typeof(DefaultUnmatchedString));

        return allTypes.ToList();
    }

    static void InitializeEmitedManyTypes()
    {
        var typesContainingManyProps = GetAllTokenTypes()
            .Where(x => x.GetProps().Any(y => y.IsDefined(typeof(OptionalManyAttribute)) || y.PropertyType.IsDefined(typeof(OptionalManyAttribute))));

        foreach (var type in typesContainingManyProps)
        {
            var emittedType = DynamicTypeEmitter.EmitManyType(type);
            EmitedOptionalManyTypes[type] = emittedType;
            SetTypeTemplate(emittedType);
        }
    }

    static void InitializeCardTokenizer()
    {
        // Reset applied orders, since the order may change during runtime
        AppliedOrderTypes = [];

        // Get all tokens except default unmatched string, which will be added last
        var allTokenTypes = GetAllTokenTypes().Where(x => x != typeof(DefaultUnmatchedString));

        // Since it's possible for multiple types to define the same order via TokenizationOrder,
        // each dictionary entry is a List, though each List should ideally only have one item.
        // An entry List may have multiple items if they each declare the same position.
        Dictionary<int, List<Type>> orderedTypes =
            allTokenTypes.Where(x => x.IsDefined(typeof(TokenizationOrderAttribute)))
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
        var unorderedRemainingTypes = allTokenTypes
            .Except(orderedTypes.SelectMany(x => x.Value))
            .OrderByDescending(x => Templates[x].RegexString.Length)
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
            .ForEach(AddClassTokenType);

        TypeRegexes = Templates.Where(x => x.Key != typeof(DefaultUnmatchedString)).ToDictionary(x => x.Key, x => x.Value.Regex);
        CardTokenizer = new(AppliedOrderTypes);
    }

    static void AddClassTokenType(Type tokenUnitType)
    {
        if (AppliedOrderTypes.Contains(tokenUnitType) || tokenUnitType.IsDefined(typeof(TokenUnitPropertyAttribute)))
            return;

        if (EmitedOptionalManyTypes.TryGetValue(tokenUnitType, out Type multiVersionType))
        {
            // If the tokenCaptureType emitted an optional many type version of itself, add that one first.
            // We do this so that the more specific many-item version of the token is not preempted by the
            // less-specific single version during tokenization.
            AppliedOrderTypes.Add(multiVersionType);
        }

        AppliedOrderTypes.Add(tokenUnitType);
    }

    public static void AddNewTypeAndSaveToDisk(DynamicTokenType dynamicTokenType)
    {
        var newType = CreateDynamicTokenUnitType(dynamicTokenType);
        var outputPath = Path.Combine(_sourceCodeDir, dynamicTokenType.ClassName + ".cs");
        File.WriteAllText(outputPath, dynamicTokenType.ClassString);
    }

    static Type CreateDynamicTokenUnitType(DynamicTokenType dynamicTokenType)
    {
        var baseType = typeof(TokenUnit);
        var tb = _moduleBuilder.DefineType(
                              dynamicTokenType.ClassName,
                              TypeAttributes.Public | TypeAttributes.Class,
                              baseType
                          );

        var orderCtor = typeof(TokenizationOrderAttribute)
                    .GetConstructor(new[] { typeof(int) })!;
        var orderAttr = new CustomAttributeBuilder(
                              orderCtor,
                              new object[] { -1 }
                          );
        tb.SetCustomAttribute(orderAttr);

        // 1) Walk your snippets: if it's a Type, define an auto‑property; always remember the string to pass to base(...)
        var snippetStrings = new string[dynamicTokenType.DynamicSnippets.Count];
        for (int i = 0; i < dynamicTokenType.DynamicSnippets.Count; i++)
        {
            var snippet = dynamicTokenType.DynamicSnippets[i];
            object resolvedSnippet = NameToType.TryGetValue(snippet, out Type resolvedType) ? resolvedType : snippet;

            switch (resolvedSnippet)
            {
                case Type t:
                    // define public T T { get; set; }
                    DefineAutoProperty(tb, t.Name, t);
                    snippetStrings[i] = t.Name;
                    break;

                case string s:
                    snippetStrings[i] = s;
                    break;

                default:
                    throw new ArgumentException(
                        $"snippets[{i}] must be either a Type or string"
                    );
            }
        }

        // 2) Define a parameterless ctor that does : base(snippetStrings...)
        var ctor = tb.DefineConstructor(
                       MethodAttributes.Public,
                       CallingConventions.Standard,
                       Type.EmptyTypes
                   );
        var il = ctor.GetILGenerator();

        // load `this`
        il.Emit(OpCodes.Ldarg_0);

        // create new string[snippetStrings.Length]
        il.Emit(OpCodes.Ldc_I4, snippetStrings.Length);
        il.Emit(OpCodes.Newarr, typeof(string));

        // fill the array
        for (int idx = 0; idx < snippetStrings.Length; idx++)
        {
            il.Emit(OpCodes.Dup);                             // keep array
            il.Emit(OpCodes.Ldc_I4, idx);                     // index
            il.Emit(OpCodes.Ldstr, snippetStrings[idx]);      // value
            il.Emit(OpCodes.Stelem_Ref);                      // array[idx] = value
        }

        // call protected TokenUnit .ctor(string[])
        var baseCtor = baseType.GetConstructor(
                           BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                           binder: null,
                           new[] { typeof(string[]) },
                           modifiers: null
                       )!;
        il.Emit(OpCodes.Call, baseCtor);
        il.Emit(OpCodes.Ret);

        // 3) Bake and return
        var type = tb.CreateType()!;
        SetTypeTemplate(type);
        _dynamicAssemblyTypes.Add(type);
        InitializeCardTokenizer();

        return type;
    }

    private static void DefineAutoProperty(TypeBuilder tb, string name, Type propertyType)
    {
        // backing field
        var field = tb.DefineField(
                        $"_{char.ToLowerInvariant(name[0])}{name.Substring(1)}",
                        propertyType,
                        FieldAttributes.Private
                    );

        // the Property itself
        var prop = tb.DefineProperty(
                       name,
                       PropertyAttributes.HasDefault,
                       propertyType,
                       null
                   );

        // getter
        var getter = tb.DefineMethod(
                         $"get_{name}",
                         MethodAttributes.Public |
                         MethodAttributes.SpecialName |
                         MethodAttributes.HideBySig,
                         propertyType,
                         Type.EmptyTypes
                     );
        var getIL = getter.GetILGenerator();
        getIL.Emit(OpCodes.Ldarg_0);
        getIL.Emit(OpCodes.Ldfld, field);
        getIL.Emit(OpCodes.Ret);
        prop.SetGetMethod(getter);

        // setter
        var setter = tb.DefineMethod(
                         $"set_{name}",
                         MethodAttributes.Public |
                         MethodAttributes.SpecialName |
                         MethodAttributes.HideBySig,
                         null,
                         new[] { propertyType }
                     );
        var setIL = setter.GetILGenerator();
        setIL.Emit(OpCodes.Ldarg_0);
        setIL.Emit(OpCodes.Ldarg_1);
        setIL.Emit(OpCodes.Stfld, field);
        setIL.Emit(OpCodes.Ret);
        prop.SetSetMethod(setter);
    }
}
