namespace Glyphotype.NibHelpers;

public record NibAlternatives : Nib
{
    public NibAlternatives(params string[] alternatives) : base("(" + string.Join('|', alternatives) +")") { }
}
