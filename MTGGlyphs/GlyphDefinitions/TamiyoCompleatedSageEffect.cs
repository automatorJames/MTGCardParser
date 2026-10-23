namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tamiyo, Compleated Sage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tamiyo, Compleated Sage</exampledoc>
[Dependent]
public class TamiyoCompleatedSageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create tamiyo's notebook, a legendary colorless book artifact token with \"spells you cast cost {2} less to cast\" and \"{t}: draw a card"];
}
