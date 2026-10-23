namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Apocalypse Chime, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Apocalypse Chime</exampledoc>
[Dependent]
public class ApocalypseChimeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all nontoken permanents with a name originally printed in the homelands expansion"];
}
