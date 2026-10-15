namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// The creatures in combat with something earlier in the line or with {this}, e.g. "creatures blocking or blocked by it", "creatures blocking or blocked by {this}". The phrase
/// is a referent itself, for a later "they".
/// </summary>
/// <exampledoc>Abu Ja'far</exampledoc>
/// <examplecapture>creatures blocking or blocked by it</examplecapture>
[Dependent]
[Referent]
[Plural]
public class CreaturesBlockingOrBlockedBy : Glyph
{
    public override Nib[] Nibs => ["creatures blocking or blocked by", Prop(Combatant)];

    public PermanentPhrase Combatant { get; set; }
}
