using System.Text.Json.Serialization;

namespace DotBahn.Stations.Models.Enumerations;

/// <summary>
/// Represents the federal states of Germany.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<FederalState>))]
public enum FederalState {
    /// <summary>
    /// State of Baden-Württemberg.
    /// </summary>
    [JsonStringEnumMemberName("Baden-Württemberg")]
    BadenWuerttemberg,

    /// <summary>
    /// State of Bavaria.
    /// </summary>
    [JsonStringEnumMemberName("Bayern")]
    Bavaria,

    /// <summary>
    /// State of Berlin.
    /// </summary>
    [JsonStringEnumMemberName("Berlin")]
    Berlin,

    /// <summary>
    /// State of Brandenburg.
    /// </summary>
    [JsonStringEnumMemberName("Brandenburg")]
    Brandenburg,

    /// <summary>
    /// State of Bremen.
    /// </summary>
    [JsonStringEnumMemberName("Bremen")]
    Bremen,

    /// <summary>
    /// State of Hamburg.
    /// </summary>
    [JsonStringEnumMemberName("Hamburg")]
    Hamburg,

    /// <summary>
    /// State of Hesse.
    /// </summary>
    [JsonStringEnumMemberName("Hessen")]
    Hesse,

    /// <summary>
    /// State of Mecklenburg-Vorpommern.
    /// </summary>
    [JsonStringEnumMemberName("Mecklenburg-Vorpommern")]
    MecklenburgVorpommern,

    /// <summary>
    /// State of Lower Saxony.
    /// </summary>
    [JsonStringEnumMemberName("Niedersachsen")]
    LowerSaxony,

    /// <summary>
    /// State of North Rhine-Westphalia.
    /// </summary>
    [JsonStringEnumMemberName("Nordrhein-Westfalen")]
    NorthRhineWestphalia,

    /// <summary>
    /// State of Rhineland-Palatinate.
    /// </summary>
    [JsonStringEnumMemberName("Rheinland-Pfalz")]
    RhinelandPalatinate,

    /// <summary>
    /// State of Saarland.
    /// </summary>
    [JsonStringEnumMemberName("Saarland")]
    Saarland,

    /// <summary>
    /// State of Saxony.
    /// </summary>
    [JsonStringEnumMemberName("Sachsen")]
    Saxony,

    /// <summary>
    /// State of Saxony-Anhalt.
    /// </summary>
    [JsonStringEnumMemberName("Sachsen-Anhalt")]
    SaxonyAnhalt,

    /// <summary>
    /// State of Schleswig-Holstein.
    /// </summary>
    [JsonStringEnumMemberName("Schleswig-Holstein")]
    SchleswigHolstein,

    /// <summary>
    /// State of Thuringia.
    /// </summary>
    [JsonStringEnumMemberName("Thüringen")]
    Thuringia,

    /// <summary>
    /// There is no further information available.
    /// </summary>
    Unknown
}
