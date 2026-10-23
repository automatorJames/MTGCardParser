namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Exiled Doomsayer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Exiled Doomsayer</exampledoc>
[Dependent]
public class ExiledDoomsayerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this doesn't affect the cost to cast creature spells face down"];
}
