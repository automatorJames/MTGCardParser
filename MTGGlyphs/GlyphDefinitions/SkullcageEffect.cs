namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Skullcage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skullcage</exampledoc>
/// <examplecapture>this artifact deals 2 damage to that player unless they have exactly three or exactly four cards in hand</examplecapture>
[Dependent]
public class SkullcageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this artifact deals 2 damage to that player unless they have exactly three or exactly four cards in hand"];
}
