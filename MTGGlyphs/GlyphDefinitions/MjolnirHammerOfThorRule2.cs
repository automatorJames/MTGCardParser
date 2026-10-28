namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mjölnir, Hammer of Thor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mjölnir, Hammer of Thor</exampledoc>
[TokenizationOrder(-1)]
public class MjolnirHammerOfThorRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equip worthy {1} (a creature is worthy if it's a legendary non-villain that's red and/or white"];
}
