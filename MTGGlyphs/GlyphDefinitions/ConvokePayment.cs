namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Convoke's second reminder sentence: "each creature you tap while casting this spell pays for {1} or one mana of that creature's color", or, where convoke is granted to spells of a kind, "… while casting a creature spell pays for …".</summary>
/// <exampledoc>Autochthon Wurm</exampledoc>
/// <examplecapture>each creature you tap while casting this spell pays for {1} or one mana of that creature's color</examplecapture>
public class ConvokePayment : Glyph
{
    public override Nib[] Nibs => ["each creature you tap while casting", Prop(Spell), "pays for {1} or one mana of that creature's color"];

    public OneOf<ThisSpell, IndefiniteSpell> Spell { get; set; }
}
