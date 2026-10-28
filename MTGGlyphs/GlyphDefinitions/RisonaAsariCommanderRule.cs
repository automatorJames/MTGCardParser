namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Risona, Asari Commander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Risona, Asari Commander</exampledoc>
[TokenizationOrder(-1)]
public class RisonaAsariCommanderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever combat damage is dealt to you, remove an indestructible counter from risona"];
}
