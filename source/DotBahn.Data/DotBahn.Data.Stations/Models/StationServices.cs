namespace DotBahn.Data.Stations.Models;

/// <summary>
/// Services and facilities available at a railway station.
/// </summary>
public class StationServices {
    /// <summary>
    /// Whether the station has car parking facilities.
    /// </summary>
    public bool HasParking { get; init; }

    /// <summary>
    /// Whether the station has bicycle parking facilities.
    /// </summary>
    public bool HasBicycleParking { get; init; }

    /// <summary>
    /// Whether the station has public restroom facilities.
    /// </summary>
    public bool HasPublicFacilities { get; init; }

    /// <summary>
    /// Whether the station has luggage lockers.
    /// </summary>
    public bool HasLockerSystem { get; init; }

    /// <summary>
    /// Whether the station has step-free access to platforms (elevators, ramps).
    /// </summary>
    public bool HasStepFreeAccess { get; init; }

    /// <summary>
    /// Whether the station has stepless access (level boarding).
    /// </summary>
    public bool HasSteplessAccess { get; init; }

    /// <summary>
    /// Whether the station has a DB travel center (Reisezentrum) for ticket sales and customer service.
    /// </summary>
    public bool HasTravelCenter { get; init; }

    /// <summary>
    /// Whether the station has shops for travel necessities.
    /// </summary>
    public bool HasTravelNecessities { get; init; }

    /// <summary>
    /// Whether the station offers mobility assistance services for passengers with reduced mobility.
    /// </summary>
    public bool HasMobilityService { get; init; }

    /// <summary>
    /// Whether the station has public WiFi access.
    /// </summary>
    public bool HasWiFi { get; init; }
}
