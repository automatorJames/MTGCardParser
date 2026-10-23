namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Teferi's Realm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi's Realm</exampledoc>
[Dependent]
public class TeferisRealmEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player chooses artifact, creature, land, or non-aura enchantment"];
}
