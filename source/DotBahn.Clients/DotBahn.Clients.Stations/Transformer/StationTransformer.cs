using DotBahn.Clients.Stations.Contracts;
using DotBahn.Data.Shared.Transformer;
using DotBahn.Data.Stations.Enumerations;
using DotBahn.Data.Stations.Models;

namespace DotBahn.Clients.Stations.Transformer;

/// <summary>
/// Transforms station contracts into domain models.
/// </summary>
public class StationTransformer : ITransformer<IEnumerable<Station>, StationsResponseContract> {
    /// <inheritdoc />
    public IEnumerable<Station> Transform(in StationsResponseContract contract) =>
        contract.Stations.Select(TransformStation);

    /// <summary>
    /// Transforms the <see cref="StationContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model.</returns>
    private static Station TransformStation(StationContract contract) => new() {
        Number = contract.Number,
        Name = contract.Name,
        Category = TransformCategory(contract.Category),
        Address = TransformAddress(contract.MailingAddress),
        RegionalArea = TransformRegionalArea(contract.RegionalArea),
        Ril100Identifiers = contract.Ril100Identifiers.Select(TransformRil100Identifier).ToList(),
        EvaNumbers = contract.EvaNumbers.Select(TransformEvaNumber).ToList(),
        Services = TransformServices(contract)
    };

    /// <summary>
    /// Transforms the category number into the <see cref="StationCategory"/> enum.
    /// </summary>
    /// <param name="category">The category number (1-7).</param>
    /// <returns>The corresponding station category.</returns>
    private static StationCategory TransformCategory(int category) =>
        category is >= 1 and <= 7 ? (StationCategory)category : StationCategory.Unknown;

    /// <summary>
    /// Transforms the <see cref="MailingAddressContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model, or null if the contract is null.</returns>
    private static StationAddress? TransformAddress(MailingAddressContract? contract) {
        if (contract == null) {
            return null;
        }

        return new StationAddress {
            Street = contract.Street,
            ZipCode = contract.ZipCode,
            City = contract.City
        };
    }

    /// <summary>
    /// Transforms the <see cref="RegionalAreaContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model, or null if the contract is null.</returns>
    private static RegionalArea? TransformRegionalArea(RegionalAreaContract? contract) {
        if (contract == null) {
            return null;
        }

        return new RegionalArea {
            Number = contract.Number,
            Name = contract.Name,
            ShortName = contract.ShortName
        };
    }

    /// <summary>
    /// Transforms the <see cref="Ril100IdentifierContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model.</returns>
    private static Ril100Identifier TransformRil100Identifier(Ril100IdentifierContract contract) => new() {
        Identifier = contract.RilIdentifier,
        IsMain = contract.IsMain
    };

    /// <summary>
    /// Transforms the <see cref="EvaNumberContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model.</returns>
    private static EvaNumber TransformEvaNumber(EvaNumberContract contract) => new() {
        Number = contract.Number,
        IsMain = contract.IsMain,
        Coordinates = TransformCoordinates(contract.GeographicCoordinates)
    };

    /// <summary>
    /// Transforms the <see cref="GeographicCoordinatesContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model, or null if the contract is null or has insufficient coordinates.</returns>
    private static Coordinates? TransformCoordinates(GeographicCoordinatesContract? contract) {
        if (contract?.Coordinates == null || contract.Coordinates.Count < 2) {
            return null;
        }

        return new Coordinates {
            Longitude = contract.Coordinates[0],
            Latitude = contract.Coordinates[1]
        };
    }

    /// <summary>
    /// Transforms the service flags from the <see cref="StationContract"/> into a <see cref="StationServices"/> model.
    /// </summary>
    /// <param name="contract">The station contract containing service flags.</param>
    /// <returns>The transformed services model.</returns>
    private static StationServices TransformServices(StationContract contract) => new() {
        HasParking = contract.HasParking,
        HasBicycleParking = contract.HasBicycleParking,
        HasPublicFacilities = contract.HasPublicFacilities,
        HasLockerSystem = contract.HasLockerSystem,
        HasStepFreeAccess = contract.HasStepFreeAccess,
        HasSteplessAccess = contract.HasSteplessAccess,
        HasTravelCenter = contract.HasTravelCenter,
        HasTravelNecessities = contract.HasTravelNecessities,
        HasMobilityService = contract.HasMobilityService,
        HasWiFi = contract.HasWiFi
    };
}
