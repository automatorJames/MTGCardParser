namespace Glyphotype.StaticRegistry;

public record GlyphTypeConfiguration
(
    Type Type,
    Nib[] Nibs,
    Joiner ChildJoiner
);