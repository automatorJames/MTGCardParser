using MTGPlexer.TokenEditor;
using System.Reflection.Emit;

namespace MTGPlexer.StaticRegistry;

public static partial class TokenTypeRegistry
{
    const string _dynamicAssemblyName = "MTGPlexer.DynamicTokenUnits";
    static AssemblyBuilder _asmBuilder =AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(_dynamicAssemblyName), AssemblyBuilderAccess.Run);
    static ModuleBuilder _moduleBuilder =_asmBuilder.DefineDynamicModule("MainModule");
    static Type[] _staticAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
    static List<Type> _dynamicAssemblyTypes = [];
    static string _sourceCodeDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", nameof(MTGPlexer), nameof(TokenUnits)));

    public static Dictionary<Type, RootNode> RootNodes { get; set; } = [];
    public static Dictionary<Type, RegexTemplate> Templates { get; set; } = [];
    public static Dictionary<Type, Regex> TypeRegexes { get; set; } = [];
    public static Dictionary<string, Type> NameToType { get; set; } = [];
    public static Dictionary<Type, string> EnumRegexStrings { get; set; } = [];
    public static Dictionary<Type, EnumScalarAlternateSet> EnumScalarAlternativeSets { get; set; } = [];
    public static Dictionary<TemplatePropInfo, ScalarAlternateSet> PropScalarAlternativeSets { get; set; } = [];
    public static Dictionary<Type, Regex> ManyOfRegexes { get; set; } = [];
    public static Dictionary<Type, Dictionary<TemplatePropInfo, List<TemplatePropInfo>>> PropDistillationMaps { get; set; } = [];
    public static Dictionary<Type, Type> EmittedOptionalManyTypes { get; set; } = [];
    public static List<Type> AppliedOrderTypes { get; set; } = [];
    public static HashSet<Type> ReferencedEnumTypes { get; set; } = [];
    public static Tokenizer ClassTokenizer { get; set; }
    public static Tokenizer OriginalTextTokenizer { get; set; }
    public static NodeTokenizer NodeTokenizer { get; set; }

    static TokenTypeRegistry()
    {
        InitializeEmitedManyTypes();

        var allTokenTypes = GetAllTopLevelTokenTypes();

        foreach (var type in allTokenTypes)
            SetTypeTemplate(type);


        var thing1 = NameToType["TargetGainsOrLosesBuff_Many"];
        var thing2 = RootNodes[thing1];
        var sayrEal = Templates[thing1];

        InitializeClassTokenizer();
        OriginalTextTokenizer = new([typeof(DefaultUnmatchedString)]);
    }

    public static RegexTemplate GetTypeTemplate(Type type)
    {
        if (!Templates.ContainsKey(type))
            SetTypeTemplate(type);

        var template = Templates[type];

        return template;
    }

    static void SetTypeTemplate(Type type)
    {
        NameToType[type.Name] = type;
        RootNodes[type] = new(type);
        RegexTemplate typeTemplate = new(type);
        Templates[type] = typeTemplate;
        var propCaptureSegments = typeTemplate.CaptureGroupProps;

        // Register all newly encountered enums (we use the EnumRegexProp instance for this,
        // but steps taken during registration only care about the enum type itself)
        propCaptureSegments
            .OfType<EnumSegment>()
            .Where(x => !EnumScalarAlternativeSets.ContainsKey(x.TemplatePropInfo.UnderlyingType))
            .ToList()
            .ForEach(enumRegexPropWithNewEnumType =>
            {
                var enumType = enumRegexPropWithNewEnumType.TemplatePropInfo.UnderlyingType;
                EnumRegexStrings[enumType] = enumRegexPropWithNewEnumType.RegexString;
                ReferencedEnumTypes.Add(enumType);
                NameToType[enumType.Name] = enumType;
                EnumScalarAlternativeSets[enumType] = enumRegexPropWithNewEnumType.EnumSet;
            });

        // Register enums that appear as the T types within XOf<T> properties
        propCaptureSegments
            .OfType<XOfSegmentBase>()
            .SelectMany(x => x.GenericTypes)
            .Where(x => x.IsEnum && !EnumScalarAlternativeSets.ContainsKey(x))
            .ToList()
            .ForEach(newEnumType => 
            {
                var enumSet = EnumSegment.EnumTypetoScalarSet(newEnumType);
                EnumRegexStrings[newEnumType] = enumSet.CollectiveRegex.ToString();
                ReferencedEnumTypes.Add(newEnumType);
                NameToType[newEnumType.Name] = newEnumType;
                EnumScalarAlternativeSets[newEnumType] = enumSet;
            });

        // Register all newly encountered scalar capture props that aren't enums (i.e. bools & placeholders)
        propCaptureSegments
            .OfType<ScalarCaptureSegmentBase>()
            .Where(x => x.TemplatePropInfo.TemplatePropType != TemplatePropType.Enum)
            .ToList()
            .ForEach(x => PropScalarAlternativeSets.TryAdd(x.TemplatePropInfo, x.ScalarAlternativeSet));

        // Register all newly encountered ManyProps (we use BaseType as the key, not UnderlyingType which is List<T>)
        propCaptureSegments
            .OfType<ManyOfSegment>()
            .ToList()
            .ForEach(x => ManyOfRegexes.TryAdd(x.TemplatePropInfo.UnderlyingType, typeTemplate.Builder.ExtractGroupRegex(x.TemplatePropInfo)));

        if (((TokenUnit)Activator.CreateInstance(type)).ValidateStructure() is string errorString)
            throw new Exception($"Type '{type.Name}' failed validation: {errorString}");
    }

    public static List<TokenUnit> Tokenize(SourceTextDTO sourceText, bool originalTextOnly = false)
    {
        var tokens = originalTextOnly ? OriginalTextTokenizer.Tokenize(sourceText) : ClassTokenizer.Tokenize(sourceText);
        return tokens;
    }

    /// <summary>
    /// Return all TokenUnit derived types except for DefaultUnmatchedString
    /// </summary>
    public static List<Type> GetAllTopLevelTokenTypes()
    {
        var allTypes = _staticAssemblyTypes
            .Where(x =>
                x.IsClass && !x.IsAbstract
                && typeof(TokenUnit).IsAssignableFrom(x)
                && !x.IsDefined(typeof(DependentAttribute)))
            .Concat(_dynamicAssemblyTypes);

        if (allTypes.Any(x => x.IsDefined(typeof(IsolateForTestingAttribute))))
            allTypes = allTypes.Where(x => x.IsDefined(typeof(IsolateForTestingAttribute)) || x == typeof(DefaultUnmatchedString));

        return allTypes.ToList();
    }

    public static List<Type> GetAllTypesExhaustive()
    {
        return _staticAssemblyTypes
            .Where(x =>
                x.IsClass
                && !x.IsAbstract
                && typeof(TokenUnit).IsAssignableFrom(x))
            .Concat(_dynamicAssemblyTypes)
            .Concat(ReferencedEnumTypes)
            .Concat(EmittedOptionalManyTypes.Values)
            .Distinct()
            .OrderBy(x => x.Name)
            .ToList();
    }

    static void InitializeEmitedManyTypes()
    {
        var typesContainingManyProps = GetAllTopLevelTokenTypes()
            .Where(x => x.GetProps().Any(y => y.IsDefined(typeof(OptionalManyAttribute)) || y.PropertyType.IsDefined(typeof(OptionalManyAttribute))));

        foreach (var type in typesContainingManyProps)
        {
            var emittedType = DynamicTypeEmitter.EmitManyType(type);
            EmittedOptionalManyTypes[type] = emittedType;
            SetTypeTemplate(emittedType);
        }
    }

    static void InitializeClassTokenizer()
    {
        // Reset applied orders, since the order may change during runtime
        AppliedOrderTypes = [];

        // Get all tokens except default unmatched string, which will be added last
        var allTokenTypes = GetAllTopLevelTokenTypes().Where(x => x != typeof(DefaultUnmatchedString));

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
        ClassTokenizer = new(AppliedOrderTypes);
        NodeTokenizer = new(AppliedOrderTypes);
    }

    static void AddClassTokenType(Type tokenUnitType)
    {
        if (AppliedOrderTypes.Contains(tokenUnitType) || tokenUnitType.IsDefined(typeof(DependentAttribute)))
            return;

        if (EmittedOptionalManyTypes.TryGetValue(tokenUnitType, out Type multiVersionType))
        {
            // If the tokenCaptureType emitted an optional many type version of itself, add that one first.
            // We do this so that the more specific many-item version of the token is not preempted by the
            // less-specific single version during tokenization.
            AppliedOrderTypes.Add(multiVersionType);
        }

        AppliedOrderTypes.Add(tokenUnitType);
    }

    public static void CreateAndRegisterNewTypeAndSaveToDisk(EditorTokenUnit dynamicTokenType)
    {
        var newType = CreateDynamicTokenUnitType(dynamicTokenType);
        SetTypeTemplate(newType);
        DeterministicPalette.RefreshTypePaletteSet();
        var outputPath = Path.Combine(_sourceCodeDir, dynamicTokenType.ClassName + ".cs");
        File.WriteAllText(outputPath, dynamicTokenType.ClassStringForSavingToFile);
    }

    static Type CreateDynamicTokenUnitType(EditorTokenUnit editorTokenUnit)
    {
        var baseType = typeof(TokenUnit);
        var snippetType = typeof(Snippet);
        var shortcutsType = typeof(SnippetShortcuts);

        var tb = _moduleBuilder.DefineType(
            editorTokenUnit.ClassName,
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
            baseType
        );

        // 1) Set TokenizationOrder Attribute
        var orderCtor = typeof(TokenizationOrderAttribute).GetConstructor([typeof(int)])!;
        var orderAttr = new CustomAttributeBuilder(orderCtor, [-1]);
        tb.SetCustomAttribute(orderAttr);

        // 2) Define Auto-Properties (Non-Virtual)
        foreach (var snippet in editorTokenUnit.Snippets.OfType<EditorPropertySnippet>())
        {
            DefineAutoProperty(tb, snippet.PropertyNameRepresentation, snippet.ResolvedType);
        }

        // 3) Override "protected virtual Snippet[] Snippets" (This one MUST be virtual to override)
        var getSnippetsMethod = tb.DefineMethod(
            "get_Snippets",
            MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
            snippetType.MakeArrayType(),
            Type.EmptyTypes);

        var il = getSnippetsMethod.GetILGenerator();
        var snippets = editorTokenUnit.Snippets;

        il.Emit(OpCodes.Ldc_I4, snippets.Count);
        il.Emit(OpCodes.Newarr, snippetType);

        var snippetFromString = snippetType.GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, null, [typeof(string)], null)
            ?? throw new InvalidOperationException("Snippet.op_Implicit(string) not found.");

        for (int i = 0; i < snippets.Count; i++)
        {
            var snippet = snippets[i];
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldc_I4, i);

            if (snippet is EditorPropertySnippet propSnippet)
            {
                var propMethod = shortcutsType.GetMethod(nameof(SnippetShortcuts.Prop))
                    ?? throw new Exception("SnippetShortcuts.Prop not found.");

                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ldc_I4, (int)propSnippet.Proptions);
                il.Emit(OpCodes.Ldstr, propSnippet.PropertyNameRepresentation);

                il.Emit(OpCodes.Call, propMethod);
            }
            else if (snippet is EditorMethodSnippet methodSnippet)
            {
                var method = methodSnippet.Method;
                var paras = method.GetParameters();

                for (int pIdx = 0; pIdx < paras.Length; pIdx++)
                {
                    var pType = paras[pIdx].ParameterType;
                    if (pType == typeof(string[]))
                    {
                        var args = methodSnippet.Args;
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
                        string val = methodSnippet.Args.Length > 0 ? methodSnippet.Args[0] : "";
                        il.Emit(OpCodes.Ldstr, val);
                    }
                    else il.Emit(OpCodes.Ldnull);
                }
                il.Emit(OpCodes.Call, method);
            }
            else if (snippet is EditorTextSnippet textSnippet)
            {
                il.Emit(OpCodes.Ldstr, textSnippet.TrimmedText);
                il.Emit(OpCodes.Call, snippetFromString);
            }

            il.Emit(OpCodes.Stelem_Ref);
        }

        il.Emit(OpCodes.Ret);

        var propSnippets = tb.DefineProperty("Snippets", PropertyAttributes.None, snippetType.MakeArrayType(), null);
        propSnippets.SetGetMethod(getSnippetsMethod);

        // 4) Constructor
        var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
        var ctorIl = ctor.GetILGenerator();
        ctorIl.Emit(OpCodes.Ldarg_0);
        var baseDefaultCtor = baseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null)!;
        ctorIl.Emit(OpCodes.Call, baseDefaultCtor);
        ctorIl.Emit(OpCodes.Ret);

        // 5) Finalize
        var type = tb.CreateType()!;
        SetTypeTemplate(type);
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

