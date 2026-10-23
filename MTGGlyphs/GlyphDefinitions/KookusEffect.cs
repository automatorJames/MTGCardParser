namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kookus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kookus</exampledoc>
/// <examplecapture>if you don't control a creature named keeper of {this}, this creature deals 3 damage to you and attacks this turn if able</examplecapture>
[Dependent]
public class KookusEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't control a creature named keeper of", Nib.This, ", this creature deals 3 damage to you and attacks this turn if able"];
}
