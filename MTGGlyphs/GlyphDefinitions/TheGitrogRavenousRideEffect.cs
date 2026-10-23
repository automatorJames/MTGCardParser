namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Gitrog, Ravenous Ride, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Gitrog, Ravenous Ride</exampledoc>
[Dependent]
public class TheGitrogRavenousRideEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may sacrifice a creature that saddled it this turn"];
}
