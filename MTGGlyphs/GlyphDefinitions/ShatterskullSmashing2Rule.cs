namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shatterskull Smashing // Shatterskull, the Hammer Pass, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shatterskull Smashing // Shatterskull, the Hammer Pass</exampledoc>
[TokenizationOrder(-1)]
public class ShatterskullSmashing2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shatterskull smashing deals x damage divided as you choose among up to two target creatures and/or planeswalkers"];
}
