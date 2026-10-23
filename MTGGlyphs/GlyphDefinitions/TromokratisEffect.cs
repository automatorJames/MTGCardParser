namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tromokratis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tromokratis</exampledoc>
[Dependent]
public class TromokratisEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any creature that player controls doesn't block this creature, it can't be blocked"];
}
