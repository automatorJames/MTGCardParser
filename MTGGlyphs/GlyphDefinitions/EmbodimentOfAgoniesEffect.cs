namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Embodiment of Agonies, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Embodiment of Agonies</exampledoc>
[Dependent]
public class EmbodimentOfAgoniesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, {2}{b} and {1}{b}{b} are different mana costs"];
}
