namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bazaar of Wonders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bazaar of Wonders</exampledoc>
/// <examplecapture>counter it if a card with the same name is in a graveyard or a nontoken permanent with the same name is on the battlefield</examplecapture>
[Dependent]
public class BazaarOfWondersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter it if a card with the same name is in a graveyard or a nontoken permanent with the same name is on the battlefield"];
}
