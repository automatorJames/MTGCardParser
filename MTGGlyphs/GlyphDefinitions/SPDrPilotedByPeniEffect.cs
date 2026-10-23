namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of SP//dr, Piloted by Peni, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>SP//dr, Piloted by Peni</exampledoc>
[Dependent]
public class SPDrPilotedByPeniEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on target creature"];
}
