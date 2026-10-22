namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Casting this spell for free, as an alternative cost granted under a condition: "you may cast this spell without paying its mana cost" (the Legates). Kept apart from PlayCard so "you cast this spell" stays a trigger condition (CastingThisSpell).</summary>
/// <exampledoc>Cho-Arrim Legate</exampledoc>
/// <examplecapture>cast this spell without paying its mana cost</examplecapture>
[Dependent]
public class CastThisSpellFree : Glyph, IPredicate
{
    public override Nib[] Nibs => ["cast", Prop(Spell), "without paying its mana cost"];

    public ThisSpell Spell { get; set; }
}
