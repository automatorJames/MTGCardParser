namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nivmagus Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nivmagus Elemental</exampledoc>
[TokenizationOrder(-1)]
public class NivmagusElementalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile an instant or sorcery spell you control: put two +1/+1 counters on this creature"];
}
