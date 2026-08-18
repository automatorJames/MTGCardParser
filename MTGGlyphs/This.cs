namespace MTGGlyphs;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
[Dependent]
public class This : Glyph
{
    public override Nib[] Nibs => [@"{this}"];

}