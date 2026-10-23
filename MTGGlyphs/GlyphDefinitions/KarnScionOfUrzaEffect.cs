namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Karn, Scion of Urza, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karn, Scion of Urza</exampledoc>
[Dependent]
public class KarnScionOfUrzaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a card you own with a silver counter on it from exile into your hand"];
}
