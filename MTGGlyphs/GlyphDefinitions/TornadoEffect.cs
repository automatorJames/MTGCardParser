namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tornado, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tornado</exampledoc>
[Dependent]
public class TornadoEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target permanent and put a velocity counter on this enchantment"];
}
