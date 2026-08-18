namespace Glyphotype.NibHelpers;

public record Nib
{
    public string Text { get; init; }
    public bool IsPlural { get; init; }
    public bool IsOptional { get; init; }

    public Nib(string text)
    {
        Text = text;
        IsOptional = this is OptionalNib;
    }

    // Implicitly create a Nib from a string
    public static implicit operator Nib(string str) => new(str);
}