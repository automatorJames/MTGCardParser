namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Laccolith Rig, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Laccolith Rig</exampledoc>
[TokenizationOrder(-1)]
public class LaccolithRigRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, the first creature assigns no combat damage this turn"];
}
