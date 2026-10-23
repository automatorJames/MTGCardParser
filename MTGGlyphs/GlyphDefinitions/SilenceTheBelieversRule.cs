namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Silence the Believers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silence the Believers</exampledoc>
[TokenizationOrder(-1)]
public class SilenceTheBelieversRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile any number of target creatures and all auras attached to them"];
}
