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
                children.Add(GetNodeForNavigaton(this, propertyNib.Navigation));
            else
                children.Add(new TextNode(this, nib.Text));
    }

    /// <summary>Picks the concrete <see cref="RegexNode"/> subtype for a property nib, based on its underlying CLR type (enum, bool, int, nested token unit, etc.).</summary>
    public static RegexNode GetNodeForNavigaton(RegexNode parentNode, Navigation navigation)
    {
        return navigation.NodeType switch
        {
            { } t when t == typeof(UnmatchedString) => new UnmatchedGlyphNode(parentNode, navigation),
            { } t when typeof(OneOfBase).IsAssignableFrom(t) => new GlyphOneOfNode(parentNode, navigation),
            { } t when typeof(CompoundOfBase).IsAssignableFrom(t) => new GlyphCompoundOfNode(parentNode, navigation),
            { } t when typeof(DynamicGlyph).IsAssignableFrom(t) => new DynamicGlyphNode(parentNode, navigation),
            { } t when IsClosedGeneric(t, typeof(CompoundOfSecondItem<>)) => new GlyphCompoundOfSecondItemNode(parentNode, navigation),
            { } t when IsClosedGeneric(t, typeof(OptionalOf<>)) => new OptionalGlyphNode(parentNode, navigation),
            { } t when typeof(Glyph).IsAssignableFrom(t) && navigation.Quantifier == Glyphotype.Quantifier.Optional => new OptionalGlyphNode(parentNode, navigation),
            { } t when typeof(Glyph).IsAssignableFrom(t) => new GlyphNode(parentNode, navigation),
            { IsEnum: true } => new EnumNode(parentNode, navigation),
            { } t when t == typeof(bool) => new BoolNode(parentNode, navigation),
            { } t when t == typeof(int) => new IntNode(parentNode, navigation),
            _ => throw new Exception($"'{navigation.NodeType}' is not a valid {nameof(PropertyNib)} type")
        };
    }

    static bool IsClosedGeneric(Type type, Type openGeneric) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == openGeneric;

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
