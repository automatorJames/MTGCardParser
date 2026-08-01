namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumNode : ScalarContainerNode
{
    protected override Joiner Joiner => Joiner.Pipe;
    public override CaptureNodeType NodeType => CaptureNodeType.Enum;

    public EnumNode(RegexNode parentNode, Navigation navigation) : base(parentNode, navigation)
    {
    }

    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        var enumType = Navigation.UnderlyingType;
        var scalarValues = Enum.GetValues(enumType).Cast<object>().ToList();

        for (int i = 0; i < scalarValues.Count; i++)
        {
            var scalarValue = scalarValues[i];
            var valueAsString = scalarValue.ToString();
            var field = enumType.GetField(valueAsString);

            List<string> patterns =
                field.GetCustomAttribute<RegexPatternAttribute>()?.Patterns.ToList()
                ?? [valueAsString.ToFriendlyCase()];

            if (enumType.IsDefined(typeof(OptionalPluralAttribute)))
            {
                patterns = patterns
                    .SelectMany(x => new[] { x, x.AddPluralization(makeOptional: false) })
                    .ToList();
            }

            bool isFirst = i == 0;

            if (patterns.Count > 1)
                children.Add(new ScalarSynonymSet(
                    parentNode: this,
                    name: valueAsString,
                    scalarValue: scalarValue,
                    scalarSynonyms: patterns,
                    positionAmongSiblings: i));
            else
                children.Add(new ScalarNode(
                    parentNode: this,
                    name: valueAsString,
                    scalarValue: scalarValue,
                    regex: patterns[0],
                    positionAmongSiblings: i));
        }
    }

    public override object GetValueSingle(CaptureTrace captureInfo)
    {
        return Children
            .OfType<INamedScalarValue>()
            //.FirstOrDefault(x => x.Name.Equals(capture.Value, StringComparison.InvariantCultureIgnoreCase))?
            .FirstOrDefault(x => x.Regex.IsMatch(captureInfo.CaptureValue))
            .ScalarValue
            ?? throw new Exception($"Found no matching values for enum '{Navigation.UnderlyingType.Name}' from match string '{captureInfo.CaptureValue}'");
    }
}
