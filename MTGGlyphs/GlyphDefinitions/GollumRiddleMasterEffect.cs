namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gollum, Riddle Master, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gollum, Riddle Master</exampledoc>
[Dependent]
public class GollumRiddleMasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose one that hasn't been chosen —"];
}
