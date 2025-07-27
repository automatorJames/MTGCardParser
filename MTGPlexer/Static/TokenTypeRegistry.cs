using System.Reflection.Emit;

namespace MTGPlexer.Static;

public static partial class TokenTypeRegistry
{
    static HashSet<Type> _invalidTypes = [];
    static AssemblyBuilder _asmBuilder =AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicTokenUnits"), AssemblyBuilderAccess.Run);
    static ModuleBuilder _moduleBuilder =_asmBuilder.DefineDynamicModule("MainModule");
    static Type[] _staticAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
    static List<Type> _dynamicAssemblyTypes = [];
    static string _sourceCodeDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", nameof(MTGPlexer), nameof(TokenUnits)));

    public static Dictionary<Type, RegexTemplate> Templates { get; set; } = [];
    public static Dictionary<string, Type> NameToType { get; set; } = [];
    public static Dictionary<Type, Dictionary<object, Regex>> EnumMemberRegexes { get; set; } = [];
    public static Dictionary<Type, string> EnumRegexStrings { get; set; } = [];
    public static Dictionary<Type, Dictionary<PropertyInfo, List<PropertyInfo>>> DistilledProperties { get; set; } = [];
    public static Dictionary<Type, DeterministicPalette> Palettes { get; set; } = [];
    public static List<Type> AppliedOrderTypes { get; set; } = [];
    public static HashSet<Type> ReferencedEnumTypes { get; set; } = [];
    public static Tokenizer<Type> Tokenizer { get; set; }

    static TokenTypeRegistry()
    {
        foreach (var type in GetAllTokenTypes())
            SetTypeTemplate(type);

        InitializeTokenizer();
    }

    public static RegexTemplate GetTypeTemplate(Type type)
    {
        if (!Templates.ContainsKey(type))
            SetTypeTemplate(type);

        return Templates[type];
    }

    public static void SetTypeTemplate(Type type)
    {
        var instance = (TokenUnit)Activator.CreateInstance(type);

        if (!instance.ValidateStructure())
        {
            _invalidTypes.Add(type);
            return;
        }

        Templates[type] = instance.Template;
        NameToType[type.Name] = type;
        var propCaptureSegments = instance.Template.PropCaptureSegments;

        var unregisteredEnums = propCaptureSegments
            .OfType<EnumRegexProp>()
            .Where(x => !EnumMemberRegexes.ContainsKey(x.RegexPropInfo.UnderlyingType));

        foreach (var enumEntry in unregisteredEnums)
        {
            var enumType = enumEntry.RegexPropInfo.UnderlyingType;
            EnumMemberRegexes[enumType] = enumEntry.EnumMemberRegexes;
            EnumRegexStrings[enumType] = enumEntry.RegexString;
            ReferencedEnumTypes.Add(enumType);
            Palettes[enumType] = new(enumType, baseSaturation: .4, baseLightness: .4);
            NameToType[enumType.Name] = enumType;
        }

        Palettes[type] = new(type);

        if (instance is TokenUnitDistilled tokenUnitComplex)
        {
            DistilledProperties[type] = new();

            foreach (var item in tokenUnitComplex.GetDistilledPropAssociations())
                DistilledProperties[type][item.Key] = item.Value;
        }
    }

    public static List<Token<Type>> TokenizeAndCoallesceUnmatched(string text)
    {
        List<Token<Type>> coallescedTokens = [];
        List<Token<Type>> unmatchedBuffer = [];
        var tokens = Tokenizer.Tokenize(text);

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

    public static TokenUnit HydrateFromToken(Token<Type> token) 
        => TokenUnit.InstantiateFromMatchString(token.Kind, token.Span);

    /// <summary>
    /// Return all TokenUnit derived types except for DefaultUnmatchedString
    /// </summary>
    static List<Type> GetAllTokenTypes() =>
        _staticAssemblyTypes
        .Where(x =>
            x.IsClass && !x.IsAbstract
            && typeof(TokenUnit).IsAssignableFrom(x))
        .Concat(_dynamicAssemblyTypes)
        .ToList();

    static void InitializeTokenizer()
    {
        // Reset applied orders, since the order may change during runtime
        AppliedOrderTypes = [];

        // Get all tokens except default unmatched string, which will be added last
        var allTokenTypes = GetAllTokenTypes().Where(x => x != typeof(DefaultUnmatchedString));

        var tokenizerBuilder = new TokenizerBuilder<Type>();
        tokenizerBuilder.Ignore(Span.Regex(@"\s+"));

        // Since it's possible for multiple types to define the same order via TokenizationOrder,
        // Each dictionary entry is a List, though each List should normally only have one item.
        Dictionary<int, List<Type>> _definedOrderTypes =
            allTokenTypes.Where(x => x.IsDefined(typeof(TokenizationOrderAttribute)))
            .GroupBy(x => x.GetCustomAttribute<TokenizationOrderAttribute>().Order)
            .ToDictionary(x => x.Key, x => x.ToList());

        // Ensure types aren't represented twice, once in the static list, and once in the attribute-having list
        var typeOrderedItems = TypeOrderList.Except(_definedOrderTypes.SelectMany(x => x.Value)).ToList();

        // Ensure our range spans the entirety of both ordered sources
        var minPosition = Math.Min(0, _definedOrderTypes.Keys.Min());
        var maxPosition = Math.Max(typeOrderedItems.Count - 1, _definedOrderTypes.Keys.Max());

        // Ensure we handle (in order) all attribute defined order types, listed ordered types, and all other types
        for (int i = minPosition; i <= maxPosition; i++)
        {
            // handle defined attribute order first
            // since any key can have N items defined, process all N
            if (_definedOrderTypes.ContainsKey(i))
                _definedOrderTypes[i].ForEach(x => tokenizerBuilder.Match(x));

            // handle static listed types next (might be at the same index
            if (i >= 0 && i < typeOrderedItems.Count)
                tokenizerBuilder.Match(typeOrderedItems[i]);
        }

        // handle all remaining types (i.e. those the user didn't bother to define anywhere)
        // order by descending length, which is a rough approximate of complexity/match length (not exact)
        var unorderedRemainingTypes = allTokenTypes
            .Except(AppliedOrderTypes)
            .OrderByDescending(x => Templates[x].RenderedRegexString.Length);

        foreach (var type in unorderedRemainingTypes)
            tokenizerBuilder.Match(type);

        // Catch anything else with the default string pattern
        tokenizerBuilder.Match(typeof(DefaultUnmatchedString));

        Tokenizer = tokenizerBuilder.Build();
    }

    static TokenizerBuilder<Type> Match(this TokenizerBuilder<Type> tokenizerBuilder, Type tokenCaptureType)
    {
        if (AppliedOrderTypes.Contains(tokenCaptureType) || _invalidTypes.Contains(tokenCaptureType) || tokenCaptureType.IsAssignableTo(typeof(TokenUnitProperty)))
            return tokenizerBuilder;

        tokenizerBuilder.Match(Span.Regex(Templates[tokenCaptureType].RenderedRegexString), tokenCaptureType);
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
        InitializeTokenizer();

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
