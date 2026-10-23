namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Helm of Kaldra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Helm of Kaldra</exampledoc>
[Dependent]
public class HelmOfKaldraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control equipment named", Nib.This, ", sword of kaldra, and shield of kaldra, create kaldra, a legendary 4/4 colorless avatar creature token"];
}
