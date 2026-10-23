namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kaya's Ghostform, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya's Ghostform</exampledoc>
[Dependent]
public class KayasGhostformEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return that card to the battlefield under your control"];
}
