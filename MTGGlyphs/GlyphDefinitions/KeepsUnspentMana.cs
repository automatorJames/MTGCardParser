namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Players keep their unspent mana, of one color or all, from step to step: "don't lose unspent red mana as steps and phases end", "don't lose unspent mana as steps and phases end". What a subject does ("you …", "players …").</summary>
/// <exampledoc>Leyline Tyrant</exampledoc>
/// <examplecapture>don't lose unspent red mana as steps and phases end</examplecapture>
[Dependent]
public class KeepsUnspentMana : Glyph, IPredicate
{
    public override Nib[] Nibs => ["don't lose unspent", Prop(Color), "mana as steps and phases end"];

    [Optional]
    public ManaColor? Color { get; set; }
}
