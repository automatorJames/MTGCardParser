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

    static TokenTypeRegistry()
    {
        InitializeEmitedManyTypes();

        var allTokenTypes = GetAllTopLevelTokenTypes();

        foreach (var type in allTokenTypes)
            SetTypeTemplate(type);

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
                              TypeAttributes.Public | TypeAttributes.Class,
                              baseType
                          );

        // 1) Set the Order Attribute
        var orderCtor = typeof(TokenizationOrderAttribute).GetConstructor(new[] { typeof(int) })!;
        var orderAttr = new CustomAttributeBuilder(orderCtor, new object[] { -1 });
        tb.SetCustomAttribute(orderAttr);

        // 2) Define Auto-Properties for referenced types
        foreach (var snippet in editorTokenUnit.Snippets.OfType<EditorPropertySnippet>())
            DefineAutoProperty(tb, snippet.ResolvedType.Name, snippet.ResolvedType);

        // 3) Override "protected virtual Snippet[] Snippets { get; }"
        var getSnippetsMethod = tb.DefineMethod(
            "get_Snippets",
            MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
            snippetType.MakeArrayType(),
            Type.EmptyTypes);

        var ilGen = getSnippetsMethod.GetILGenerator();
        var parts = editorTokenUnit.Snippets;

        // Implementation: return new Snippet[] { ... }
        ilGen.Emit(OpCodes.Ldc_I4, parts.Count);
        ilGen.Emit(OpCodes.Newarr, snippetType);

        for (int i = 0; i < parts.Count; i++)
        {
            var snippet = parts[i];
            ilGen.Emit(OpCodes.Dup);           // Duplicate array reference
            ilGen.Emit(OpCodes.Ldc_I4, i);     // Load index

            if (snippet is EditorPropertySnippet propertySnippet)
            {
                // Call SnippetShortcuts.Prop(null, "TypeName")
                var propMethod = shortcutsType.GetMethod(nameof(SnippetShortcuts.Prop))!;
                ilGen.Emit(OpCodes.Ldnull);         // First arg: null
                ilGen.Emit(OpCodes.Ldstr, propertySnippet.PropertyNameRepresentation); // Second arg: property name
                ilGen.Emit(OpCodes.Call, propMethod);
            }
            else if (snippet is EditorMethodSnippet methodSnippet)
            {
                var method = methodSnippet.Method;
                var parameters = method.GetParameters();

                // Special Case: Alt(params string[])
                if (method.Name == nameof(SnippetShortcuts.Alt))
                {
                    var alts = methodSnippet.Args;
                    ilGen.Emit(OpCodes.Ldc_I4, alts.Length);
                    ilGen.Emit(OpCodes.Newarr, typeof(string));
                    for (int j = 0; j < alts.Length; j++)
                    {
                        ilGen.Emit(OpCodes.Dup);
                        ilGen.Emit(OpCodes.Ldc_I4, j);
                        ilGen.Emit(OpCodes.Ldstr, alts[j]);
                        ilGen.Emit(OpCodes.Stelem_Ref);
                    }
                }
                // Methods with 1 string parameter (Opt, NoSpace)
                else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                {
                    ilGen.Emit(OpCodes.Ldstr, methodSnippet.Args[0]);
                }
                // Methods with 0 parameters (Plural)
                else if (parameters.Length == 0)
                {
                    // No args to load
                }

                ilGen.Emit(OpCodes.Call, method);
            }
            else if (snippet is EditorTextSnippet textSnippet)
            {
                // Plain text: new Snippet("text")
                var snippetCtor = snippetType.GetConstructor(new[] { typeof(string) })!;
                ilGen.Emit(OpCodes.Ldstr, textSnippet.Text);
                ilGen.Emit(OpCodes.Newobj, snippetCtor);
            }

            ilGen.Emit(OpCodes.Stelem_Ref); // array[i] = createdSnippet
        }
        ilGen.Emit(OpCodes.Ret);

        // Link the getter to the Property
        var propSnippets = tb.DefineProperty("Snippets", PropertyAttributes.None, snippetType.MakeArrayType(), null);
        propSnippets.SetGetMethod(getSnippetsMethod);

        // 4) Define Parameterless Constructor
        var ctor = tb.DefineConstructor(
                       MethodAttributes.Public,
                       CallingConventions.Standard,
                       Type.EmptyTypes
                   );
        var ctorIl = ctor.GetILGenerator();

        ctorIl.Emit(OpCodes.Ldarg_0);
        var baseDefaultCtor = baseType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, Type.EmptyTypes, null)!;
        ctorIl.Emit(OpCodes.Call, baseDefaultCtor);
        ctorIl.Emit(OpCodes.Ret);

        // 5) Finalize
        var type = tb.CreateType()!;
        SetTypeTemplate(type);
        _dynamicAssemblyTypes.Add(type);
        InitializeClassTokenizer();

        return type;

        // local helper
        void DefineAutoProperty(TypeBuilder tb, string name, Type propertyType)
        {
            var field = tb.DefineField($"_{char.ToLowerInvariant(name[0])}{name[1..]}", propertyType, FieldAttributes.Private);
            var prop = tb.DefineProperty(name, PropertyAttributes.HasDefault, propertyType, null);

            // getter
            var getter = tb.DefineMethod($"get_{name}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            var getIL = getter.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, field);
            getIL.Emit(OpCodes.Ret);
            prop.SetGetMethod(getter);

            // setter
            var setter = tb.DefineMethod($"set_{name}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
            var setIL = setter.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, field);
            setIL.Emit(OpCodes.Ret);
            prop.SetSetMethod(setter);
        }
    }

    static List<Type> TypeOrderList =
    [
    
    ];
}

