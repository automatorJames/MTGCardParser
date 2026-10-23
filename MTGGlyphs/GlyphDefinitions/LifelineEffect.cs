namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lifeline, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lifeline</exampledoc>
/// <examplecapture>if another creature is on the battlefield, return the first card to the battlefield under its owner's control at the beginning of the next end step</examplecapture>
[Dependent]
public class LifelineEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if another creature is on the battlefield, return the first card to the battlefield under its owner's control at the beginning of the next end step"];
}
