namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Autarch Mammoth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Autarch Mammoth</exampledoc>
[Dependent]
public class AutarchMammothEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 3/3 green elephant creature token"];
}
