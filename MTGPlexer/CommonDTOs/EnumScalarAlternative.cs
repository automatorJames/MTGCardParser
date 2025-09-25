namespace MTGPlexer.CommonDTOs;

public record EnumScalarAlternative
(
    Type EnumType,
    object EnumValue,
    string RegexString,
    int Ordinal
)
{
    // Matches a single space that is NOT exactly the [ ] token (i.e., not preceded by '[' and not followed by ']').
    static readonly Regex SpaceNotBracketToken = new Regex(@"(?<!\[) (?!\])", RegexOptions.Compiled);
    public string DisplayName { get; } = EnumValue.ToString();
    public Regex ItemRegex { get; } = new Regex("^" + SpaceNotBracketToken.Replace(RegexString, "") + "$", RegexOptions.Compiled);

    public override string ToString() => RegexString;
}