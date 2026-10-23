namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mordenkainen, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mordenkainen</exampledoc>
[Dependent]
public class MordenkainenEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange your hand and library, then shuffle"];
}
