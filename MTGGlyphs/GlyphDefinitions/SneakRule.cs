namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Sneak's reminder text: "you may cast this spell for {w} if you also return an unblocked attacker you control to hand during the declare blockers step". The cost repeats the keyword's; the next sentence ("he enters tapped and attacking") is an ordinary enters predicate.</summary>
/// <exampledoc>Leonardo, Big Brother</exampledoc>
/// <examplecapture>you may cast this spell for {w} if you also return an unblocked attacker you control to hand during the declare blockers step</examplecapture>
[Dependent]
public class SneakRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this spell for", Prop(Cost), "if you also return an unblocked attacker you control to hand during the declare blockers step"];

    public ManaCost Cost { get; set; }
}
