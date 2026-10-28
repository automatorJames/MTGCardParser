namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to you", "to this creature", "to an opponent", "to another player", "to you or a permanent you control": who or what a source dealt damage to, in a damage trigger.</summary>
/// <exampledoc>Night Dealings</exampledoc>
/// <examplecapture>to another player</examplecapture>
[Dependent]
public class DamageRecipient : Glyph
{
    public override Nib[] Nibs => ["to", Prop(Recipient)];

    public OneOf<PermanentPhrase, WhichPlayer?, YouOrAPermanentYouControl, AnotherPlayer> Recipient { get; set; }
}
