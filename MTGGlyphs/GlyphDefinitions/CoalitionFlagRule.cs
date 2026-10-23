namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Coalition Flag, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coalition Flag</exampledoc>
/// <examplecapture>while an opponent is choosing targets as part of casting a spell they control or activating an ability they control, that player must choose at least one flagbearer on the battlefield if able</examplecapture>
[TokenizationOrder(-1)]
public class CoalitionFlagRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["while an opponent is choosing targets as part of casting a spell they control or activating an ability they control, that player must choose at least one flagbearer on the battlefield if able"];
}
