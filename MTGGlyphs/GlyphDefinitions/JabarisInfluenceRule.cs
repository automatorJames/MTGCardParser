namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jabari's Influence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jabari's Influence</exampledoc>
[TokenizationOrder(-1)]
public class JabarisInfluenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target nonartifact, nonblack creature that attacked you this turn and put a -1/-0 counter on it"];
}
