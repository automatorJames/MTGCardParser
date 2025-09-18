namespace MTGPlexer;

public class TokenCaptureSummary
{
    public string Name { get; private set; }
    public string Path { get; private set; }
    public string OriginalFullText { get; private set; }
    public string CaptureTextLower { get; private set; }
    public string CaptureTextOriginal { get; private set; }
    public int Start { get; private set; }
    public int End { get; private set; }
    public int Length { get; private set; }
    public string TerminalValString { get; private set; }
    public string TerminalType { get; private set; }
    public TokenAnalysisElementType ElementType { get; private set; }
    public Palette UnderlinePalette { get; private set; }
    public Palette OverlinePalette { get; private set; }
    public Type RootTokenType { get; private set; }
    public TokenCaptureSummary Parent { get; private set; }
    public List<TokenCaptureSummary> Children { get; private set; } = [];

    // --- Public Entry Point ---

    /// <summary>
    /// Single public entry point to create a summary tree from a root TokenUnit.
    /// </summary>
    public static TokenCaptureSummary CreateFrom(TokenUnit root, string originalFullText)
    {
        var rootSummary = new TokenCaptureSummary(root.Capture, originalFullText)
        {
            RootTokenType = root.Type,
            Name = root.Type.Name.ToFriendlyCase(TitleDisplayOption.Title),
            Path = root.Path,
            UnderlinePalette = TokenTypeRegistry.Palettes[root.Type],
            ElementType = root is DefaultUnmatchedString
                ? TokenAnalysisElementType.UnmatchedTokenUnitRoot
                : TokenAnalysisElementType.TokenUnitRoot
        };

        foreach (var propCapture in root.IndexedPropertyCaptures)
            rootSummary.Children.Add(CreateSummaryFor(rootSummary, propCapture));

        return rootSummary;
    }

    // --- Private Constructors ---

    /// <summary>
    /// Private base constructor for all nodes. Sets common capture-related properties.
    /// </summary>
    private TokenCaptureSummary(Capture capture, string originalFullText)
    {
        OriginalFullText = originalFullText;
        if (capture != null)
        {
            Start = capture.Index;
            Length = capture.Length;
            End = Start + Length;
            CaptureTextLower = capture.Value;
            CaptureTextOriginal = originalFullText.Substring(Start, Length);
        }
    }

    /// <summary>
    /// Private constructor for nodes that inherit capture info from their parent (e.g., DistilledValue).
    /// </summary>
    private TokenCaptureSummary(TokenCaptureSummary parent) : this(null, parent.OriginalFullText)
    {
        Parent = parent;
        // Copy capture info from parent
        Start = parent.Start;
        Length = parent.Length;
        End = parent.End;
        CaptureTextLower = parent.CaptureTextLower;
        CaptureTextOriginal = parent.CaptureTextOriginal;
    }


    // --- Core Logic: Dispatcher ---

    /// <summary>
    /// Acts as a router, dispatching to the correct factory method based on the property's value type.
    /// </summary>
    private static TokenCaptureSummary CreateSummaryFor(TokenCaptureSummary parent, IndexedPropertyCapture propCapture)
    {
        var val = propCapture.Value;

        // Branches (Parents to further children)
        if (val is TokenUnitOneOf tuOneOf) return CreateForTokenUnitOneOf(parent, propCapture, tuOneOf);
        if (val is TokenUnitDistilled tuDistilled) return CreateForTokenUnitDistilled(parent, propCapture, tuDistilled);
        if (val is TokenUnit tokenUnit) return CreateForTokenUnit(parent, propCapture, tokenUnit);
        if (val is ManyOf manyOf) return CreateForManyOf(parent, propCapture, manyOf);
        if (val is DynamicCapture dynamicCapture) return CreateForDynamicCapture(parent, propCapture, dynamicCapture);

        // Leaves (Scalar values within parent summaries)
        if (propCapture.RegexPropInfo.RegexPropType == RegexPropType.Enum) return CreateForEnum(parent, propCapture);
        if (propCapture.RegexPropInfo.RegexPropType == RegexPropType.Bool) return CreateForBool(parent, propCapture);
        if (val is PlaceholderCapture placeholder) return CreateForPlaceholder(parent, propCapture, placeholder);

        throw new ArgumentException($"Unsupported TokenUnit property type: {val?.GetType().Name}");
    }


    // --- Private Factory Methods for Each Type ---

