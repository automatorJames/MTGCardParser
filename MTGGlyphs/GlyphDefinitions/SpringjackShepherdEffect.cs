namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Springjack Shepherd, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Springjack Shepherd</exampledoc>
/// <examplecapture>create a 0/1 white goat creature token for each white mana symbol in the mana costs of permanents you control</examplecapture>
[Dependent]
public class SpringjackShepherdEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 0/1 white goat creature token for each white mana symbol in the mana costs of permanents you control"];
}
