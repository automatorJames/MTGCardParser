namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Soulfire Grand Master, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soulfire Grand Master</exampledoc>
/// <examplecapture>the next time you cast an instant or sorcery spell from your hand this turn, put that card into your hand instead of into your graveyard as it resolves</examplecapture>
[Dependent]
public class SoulfireGrandMasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time you cast an instant or sorcery spell from your hand this turn, put that card into your hand instead of into your graveyard as it resolves"];
}
