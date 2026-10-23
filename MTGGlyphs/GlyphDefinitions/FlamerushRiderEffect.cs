namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Flamerush Rider, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flamerush Rider</exampledoc>
[Dependent]
public class FlamerushRiderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a token that's a copy of another target attacking creature and that's tapped and attacking"];
}
