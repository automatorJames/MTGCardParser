namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Triad of Fates, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Triad of Fates</exampledoc>
[Dependent]
public class TriadOfFatesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target creature that has a fate counter on it, then return it to the battlefield under its owner's control"];
}
