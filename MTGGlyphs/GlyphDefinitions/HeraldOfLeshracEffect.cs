namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Herald of Leshrac, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Herald of Leshrac</exampledoc>
[Dependent]
public class HeraldOfLeshracEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player gains control of each land they own that you control"];
}
