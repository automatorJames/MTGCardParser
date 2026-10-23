namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Blazing Effigy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blazing Effigy</exampledoc>
/// <examplecapture>it deals x damage to target creature, where x is 3 plus the amount of damage dealt to this creature this turn by other sources named {this}</examplecapture>
[Dependent]
public class BlazingEffigyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals x damage to target creature, where x is 3 plus the amount of damage dealt to this creature this turn by other sources named", Nib.This];
}
