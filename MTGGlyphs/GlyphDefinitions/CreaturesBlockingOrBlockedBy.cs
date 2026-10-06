namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// The creatures in combat with something earlier in the line, e.g. "creatures blocking or blocked by it". The phrase
/// is a referent itself, for a later "they".
/// </summary>
/// <exampledoc>Abu Ja'far</exampledoc>
/// <examplecapture>creatures blocking or blocked by it</examplecapture>
[Dependent]
[Referent(GrammaticalNumber.Plural)]
public class CreaturesBlockingOrBlockedBy : Glyph
{
    public override Nib[] Nibs => ["creatures blocking or blocked by", Prop(Combatant)];

    public It Combatant { get; set; }
}
