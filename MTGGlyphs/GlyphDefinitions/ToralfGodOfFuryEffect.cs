namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Toralf, God of Fury // Toralf's Hammer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Toralf, God of Fury // Toralf's Hammer</exampledoc>
/// <examplecapture>toralf deals damage equal to the excess to any target other than that permanent</examplecapture>
[Dependent]
public class ToralfGodOfFuryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["toralf deals damage equal to the excess to any target other than that permanent"];
}
