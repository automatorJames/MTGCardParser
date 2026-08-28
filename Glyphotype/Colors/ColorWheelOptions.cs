using Glyphotype.PresentationRules;

namespace Glyphotype.Colors;

/// <summary>
/// Which arc of the color wheel a rainbow set is drawn from. The defaults live in
/// <see cref="RainbowPaletteKnobs"/> alongside every other "what does a rainbow look like" knob.
/// </summary>
public record ColorWheelOptions
(
    double StartingDegree = RainbowPaletteKnobs.WheelStartingDegree,
    double MaxDegreesPerRotation = RainbowPaletteKnobs.WheelMaxDegreesPerRotation,
    bool Reverse = false
);