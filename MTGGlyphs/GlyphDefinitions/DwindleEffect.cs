namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dwindle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dwindle</exampledoc>
[Dependent]
public class DwindleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the attacking creature remains blocked"];
}
