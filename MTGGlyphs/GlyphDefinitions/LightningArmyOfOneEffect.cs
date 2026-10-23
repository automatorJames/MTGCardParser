namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lightning, Army of One, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lightning, Army of One</exampledoc>
/// <examplecapture>until your next turn, if a source would deal damage to that player or a permanent that player controls, it deals double that damage instead</examplecapture>
[Dependent]
public class LightningArmyOfOneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, if a source would deal damage to that player or a permanent that player controls, it deals double that damage instead"];
}
