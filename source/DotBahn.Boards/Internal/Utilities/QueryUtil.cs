using System.Globalization;
using System.Runtime.CompilerServices;
using DotBahn.Boards.Internal.Contracts;

namespace DotBahn.Boards.Internal.Utilities;

/// <summary>
/// Maps public board queries to internal OpenBoards request contracts.
/// </summary>
internal static class QueryUtil {
    /// <summary>
    /// Maps a <see cref="BoardQuery"/> to the internal arrival request contract.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static OpenArrivalBoardRequestContract ToArrivalContract(BoardQuery query) => new() {
        StopPlaceOrStation = MapStopPlaceOrStation(query),
        TimeWindow = MapTimeWindow(query),
        TransportTypes = query.TransportTypes.Count > 0 ? [.. query.TransportTypes] : null,
        Languages = query.Languages.Count > 0 ? [.. query.Languages] : null,
        MaxEntriesPerTransportType = query.MaxEntriesPerTransportType,
        MaxViaStops = query.MaxViaStops ?? 3,
        NoCancellations = query.NoCancellations,
        OnlyReplacementTransports = query.OnlyReplacementTransports,
        SortBy = query.SortBy,
        IncludeLineColours = query.IncludeLineColours,
        IncludePlatformAccessibility = query.IncludePlatformAccessibility
    };

    /// <summary>
    /// Maps a <see cref="BoardQuery"/> to the internal departure request contract.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static OpenDepartureBoardRequestContract ToDepartureContract(BoardQuery query) => new() {
        StopPlaceOrStation = MapStopPlaceOrStation(query),
        TimeWindow = MapTimeWindow(query),
        TransportTypes = query.TransportTypes.Count > 0 ? [.. query.TransportTypes] : null,
        Languages = query.Languages.Count > 0 ? [.. query.Languages] : null,
        MaxEntriesPerTransportType = query.MaxEntriesPerTransportType,
        MaxViaStops = query.MaxViaStops ?? 3,
        NoCancellations = query.NoCancellations,
        OnlyReplacementTransports = query.OnlyReplacementTransports,
        SortBy = query.SortBy,
        IncludeLineColours = query.IncludeLineColours,
        IncludePlatformAccessibility = query.IncludePlatformAccessibility
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StopPlaceOrStationRequestContract MapStopPlaceOrStation(BoardQuery query) => new() {
        EvaNumbers = query.EvaNumbers is { Count: > 0 } ? [.. query.EvaNumbers] : null,
        Stada = query.StadaId,
        Dhids = query.Dhids is { Count: > 0 } ? [.. query.Dhids] : null,
        Platforms = query.Platforms is { Count: > 0 } ? [.. query.Platforms] : null,
        BoardConfigurations = query.BoardConfigurations is { Count: > 0 } ? [.. query.BoardConfigurations] : null
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TimeWindowContract? MapTimeWindow(BoardQuery query) {
        var start = query.Start;
        var end = query.End;
        var count = query.EntryCount;

        if (!start.HasValue && !end.HasValue && !count.HasValue) {
            return null;
        }

        return new TimeWindowContract {
            TimeStart = start.HasValue ? FormatDateTime(start.Value) : null,
            TimeEnd = end.HasValue ? FormatDateTime(end.Value) : null,
            EntryCount = count
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string FormatDateTime(DateTime value) {
        var utc = value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime();
        return utc.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
    }
}
