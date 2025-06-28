namespace MTGCardParser;

public class RegexPatternGetter
{
    //readonly Dictionary<MtgToken, string> _patterns = new();
    //
    //public string this[MtgToken token] => _patterns.ContainsKey(token) ? _patterns[token] : null;
    //
    //public RegexPatternGetter()   
    //{
    //    _patterns[MtgToken.LifeChangeQuantity] = @"\b(?<lifeverb>gain|gains|lose|loses)\s+(?<amount>\d+)\s+life\b";
    //    _patterns[MtgToken.CardQuantityChange] = @"\b(?<verb>discard|discards|draw|draws)\s+(?<amount>a|\d+|one|two|three|four|five|six|seven|eight|nine|ten)\s+card(s)?\b";
    //    _patterns[MtgToken.PlusMinusPowerToughness] = @"\+(?<power>[0-9]+)/\+(?<toughness>[0-9]+)";
    //    _patterns[MtgToken.ManaCost] = @"(?<manacost>(?:\{(?:[0-9]+|[wubrgxyzc∞]|w/u|w/b|u/b|u/r|b/r|b/g|r/g|r/w|g/w|g/u|2/w|2/u|2/b|2/r|2/g|p|s)\})+)";
    //    _patterns[MtgToken.DealDamageAmount] = @"\bdeal(s)?\s+(?<amount>\d+)\s+damage\s+to\b";
    //    _patterns[MtgToken.NextDamageAmount] = @"\bnext\s+(?<amount>\d+)\s+damage\b";
    //    _patterns[MtgToken.AddMana] = @"\badd\s+(?<mana>(?:\{[^\}]+\})+(?:\s+or\s+\{[^\}]+\})?)";
    //    _patterns[MtgToken.PayCost] = @"\b(?<verb>pay|pays|paid)\s+(?<cost>(?:\{[^\}]+\})+(?:\s+or\s+\{[^\}]+\})?|\d+\s+life)(?=(?:\s|[.,]|$))";
    //    _patterns[MtgToken.UponPlayerPhase] = @"\bat the\s+(?<when>beginning|end)\s+of\s+(?<whose>your opponent's|your|each)\b";
    //    _patterns[MtgToken.Who] = @"\b(?<who>you|your opponent|any opponent)\b";
    //    _patterns[MtgToken.Quantity] = @"\b(?<upto>up to )?(?<quantity>a|one|two|three|four|five|six|seven|eight|nine|ten|\d+)\b";
    //    //_patterns[MtgToken.CardType] = $@"\b(?<isEnchanted>enchanted )?(?<cardtype>{GetAlternation(Lists.CardTypes)})\b";
    //    //_patterns[MtgToken.EnchantCardType] = $@"\benchant (?<cardtype>{GetAlternation(Lists.CardTypes)})\b";
    //    _patterns[MtgToken.Color] = Lists.Colors.GetAlternation("color");
    //    _patterns[MtgToken.Keyword] = Lists.Keywords.GetAlternation("keyword");
    //    _patterns[MtgToken.SubType] = Lists.Subtypes.GetAlternation("subtype");
    //    //_patterns[MtgToken.GamePhase] = GetGamePhase();
    //}

    //string GetGamePhase() 
    //{
    //    var playerSegment = $@"(?<player>{StringExtensions.GetAlternation(Lists.PlayerIdentifiers)})";
    //    var phaseSegment = $@"(?<phase>{GetAlternation(Lists.GamePhases)})";
    //    //$@"\b(?<phasePart>beginning|end)?(?: of)?\s*(?:(?<player>{GetAlternation(Lists.PlayerIdentifiers)})|\b(?:the|an?)\b)?\s*(?<phase>{GetAlternation(Lists.GamePhases)})\b";
    //    return $@"\b(?<phasePart>beginning|end)?(?: of)?\s*(?:{playerSegment}|\b(?:the|an?)\b)?\s*{phaseSegment}\b";
    //}
}

