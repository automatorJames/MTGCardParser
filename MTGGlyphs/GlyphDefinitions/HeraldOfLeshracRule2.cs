namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Herald of Leshrac, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Herald of Leshrac</exampledoc>
[TokenizationOrder(-1)]
public class HeraldOfLeshracRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +1/+1 for each land you control but don't own"];
}
