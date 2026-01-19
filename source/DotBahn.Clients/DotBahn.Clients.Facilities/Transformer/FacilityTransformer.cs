using DotBahn.Clients.Facilities.Contracts;
using DotBahn.Data.Facilities.Enumerations;
using DotBahn.Data.Facilities.Models;
using DotBahn.Data.Shared.Enumerations;
using DotBahn.Data.Shared.Models;
using DotBahn.Data.Shared.Transformer;

namespace DotBahn.Clients.Facilities.Transformer;

/// <summary>
/// Transforms facility contracts into domain models.
/// </summary>
public class FacilityTransformer : ITransformer<IEnumerable<Facility>, IEnumerable<FacilityContract>> {
    /// <inheritdoc />
    public IEnumerable<Facility> Transform(in IEnumerable<FacilityContract> contract) =>
        contract.Where(c => c is { Longitude: not null, Latitude: not null }).Select(TransformFacility);

    /// <summary>
    /// Transforms the <see cref="FacilityContract"/> into its domain model.
    /// </summary>
    /// <param name="contract">The contract to transform.</param>
    /// <returns>The transformed model.</returns>
    private static Facility TransformFacility(FacilityContract contract) => new() {
        EquipmentNumber = contract.EquipmentNumber,
        Type = EnumExtensions.FromAssociatedValue(contract.Type, FacilityType.Unknown),
        Description = contract.Description,
        State = EnumExtensions.FromAssociatedValue(contract.State, FacilityState.Unknown),
        StateExplanation = contract.StateExplanation,
        StationNumber = contract.StationNumber,
        Coordinates = TransformCoordinates(contract.Longitude!.Value, contract.Latitude!.Value),
        OperatorName = contract.OperatorName
    };

    /// <summary>
    /// Transforms coordinate values into a <see cref="Coordinates"/> model.
    /// </summary>
    /// <param name="longitude">The longitude value.</param>
    /// <param name="latitude">The latitude value.</param>
    /// <returns>The transformed coordinates.</returns>
    private static Coordinates TransformCoordinates(double longitude, double latitude) => new() {
            Longitude = longitude,
            Latitude = latitude 
    };
}