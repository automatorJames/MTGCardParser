using Glyphotype.GlyphPrimitives.Internal;

namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents a <see cref="Glyph"/> type (root or nested): its children mirror the type's declared
/// <see cref="Nib"/>s, one child per literal text nib or per property nib's own node type.
/// </summary>
public class GlyphNode : NamedGroupNode
{
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
                // Pass nib itself, not nib.Text - TextNode's own optional-wrapping depends on nib's actual
                // runtime type (e.g. OptionalNib), which passing just the string would silently discard via
                // the implicit string->Nib conversion (producing a fresh, always-non-optional plain Nib).
                children.Add(new TextNode(this, nib));
    }

    /// <summary>
    /// Picks the concrete <see cref="RegexNode"/> subtype for a property nib, based on its underlying CLR
    /// type (enum, bool, int, nested token unit, etc.). Every arm here reflects a genuine behavioral
    /// difference (how children are reflected, how hydration validates, how a leading separator renders);
    /// a CLR shape with no such difference - a plain nested <see cref="Glyph"/>, whether or not it's
    /// wrapped in <see cref="OptionalOf{T}"/>, decorated with <see cref="OptionalAttribute"/>, or a
    /// <see cref="CompoundOfBase"/> - falls through to the plain <see cref="GlyphNode"/> case, since
    /// <see cref="GroupNode.IsNullable"/> (driven by <see cref="Navigation.Quantifier"/>) and
    /// <see cref="Navigation"/> itself already carry everything else about it needs.
    /// </summary>
    public static RegexNode GetNodeForNavigaton(RegexNode parentNode, Navigation navigation)
    {
        return navigation.NodeType switch
        {
            { } t when t == typeof(UnmatchedString) => new UnmatchedGlyphNode(parentNode, navigation),
            { } t when typeof(OneOfBase).IsAssignableFrom(t) => new GlyphOneOfNode(parentNode, navigation),
            { } t when typeof(DynamicGlyph).IsAssignableFrom(t) => new DynamicGlyphNode(parentNode, navigation),
            { } t when IsClosedGeneric(t, typeof(CompoundOfSecondItem<>)) => new CommaSeparatedItemNode(parentNode, navigation),
            { } t when IsClosedGeneric(t, typeof(ManyOfSecondItem<>)) => new CommaSeparatedItemNode(parentNode, navigation),
            { } t when typeof(Glyph).IsAssignableFrom(t) => new GlyphNode(parentNode, navigation),
            { IsEnum: true } => new EnumNode(parentNode, navigation),
            { } t when t == typeof(bool) => new BoolNode(parentNode, navigation),
            { } t when t == typeof(int) => new IntNode(parentNode, navigation),
            _ => throw new Exception($"'{navigation.NodeType}' is not a valid {nameof(PropertyNib)} type")
        };
    }

    static bool IsClosedGeneric(Type type, Type openGeneric) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == openGeneric;

    /// <summary>Instantiates this node's <see cref="Glyph"/> type and hydrates every child named-group property, scoped to <paramref name="captureTrace"/> (see <see cref="NamedGroupNode.SetPropertyValue"/>). Returns false if any required child fails to hydrate.</summary>
    public virtual bool TryHydrate(CaptureTrace captureTrace, out Glyph glyph)
    {
        glyph = null;
        var instance = (Glyph)Activator.CreateInstance(Navigation.NodeType);

        foreach (var child in NamedGroupChildren)
        {
            var setResult = child.SetPropertyValue(instance, captureTrace);

            // In a normal Glyph, all non-nullable children must be matched for the whole Glyph to match.
            // Checked via the child's own IsNullable (not child.Navigation.IsOptional) because a node type
            // can be structurally optional independent of its Navigation - e.g. BoolNode hardcodes its own
            // Quantifier rather than deriving it from Navigation, so Navigation.IsOptional would be false
            // even though the node itself is perfectly capable of matching nothing.
            if (!setResult && !child.IsNullable)
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
