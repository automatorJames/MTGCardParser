namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mayhem's reminder text: "you may cast this card from your graveyard for {3}{b} if you discarded it this turn" - the cost is the mayhem cost.</summary>
/// <exampledoc>Sandman's Quicksand</exampledoc>
/// <examplecapture>you may cast this card from your graveyard for {3}{b} if you discarded it this turn</examplecapture>
public class MayhemRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard for", Prop(Cost), "if you discarded it this turn"];

    public ManaCost Cost { get; set; }
}
