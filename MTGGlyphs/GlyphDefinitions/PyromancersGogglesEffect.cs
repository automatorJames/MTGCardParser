namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pyromancer's Goggles, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pyromancer's Goggles</exampledoc>
[Dependent]
public class PyromancersGogglesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy that spell and you may choose new targets for the copy"];
}
