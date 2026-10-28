namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mjölnir, Hammer of Thor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mjölnir, Hammer of Thor</exampledoc>
[TokenizationOrder(-1)]
public class MjolnirHammerOfThorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["double all damage equipped creature would deal"];
}
