namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Skyship Plunderer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skyship Plunderer</exampledoc>
/// <examplecapture>for each kind of counter on target permanent or player, give that permanent or player another counter of that kind</examplecapture>
[Dependent]
public class SkyshipPlundererEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each kind of counter on target permanent or player, give that permanent or player another counter of that kind"];
}
