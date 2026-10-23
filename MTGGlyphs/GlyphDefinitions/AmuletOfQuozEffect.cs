namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Amulet of Quoz, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Amulet of Quoz</exampledoc>
[Dependent]
public class AmuletOfQuozEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent may ante the top card of their library"];
}
