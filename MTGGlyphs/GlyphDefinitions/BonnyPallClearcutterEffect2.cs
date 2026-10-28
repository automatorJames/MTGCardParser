namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bonny Pall, Clearcutter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bonny Pall, Clearcutter</exampledoc>
/// <examplecapture>create beau, a legendary blue ox creature token with "beau's power and toughness are each equal to the number of lands you control</examplecapture>
[Dependent]
public class BonnyPallClearcutterEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create beau, a legendary blue ox creature token with \"beau's power and toughness are each equal to the number of lands you control"];
}
