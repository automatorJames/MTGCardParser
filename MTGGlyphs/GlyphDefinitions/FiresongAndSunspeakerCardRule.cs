namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Firesong and Sunspeaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firesong and Sunspeaker</exampledoc>
[TokenizationOrder(-1)]
public class FiresongAndSunspeakerCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a white instant or sorcery spell causes you to gain life,", Nib.This, "deals 3 damage to target creature or player"];
}
