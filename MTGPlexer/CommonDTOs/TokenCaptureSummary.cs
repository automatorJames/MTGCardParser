
using System.Diagnostics;

namespace MTGPlexer.TokenAnalysisDTOs.TokenAnalysis;

/// <summary>
/// A static factory class that analyzes a root TokenUnit and produces a tree of immutable DTOs.
/// </summary>
public static class TokenCaptureSummary
{
    /// <summary>
    /// Internal mutable class for building the tree structure before converting to immutable DTOs.
    /// </summary>
    private class PrecursorNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string OriginalFullText { get; set; }
        public string CaptureTextOriginal { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Length { get; set; }
        public string TerminalValString { get; set; }
        public string TerminalType { get; set; }
        public TokenAnalysisElementType ElementType { get; set; }
        public Palette Palette { get; set; }
        public Type RootTokenType { get; set; }
        public List<PrecursorNode> Children { get; } = new();
    }

    // --- Public Entry Point ---

    /// <summary>
    /// Single public entry point to create a DTO summary tree from a root TokenUnit.
    /// It now accepts card and clause context to apply to the final DTO tree.
    /// </summary>
    public static TokenAnalysisRoot CreateFrom(TokenUnit root, string originalFullText, string cardName, int clauseIndex)
    {
        // Stage 1: Build the mutable precursor tree with local, non-prepended paths.
        var precursorRoot = CreatePrecursorForRoot(root, originalFullText);

        // Stage 2: Post-process the precursor tree to prepend global paths to every node.
        var pathPrefix = $"{cardName.Replace(' ', '_')}-{clauseIndex}-";
        PrependPathsToPrecursorTree(precursorRoot, pathPrefix);

        // Stage 3: Convert the precursor tree into the final, immutable DTO tree.
        var rootDtoBase = ConvertToDto(precursorRoot, []);

        // Stage 4: Add the final card-specific properties to the root DTO.
        if (rootDtoBase is TokenAnalysisRoot finalRootDto)
        {
            return finalRootDto with { CardName = cardName, ClauseIndex = clauseIndex };
        }

        // This should not happen if the logic is correct.
        throw new InvalidOperationException("The root of the DTO tree was not a TokenAnalysisRoot.");
    }

    /// <summary>
    /// Recursively traverses the precursor tree and prepends the given prefix to every node's Path.
    /// </summary>
    static void PrependPathsToPrecursorTree(PrecursorNode node, string prefix)
    {
        node.Path = prefix + node.Path;
        foreach (var child in node.Children)
        {
            PrependPathsToPrecursorTree(child, prefix);
        }
    }

    // --- Stage 1: Core Logic for Building the Precursor Tree ---

    static PrecursorNode CreatePrecursorFor(IndexedPropertyCapture propCapture, string originalFullText)
    {
        var val = propCapture.Value;

        if (val is TokenUnitOneOf tuOneOf) 
            return CreatePrecursorForTokenUnitOneOf(propCapture, tuOneOf, originalFullText);

        if (val is TokenUnitDistilled tuDistilled) 
            return CreatePrecursorForTokenUnitDistilled(propCapture, tuDistilled, originalFullText);

        if (val is TokenUnit tokenUnit) 
            return CreatePrecursorForTokenUnit(propCapture, tokenUnit, originalFullText);

        if (val is ManyOf manyOf) 
            return CreatePrecursorForManyOf(propCapture, manyOf, originalFullText);

        if (val is DynamicCapture dynamicCapture) 
            return CreatePrecursorForDynamicCapture(propCapture, dynamicCapture, originalFullText);

        if (propCapture.RegexPropInfo.RegexPropType == RegexPropType.Enum) 
            return CreatePrecursorForEnum(propCapture, originalFullText);

        if (propCapture.RegexPropInfo.RegexPropType == RegexPropType.Bool) 
            return CreatePrecursorForBool(propCapture, originalFullText);

        if (val is PlaceholderCapture placeholder) 
            return CreatePrecursorForPlaceholder(propCapture, placeholder, originalFullText);

        throw new ArgumentException($"Unsupported TokenUnit property type for precursor creation: {val?.GetType().Name}");
    }

