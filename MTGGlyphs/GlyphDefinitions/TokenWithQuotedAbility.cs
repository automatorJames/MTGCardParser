namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A token described with a quoted ability of its own, possibly after keywords or before an equip cost: `a 1/1 black rat creature token with "this token can't block`, `a 2/2 red dragon creature token with flying and "{r}: …`, `a colorless equipment artifact token named axe with "equipped creature gets +1/+0" and equip {2}`.</summary>
/// <exampledoc>Voracious Vermin</exampledoc>
/// <examplecapture>a 1/1 black rat creature token with "this token can't block</examplecapture>
[Dependent]
public class TokenWithQuotedAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Token), Prop(Quoted)];

    public TokenDescription Token { get; set; }
    public OneOf<WithQuotedAbilityAndEquip, WithQuotedAbility, WithKeywordsAndQuotedAbility> Quoted { get; set; }
}
