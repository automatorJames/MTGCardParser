namespace Glyphotype.NibHelpers;

public record OptionalNib : Nib
{
    public OptionalNib(string optionalText) 
        : base(optionalText) { }
}
