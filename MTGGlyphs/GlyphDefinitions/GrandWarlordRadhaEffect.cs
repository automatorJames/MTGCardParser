namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Grand Warlord Radha, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grand Warlord Radha</exampledoc>
[Dependent]
public class GrandWarlordRadhaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add that much mana in any combination of {r} and/or {g}"];
}
