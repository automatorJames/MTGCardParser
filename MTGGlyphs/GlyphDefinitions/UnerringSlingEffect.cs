namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Unerring Sling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Unerring Sling</exampledoc>
/// <examplecapture>this artifact deals damage equal to the tapped creature's power to target attacking or blocking creature with flying</examplecapture>
[Dependent]
public class UnerringSlingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this artifact deals damage equal to the tapped creature's power to target attacking or blocking creature with flying"];
}
