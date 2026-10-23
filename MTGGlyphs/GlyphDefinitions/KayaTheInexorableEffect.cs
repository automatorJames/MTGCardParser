namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kaya the Inexorable, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya the Inexorable</exampledoc>
/// <examplecapture>you get an emblem with "at the beginning of your upkeep, you may cast a legendary spell from your hand, from your graveyard, or from among cards you own in exile without paying its mana cost</examplecapture>
[Dependent]
public class KayaTheInexorableEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"at the beginning of your upkeep, you may cast a legendary spell from your hand, from your graveyard, or from among cards you own in exile without paying its mana cost"];
}
