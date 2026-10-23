namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tatsumasa, the Dragon's Fang, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tatsumasa, the Dragon's Fang</exampledoc>
[Dependent]
public class TatsumasaTheDragonsFangEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 5/5 blue dragon spirit creature token with flying"];
}
