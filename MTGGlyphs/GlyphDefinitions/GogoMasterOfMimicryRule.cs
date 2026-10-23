namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gogo, Master of Mimicry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gogo, Master of Mimicry</exampledoc>
[TokenizationOrder(-1)]
public class GogoMasterOfMimicryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this ability can't be copied and x can't be 0"];
}
