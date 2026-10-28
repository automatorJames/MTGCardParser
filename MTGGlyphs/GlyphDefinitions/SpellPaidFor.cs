namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The spell some mana paid for: "to cast it", "to cast this spell", "to cast that spell", "to cast them".</summary>
/// <exampledoc>Magmablood Archaic</exampledoc>
/// <examplecapture>to cast that spell</examplecapture>
[Dependent]
public class SpellPaidFor : Glyph
{
    public override Nib[] Nibs => ["to cast", Prop(Spell)];

    public OneOf<It, Them, ThatStackObject, ThisObject, This> Spell { get; set; }
}
