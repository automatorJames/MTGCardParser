namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dancing Sword, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dancing Sword</exampledoc>
[Dependent]
public class DancingSwordEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this equipment become a 2/1 construct artifact creature with flying and ward {1}"];
}
