namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lightning Runner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lightning Runner</exampledoc>
[TokenizationOrder(-1)]
public class LightningRunnerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you pay, untap all creatures you control, and after this phase, there is an additional combat phase"];
}
