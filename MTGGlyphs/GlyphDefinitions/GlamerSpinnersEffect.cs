namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Glamer Spinners, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Glamer Spinners</exampledoc>
[Dependent]
public class GlamerSpinnersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["attach all auras enchanting target permanent to another permanent with the same controller"];
}
