namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Whipkeeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whipkeeper</exampledoc>
[Dependent]
public class WhipkeeperEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature deals damage to target creature equal to the damage already dealt to it this turn"];
}
