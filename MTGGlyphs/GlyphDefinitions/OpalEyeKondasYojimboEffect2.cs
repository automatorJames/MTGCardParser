namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Opal-Eye, Konda's Yojimbo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Opal-Eye, Konda's Yojimbo</exampledoc>
[Dependent]
public class OpalEyeKondasYojimboEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time a source of your choice would deal damage this turn, that damage is dealt to opal-eye instead"];
}
