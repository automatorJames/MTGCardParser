namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Godo, Bandit Warlord, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Godo, Bandit Warlord</exampledoc>
[Dependent]
public class GodoBanditWarlordEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["untap it and all samurai you control"];
}
