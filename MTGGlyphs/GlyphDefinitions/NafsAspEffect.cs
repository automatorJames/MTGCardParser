namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nafs Asp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nafs Asp</exampledoc>
/// <examplecapture>that player loses 1 life at the beginning of their next draw step unless they pay {1} before that draw step</examplecapture>
[Dependent]
public class NafsAspEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player loses 1 life at the beginning of their next draw step unless they pay {1} before that draw step"];
}
