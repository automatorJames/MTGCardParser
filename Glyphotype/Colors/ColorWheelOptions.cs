namespace Glyphotype.Colors;

public record ColorWheelOptions
(
    double StartingDegree = 280.0,
    double MaxDegreesPerRotation = 180.0,
    bool Reverse = false
);