    private static TokenCaptureSummary CreateForTokenUnit(TokenCaptureSummary parent, IndexedPropertyCapture propCapture, TokenUnit tokenUnit)
    {
        var summary = new TokenCaptureSummary(propCapture.Capture, parent.OriginalFullText)
        {
            Parent = parent,
            Name = propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
            Path = propCapture.Path,
            ElementType = TokenAnalysisElementType.TokenUnitBranch,
            UnderlinePalette = TokenTypeRegistry.Palettes[tokenUnit.Type],
        };

        foreach (var x in tokenUnit.IndexedPropertyCaptures)
            summary.Children.Add(CreateSummaryFor(summary, x));

        return summary;
    }

    private static TokenCaptureSummary CreateForTokenUnitOneOf(TokenCaptureSummary parent, IndexedPropertyCapture propCapture, TokenUnitOneOf tokenUnitOneOf)
    {
        var summary = new TokenCaptureSummary(propCapture.Capture, parent.OriginalFullText)
        {
            Parent = parent,
            Name = propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
            Path = propCapture.Path,
            UnderlinePalette = TokenTypeRegistry.Palettes[tokenUnitOneOf.Type],
        };

        var singleTokenCapture = tokenUnitOneOf.IndexedPropertyCaptures.Single();
        var populatedChild = singleTokenCapture.Value;

        if (populatedChild is TokenUnitOneOf)
            throw new NotImplementedException($"Nested {nameof(TokenUnitOneOf)} children not supported");

        if (populatedChild is TokenUnit tokenUnit)
        {
            summary.ElementType = TokenAnalysisElementType.OneOfItemBranch;
            summary.Children.Add(CreateForTokenUnit(summary, singleTokenCapture, tokenUnit));
        }
        else if (singleTokenCapture.RegexPropInfo.RegexPropType == RegexPropType.Enum)
        {
            summary.ElementType = TokenAnalysisElementType.OneOfItemLeaf;
            summary.SetEnumScalar(populatedChild);
            summary.OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal);
        }
        else
            throw new NotImplementedException($"{nameof(TokenUnitOneOf)} only supports {nameof(TokenUnit)} and enum children");

