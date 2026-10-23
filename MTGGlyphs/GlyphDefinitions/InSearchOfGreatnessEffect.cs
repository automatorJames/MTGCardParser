namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of In Search of Greatness, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>In Search of Greatness</exampledoc>
/// <examplecapture>you may cast a permanent spell from your hand with mana value equal to 1 plus the greatest mana value among other permanents you control without paying its mana cost</examplecapture>
[Dependent]
public class InSearchOfGreatnessEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a permanent spell from your hand with mana value equal to 1 plus the greatest mana value among other permanents you control without paying its mana cost"];
}
