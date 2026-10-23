namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shell of the Last Kappa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shell of the Last Kappa</exampledoc>
[Dependent]
public class ShellOfTheLastKappaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a spell from among cards exiled with", Nib.This, "without paying its mana cost"];
}
