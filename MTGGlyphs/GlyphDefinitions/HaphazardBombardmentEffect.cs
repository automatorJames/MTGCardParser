namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Haphazard Bombardment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haphazard Bombardment</exampledoc>
/// <examplecapture>if two or more permanents you don't control have an aim counter on them, destroy one of those permanents at random</examplecapture>
[Dependent]
public class HaphazardBombardmentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more permanents you don't control have an aim counter on them, destroy one of those permanents at random"];
}
