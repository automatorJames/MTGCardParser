namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Thorin, Mountain-king, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thorin, Mountain-king</exampledoc>
[Dependent]
public class ThorinMountainKing2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["attach any number of target equipment you control to target creature you control"];
}
