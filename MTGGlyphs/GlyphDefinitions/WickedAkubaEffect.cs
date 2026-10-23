namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wicked Akuba, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wicked Akuba</exampledoc>
[Dependent]
public class WickedAkubaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player dealt damage by this creature this turn loses 1 life"];
}
