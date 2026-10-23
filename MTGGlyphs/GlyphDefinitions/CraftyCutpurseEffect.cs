namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Crafty Cutpurse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crafty Cutpurse</exampledoc>
[Dependent]
public class CraftyCutpurseEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each token that would be created under an opponent's control this turn is created under your control instead"];
}
