namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gomazoa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gomazoa</exampledoc>
[Dependent]
public class GomazoaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this creature and each creature it's blocking on top of their owners' libraries, then those players shuffle"];
}