    static PrecursorNode CreatePrecursorForRoot(TokenUnit root, string originalFullText)
    {
        var precursor = new PrecursorNode
        {
            OriginalFullText = originalFullText,
            CaptureTextOriginal = originalFullText.Substring(root.Capture.Index, root.Capture.Length),
            Start = root.Capture.Index,
            Length = root.Capture.Length,
            End = root.Capture.Index + root.Capture.Length,
            RootTokenType = root.Type,
            Name = root.Type.Name.ToFriendlyCase(TitleDisplayOption.Title),
            Path = root.Path,
            Palette = TokenTypeRegistry.Palettes[root.Type],
            ElementType = root is DefaultUnmatchedString
                ? TokenAnalysisElementType.UnmatchedTokenUnitRoot
                : TokenAnalysisElementType.TokenUnitRoot
        };

        foreach (var propCapture in root.IndexedPropertyCaptures)
            precursor.Children.Add(CreatePrecursorFor(propCapture, originalFullText));

        return precursor;
    }

    static PrecursorNode CreatePrecursorForTokenUnit(IndexedPropertyCapture propCapture, TokenUnit tokenUnit, string originalFullText)
    {
        var precursor = CreatePrecursorBase(propCapture, originalFullText, TokenAnalysisElementType.TokenUnitBranch);
        precursor.Palette = TokenTypeRegistry.Palettes[tokenUnit.Type];

        foreach (var x in tokenUnit.IndexedPropertyCaptures)
            precursor.Children.Add(CreatePrecursorFor(x, originalFullText));

        return precursor;
    }

    static PrecursorNode CreatePrecursorForTokenUnitOneOf(IndexedPropertyCapture propCapture, TokenUnitOneOf tokenUnitOneOf, string originalFullText)
    {
        var singleTokenCapture = tokenUnitOneOf.IndexedPropertyCaptures.Single();
        var populatedChild = singleTokenCapture.Value;

        if (populatedChild is TokenUnitOneOf) throw new NotImplementedException($"Nested {nameof(TokenUnitOneOf)} children not supported");

        if (populatedChild is TokenUnitDistilled tokenUnitDistilled)
        {
            var precursor = CreatePrecursorBase(propCapture, originalFullText, TokenAnalysisElementType.OneOfItemBranch);
            precursor.Palette = TokenTypeRegistry.Palettes[tokenUnitOneOf.Type];
            precursor.Children.Add(CreatePrecursorForTokenUnitDistilled(singleTokenCapture, tokenUnitDistilled, originalFullText));
            return precursor;
        }

        if (populatedChild is TokenUnit tokenUnit)
        {
            var precursor = CreatePrecursorBase(propCapture, originalFullText, TokenAnalysisElementType.OneOfItemBranch);
            precursor.Palette = TokenTypeRegistry.Palettes[tokenUnitOneOf.Type];
            precursor.Children.Add(CreatePrecursorForTokenUnit(singleTokenCapture, tokenUnit, originalFullText));
            return precursor;
        }

        if (singleTokenCapture.RegexPropInfo.RegexPropType == RegexPropType.Enum)
        {
            var precursor = CreatePrecursorLeaf(propCapture, originalFullText, TokenAnalysisElementType.OneOfItemLeaf);
            SetEnumScalar(precursor, populatedChild);
            return precursor;
        }

        throw new NotImplementedException($"{nameof(TokenUnitOneOf)} only supports {nameof(TokenUnit)} and enum children");
    }

    static PrecursorNode CreatePrecursorForDynamicCapture(IndexedPropertyCapture propCapture, DynamicCapture dynamicCapture, string originalFullText)
    {
        var valueObject = dynamicCapture.ValueObject;
        var precursor = CreatePrecursorBase(propCapture, originalFullText, TokenAnalysisElementType.DynamicCaptureItemBranch);
        precursor.Palette = DeterministicPalette.GetStaticPalette(typeof(DynamicCapture).GetCustomAttribute<ColorAttribute>().Color);

        if (valueObject is TokenUnitOneOf tokenUnitOneOf) 
            precursor.Children.Add(CreatePrecursorForTokenUnitOneOf(propCapture, tokenUnitOneOf, originalFullText));

        else if (valueObject is TokenUnitDistilled tokenUnitDistilled)
            precursor.Children.Add(CreatePrecursorForTokenUnitDistilled(propCapture, tokenUnitDistilled, originalFullText));

        else if (valueObject is TokenUnit tokenUnit) 
            precursor.Children.Add(CreatePrecursorForTokenUnit(propCapture, tokenUnit, originalFullText));

        else if (dynamicCapture.RegexPropType == RegexPropType.Enum)
        {
            var leafPrecursor = CreatePrecursorLeaf(propCapture, originalFullText, TokenAnalysisElementType.DynamicCaptureItemLeaf);
            SetEnumScalar(leafPrecursor, propCapture.Value);
            return leafPrecursor;
        }

        else 
            throw new NotImplementedException($"{nameof(DynamicCapture)} only supports {nameof(TokenUnitOneOf)}, {nameof(TokenUnit)}, and enum children");

        return precursor;
    }

