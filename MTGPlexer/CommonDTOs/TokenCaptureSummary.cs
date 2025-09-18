namespace MTGPlexer.CommonDTOs;

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
    public TokenCaptureSummary Parent { get; private set; }
    public List<TokenCaptureSummary> Children { get; private set; } = [];

    /// <summary>
    /// Single entry point public constructor for top-level TokenUnit roots.
    /// </summary>
    public TokenCaptureSummary(TokenUnit token, string originalFullText)
    {
        SetCaptureInfo(token.Capture, originalFullText);
        OriginalFullText = originalFullText;
        Name = token.Type.Name.ToFriendlyCase(TitleDisplayOption.Title);
        Path = token.Path;
        UnderlinePalette = TokenTypeRegistry.Palettes[token.Type];
        token.IndexedPropertyCaptures.ForEach(x => Children.Add(new(this, x)));

        ElementType = token is DefaultUnmatchedString ?
            TokenAnalysisElementType.UnmatchedTokenUnitRoot
            : TokenAnalysisElementType.TokenUnitRoot;
    }

    /// <summary>
    /// Private constructor for IndexedPropertyCapture props. These represent either branches that 
    /// contain child TokenCaptureSummaries (TokenUnit, TokenUnitOneOf, TokenUnitDistilled, ManyOf, 
    /// and DynamicCapture), or leaves which represent scalar values in their parent TokenCaptureSummary 
    /// (Enum, Bool, PlaceholderCapture). Note that PlaceholderCapture are technically leaf values,
    /// since they represent the final level of text digestion "visible" to the TokenUnit hierarchy,
    /// although they can be digested further ("distilled") into the actual scalar values that we care
    /// about. 
    /// </summary>
    TokenCaptureSummary(TokenCaptureSummary parentSummary, IndexedPropertyCapture propCapture)
    {
        SetCommonChildInfo(parentSummary, propCapture.Capture);
        Name = propCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Sentence);
        Path = propCapture.Path;
        var val = propCapture.Value;

        // ----------------------------------------------------------------------------------
        // Branches (parents to further children)
        // ----------------------------------------------------------------------------------

        if (val is TokenUnitOneOf tokenUnitOneOf)
        {
            ElementType = TokenAnalysisElementType.TokenUnitOneOfBranch;
            Children.Add(new(this, tokenUnitOneOf, propCapture));
            UnderlinePalette = TokenTypeRegistry.Palettes[tokenUnitOneOf.Type];
        }

        else if (val is TokenUnitDistilled tokenUnitDistilled)
        {
            ElementType = TokenAnalysisElementType.TokenUnitDistilledBranch;

            // The TokenUnitDistilled instance may have prop captures not associated with distilled values,
            // and those should be added as children in the normal manner (recursively calling the current method
            // w/ an IndexedPropertyCapture).
            tokenUnitDistilled.IndexedPropertyCaptures
                .Where(x => !tokenUnitDistilled.DistilledVals.ContainsKey(x))
                .ToList()
                .ForEach(x => Children.Add(new(this, x)));

            // Handle placeholder captures with distilled values separately
            foreach (var (placeholderPropCapture, distilledPropVals) in tokenUnitDistilled.DistilledVals)
                Children.Add(new(this, placeholderPropCapture, distilledPropVals));

            UnderlinePalette = TokenTypeRegistry.Palettes[tokenUnitDistilled.Type];
        }

        // base type for the types above, so handled last
        // note: TokenUnitDistilled instances are handled here too b/c they don't need special treatment at this level
        else if (val is TokenUnit tokenUnit) 
        {
            ElementType = TokenAnalysisElementType.TokenUnitBranch;
            tokenUnit.IndexedPropertyCaptures.ForEach(x => Children.Add(new(this, x)));
            UnderlinePalette = TokenTypeRegistry.Palettes[tokenUnit.Type];
        }

        else if (val is ManyOf manyOf)
        {
            ElementType = TokenAnalysisElementType.ManyOfBranch;

            for (int i = 0; i < manyOf.ItemObjects.Count; i++)
                Children.Add(new(this, manyOf.ItemObjects[i], manyOf, propCapture, i));

            if (manyOf.Conjunction != null)
                Children.Add(new(this, manyOf));

            // If ManyOf.ItemType is a TokenUnit, its palette will be in the TokenTypeRegistry
            // If it's an enum we use the default ManyOf color (grey)
            UnderlinePalette =
                TokenTypeRegistry.Palettes.TryGetValue(manyOf.ItemType, out var palette) ? palette
                : DeterministicPalette.GetStaticPalette(typeof(ManyOf).GetCustomAttribute<ColorAttribute>().Color);
        }

        else if (val is DynamicCapture dynamicCapture)
        {
            ElementType = TokenAnalysisElementType.DynamicCaptureBranch;
            Children.Add(new(this, dynamicCapture, propCapture));
            UnderlinePalette = DeterministicPalette.GetStaticPalette(typeof(DynamicCapture).GetCustomAttribute<ColorAttribute>().Color);
        }

        // ----------------------------------------------------------------------------------
        // Leaves (scalar values within parent summaries)
        // ----------------------------------------------------------------------------------

        else if (propCapture.RegexPropInfo.RegexPropType == RegexPropType.Enum)
        {
            ElementType = TokenAnalysisElementType.EnumLeaf;
            SetEnumScalar(val);
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal);
        }

        else if (propCapture.RegexPropInfo.RegexPropType == RegexPropType.Bool)
        {
            ElementType = TokenAnalysisElementType.BoolLeaf;
            SetBoolScalar((bool)val);
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal);
        }

        // This is only expected to be reached for placeholders not associated with a TokenUnitDistilled instance
        else if (val is PlaceholderCapture placeholderCapture)
        {
            ElementType = TokenAnalysisElementType.PlaceholderLeaf;
            SetPlaceholderScalar(placeholderCapture.Text);
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal);
        }

    }

    /// <summary>
    /// Private constructor for TokenUnit child items.
    /// </summary>
    TokenCaptureSummary(TokenCaptureSummary parentSummary, TokenUnit tokenUnit, IndexedPropertyCapture propertyCapture)
    {
        ElementType = TokenAnalysisElementType.TokenUnitBranch;
        SetCommonChildInfo(parentSummary, propertyCapture.Capture);
        Name = propertyCapture.RegexPropInfo.Name.ToFriendlyCase(TitleDisplayOption.Title);
        Path = propertyCapture.Path;
        UnderlinePalette = TokenTypeRegistry.Palettes[tokenUnit.Type];
        tokenUnit.IndexedPropertyCaptures.ForEach(x => Children.Add(new(this, x)));
    }

    /// <summary>
    /// Private constructor for a single PlaceholderCapture property on a TokenUnitDistilled instance. Distilled values are added as children to the placeholder.
    /// Note that although this placeholder has children, it's treated as a scalar value because it represents the final level of text digestion "visible" 
    /// to the TokenUnit hierarchy. Its children are "silent", their info only visible to downstream analytic consumers that care about this granularity.
    /// </summary>
    TokenCaptureSummary(TokenCaptureSummary parentSummary, IndexedPropertyCapture placeholderPropertyCapture, Dictionary<RegexPropInfo, object> nonNullDistilledVals)
    {
        ElementType = TokenAnalysisElementType.PlaceholderPrecursorBranch;
        SetCommonChildInfo(parentSummary, placeholderPropertyCapture.Capture);
        SetPlaceholderScalar((string)placeholderPropertyCapture.Value);
        Name = placeholderPropertyCapture.RegexPropInfo.Name;
        Path = placeholderPropertyCapture.Path;
        OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(placeholderPropertyCapture.Ordinal);

        foreach (var (distilledProp, value) in nonNullDistilledVals)
            Children.Add(new(this, distilledProp, value));
    }

    /// <summary>
    /// Private constructor for a single DistilledValue associated with a PlaceholderCapture parent.
    /// </summary>
    TokenCaptureSummary(TokenCaptureSummary parentSummary, RegexPropInfo distilledProp, object distilledPropVal)
    {
        ElementType = TokenAnalysisElementType.DistilledValueSubLeaf;

        // We must reuse the parent PlaceholderCapture's info to set this child's capture info, b/c the
        // distilled child prop has no direct (or determinable) relationship with the text capture that spawned it.
        // This means all sibling children of a given PlaceholderCapture share the same capture info.

        CopyInfoFromParentSummary(parentSummary);
        SetDistilledScalar(distilledPropVal);
        Name = distilledProp.Name;
        Path = parentSummary.Path.Dot(Name);
    }

    /// <summary>
    /// Private constructor for OneOf child item.
    /// </summary>
    TokenCaptureSummary(TokenCaptureSummary parentSummary, TokenUnitOneOf tokenUnitOneOf, IndexedPropertyCapture propCapture)
    {
        // Note: ElementType must be set after we determine whether this OneOf item branches or terminates

        SetCommonChildInfo(parentSummary, propCapture.Capture);
        var singleTokenCapture = tokenUnitOneOf.IndexedPropertyCaptures.Single();
        var populatedChild = singleTokenCapture.Value;
        Name = propCapture.RegexPropInfo.Name;

        if (populatedChild is TokenUnitOneOf)
            throw new NotImplementedException($"Nested {nameof(TokenUnitOneOf)} children not supported");
        else if (populatedChild is TokenUnit tokenUnit)
        {
            ElementType = TokenAnalysisElementType.OneOfItemBranch;
            Children.Add(new(this, tokenUnit, singleTokenCapture));
        }
        else if (propCapture.RegexPropInfo.RegexPropType == RegexPropType.Enum)
        {
            ElementType = TokenAnalysisElementType.OneOfItemLeaf;
            SetEnumScalar(populatedChild);
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal);
        }
        else
            throw new NotImplementedException($"{nameof(TokenUnitOneOf)} only supports {nameof(TokenUnit)} and enum children");
    }

    /// <summary>
    /// Private constructor for a single ManyOf child item.
    /// </summary>
    TokenCaptureSummary(TokenCaptureSummary parentSummary, ManyItemCapture itemCapture, ManyOf manyOfParent, IndexedPropertyCapture propCapture, int itemNumber)
    {
        // Note: ElementType must be set after we determine whether this ManyOf item branches or terminates

        SetCommonChildInfo(parentSummary, itemCapture.Capture);
        Name = propCapture.RegexPropInfo.Name + " #" + (itemNumber + 1);
        Path = propCapture.Path + $"[{itemNumber}]";

        if (manyOfParent.ManyItemVariant == ManyItemVariant.TokenUnit && itemCapture.ItemObject is TokenUnit tokenUnit)
        {
            ElementType = TokenAnalysisElementType.ManyOfItemBranch;
            UnderlinePalette = TokenTypeRegistry.Palettes[tokenUnit.Type];
            IndexedPropertyCapture synthesizedIndexedPropertyCapture = new(itemCapture, Path);
            Children.Add(new(this, synthesizedIndexedPropertyCapture));
        }
        else if (manyOfParent.ManyItemVariant == ManyItemVariant.Enum)
        {
            ElementType = TokenAnalysisElementType.ManyOfItemLeaf;
            SetEnumScalar(itemCapture.ItemObject);
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(itemNumber);
        }
        else
            throw new NotImplementedException($"{nameof(ManyItemVariant)} '{manyOfParent.ManyItemVariant}' not supported");
    }

    /// <summary>
    /// Private constructor for the Conjunction property in a ManyOf item.
    /// </summary>
    TokenCaptureSummary(TokenCaptureSummary parentSummary, ManyOf manyOfParent)
    {
        ElementType = TokenAnalysisElementType.ConjunctionLeaf;
        SetCommonChildInfo(parentSummary, manyOfParent.ConjunctionCapture);
        SetEnumScalar(manyOfParent.Conjunction.Value);
        Name = nameof(ManyOf.Conjunction);
        Path = parentSummary.Path.Dot(Name);
        OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(manyOfParent.ItemObjects.Count + 1);
    }

    /// <summary>
    /// Private constructor for DynamicCapture child item.
    /// </summary>
    TokenCaptureSummary(TokenCaptureSummary parentSummary, DynamicCapture dynamicCapture, IndexedPropertyCapture propCapture)
    {
        // Note: ElementType must be set after we determine whether this Dynamic item branches or terminates

        SetCommonChildInfo(parentSummary, propCapture.Capture);

        if (dynamicCapture.ValueObject is TokenUnitOneOf tokenUnitOneOf)
        {
            ElementType = TokenAnalysisElementType.DynamicCaptureItemBranch;
            Children.Add(new(this, tokenUnitOneOf, propCapture));
        }
        else if (dynamicCapture.ValueObject is TokenUnit tokenUnit)
        {
            ElementType = TokenAnalysisElementType.DynamicCaptureItemBranch;
            Children.Add(new(this, tokenUnit, propCapture));
        }
        else if (dynamicCapture.RegexPropType == RegexPropType.Enum)
        {
            ElementType = TokenAnalysisElementType.DynamicCaptureItemLeaf;
            SetEnumScalar(propCapture.Value);
            OverlinePalette = DeterministicPalette.GetFixedRainbowPalette(propCapture.Ordinal);
        }
        else
            throw new NotImplementedException($"{nameof(TokenUnitOneOf)} only supports {nameof(TokenUnit)}, {nameof(TokenUnit)}, and enum children");
    }

    void SetCommonChildInfo(TokenCaptureSummary parentSummary, Capture capture)
    {
        Parent = parentSummary;
        OriginalFullText = parentSummary.OriginalFullText;
        SetCaptureInfo(capture, OriginalFullText);
    }

    void SetCaptureInfo(Capture capture, string originalFullText)
    {
        Start = capture.Index;
        Length = capture.Length;
        End = Start + Length;
        CaptureTextLower = capture.Value;
        CaptureTextOriginal = originalFullText.Substring(Start, Length);
    }

    void CopyInfoFromParentSummary(TokenCaptureSummary parentSummary)
    {
        Parent = parentSummary;
        OriginalFullText = parentSummary.OriginalFullText;
        Start = parentSummary.Start;
        Length = parentSummary.Length;
        End = parentSummary.End;
        CaptureTextLower = parentSummary.CaptureTextLower;
        CaptureTextOriginal = parentSummary.CaptureTextOriginal;
    }

    void SetEnumScalar(object enumVal)
    {
        TerminalValString = enumVal.ToString().ToFriendlyCase(TitleDisplayOption.Lower);
        TerminalType = "enum";
    }

    void SetBoolScalar(bool boolVal)
    {
        TerminalValString = boolVal.ToString().ToLower();
        TerminalType = "bool";
    }

    void SetPlaceholderScalar(string placeholderVal)
    {
        TerminalValString = placeholderVal;
        TerminalType = "placeholder";
    }

    void SetDistilledScalar(object distilledVal)
    {
        TerminalValString = distilledVal.ToString().ToLower();

        var type = distilledVal.GetType();
        TerminalType = "distilled ";
        TerminalType += type == typeof(int) ? "int" : type.Name.ToFriendlyCase(TitleDisplayOption.Lower);
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