using System.Diagnostics;
using DotBahn.Ai.Input;
using DotBahn.Ai.Utilities;
using DotBahn.Boards;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Common.Telemetry;
using DotBahn.Facilities;
using DotBahn.Facilities.Models;
using DotBahn.Facilities.Models.Enumerations;
using DotBahn.Stations;
using DotBahn.Stations.Models;

namespace DotBahn.Ai;

/// <summary>
/// AI tools backed by the DotBahn clients.
/// </summary>
/// <param name="stations">The client used for station lookups.</param>
/// <param name="boards">The client used for arrival and departure boards.</param>
/// <param name="facilities">The client used for facility status lookups.</param>
public sealed class Tools(IStationClient stations, IBoardsClient boards, IFacilityClient facilities) {
    /// <summary>
    /// Searches for railway stations by name.
    /// </summary>
    /// <param name="query">Station name or fragment to search for.</param>
    /// <param name="maxResults">Maximum number of stations to return (1-20).</param>
    /// <param name="ct">Token to cancel the request.</param>
    /// <returns>A result containing the matching stations, or an invalid_argument error.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    public Task<ToolResult<IReadOnlyList<Station>>> SearchStationsAsync(string query, int maxResults = 20, CancellationToken ct = default) =>
        ExecuteToolAsync("dotbahn_search_stations", async token => {
            var input = new StationSearchInput { Query = query, MaxResults = maxResults };
            input.Validate();

            return await stations.GetStationsAsync(new StationQuery().WithNames(input.Query).LimitTo(input.MaxResults), token).ConfigureAwait(false);
        }, ct);

    /// <summary>
    /// Gets the operational status of facilities at a station.
    /// </summary>
    /// <param name="stationNumber">DB station number returned by search_stations.</param>
    /// <param name="type">Optional facility type filter.</param>
    /// <param name="state">Optional facility state filter.</param>
    /// <param name="maxResults">Maximum number of facilities to return (1-25).</param>
    /// <param name="ct">Token to cancel the request.</param>
    /// <returns>A result containing the matching facilities, or an invalid_argument error.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    public Task<ToolResult<IReadOnlyList<Facility>>> GetFacilityStatusAsync(int stationNumber, FacilityType? type = null, FacilityState? state = null, int maxResults = 25, CancellationToken ct = default) =>
        ExecuteToolAsync<IReadOnlyList<Facility>>("dotbahn_get_facility_status", async token => {
            var input = new FacilityStatusInput { StationNumber = stationNumber, Type = type, State = state, MaxResults = maxResults };
            input.Validate();

            var query = new FacilityQuery().AtStation(input.StationNumber);
            if (input.Type.HasValue) {
                query.WithType(input.Type.Value);
            }

            if (input.State.HasValue) {
                query.WithState(input.State.Value);
            }

            var result = await facilities.GetFacilitiesAsync(query, token).ConfigureAwait(false);
            return result.Take(input.MaxResults).ToList();
        }, ct);

    /// <summary>
    /// Gets the departure board for a station.
    /// </summary>
    /// <param name="stationId">Identifier returned by search_stations.</param>
    /// <param name="stationKind">Identifier scheme used by stationId.</param>
    /// <param name="start">Optional start of the time window in ISO 8601 format.</param>
    /// <param name="maxEntries">Maximum number of board entries to return (1-25).</param>
    /// <param name="ct">Token to cancel the request.</param>
    /// <returns>A result containing the departure board, or an invalid_argument error.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    public Task<ToolResult<Board<DepartureEntry>>> GetDeparturesAsync(string stationId, StationIdentifierKind stationKind, DateTime? start = null, int maxEntries = 10, CancellationToken ct = default) =>
        ExecuteToolAsync("dotbahn_get_departures", async token => {
            var input = new BoardToolInput { StationId = stationId, StationKind = stationKind, Start = start, MaxEntries = maxEntries };
            input.Validate();

            return await boards.GetDeparturesAsync(CreateBoardQuery(input), token).ConfigureAwait(false);
        }, ct);

    /// <summary>
    /// Gets the arrival board for a station.
    /// </summary>
    /// <param name="stationId">Identifier returned by search_stations.</param>
    /// <param name="stationKind">Identifier scheme used by stationId.</param>
    /// <param name="start">Optional start of the time window in ISO 8601 format.</param>
    /// <param name="maxEntries">Maximum number of board entries to return (1-25).</param>
    /// <param name="ct">Token to cancel the request.</param>
    /// <returns>A result containing the arrival board, or an invalid_argument error.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    public Task<ToolResult<Board<ArrivalEntry>>> GetArrivalsAsync(string stationId, StationIdentifierKind stationKind, DateTime? start = null, int maxEntries = 10, CancellationToken ct = default) =>
        ExecuteToolAsync("dotbahn_get_arrivals", async token => {
            var input = new BoardToolInput { StationId = stationId, StationKind = stationKind, Start = start, MaxEntries = maxEntries };
            input.Validate();

            return await boards.GetArrivalsAsync(CreateBoardQuery(input), token).ConfigureAwait(false);
        }, ct);

    private static async Task<ToolResult<T>> ExecuteToolAsync<T>(string name, Func<CancellationToken, Task<T>> operation, CancellationToken ct) {
        using var activity = DotBahnTelemetry.StartAiTool(name);
        var start = Stopwatch.GetTimestamp();

        try {
            var value = await operation(ct).ConfigureAwait(false);
            DotBahnTelemetry.RecordAiTool(Stopwatch.GetElapsedTime(start).TotalSeconds, name);
            return ToolResult<T>.Ok(value);
        } catch (OperationCanceledException) {
            DotBahnTelemetry.RecordAiTool(Stopwatch.GetElapsedTime(start).TotalSeconds, name);
            throw;
        } catch (Exception ex) {
            var error = ToolErrorUtil.FromException(ex);

            activity?.SetStatus(ActivityStatusCode.Error, error.Message);
            activity?.SetTag(DotBahnTelemetry.ErrorCodeTag, error.Code);

            DotBahnTelemetry.RecordAiTool(Stopwatch.GetElapsedTime(start).TotalSeconds, name, errorCode: error.Code);
            return ToolResult<T>.Fail(error);
        }
    }

    private static BoardQuery CreateBoardQuery(BoardToolInput input) {
        var query = new BoardQuery().ForNext(input.MaxEntries);
        if (input.Start.HasValue) {
            query = query.StartingAt(input.Start.Value);
        }

        return input.StationKind switch {
            StationIdentifierKind.Eva => query.ForEvaNumbers(input.StationId),
            StationIdentifierKind.Stada => query.ForStada(input.StationId),
            StationIdentifierKind.Dhid => query.ForDhids(input.StationId),
            _ => throw new ArgumentOutOfRangeException(nameof(input))
        };
    }
}
