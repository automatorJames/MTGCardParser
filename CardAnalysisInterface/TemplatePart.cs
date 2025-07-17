namespace CardAnalysisInterface;

public abstract class TemplatePart { }

public class TextPart : TemplatePart
{
    public string Text { get; set; }
    public override string ToString() => Text;
}

public class TokenPart : TemplatePart
{
    public Type TokenType { get; set; }
    public string TypeName => TokenType.Name;
    public bool IsEnumType { get; set; }
    public override string ToString() => $"@{TypeName}";
}