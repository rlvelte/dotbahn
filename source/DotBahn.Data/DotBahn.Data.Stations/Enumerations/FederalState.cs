using DotBahn.Modules.Shared.Enumerations;

namespace DotBahn.Data.Stations.Enumerations;

/// <summary>
/// Represents the federal states of Germany.
/// </summary>
public enum FederalState {
    /// <summary>State of Baden-Württemberg.</summary>
    [AssociatedValue("Baden-Württemberg")]
    BadenWuerttemberg,

    /// <summary>State of Bavaria.</summary>
    [AssociatedValue("Bayern")]
    Bavaria,

    /// <summary>State of Berlin.</summary>
    [AssociatedValue("Berlin")]
    Berlin,

    /// <summary>State of Brandenburg.</summary>
    [AssociatedValue("Brandenburg")]
    Brandenburg,

    /// <summary>State of Bremen.</summary>
    [AssociatedValue("Bremen")]
    Bremen,

    /// <summary>State of Hamburg.</summary>
    [AssociatedValue("Hamburg")]
    Hamburg,

    /// <summary>State of Hesse.</summary>
    [AssociatedValue("Hessen")]
    Hesse,

    /// <summary>State of Mecklenburg-Vorpommern.</summary>
    [AssociatedValue("Mecklenburg-Vorpommern")]
    MecklenburgVorpommern,

    /// <summary>State of Lower Saxony.</summary>
    [AssociatedValue("Niedersachsen")]
    LowerSaxony,

    /// <summary>State of North Rhine-Westphalia.</summary>
    [AssociatedValue("Nordrhein-Westfalen")]
    NorthRhineWestphalia,

    /// <summary>State of Rhineland-Palatinate.</summary>
    [AssociatedValue("Rheinland-Pfalz")]
    RhinelandPalatinate,

    /// <summary>State of Saarland.</summary>
    [AssociatedValue("Saarland")]
    Saarland,

    /// <summary>State of Saxony.</summary>
    [AssociatedValue("Sachsen")]
    Saxony,

    /// <summary>State of Saxony-Anhalt.</summary>
    [AssociatedValue("Sachsen-Anhalt")]
    SaxonyAnhalt,

    /// <summary>State of Schleswig-Holstein.</summary>
    [AssociatedValue("Schleswig-Holstein")]
    SchleswigHolstein,

    /// <summary>State of Thuringia.</summary>
    [AssociatedValue("Thüringen")]
    Thuringia
}