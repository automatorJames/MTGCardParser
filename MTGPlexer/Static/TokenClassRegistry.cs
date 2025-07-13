namespace MTGPlexer.Static;

public static partial class TokenTypeRegistry
{
    static HashSet<Type> _invalidTypes = [];

    public static Dictionary<Type, RegexTemplate> TokenTemplates { get; set; } = new();
    public static Dictionary<Type, Dictionary<object, Regex>> EnumRegexes { get; set; } = new();
    public static Dictionary<Type, DeterministicPalette> TypeColorPalettes { get; set; } = new();
    public static Tokenizer<Type> Tokenizer { get; set; }
    public static HashSet<Type> AppliedOrderTypes { get; set; } = new();

    static TokenTypeRegistry()
    {
        ValidateAndRegisterTypes();
        InitializeTokenizer();
    }

    static void ValidateAndRegisterTypes()
    {
        foreach (var type in GetTokenCaptureTypes())
            SetTypeTemplate(type);
    }

    public static RegexTemplate GetTypeTemplate(Type type)
    {
        if (!TokenTemplates.ContainsKey(type))
            SetTypeTemplate(type);

        return TokenTemplates[type];
    }

    public static void SetTypeTemplate(Type type)
    {
        var instance = (TokenUnit)Activator.CreateInstance(type);

        if (!instance.ValidateStructure())
        {
            _invalidTypes.Add(type);
            return;
        }

        TokenTemplates[type] = instance.Template;
        var propCaptureSegments = instance.Template.PropCaptureSegments;

        var unregisteredEnums = propCaptureSegments
            .OfType<EnumRegexProp>()
            .Where(x => !EnumRegexes.ContainsKey(x.RegexPropInfo.UnderlyingType));

        foreach (var enumEntry in unregisteredEnums)
            EnumRegexes[enumEntry.RegexPropInfo.UnderlyingType] = enumEntry.EnumMemberRegexes;

        TypeColorPalettes[type] = new(type);
    }

    public static TokenUnit HydrateFromToken(Token<Type> token) 
        => TokenUnit.InstantiateFromMatchString(token.Kind, token.Span);

    static List<Type> GetTokenCaptureTypes() =>
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass && !t.IsAbstract
                && typeof(TokenUnit).IsAssignableFrom(t))
            .ToList();

}

