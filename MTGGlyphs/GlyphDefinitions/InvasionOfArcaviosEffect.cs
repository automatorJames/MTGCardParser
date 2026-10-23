namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Invasion of Arcavios // Invocation of the Founders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasion of Arcavios // Invocation of the Founders</exampledoc>
/// <examplecapture>search your library, graveyard, and/or outside the game for an instant or sorcery card you own, reveal it, and put it into your hand</examplecapture>
[Dependent]
public class InvasionOfArcaviosEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library, graveyard, and/or outside the game for an instant or sorcery card you own, reveal it, and put it into your hand"];
}
