namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kazuul, Tyrant of the Cliffs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kazuul, Tyrant of the Cliffs</exampledoc>
/// <examplecapture>if you're the defending player, create a 3/3 red ogre creature token unless that creature's controller pays {3}</examplecapture>
[Dependent]
public class KazuulTyrantOfTheCliffsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you're the defending player, create a 3/3 red ogre creature token unless that creature's controller pays {3}"];
}
