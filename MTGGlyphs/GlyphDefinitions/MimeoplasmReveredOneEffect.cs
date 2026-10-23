namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mimeoplasm, Revered One, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mimeoplasm, Revered One</exampledoc>
[Dependent]
public class MimeoplasmReveredOneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mimeoplasm becomes a copy of target creature card exiled with it, except it's 0/0 and has this ability"];
}