    static PrecursorNode CreatePrecursorForTokenUnitDistilled(IndexedPropertyCapture propCapture, TokenUnitDistilled distilled, string originalFullText)
    {
        var precursor = CreatePrecursorBase(propCapture, originalFullText, TokenAnalysisElementType.TokenUnitDistilledBranch);
        precursor.Palette = TokenTypeRegistry.Palettes[distilled.Type];
        var nonDistilledProps = distilled.IndexedPropertyCaptures.Where(x => !distilled.DistilledVals.ContainsKey(x));

        foreach (var x in nonDistilledProps) 
            precursor.Children.Add(CreatePrecursorFor(x, originalFullText));

        foreach (var (placeholderCap, distilledVals) in distilled.DistilledVals) 
            precursor.Children.Add(CreatePrecursorForDistilledPlaceholder(placeholderCap, distilledVals, originalFullText));

        return precursor;
    }

    static PrecursorNode CreatePrecursorForManyOf(IndexedPropertyCapture propCapture, ManyOf manyOf, string originalFullText)
    {
        var precursor = CreatePrecursorBase(propCapture, originalFullText, TokenAnalysisElementType.ManyOfBranch);
        precursor.Palette = TokenTypeRegistry.Palettes.TryGetValue(manyOf.ItemType, out var p) ? p : DeterministicPalette.GetStaticPalette(typeof(ManyOf).GetCustomAttribute<ColorAttribute>().Color);

        for (int i = 0; i < manyOf.ItemObjects.Count; i++)
        {
            var itemCapture = manyOf.ItemObjects[i];
            var itemPath = propCapture.Path + $"[{i}]";

            if (manyOf.ManyItemVariant == ManyItemVariant.TokenUnit && itemCapture.ItemObject is TokenUnit tokenUnit)
            {
                var itemPrecursor = CreatePrecursorBase(itemCapture.Capture, propCapture.RegexPropInfo.Name + " #" + (i + 1), itemPath, originalFullText, TokenAnalysisElementType.ManyOfItemBranch);
                itemPrecursor.Palette = TokenTypeRegistry.Palettes[tokenUnit.Type];
                var synthesized = new IndexedPropertyCapture(itemCapture, itemPath);
                itemPrecursor.Children.Add(CreatePrecursorFor(synthesized, originalFullText));
                precursor.Children.Add(itemPrecursor);
            }
            else if (manyOf.ManyItemVariant == ManyItemVariant.Enum)
            {
                var itemPrecursor = CreatePrecursorLeaf(itemCapture.Capture, propCapture.RegexPropInfo.Name + " #" + (i + 1), itemPath, i, originalFullText, TokenAnalysisElementType.ManyOfItemLeaf);
                SetEnumScalar(itemPrecursor, itemCapture.ItemObject);
                precursor.Children.Add(itemPrecursor);
            }
            else throw new NotImplementedException($"{nameof(ManyItemVariant)} '{manyOf.ManyItemVariant}' not supported");
        }

        if (manyOf.Conjunction != null)
        {
            var conjunctionPrecursor = CreatePrecursorLeaf(manyOf.ConjunctionCapture, nameof(ManyOf.Conjunction), precursor.Path.Dot(nameof(ManyOf.Conjunction)), manyOf.ItemObjects.Count, originalFullText, TokenAnalysisElementType.ConjunctionLeaf);
            SetEnumScalar(conjunctionPrecursor, manyOf.Conjunction.Value);
            precursor.Children.Add(conjunctionPrecursor);
        }
        return precursor;
    }

