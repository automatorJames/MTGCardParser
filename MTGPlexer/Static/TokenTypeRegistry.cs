using MTGPlexer.RegexGeneration.RegexSegments;
using System.Reflection.Emit;

namespace MTGPlexer.Static;

public static partial class TokenTypeRegistry
{
    static AssemblyBuilder _asmBuilder =AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicTokenUnits"), AssemblyBuilderAccess.Run);
    static ModuleBuilder _moduleBuilder =_asmBuilder.DefineDynamicModule("MainModule");
    static Type[] _staticAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
    static List<Type> _dynamicAssemblyTypes = [];
    static string _sourceCodeDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", nameof(MTGPlexer), nameof(TokenUnits)));
    static string _tokenizerIgnorePattern = @"\s+";

    public static Dictionary<Type, RegexTemplate> Templates { get; set; } = [];
    public static Dictionary<string, Type> NameToType { get; set; } = [];
    public static Dictionary<Type, Dictionary<object, Regex>> EnumMemberRegexes { get; set; } = [];
    public static Dictionary<Type, string> EnumRegexStrings { get; set; } = [];
    public static Dictionary<Type, ScalarAlternativeSet> EnumScalarAlternativeSets { get; set; } = [];
    public static Dictionary<RegexPropInfo, ScalarAlternativeSet> PropScalarAlternativeSets { get; set; } = [];
    public static Dictionary<Type, Dictionary<RegexPropInfo, List<RegexPropInfo>>> DistilledProperties { get; set; } = [];
    public static Dictionary<Type, DeterministicPalette> Palettes { get; set; } = [];
    public static Dictionary<Type, Type> EmitedOptionalManyTypes { get; set; } = [];
    public static List<Type> AppliedOrderTypes { get; set; } = [];
    public static HashSet<Type> ReferencedEnumTypes { get; set; } = [];
    public static Tokenizer<Type> ClassTokenizer { get; set; }
    public static Tokenizer<Type> OriginalTextTokenizer { get; set; }

    static TokenTypeRegistry()
    {
        InitializeEmitedManyTypes();

        foreach (var type in GetAllTokenTypes())
            SetTypeTemplate(type);

        InitializeClassTokenizer();
        InitializeOriginalTextTokenizer();
    }

    public static RegexTemplate GetTypeTemplate(Type type)
    {
        if (!Templates.ContainsKey(type))
            SetTypeTemplate(type);

        return Templates[type];
    }

    static void SetTypeTemplate(Type type)
    {
        Palettes[type] = new(type);
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

        if (instance is TokenUnitDistilled tokenUnitDistilled)
        {
            DistilledProperties[type] = new();

            foreach (var item in tokenUnitDistilled.GetDistilledPropAssociations())
                DistilledProperties[type][item.Key] = item.Value;
        }
    }

    static void RegisterEnum(EnumRegexProp newEnumType)
    {
        var enumType = newEnumType.RegexPropInfo.UnderlyingType;
        EnumMemberRegexes[enumType] = newEnumType.EnumMemberRegexes;
        EnumRegexStrings[enumType] = newEnumType.RegexString;
        ReferencedEnumTypes.Add(enumType);
        Palettes[enumType] = new(enumType, baseSaturation: .4, baseLightness: .4);
        NameToType[enumType.Name] = enumType;
        EnumScalarAlternativeSets[enumType] = newEnumType.ScalarAlternativeSet;
    }

    public static List<Token<Type>> TokenizeAndCoallesceUnmatched(string text, bool originalTextOnly)
    {
        List<Token<Type>> coallescedTokens = [];
        List<Token<Type>> unmatchedBuffer = [];
        var tokens = originalTextOnly ? OriginalTextTokenizer.Tokenize(text).ToList() : ClassTokenizer.Tokenize(text).ToList();
        foreach (var token in tokens)
        {
            if (token.Kind == typeof(DefaultUnmatchedString))
                unmatchedBuffer.Add(token);
            else
            {
                // flush the buffer and append
                FlushBuffer();
                coallescedTokens.Add(token);
            }
        }

        FlushBuffer();

        // local helper
        void FlushBuffer()
        {
            if (unmatchedBuffer.Count > 0)
            {
                Token<Type> combinedUnmatchedStringToken = default;

                if (unmatchedBuffer.Count > 1)
                    combinedUnmatchedStringToken = CoallesceUnmatchedStringTokens(unmatchedBuffer);
                else if (unmatchedBuffer.Count == 1)
                    combinedUnmatchedStringToken = unmatchedBuffer[0];

                coallescedTokens.Add(combinedUnmatchedStringToken);
            }

            unmatchedBuffer = [];
        }

        return coallescedTokens;
    }

