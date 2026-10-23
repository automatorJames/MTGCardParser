namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Heartlash Cinder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heartlash Cinder</exampledoc>
/// <examplecapture>it gets +x/+0 until end of turn, where x is the number of red mana symbols in the mana costs of permanents you control</examplecapture>
[Dependent]
public class HeartlashCinderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets +x/+0 until end of turn, where x is the number of red mana symbols in the mana costs of permanents you control"];
}