    static PrecursorNode CreatePrecursorForDistilledPlaceholder(IndexedPropertyCapture placeholder, Dictionary<RegexPropInfo, object> distilledVals, string originalFullText)
    {
        var precursor = CreatePrecursorBase(placeholder, originalFullText, TokenAnalysisElementType.PlaceholderPrecursorLeaf);
        precursor.Palette = DeterministicPalette.GetFixedRainbowPalette(placeholder.Ordinal);

        foreach (var (distilledProp, value) in distilledVals)
        {
            var distilledPrecursor = CreatePrecursorFromParent(precursor, distilledProp.Name, precursor.Path.Dot(distilledProp.Name), TokenAnalysisElementType.DistilledValueSubLeaf);
            SetDistilledScalar(distilledPrecursor, value);
            precursor.Children.Add(distilledPrecursor);
        }
        return precursor;
    }

    static PrecursorNode CreatePrecursorForEnum(IndexedPropertyCapture propCapture, string originalFullText)
    {
        var precursor = CreatePrecursorLeaf(propCapture, originalFullText, TokenAnalysisElementType.EnumLeaf);
        SetEnumScalar(precursor, propCapture.Value);
        return precursor;
    }

    static PrecursorNode CreatePrecursorForBool(IndexedPropertyCapture propCapture, string originalFullText)
    {
        var precursor = CreatePrecursorLeaf(propCapture, originalFullText, TokenAnalysisElementType.BoolLeaf);
        SetBoolScalar(precursor, (bool)propCapture.Value);
        return precursor;
    }

    static PrecursorNode CreatePrecursorForPlaceholder(IndexedPropertyCapture propCapture, PlaceholderCapture placeholder, string originalFullText)
    {
        var precursor = CreatePrecursorLeaf(propCapture, originalFullText, TokenAnalysisElementType.PlaceholderLeaf);
        SetPlaceholderScalar(precursor, placeholder.Text);
        return precursor;
    }

    private static PrecursorNode CreatePrecursorBase(IndexedPropertyCapture propCapture, string originalFullText, TokenAnalysisElementType elementType) =>
            CreatePrecursorBase(
                propCapture.Capture,
                propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
                propCapture.Path,
                originalFullText,
                elementType
            );

    private static PrecursorNode CreatePrecursorBase(Capture capture, string name, string path, string originalFullText, TokenAnalysisElementType elementType) =>
        new()
        {
            Name = name,
            Path = path,
            OriginalFullText = originalFullText,
            ElementType = elementType,
            Start = capture.Index,
            Length = capture.Length,
            End = capture.Index + capture.Length,
            CaptureTextOriginal = originalFullText.Substring(capture.Index, capture.Length),
        };

