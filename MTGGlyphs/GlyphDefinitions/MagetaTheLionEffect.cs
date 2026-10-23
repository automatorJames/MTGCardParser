namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mageta the Lion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mageta the Lion</exampledoc>
[Dependent]
public class MagetaTheLionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all creatures except for mageta"];
}
