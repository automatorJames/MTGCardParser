namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Glass Asp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glass Asp</exampledoc>
/// <examplecapture>that player loses 2 life at the beginning of their next draw step unless they pay {2} before that step</examplecapture>
[Dependent]
public class GlassAspEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player loses 2 life at the beginning of their next draw step unless they pay {2} before that step"];
}
