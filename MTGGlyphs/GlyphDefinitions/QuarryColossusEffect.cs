namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quarry Colossus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quarry Colossus</exampledoc>
/// <examplecapture>put target creature into its owner's library just beneath the top x cards of that library, where x is the number of plains you control</examplecapture>
[Dependent]
public class QuarryColossusEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put target creature into its owner's library just beneath the top x cards of that library, where x is the number of plains you control"];
}
