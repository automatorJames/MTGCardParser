namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Renegade's Getaway, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Renegade's Getaway</exampledoc>
[Dependent]
public class RenegadesGetawayEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["effects that say \"destroy\" don't destroy a permanent with indestructible, and if it's a creature, it can't be destroyed by damage"];
}
