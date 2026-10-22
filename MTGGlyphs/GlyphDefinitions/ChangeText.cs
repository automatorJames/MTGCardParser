namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Text-changing effects (Sleight of Mind, Magical Hack): "change the text of target spell or permanent by replacing all instances of one color word with another", optionally of either of two kinds and for a time.</summary>
/// <exampledoc>Sleight of Mind</exampledoc>
/// <examplecapture>change the text of target spell or permanent by replacing all instances of one color word with another</examplecapture>
public class ChangeText : Glyph, IPredicate
{
    public override Nib[] Nibs => ["change the text of", Prop(Changed), "by replacing all instances of", Prop(Replacements), Prop(Duration)];

    public OneOf<TargetSpellOrPermanent, PermanentPhrase> Changed { get; set; }
    public OneOf<ManyOf<TextReplacement>, TextReplacement> Replacements { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
