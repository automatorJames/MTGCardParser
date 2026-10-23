namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Floodpits Drowner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Floodpits Drowner</exampledoc>
[Dependent]
public class FloodpitsDrownerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle this creature and target creature with a stun counter on it into their owners' libraries"];
}
