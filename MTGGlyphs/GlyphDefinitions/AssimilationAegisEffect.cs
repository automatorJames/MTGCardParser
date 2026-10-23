namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Assimilation Aegis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Assimilation Aegis</exampledoc>
/// <examplecapture>for as long as this equipment remains attached to it, that creature becomes a copy of a creature card exiled with this equipment</examplecapture>
[Dependent]
public class AssimilationAegisEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for as long as this equipment remains attached to it, that creature becomes a copy of a creature card exiled with this equipment"];
}
