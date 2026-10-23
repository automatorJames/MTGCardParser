namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tergrid, God of Fright // Tergrid's Lantern, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tergrid, God of Fright // Tergrid's Lantern</exampledoc>
/// <examplecapture>you may put that card from a graveyard onto the battlefield under your control</examplecapture>
[Dependent]
public class TergridGodOfFrightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put that card from a graveyard onto the battlefield under your control"];
}
