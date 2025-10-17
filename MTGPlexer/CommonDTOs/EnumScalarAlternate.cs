namespace MTGPlexer.CommonDTOs;

public class EnumScalarAlternate
{
    // Matches a single space that is NOT exactly the [ ] token (i.e., not preceded by '[' and not followed by ']').
    static readonly Regex _spaceNotBracketToken = new Regex(@"(?<!\[) (?!\])", RegexOptions.Compiled);

    public Type EnumType { get; }
    public object EnumValue { get; }
    public List<string> Synonyms { get; }
    public int Ordinal { get; }
    public string DisplayName { get; }
    public string RegexString { get; }
    public Regex ItemRegex { get; }

    public EnumScalarAlternate(Type enumType, object enumValue, List<string> synonyms, int ordinal)
    {
        EnumType = enumType;
        EnumValue = enumValue;
        Ordinal = ordinal;

        if (synonyms.Count == 1 && enumType.IsDefined(typeof(OptionalPluralAttribute)))
        {
            RegexString = synonyms[0].AddOptionalPluralization();
            Synonyms = [synonyms[0], synonyms[0].AddOptionalPluralization()];
        }
        else
        {
            RegexString = string.Join(" | ", synonyms);
            Synonyms = synonyms;
        }

        ItemRegex = new Regex("^" + _spaceNotBracketToken.Replace(RegexString, "") + "$", RegexOptions.Compiled);
        DisplayName = EnumValue.ToString();
    }

    public override string ToString() => RegexString;
}