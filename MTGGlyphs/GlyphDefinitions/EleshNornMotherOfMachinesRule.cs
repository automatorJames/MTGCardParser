namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elesh Norn, Mother of Machines, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elesh Norn, Mother of Machines</exampledoc>
[TokenizationOrder(-1)]
public class EleshNornMotherOfMachinesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["permanents entering don't cause abilities of permanents your opponents control to trigger"];
}
