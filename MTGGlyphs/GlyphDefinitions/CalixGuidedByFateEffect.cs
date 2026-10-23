namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Calix, Guided by Fate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Calix, Guided by Fate</exampledoc>
/// <examplecapture>you may create a token that's a copy of a nonlegendary enchantment you control</examplecapture>
[Dependent]
public class CalixGuidedByFateEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may create a token that's a copy of a nonlegendary enchantment you control"];
}
