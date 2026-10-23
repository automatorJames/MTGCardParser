namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nullstone Gargoyle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nullstone Gargoyle</exampledoc>
[Dependent]
public class NullstoneGargoyleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter that spell"];
}
