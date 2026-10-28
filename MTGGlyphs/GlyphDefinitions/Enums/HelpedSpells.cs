namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which spells convoke's and improvise's reminder says permanents can help cast: "this spell", or "those spells" where the keyword is granted - "your creatures can help cast this spell".</summary>
public enum HelpedSpells
{
    [RegexPattern("this spell")]
    ThisSpell,

    [RegexPattern("those spells")]
    ThoseSpells
}
