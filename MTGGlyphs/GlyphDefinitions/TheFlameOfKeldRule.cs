namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Flame of Keld, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Flame of Keld</exampledoc>
/// <examplecapture>iii — if a red source you control would deal damage to a permanent or player this turn, it deals that much damage plus 2 to that permanent or player instead</examplecapture>
[TokenizationOrder(-1)]
public class TheFlameOfKeldRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["iii — if a red source you control would deal damage to a permanent or player this turn, it deals that much damage plus 2 to that permanent or player instead"];
}
