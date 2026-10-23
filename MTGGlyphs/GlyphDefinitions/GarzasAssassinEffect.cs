namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Garza's Assassin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Garza's Assassin</exampledoc>
[Dependent]
public class GarzasAssassinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when another creature is put into your graveyard from the battlefield, you may pay half your life, rounded up"];
}
