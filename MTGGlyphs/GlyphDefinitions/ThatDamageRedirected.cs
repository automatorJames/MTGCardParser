namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A redirection replacement: "that damage is dealt to this creature instead", "that damage is dealt to target creature you control instead" - the damage a "would deal" condition just named goes to a new recipient.</summary>
/// <exampledoc>Beacon of Destiny</exampledoc>
/// <examplecapture>that damage is dealt to this creature instead</examplecapture>
public class ThatDamageRedirected : Glyph
{
    public override Nib[] Nibs => ["that damage is dealt to", Prop(NewRecipient), "instead"];

    public Recipient NewRecipient { get; set; }
}
