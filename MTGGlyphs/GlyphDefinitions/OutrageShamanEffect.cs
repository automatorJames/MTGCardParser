namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Outrage Shaman, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Outrage Shaman</exampledoc>
/// <examplecapture>it deals damage to target creature equal to the number of red mana symbols in the mana costs of permanents you control</examplecapture>
[Dependent]
public class OutrageShamanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals damage to target creature equal to the number of red mana symbols in the mana costs of permanents you control"];
}
