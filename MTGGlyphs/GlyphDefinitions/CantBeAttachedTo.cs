namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent that can't have attachments: "can't be equipped", "can't be enchanted", "can't be enchanted by other auras".</summary>
/// <exampledoc>Goblin Brawler</exampledoc>
/// <examplecapture>can't be equipped</examplecapture>
[Dependent]
public class CantBeAttachedTo : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can't be", Alt("equipped", "enchanted"), Prop(OtherAuras)];

    [RegexPattern("by other auras")]
    public bool OtherAuras { get; set; }
}
