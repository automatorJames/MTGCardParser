namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Pride of Hull Clade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Pride of Hull Clade</exampledoc>
/// <examplecapture>until end of turn, target creature you control gets +1/+0, gains "whenever this creature deals combat damage to a player, draw cards equal to its toughness," and can attack as though it didn't have defender</examplecapture>
[Dependent]
public class ThePrideOfHullCladeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, target creature you control gets +1/+0, gains \"whenever this creature deals combat damage to a player, draw cards equal to its toughness,\" and can attack as though it didn't have defender"];
}
