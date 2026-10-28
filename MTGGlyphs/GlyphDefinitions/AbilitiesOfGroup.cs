namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"abilities of creatures you control": abilities of a group of permanents, where "activate" already says which kind.</summary>
/// <exampledoc>Agatha's Soul Cauldron</exampledoc>
/// <examplecapture>abilities of creatures you control</examplecapture>
[Dependent]
public class AbilitiesOfGroup : Glyph
{
    public override Nib[] Nibs => ["abilities of", Prop(Group)];

    public OneOf<EnchantedPermanent, Permanents, NamedSources, PermanentPhrase> Group { get; set; }
}
