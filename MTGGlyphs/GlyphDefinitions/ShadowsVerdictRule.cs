namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shadows' Verdict, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shadows' Verdict</exampledoc>
/// <examplecapture>exile all creatures and planeswalkers with mana value 3 or less from the battlefield and all creature and planeswalker cards with mana value 3 or less from all graveyards</examplecapture>
[TokenizationOrder(-1)]
public class ShadowsVerdictRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all creatures and planeswalkers with mana value 3 or less from the battlefield and all creature and planeswalker cards with mana value 3 or less from all graveyards"];
}
