using DotBahn.Timetables.Models.Enumerations;

namespace DotBahn.Timetables.Models;

/// <summary>
/// Identifies a train with its category, number, operator, and service type information
/// </summary>
public class TrainLabel {
    /// <summary>
    /// Trip category (e.g. "ICE", "RE", "RB", "S")
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Trip/train number (e.g. "4523")
    /// </summary>
    public required string Number { get; init; }

    /// <summary>
    /// A unique short-form code intended to map a trip to a specific railway undertaking (EVU)
    /// </summary>
    public required string Owner { get; init; }

    /// <summary>
    /// Customer-facing line name (e.g. "RE14" or "S1")
    /// </summary>
    public string? Line { get; init; }

    /// <summary>
    /// Trip type indicating the kind of service
    /// </summary>
    public TripType? Type { get; init; }

    /// <summary>
    /// Filter flags indicating service classification (e.g. "N" for regional, "F" for long-distance, "D" for international)
    /// </summary>
    public string? FilterFlags { get; init; }

    /// <summary>
    /// Display the name for the train, preferring customer-facing line.
    /// </summary>
    public string DisplayName => !string.IsNullOrWhiteSpace(Line) ? $"{Line}" : $"{Category}{Number}";
}
