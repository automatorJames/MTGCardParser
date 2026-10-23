namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cemetery Prowler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cemetery Prowler</exampledoc>
[TokenizationOrder(-1)]
public class CemeteryProwlerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells you cast cost {1} less to cast for each card type they share with cards exiled with this creature"];
}
