namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tectonic Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tectonic Giant</exampledoc>
[TokenizationOrder(-1)]
public class TectonicGiantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature attacks or becomes the target of a spell an opponent controls, choose one —"];
}
