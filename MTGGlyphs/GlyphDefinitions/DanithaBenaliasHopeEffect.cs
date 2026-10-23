namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Danitha, Benalia's Hope, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Danitha, Benalia's Hope</exampledoc>
[Dependent]
public class DanithaBenaliasHopeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put an aura or equipment card from your hand or graveyard onto the battlefield attached to danitha"];
}
