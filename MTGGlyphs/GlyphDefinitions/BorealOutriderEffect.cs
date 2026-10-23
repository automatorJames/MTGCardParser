namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Boreal Outrider, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Boreal Outrider</exampledoc>
/// <examplecapture>if {s} of any of that spell's colors was spent to cast it, that creature enters with an additional +1/+1 counter on it</examplecapture>
[Dependent]
public class BorealOutriderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if {s} of any of that spell's colors was spent to cast it, that creature enters with an additional +1/+1 counter on it"];
}
