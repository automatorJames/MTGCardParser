namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mobilizer Mech, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mobilizer Mech</exampledoc>
[TokenizationOrder(-1)]
public class MobilizerMechCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this vehicle becomes crewed, up to one other target vehicle you control becomes an artifact creature until end of turn"];
}
