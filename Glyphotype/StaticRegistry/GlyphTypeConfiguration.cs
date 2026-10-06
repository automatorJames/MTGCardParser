namespace Glyphotype.StaticRegistry;

/// <param name="Type">The glyph type.</param>
/// <param name="Nibs">Its nibs as the engine matches them - each <see cref="PluralNib"/> flattened into its inner nib and a suffix (see <see cref="PluralNib.Flatten"/>).</param>
/// <param name="ChildJoiner">What separates its nibs.</param>
/// <param name="AuthoredNibs">Its nibs as written, for showing the source they came from.</param>
public record GlyphTypeConfiguration
(
    Type Type,
    Nib[] Nibs,
    Joiner ChildJoiner,
    Nib[] AuthoredNibs
);