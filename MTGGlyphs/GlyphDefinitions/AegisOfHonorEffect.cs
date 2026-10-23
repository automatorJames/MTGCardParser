namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Aegis of Honor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aegis of Honor</exampledoc>
/// <examplecapture>the next time an instant or sorcery spell would deal damage to you this turn, that spell deals that damage to its controller instead</examplecapture>
[Dependent]
public class AegisOfHonorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time an instant or sorcery spell would deal damage to you this turn, that spell deals that damage to its controller instead"];
}
