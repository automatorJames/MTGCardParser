namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Emrakul's Evangel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Emrakul's Evangel</exampledoc>
/// <examplecapture>create a 3/2 colorless eldrazi horror creature token for each creature sacrificed this way</examplecapture>
[Dependent]
public class EmrakulsEvangelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 3/2 colorless eldrazi horror creature token for each creature sacrificed this way"];
}
