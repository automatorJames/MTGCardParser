namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The second thing a permanent enters with: "and with "whenever this creature deals damage, you gain that much life"", "and with x +1/+1 counters on it".</summary>
/// <exampledoc>Anavolver</exampledoc>
/// <examplecapture>and with "pay 3 life: regenerate this creature</examplecapture>
[Dependent]
public class AndEntersWith : Glyph
{
    public override Nib[] Nibs => ["and", Prop(With)];

    public OneOf<EntersWithCounters, EntersWithAbility> With { get; set; }
}
