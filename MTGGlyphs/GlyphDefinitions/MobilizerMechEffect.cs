namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mobilizer Mech, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mobilizer Mech</exampledoc>
[Dependent]
public class MobilizerMechEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["up to one other target vehicle you control becomes an artifact creature until end of turn"];
}
