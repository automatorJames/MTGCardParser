namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wurmcoil Engine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wurmcoil Engine</exampledoc>
/// <examplecapture>create a 3/3 colorless phyrexian wurm artifact creature token with deathtouch and a 3/3 colorless phyrexian wurm artifact creature token with lifelink</examplecapture>
[Dependent]
public class WurmcoilEngineEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 3/3 colorless phyrexian wurm artifact creature token with deathtouch and a 3/3 colorless phyrexian wurm artifact creature token with lifelink"];
}
