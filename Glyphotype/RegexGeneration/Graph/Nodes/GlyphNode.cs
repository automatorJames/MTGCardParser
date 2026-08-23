using Glyphotype.GlyphPrimitives.Internal;

namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a <see cref="Glyph"/> type (root or nested): its children mirror the type's declared
/// <see cref="Nib"/>s, one child per literal text nib or per property nib's own node type.
/// </summary>
public class GlyphNode : NamedGroupNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.Token;

    public GlyphNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        foreach (var nib in Navigation.GlyphTypeConfiguration.Nibs)
            if (nib is PropertyNib propertyNib)
            {
                Navigation navigation = new(propertyNib, GetDescriptiveChildName(propertyNib));
                children.Add(GetNodeForNavigaton(this, navigation));
            }
            else
                children.Add(new TextNode(this, nib.Text));
    }

    /// <summary>
    /// Several container <see cref="Glyph"/> types declare structurally-named properties
    /// (Alternative1, FirstItem, Item, FusedContent, ...) that are boring on their own - the useful
    /// name is either the property's own CLR type (<see cref="OneOfBase"/>, <see cref="GlyphFused{T}"/>,
    /// since each holds exactly one distinctly-typed value) or this node's own capture-group name,
    /// which the property's value merely elaborates on (<see cref="CompoundOf{T}"/>/<see cref="ManyOf{T}"/>
    /// and their internal "second item" helpers). Returns null for anything else, which keeps the
    /// property's own declared name.
    /// </summary>
    string GetDescriptiveChildName(PropertyNib propertyNib)
    {
        if (typeof(OneOfBase).IsAssignableFrom(Navigation.NodeType) || IsClosedGeneric(Navigation.NodeType, typeof(GlyphFused<>)))
            return Navigation.GetRegexSafeTypeName(Nullable.GetUnderlyingType(propertyNib.Type) ?? propertyNib.Type);

        if (IsClosedGeneric(Navigation.NodeType, typeof(CompoundOf<>)) && propertyNib.Name == nameof(CompoundOf<object>.FirstItem))
            return $"{Name}Primary";

        if (IsClosedGeneric(Navigation.NodeType, typeof(CompoundOfSecondItem<>)) && propertyNib.Name == nameof(CompoundOfSecondItem<object>.Item))
            return $"{ParentNode.Name}Secondary";

        if (IsClosedGeneric(Navigation.NodeType, typeof(ManyOf<>)) && propertyNib.Name == nameof(ManyOf<object>.FirstItem))
            return $"{Name}First";

        if (IsClosedGeneric(Navigation.NodeType, typeof(ManyOf<>)) && propertyNib.Name == nameof(ManyOf<object>.LastItem))
            return $"{Name}Last";

        if (IsClosedGeneric(Navigation.NodeType, typeof(ManyOfSecondItem<>)) && propertyNib.Name == nameof(ManyOfSecondItem<object>.Item))
            return $"{ParentNode.Name}Middle";

        return null;
    }

    static bool IsClosedGeneric(Type type, Type openGeneric) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == openGeneric;

    /// <summary>Picks the concrete <see cref="RegexNode"/> subtype for a property nib, based on its underlying CLR type (enum, bool, int, nested token unit, etc.).</summary>
    public static RegexNode GetNodeForNavigaton(RegexNode parentNode, Navigation navigation)
    {
        return navigation.NodeType switch
        {
            { } t when t == typeof(UnmatchedString) => new UnmatchedGlyphNode(parentNode, navigation),
            { } t when typeof(OneOfBase).IsAssignableFrom(t) => new GlyphOneOfNode(parentNode, navigation),
            { } t when typeof(DynamicGlyph).IsAssignableFrom(t) => new DynamicGlyphNode(parentNode, navigation),
            { } t when typeof(Glyph).IsAssignableFrom(t) => new GlyphNode(parentNode, navigation),
            { IsEnum: true } => new EnumNode(parentNode, navigation),
            { } t when t == typeof(bool) => new BoolNode(parentNode, navigation),
            { } t when t == typeof(int) => new IntNode(parentNode, navigation),
            _ => throw new Exception($"'{navigation.NodeType}' is not a valid {nameof(PropertyNib)} type")
        };
    }

    /// <summary>Instantiates this node's <see cref="Glyph"/> type and hydrates every child named-group property from <paramref name="captureTrace"/>'s <see cref="CaptureTrace.CaptureContext"/>. Returns false if any required child fails to hydrate.</summary>
    public virtual bool TryHydrate(CaptureTrace captureTrace, out Glyph glyph)
    {
        glyph = null;
        var instance = (Glyph)Activator.CreateInstance(Navigation.NodeType);

        foreach (var child in NamedGroupChildren)
        {
            var setResult = child.SetPropertyValue(instance, captureTrace.CaptureContext);

            // In a normal Glyph, all non-optional children must be matched for the whole Glyph to match
            if (!setResult && !child.Navigation.IsOptional)
                return false;
        }

        instance.CaptureContext = captureTrace.CaptureContext;
        glyph = instance;

        return true;
    }

    /// <inheritdoc/>
    protected override object GetValue(CaptureTrace captureTrace)
    {
        TryHydrate(captureTrace, out var glyph);
        return glyph;
    }

    /// <summary>
    /// Validates the capture structure based on two rules:
    /// 1. There must be at least one CaptureNode present.
    /// 2. All CaptureNodes must form a single, contiguous block (no gaps allowed).
    /// </summary>
    /// <returns>True if exactly one contiguous group of CaptureNodes exists.</returns>
    public bool ValidateCapturePropertiesAreContiguous()
    {
        int groups = 0;
        bool inGroup = false;

        foreach (var node in Children)
        {
            if (node is NamedGroupNode)
            {
                // If we hit a capture and weren't already in a group, 
                // we've discovered a new "island"
                if (!inGroup)
                {
                    groups++;
                    inGroup = true;
                }
            }
            else
            {
                // Any non-capture node (TextNode, etc.) terminates the current group
                inGroup = false;
            }
        }

        // Returns true only if we found exactly one contiguous cluster
        return groups == 1;
    }
}