        return summary;
    }

    private static TokenCaptureSummary CreateForDynamicCapture(TokenCaptureSummary parent, IndexedPropertyCapture propCapture, DynamicCapture dynamicCapture)
    {
        var summary = new TokenCaptureSummary(propCapture.Capture, parent.OriginalFullText)
        {
            Parent = parent,
            Name = propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
            Path = propCapture.Path,
            UnderlinePalette = DeterministicPalette.GetStaticPalette(typeof(DynamicCapture).GetCustomAttribute<ColorAttribute>().Color),
        };

        var valueObject = dynamicCapture.ValueObject;
        if (valueObject is TokenUnitOneOf tokenUnitOneOf)
        {
            summary.ElementType = TokenAnalysisElementType.DynamicCaptureItemBranch;
            summary.Children.Add(CreateForTokenUnitOneOf(summary, propCapture, tokenUnitOneOf));
        }
        else if (valueObject is TokenUnit tokenUnit)
        {
            summary.ElementType = TokenAnalysisElementType.DynamicCaptureItemBranch;
            summary.Children.Add(CreateForTokenUnit(summary, propCapture, tokenUnit));
        }
        else if (dynamicCapture.RegexPropType == RegexPropType.Enum)
        {
            summary.ElementType = TokenAnalysisElementType.DynamicCaptureItemLeaf;
            summary.SetEnumScalar(propCapture.Value);
            summary.OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal);
        }
        else
            throw new NotImplementedException($"{nameof(DynamicCapture)} only supports {nameof(TokenUnitOneOf)}, {nameof(TokenUnit)}, and enum children");

        return summary;
    }

    private static TokenCaptureSummary CreateForTokenUnitDistilled(TokenCaptureSummary parent, IndexedPropertyCapture propCapture, TokenUnitDistilled distilled)
    {
        var summary = new TokenCaptureSummary(propCapture.Capture, parent.OriginalFullText)
        {
            Parent = parent,
            Name = propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
            Path = propCapture.Path,
            ElementType = TokenAnalysisElementType.TokenUnitDistilledBranch,
            UnderlinePalette = TokenTypeRegistry.Palettes[distilled.Type],
        };

        var nonDistilledProps = distilled.IndexedPropertyCaptures.Where(x => !distilled.DistilledVals.ContainsKey(x));
        foreach (var x in nonDistilledProps)
            summary.Children.Add(CreateSummaryFor(summary, x));

        foreach (var (placeholderCap, distilledVals) in distilled.DistilledVals)
            summary.Children.Add(CreateForDistilledPlaceholder(summary, placeholderCap, distilledVals));

        return summary;
    }

    private static TokenCaptureSummary CreateForManyOf(TokenCaptureSummary parent, IndexedPropertyCapture propCapture, ManyOf manyOf)
    {
        var summary = new TokenCaptureSummary(propCapture.Capture, parent.OriginalFullText)
        {
            Parent = parent,
            Name = propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
            Path = propCapture.Path,
            ElementType = TokenAnalysisElementType.ManyOfBranch,
            UnderlinePalette = TokenTypeRegistry.Palettes.TryGetValue(manyOf.ItemType, out var p) ? p
                : DeterministicPalette.GetStaticPalette(typeof(ManyOf).GetCustomAttribute<ColorAttribute>().Color)
        };

        for (int i = 0; i < manyOf.ItemObjects.Count; i++)
        {
            var itemCapture = manyOf.ItemObjects[i];
            var itemSummary = new TokenCaptureSummary(itemCapture.Capture, parent.OriginalFullText)
            {
                Parent = summary,
                Name = propCapture.RegexPropInfo.Name + " #" + (i + 1),
                Path = propCapture.Path + $"[{i}]",
            };

            if (manyOf.ManyItemVariant == ManyItemVariant.TokenUnit && itemCapture.ItemObject is TokenUnit tokenUnit)
            {
                itemSummary.ElementType = TokenAnalysisElementType.ManyOfItemBranch;
                itemSummary.UnderlinePalette = TokenTypeRegistry.Palettes[tokenUnit.Type];
                var synthesized = new IndexedPropertyCapture(itemCapture, itemSummary.Path);
                itemSummary.Children.Add(CreateSummaryFor(itemSummary, synthesized));
            }
            else if (manyOf.ManyItemVariant == ManyItemVariant.Enum)
            {
                itemSummary.ElementType = TokenAnalysisElementType.ManyOfItemLeaf;
                itemSummary.SetEnumScalar(itemCapture.ItemObject);
                itemSummary.OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(i);
            }
            else
                throw new NotImplementedException($"{nameof(ManyItemVariant)} '{manyOf.ManyItemVariant}' not supported");

            summary.Children.Add(itemSummary);
        }

        if (manyOf.Conjunction != null)
        {
            var conjunctionSummary = new TokenCaptureSummary(manyOf.ConjunctionCapture, parent.OriginalFullText)
            {
                Parent = summary,
                Name = nameof(ManyOf.Conjunction),
                Path = summary.Path.Dot(nameof(ManyOf.Conjunction)),
                ElementType = TokenAnalysisElementType.ConjunctionLeaf,
                OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(manyOf.ItemObjects.Count + 1),
            };
            conjunctionSummary.SetEnumScalar(manyOf.Conjunction.Value);
            summary.Children.Add(conjunctionSummary);
        }
        return summary;
    }

    private static TokenCaptureSummary CreateForDistilledPlaceholder(TokenCaptureSummary parent, IndexedPropertyCapture placeholder, Dictionary<RegexPropInfo, object> distilledVals)
    {
        var summary = new TokenCaptureSummary(placeholder.Capture, parent.OriginalFullText)
        {
            Parent = parent,
            Name = placeholder.RegexPropInfo.Name,
            Path = placeholder.Path,
            ElementType = TokenAnalysisElementType.PlaceholderPrecursorBranch,
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(placeholder.Ordinal),
        };
        summary.SetPlaceholderScalar(placeholder.Text);

        foreach (var (distilledProp, value) in distilledVals)
        {
            var distilledSummary = new TokenCaptureSummary(summary)
            {
                Name = distilledProp.Name,
                Path = summary.Path.Dot(distilledProp.Name),
                ElementType = TokenAnalysisElementType.DistilledValueSubLeaf,
            };
            distilledSummary.SetDistilledScalar(value);
            summary.Children.Add(distilledSummary);
        }
        return summary;
    }

    // --- Leaf Node Factories ---

    private static TokenCaptureSummary CreateForEnum(TokenCaptureSummary parent, IndexedPropertyCapture propCapture)
    {
        var summary = new TokenCaptureSummary(propCapture.Capture, parent.OriginalFullText)
        {
            Parent = parent,
            Name = propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
            Path = propCapture.Path,
            ElementType = TokenAnalysisElementType.EnumLeaf,
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal),
        };
        summary.SetEnumScalar(propCapture.Value);
        return summary;
    }

    private static TokenCaptureSummary CreateForBool(TokenCaptureSummary parent, IndexedPropertyCapture propCapture)
    {
        var summary = new TokenCaptureSummary(propCapture.Capture, parent.OriginalFullText)
        {
            Parent = parent,
            Name = propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
            Path = propCapture.Path,
            ElementType = TokenAnalysisElementType.BoolLeaf,
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal),
        };
        summary.SetBoolScalar((bool)propCapture.Value);
        return summary;
    }

    private static TokenCaptureSummary CreateForPlaceholder(TokenCaptureSummary parent, IndexedPropertyCapture propCapture, PlaceholderCapture placeholder)
    {
        var summary = new TokenCaptureSummary(propCapture.Capture, parent.OriginalFullText)
        {
            Parent = parent,
            Name = propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
            Path = propCapture.Path,
            ElementType = TokenAnalysisElementType.PlaceholderLeaf,
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal),
        };
        summary.SetPlaceholderScalar(placeholder.Text);
        return summary;
    }

    // --- Helper Methods for Setting Scalar Values ---

    private void SetEnumScalar(object enumVal)
    {
        TerminalValString = enumVal.ToString().ToFriendlyCase(TitleDisplayOption.Lower);
        TerminalType = "enum";
    }

    private void SetBoolScalar(bool boolVal)
    {
        TerminalValString = boolVal.ToString().ToLower();
        TerminalType = "bool";
    }

    private void SetPlaceholderScalar(string placeholderVal)
    {
        TerminalValString = placeholderVal;
        TerminalType = "placeholder";
    }

    private void SetDistilledScalar(object distilledVal)
    {
        TerminalValString = distilledVal.ToString().ToLower();
        var type = distilledVal.GetType();
        TerminalType = "distilled " + (type == typeof(int) ? "int" : type.Name.ToFriendlyCase(TitleDisplayOption.Lower));
    }

    /// <summary>
    /// Recursively builds a string that shows the nesting of captures within the current summary's text.
    /// Example: "The ((dog) runs fast)"
    /// </summary>
    public string GetNestedCaptureString()
    {
        if (Children.Count == 0 || string.IsNullOrEmpty(CaptureTextOriginal))
            return CaptureTextOriginal;

        var builder = new StringBuilder(CaptureTextOriginal);

        // Process children from last to first to avoid invalidating indices.
        foreach (var child in Children.OrderByDescending(c => c.Start))
        {
            // Only process children that have a distinct sub-capture within this parent.
            if (child.Start < this.Start || child.End > this.End || child.Length == 0)
                continue;

            string childNestedString = child.GetNestedCaptureString();
            string replacement = $"({childNestedString})";

            int relativeStart = child.Start - this.Start;

            builder.Remove(relativeStart, child.Length);
            builder.Insert(relativeStart, replacement);
        }

        return builder.ToString();
    }

    // --- ToString() Override ---

    /// <summary>
    /// Provides an enriched, single-line summary of the node, including its nested capture text.
    /// Omits character indices for the root node for clarity.
    /// </summary>
    public override string ToString()
    {
        string nestedCapture = GetNestedCaptureString();
        string friendlyElementType = ElementType.ToString().ToFriendlyCase();

        // Conditionally format the capture part based on whether it's a root node
        string captureDisplay = (Parent == null)
            ? $"\"{nestedCapture}\""
            : $"[{Start}] \"{nestedCapture}\" [{End}]";

        return $"{Path} | {captureDisplay} | {friendlyElementType} | Children: {Children.Count}";
    }
}

public enum TokenAnalysisElementType
{
    UnmatchedTokenUnitRoot,
    TokenUnitRoot,

    TokenUnitBranch,
    TokenUnitOneOfBranch,
    TokenUnitDistilledBranch,
    OneOfItemBranch,
    ManyOfBranch,
    ManyOfItemBranch,
    DynamicCaptureBranch,
    DynamicCaptureItemBranch,
    PlaceholderPrecursorBranch,

    EnumLeaf,
    BoolLeaf,
    OneOfItemLeaf,
    ManyOfItemLeaf,
    PlaceholderLeaf,
    DynamicCaptureItemLeaf,
    ConjunctionLeaf,

    DistilledValueSubLeaf,
}