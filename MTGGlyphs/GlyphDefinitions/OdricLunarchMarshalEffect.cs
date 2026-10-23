namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Odric, Lunarch Marshal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Odric, Lunarch Marshal</exampledoc>
[Dependent]
public class OdricLunarchMarshalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control gain first strike until end of turn if a creature you control has first strike"];
}
