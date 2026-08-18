namespace Glyphotype.NibHelpers;

public record OptionalPluralNib : Nib
{
    public OptionalPluralNib() 
        : base("(s|es|ies)?") { }
}
