namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sacred Ground, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sacred Ground</exampledoc>
/// <examplecapture>return that card to the battlefield</examplecapture>
[Dependent]
public class SacredGroundEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return that card to the battlefield"];
}
