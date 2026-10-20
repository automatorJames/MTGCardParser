namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"can't be activated", "can't be activated unless they're mana abilities": a ban on activating abilities.</summary>
/// <exampledoc>Arachnus Web</exampledoc>
/// <examplecapture>can't be activated</examplecapture>
[Dependent]
public class CantBeActivated : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't be activated", Prop(ExceptMana)];

    [RegexPattern("unless they're mana abilities")]
    public bool ExceptMana { get; set; }
}
