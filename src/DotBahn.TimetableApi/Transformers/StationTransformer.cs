using DotBahn.Parsing.Base;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Models;

namespace DotBahn.TimetableApi.Transformers;

/// <summary>
/// Transformer for converting <see cref="StationContract"/> to <see cref="StationInfo"/>.
/// </summary>
public class StationTransformer : ITransformer<StationContract, StationInfo> {
    /// <inheritdoc />
    public StationInfo Transform(StationContract contract) {
        return new StationInfo {
            Eva = contract.Eva,
            Name = contract.Name
        };
    }
}
