namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gilt-Leaf Winnower, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gilt-Leaf Winnower</exampledoc>
[Dependent]
public class GiltLeafWinnowerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may destroy target non-elf creature whose power and toughness aren't equal"];
}
