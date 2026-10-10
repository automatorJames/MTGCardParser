namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A mana cost: a run of mana symbols written with nothing between them, e.g. "{2}{w}{w}".</summary>
/// <exampledoc>Conversion</exampledoc>
/// <examplecapture>{w}{w}</examplecapture>
[Dependent]
[JoinedBy(Joiner.None)]
public class ManaCost : CompoundOf<ManaSymbol>
{
}

/// <summary>One braced mana symbol: an amount of colorless mana ("{2}"), or a symbol ("{w}", "{w/u}", "{x}").</summary>
/// <exampledoc>Phantasmal Forces</exampledoc>
/// <examplecapture>{u}</examplecapture>
[Dependent]
public class ManaSymbol : GlyphOneOf
{
    public override Nib[] Nibs => ["{", Prop(Colorless), Prop(Symbol), "}"];

    public int? Colorless { get; set; }
    public ManaSymbolKind? Symbol { get; set; }
}
