using DotBahn.Boards.Internal.Contracts;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Common.Transformer;

namespace DotBahn.Boards.Internal.Transformers;

/// <summary>
/// Transforms an arrival-board contract into the public domain model.
/// </summary>
internal sealed class ArrivalBoardTransformer : TransformerBase, ITransformer<Board<ArrivalEntry>, ArrivalBoardResponseContract> {
    /// <inheritdoc />
    public Board<ArrivalEntry> Transform(ArrivalBoardResponseContract contract) {
        ArgumentNullException.ThrowIfNull(contract);
        return TransformBoard(contract.StopPlaces ?? [], contract.Stations ?? [], contract.Administrations ?? [], contract.Messages, contract.Meta, contract.DisruptionCommunications ?? [], contract.Arrivals ?? [], TransformArrival);
    }

    private static ArrivalEntry TransformArrival(ArrivalContract contract, TransformContext ctx) => new() {
        Id = contract.ArrivalId,
        StopPlace = ResolveStopPlace(contract.EvaNumber, ctx) ?? new StopPlace { EvaNumber = contract.EvaNumber, Name = contract.EvaNumber },
        Journey = TransformJourneyInfo(contract.JourneyId, contract.JourneyInfo, ctx),
        Time = BuildTimeChanged(contract.TimeSchedule, contract.Time, contract.TimeType),
        Platform = BuildPlatformChanged(contract.PlatformSchedule, contract.Platform),
        Transport = TransformTransport(contract.Transport, ctx),
        Messages = ResolveMessages(contract.Messages, ctx.Messages).ToList(),
        Cancelled = contract.Cancelled,
        Additional = contract.Additional,
        OnDemand = contract.OnDemand,
        NoPassengerChange = contract.NoPassengerChange,
        VehicleSequenceAvailable = contract.VehicleSequenceAvailable ?? false,
        Codeshares = TransformCodeshares(contract.Codeshares),
        ContinuationBy = (contract.ContinuationBy ?? []).Select(c => TransformTransportReference(c, ctx)).ToList(),
        ReliefBy = (contract.ReliefBy ?? []).Select(c => TransformTransportReference(c, ctx)).ToList(),
        ReliefFor = (contract.ReliefFor ?? []).Select(c => TransformTransportReference(c, ctx)).ToList(),
        ReplacedBy = (contract.ReplacedBy ?? []).Select(c => TransformTransportReference(c, ctx)).ToList(),
        ReplacementFor = (contract.ReplacementFor ?? []).Select(c => TransformTransportReference(c, ctx)).ToList(),
        TravelsWith = (contract.TravelsWith ?? []).Select(c => TransformTransportReference(c, ctx)).ToList(),
        PastDisruptions = contract.PastDisruptions ?? false
    };
}
