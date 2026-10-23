namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sanguine Praetor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sanguine Praetor</exampledoc>
[Dependent]
public class SanguinePraetorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each creature with the same mana value as the sacrificed creature"];
}
