namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Terminal Velocity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Terminal Velocity</exampledoc>
/// <examplecapture>that permanent gains haste, "when this permanent leaves the battlefield, it deals damage equal to its mana value to each creature," and "at the beginning of your end step, sacrifice this permanent</examplecapture>
[TokenizationOrder(-1)]
public class TerminalVelocityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that permanent gains haste, \"when this permanent leaves the battlefield, it deals damage equal to its mana value to each creature,\" and \"at the beginning of your end step, sacrifice this permanent"];
}
