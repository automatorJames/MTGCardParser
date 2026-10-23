namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Losing unspent mana: "lose unspent mana" (what a replacement effect watches for), "loses all unspent mana" (Mana Short). What a subject does.</summary>
/// <exampledoc>Mana Short</exampledoc>
/// <examplecapture>loses all unspent mana</examplecapture>
[Dependent]
public class LosesUnspentMana : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("loses?"), Prop(All), "unspent mana"];

    [RegexPattern("all")]
    public bool All { get; set; }
}
