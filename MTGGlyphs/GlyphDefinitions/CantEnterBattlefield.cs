namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Objects barred from entering the battlefield: "lands can't enter the battlefield", "creature cards in graveyards can't enter the battlefield".</summary>
/// <exampledoc>Grafdigger's Cage</exampledoc>
/// <examplecapture>can't enter the battlefield</examplecapture>
[Dependent]
public class CantEnterBattlefield : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can't enter the battlefield"];
}
