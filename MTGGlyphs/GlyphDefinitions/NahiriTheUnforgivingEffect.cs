namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nahiri, the Unforgiving, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nahiri, the Unforgiving</exampledoc>
[Dependent]
public class NahiriTheUnforgivingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target creature or equipment card with mana value less than nahiri's loyalty from your graveyard"];
}