    static Token<Type> CoallesceUnmatchedStringTokens(List<Token<Type>> unmatchedStringTokens)
    {
        var originalSource = unmatchedStringTokens[0].Span;
        var firstItem = unmatchedStringTokens[0];
        var lastItem = unmatchedStringTokens[^1];
        var start = firstItem.Span.Position.Absolute;
        var combinedLength = lastItem.Span.Position.Absolute + lastItem.Span.Length - start;
        var position = new Position(firstItem.Span.Position.Absolute, firstItem.Span.Position.Line, firstItem.Span.Position.Line);
        var combinedTextSpan = new TextSpan(originalSource.Source, position, combinedLength);
        var token = new Token<Type>(typeof(DefaultUnmatchedString), combinedTextSpan);

        return token;
    }

    //public static TokenUnit HydrateFromToken(Token<Type> token) 
    //    => TokenUnit.InstantiateFromMatchString(token.Kind, token.Span);

    public static TokenUnit HydrateFromToken(Token<Type> token)
    {
        var match = Templates[token.Kind].Regex.Match(token.Span.ToStringValue());
        return HydrateFromMatch(token.Kind, match);
    }

    public static TokenUnit HydrateFromMatch(Type tokenUnitType, Match match)
    {
        var tokenUnit = (TokenUnit)Activator.CreateInstance(tokenUnitType);
        tokenUnit.Match = match;

        foreach (var captureProp in tokenUnit.Template.CaptureGroupProps)
            captureProp.SetValueFromMatch(tokenUnit, match);

        return tokenUnit;
    }

    /// <summary>
    /// Return all TokenUnit derived types except for DefaultUnmatchedString
    /// </summary>
    static List<Type> GetAllTokenTypes()
    {
        var allTypes = _staticAssemblyTypes
        .Where(x =>
            x.IsClass && !x.IsAbstract
            && typeof(TokenUnit).IsAssignableFrom(x))
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

    static void InitializeClassTokenizer()
    {
        // Reset applied orders, since the order may change during runtime
        AppliedOrderTypes = [];

        // Get all tokens except default unmatched string, which will be added last
        var allTokenTypes = GetAllTokenTypes().Where(x => x != typeof(DefaultUnmatchedString));

        var tokenizerBuilder = new TokenizerBuilder<Type>();
        tokenizerBuilder.Ignore(Span.Regex(_tokenizerIgnorePattern));

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

        List<Type> flattenedOrderedTypes = orderedTypes
            .Where(x => x.Key >= 0)
            .OrderBy(x => x.Key)
            .SelectMany(x => x.Value)
            .Concat(orderedTypes
                .Where(x => x.Key < 0)
                .SelectMany(x => x.Value))
            .Distinct()
            .ToList();

        flattenedOrderedTypes.ForEach(x => tokenizerBuilder.Match(x));

        // Catch anything else with the default string pattern
        tokenizerBuilder.Match(typeof(DefaultUnmatchedString));

        ClassTokenizer = tokenizerBuilder.Build();
    }

    static void InitializeOriginalTextTokenizer()
    {
        var tokenizerBuilder = new TokenizerBuilder<Type>();
        tokenizerBuilder.Ignore(Span.Regex(_tokenizerIgnorePattern));
        tokenizerBuilder.Match(Span.Regex(Templates[typeof(DefaultUnmatchedString)].RegexString), typeof(DefaultUnmatchedString));
        OriginalTextTokenizer = tokenizerBuilder.Build();
    }

    static TokenizerBuilder<Type> Match(this TokenizerBuilder<Type> tokenizerBuilder, Type tokenCaptureType)
    {
        if (AppliedOrderTypes.Contains(tokenCaptureType) || tokenCaptureType.IsDefined(typeof(TokenUnitPropertyAttribute)))
            return tokenizerBuilder;

        if (EmitedOptionalManyTypes.TryGetValue(tokenCaptureType, out Type multiVersionType))
        {
            // If the tokenCaptureType emitted an optional many type version of itself, add that one first.
            // We do this so that the more specific many-item version of the token is not preempted by the
            // less-specific single version during tokenization.
            tokenizerBuilder.Match(multiVersionType);
        }

        tokenizerBuilder.Match(Span.Regex(Templates[tokenCaptureType].RegexString), tokenCaptureType);
        AppliedOrderTypes.Add(tokenCaptureType);

        return tokenizerBuilder;
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
        InitializeClassTokenizer();

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
