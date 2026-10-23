namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of SP//dr, Piloted by Peni, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>SP//dr, Piloted by Peni</exampledoc>
[TokenizationOrder(-1)]
public class SPDrPilotedByPeniCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when sp//dr enters, put a +1/+1 counter on target creature"];
}
