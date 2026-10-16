namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Milling: putting the top cards of your library into your graveyard, "mill four cards"; also what a subject does ("target player mills three cards").</summary>
/// <exampledoc>Armored Skaab</exampledoc>
/// <examplecapture>mill four cards</examplecapture>
public class Mill : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("mills?"), Prop(Quantity), Plural("card")];

    public Quantity Quantity { get; set; }
}
