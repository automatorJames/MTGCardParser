namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Castle Garenbrig, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Castle Garenbrig</exampledoc>
[Dependent]
public class CastleGarenbrigEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add six {g}"];
}
