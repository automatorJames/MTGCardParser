namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Reconnaissance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reconnaissance</exampledoc>
[Dependent]
public class ReconnaissanceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you activate during end of combat, the creature will untap after it deals combat damage"];
}
