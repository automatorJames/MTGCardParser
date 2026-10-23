namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shieldmage Elder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shieldmage Elder</exampledoc>
[Dependent]
public class ShieldmageElderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage target spell would deal this turn"];
}
