namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Invasion of Ravnica // Guildpact Paragon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasion of Ravnica // Guildpact Paragon</exampledoc>
[Dependent]
public class InvasionOfRavnicaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target nonland permanent an opponent controls that isn't exactly two colors"];
}
