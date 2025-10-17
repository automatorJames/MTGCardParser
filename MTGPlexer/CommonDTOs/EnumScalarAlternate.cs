namespace MTGPlexer.CommonDTOs;

public class EnumScalarAlternate
{
    // Matches a single space that is NOT exactly the [ ] token (i.e., not preceded by '[' and not followed by ']').
    static readonly Regex _spaceNotBracketToken = new Regex(@"(?<!\[) (?!\])", RegexOptions.Compiled);

    public object EnumValue { get; }
    public List<string> Synonyms { get; } = [];
    public List<string> SpaceEscapedSynonyms { get; } = [];
    public string DisplayName { get; }
    public string RegexString { get; }
    public Regex ItemRegex { get; }

    public EnumScalarAlternate(Type enumType, object enumValue)
    {
        EnumValue = enumValue;
        List<string> synonyms = [];

        var enumAsString = enumValue.ToString();
        var regexPatternAttribute = enumType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

        if (regexPatternAttribute != null)
            synonyms.AddRange(regexPatternAttribute.Patterns);
        else
            synonyms.Add(enumAsString.ToFriendlyCase());

        if (synonyms.Count == 1 && enumType.IsDefined(typeof(OptionalPluralAttribute)))
        {
            RegexString = synonyms[0].AddPluralization(makeOptional: true);
            synonyms = [synonyms[0], synonyms[0].AddPluralization(makeOptional: false)];
        }
        else
            RegexString = string.Join(" | ", synonyms);

        Synonyms = synonyms;
        SpaceEscapedSynonyms = synonyms.Select(x => x.Replace(" ", "[ ]")).ToList();
        ItemRegex = new Regex("^" + _spaceNotBracketToken.Replace(RegexString, "") + "$", RegexOptions.Compiled);
        DisplayName = EnumValue.ToString();
    }

    public override string ToString() => RegexString;
}