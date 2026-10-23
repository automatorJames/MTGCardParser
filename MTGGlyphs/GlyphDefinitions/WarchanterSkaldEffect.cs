namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Warchanter Skald, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Warchanter Skald</exampledoc>
[Dependent]
public class WarchanterSkaldEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's enchanted or equipped, create a 2/1 red dwarf berserker creature token"];
}
