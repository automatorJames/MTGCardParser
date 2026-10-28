namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Firion, Wild Rose Warrior, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firion, Wild Rose Warrior</exampledoc>
[TokenizationOrder(-1)]
public class FirionWildRoseWarriorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a nontoken equipment you control enters, create a token that's a copy of it, except it has \"this equipment's equip abilities cost {2} less to activate"];
}
