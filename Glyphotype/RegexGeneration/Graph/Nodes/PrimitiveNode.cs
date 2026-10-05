namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>A property of a supported CLR primitive type (see <see cref="PrimitiveTerminal"/>): matches its <see cref="RegexPatternAttribute"/> pattern(s), else the type's default pattern, and hydrates to the parsed value.</summary>
public class PrimitiveNode : NamedGroupNode
{
    public PrimitiveTerminal Terminal { get; }

    protected override string DefaultPattern => Terminal.DefaultPattern;

    public PrimitiveNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
        Terminal = PrimitiveTerminal.TryGet(navigation.NodeType, out var terminal)
            ? terminal
            : throw new Exception($"'{navigation.NodeType.Name}' is not a supported primitive type");
    }

    protected override object GetValue(CaptureTrace captureTrace)
    {
        if (captureTrace.Count != 1)
            throw new Exception($"{nameof(PrimitiveNode)} expects exactly one capture");

        return Terminal.Parse(ContentText(captureTrace));
    }
}
