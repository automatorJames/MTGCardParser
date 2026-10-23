namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Savage Punch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Savage Punch</exampledoc>
[TokenizationOrder(-1)]
public class SavagePunchRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ferocious — the creature you control gets +2/+2 until end of turn before it fights if you control a creature with power 4 or greater"];
}
