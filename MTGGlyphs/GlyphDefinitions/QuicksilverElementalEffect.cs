namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quicksilver Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quicksilver Elemental</exampledoc>
[Dependent]
public class QuicksilverElementalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any of the abilities use that creature's name, use this creature's name instead"];
}
