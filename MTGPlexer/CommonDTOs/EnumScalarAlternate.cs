namespace MTGPlexer.CommonDTOs;

public class EnumScalarAlternate
{
    // Matches a single space that is NOT exactly the [ ] token (i.e., not preceded by '[' and not followed by ']').
    static readonly Regex _spaceNotBracketToken = new Regex(@"(?<!\[) (?!\])", RegexOptions.Compiled);

    public Type EnumType { get; }
    public object EnumValue { get; }
    public List<string> Synonyms { get; } = [];
    public List<string> SpaceEscapedSynonyms { get; } = [];
    public int Ordinal { get; }
    public string DisplayName { get; }
    public string RegexString { get; }
    public Regex ItemRegex { get; }

    public EnumScalarAlternate(Type enumType, object enumValue, int ordinal)
    {
        EnumType = enumType;
        EnumValue = enumValue;
        Ordinal = ordinal;

        var enumAsString = enumValue.ToString();
        var regexPatternAttribute = enumType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

        if (regexPatternAttribute != null)
            Synonyms.AddRange(regexPatternAttribute.Patterns);
        else
            Synonyms.Add(enumAsString.ToFriendlyCase());

        if (Synonyms.Count == 1 && enumType.IsDefined(typeof(OptionalPluralAttribute)))
        {
            RegexString = Synonyms[0].AddPluralization(makeOptional: true);
            Synonyms = [Synonyms[0], Synonyms[0].AddPluralization(makeOptional: false)];
        }
        else
            RegexString = string.Join(" | ", Synonyms);

        SpaceEscapedSynonyms = Synonyms.Select(x => x.Replace(" ", "[ ]")).ToList();

        ItemRegex = new Regex("^" + _spaceNotBracketToken.Replace(RegexString, "") + "$", RegexOptions.Compiled);
        DisplayName = EnumValue.ToString();
    }

    public override string ToString() => RegexString;
}