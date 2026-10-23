namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of A Killer Among Us, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>A Killer Among Us</exampledoc>
/// <examplecapture>if target attacking creature token is the chosen type, put three +1/+1 counters on it and it gains deathtouch until end of turn</examplecapture>
[Dependent]
public class AKillerAmongUsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if target attacking creature token is the chosen type, put three +1/+1 counters on it and it gains deathtouch until end of turn"];
}
