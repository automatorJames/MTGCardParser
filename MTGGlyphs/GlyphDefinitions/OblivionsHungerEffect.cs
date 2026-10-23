namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Oblivion's Hunger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oblivion's Hunger</exampledoc>
[Dependent]
public class OblivionsHungerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage and effects that say \"destroy\" don't destroy the creature"];
}
