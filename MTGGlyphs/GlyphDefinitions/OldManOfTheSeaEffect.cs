namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Old Man of the Sea, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Old Man of the Sea</exampledoc>
/// <examplecapture>gain control of target creature with power less than or equal to this creature's power for as long as this creature remains tapped and that creature's power remains less than or equal to this creature's power</examplecapture>
[Dependent]
public class OldManOfTheSeaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target creature with power less than or equal to this creature's power for as long as this creature remains tapped and that creature's power remains less than or equal to this creature's power"];
}
