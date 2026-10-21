namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Removing a creature from combat: "remove target attacking or blocking creature from combat", "remove it from combat", also after another action: "untap it and remove it from combat", and the reminder "(this removes this creature from combat".</summary>
/// <exampledoc>Gustcloak Harrier</exampledoc>
/// <examplecapture>remove it from combat</examplecapture>
public class RemoveFromCombat : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Alt("remove", "removes"), Prop(Removed), "from combat"];

    public OneOf<PermanentPhrase, PermanentGroup> Removed { get; set; }
}
