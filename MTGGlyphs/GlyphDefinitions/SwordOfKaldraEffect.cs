namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sword of Kaldra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sword of Kaldra</exampledoc>
[Dependent]
public class SwordOfKaldraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile it only if it's still on the battlefield"];
}