    private static PrecursorNode CreatePrecursorLeaf(IndexedPropertyCapture propCapture, string originalFullText, TokenAnalysisElementType elementType) =>
        CreatePrecursorLeaf(
            propCapture.Capture,
            propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence),
            propCapture.Path,
            propCapture.Ordinal,
            originalFullText,
            elementType
        );

    private static PrecursorNode CreatePrecursorLeaf(Capture capture, string name, string path, int ordinal, string originalFullText, TokenAnalysisElementType elementType)
    {
        var precursor = CreatePrecursorBase(capture, name, path, originalFullText, elementType);
        precursor.Palette = DeterministicPalette.GetFixedRainbowPalette(ordinal);
        return precursor;
    }

    private static PrecursorNode CreatePrecursorFromParent(PrecursorNode parent, string name, string path, TokenAnalysisElementType elementType) =>
        new()
        {
            Name = name,
            Path = path,
            ElementType = elementType,
            OriginalFullText = parent.OriginalFullText,
            Start = parent.Start,
            Length = parent.Length,
            End = parent.End,
            Palette = parent.Palette,
            CaptureTextOriginal = parent.CaptureTextOriginal,
        };

    static void SetEnumScalar(PrecursorNode n, object v)
    { n.TerminalValString = v.ToString().ToFriendlyCase(TitleDisplayOption.Lower); n.TerminalType = "enum"; }

    static void SetBoolScalar(PrecursorNode n, bool v)
    { n.TerminalValString = v.ToString().ToLower(); n.TerminalType = "bool"; }

    static void SetPlaceholderScalar(PrecursorNode n, string v)
    { n.TerminalValString = v; n.TerminalType = "placeholder"; }

    static void SetDistilledScalar(PrecursorNode n, object v)
    { n.TerminalValString = v.ToString().ToLower(); var t = v.GetType(); n.TerminalType = "distilled " + (t == typeof(int) ? "int" : t.Name.ToFriendlyCase(TitleDisplayOption.Lower)); }

    // --- Stage 3: Precursor-to-DTO Conversion ---

    private static TokenAnalysisBase ConvertToDto(PrecursorNode precursor, IReadOnlyList<string> collapsedNameChain)
    {
        bool isBranchType = precursor.ElementType.ToString().Contains("Branch") || precursor.ElementType.ToString().Contains("Root");
        bool isCollapsed = isBranchType && precursor.Children.Any() && precursor.Children.All(c => c.ElementType.ToString().Contains("Branch"));
        string finalName = precursor.Name;
        if (isBranchType && collapsedNameChain.Any())
        {
            finalName = $"{string.Join(": ", collapsedNameChain)}: {precursor.Name}";
        }
        List<string> nextNameChain = isCollapsed ? new List<string>(collapsedNameChain) { precursor.Name } : [];
        var dtoChildren = precursor.Children.Select(child => ConvertToDto(child, nextNameChain)).ToList();

        switch (precursor.ElementType)
        {
            case TokenAnalysisElementType.UnmatchedTokenUnitRoot:
            case TokenAnalysisElementType.TokenUnitRoot:
                return new TokenAnalysisRoot
                {
                    // Base Properties
                    Name = finalName,
                    Path = precursor.Path,
                    CaptureTextOriginal = precursor.CaptureTextOriginal,
                    Start = precursor.Start,
                    End = precursor.End,
                    Length = precursor.Length,
                    ElementType = precursor.ElementType,
                    Children = dtoChildren,

                    // Branch Properties
                    Palette = precursor.Palette,
                    IsCollapsed = isCollapsed,

                    // Root Properties
                    OriginalFullText = precursor.OriginalFullText,
                    RootTokenType = precursor.RootTokenType,
                };

            case var e when e.ToString().Contains("Branch"):
                return new TokenAnalysisBranch
                {
                    // Base Properties
                    Name = finalName,
                    Path = precursor.Path,
                    CaptureTextOriginal = precursor.CaptureTextOriginal,
                    Start = precursor.Start,
                    End = precursor.End,
                    Length = precursor.Length,
                    ElementType = precursor.ElementType,
                    Children = dtoChildren,

                    // Branch Properties
                    Palette = precursor.Palette,
                    IsCollapsed = isCollapsed,
                };

            case var e when e.ToString().Contains("SubLeaf"):
                return new TokenAnalysisSubLeaf
                {
                    // Base Properties
                    Name = precursor.Name, // Sub-leaves do not get prepended names
                    Path = precursor.Path,
                    CaptureTextOriginal = precursor.CaptureTextOriginal,
                    Start = precursor.Start,
                    End = precursor.End,
                    Length = precursor.Length,
                    ElementType = precursor.ElementType,
                    Children = dtoChildren,

                    // Leaf Properties
                    Palette = precursor.Palette,
                    TerminalValString = precursor.TerminalValString,
                    TerminalType = precursor.TerminalType,
                };

            case var e when e.ToString().Contains("Leaf"):
                return new TokenAnalysisLeaf
                {
                    // Base Properties
                    Name = precursor.Name, // Leaves do not get prepended names
                    Path = precursor.Path,
                    CaptureTextOriginal = precursor.CaptureTextOriginal,
                    Start = precursor.Start,
                    End = precursor.End,
                    Length = precursor.Length,
                    ElementType = precursor.ElementType,
                    Children = dtoChildren,

                    // Leaf Properties
                    Palette = precursor.Palette,
                    TerminalValString = precursor.TerminalValString,
                    TerminalType = precursor.TerminalType,
                };

            default:
                throw new InvalidOperationException($"Unsupported ElementType for DTO conversion: {precursor.ElementType}");
        }
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

    EnumLeaf,
    BoolLeaf,
    OneOfItemLeaf,
    ManyOfItemLeaf,
    PlaceholderLeaf,
    PlaceholderPrecursorLeaf,
    DynamicCaptureItemLeaf,
    ConjunctionLeaf,

    DistilledValueSubLeaf,
}