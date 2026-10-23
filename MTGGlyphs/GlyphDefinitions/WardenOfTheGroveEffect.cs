namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Warden of the Grove, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Warden of the Grove</exampledoc>
[Dependent]
public class WardenOfTheGroveEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put x +1/+1 counters on the creature that entered or create an x/x white spirit creature token"];
}
