namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Faith's Shield, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Faith's Shield</exampledoc>
/// <examplecapture>fateful hour — if you have 5 or less life, instead you and each permanent you control gain protection from the color of your choice until end of turn</examplecapture>
[TokenizationOrder(-1)]
public class FaithsShieldRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["fateful hour — if you have 5 or less life, instead you and each permanent you control gain protection from the color of your choice until end of turn"];
}
