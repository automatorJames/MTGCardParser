namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Baral and Kari Zev, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Baral and Kari Zev</exampledoc>
/// <examplecapture>you may cast a spell with lesser mana value that shares a card type with it from your hand without paying its mana cost</examplecapture>
[Dependent]
public class BaralAndKariZevEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a spell with lesser mana value that shares a card type with it from your hand without paying its mana cost"];
}
