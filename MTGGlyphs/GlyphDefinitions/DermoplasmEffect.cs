namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dermoplasm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dermoplasm</exampledoc>
/// <examplecapture>you may put a creature card with a morph ability from your hand onto the battlefield face up</examplecapture>
[Dependent]
public class DermoplasmEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a creature card with a morph ability from your hand onto the battlefield face up"];
}
