using System.Xml.Linq;
using DotBahn.Shared.Parsing;
using DotBahn.Timetables.Internal.Contracts;

namespace DotBahn.Timetables.Internal.Parsing;

/// <summary>
/// Manual XML parser for <see cref="TimetableResponseContract"/>.
/// </summary>
internal sealed class TimetableXmlParser : IParser<TimetableResponseContract> {
    private static readonly XName StationName = "timetable";
    private static readonly XName StopName = "s";
    private static readonly XName TripInfoName = "tl";
    private static readonly XName ArrivalName = "ar";
    private static readonly XName DepartureName = "dp";
    private static readonly XName MessageName = "m";

    /// <inheritdoc />
    public TimetableResponseContract Parse(string input) {
        if (string.IsNullOrWhiteSpace(input)) {
            return new TimetableResponseContract();
        }

        var doc = XDocument.Parse(input);
        var root = doc.Root;
        if (root == null || root.Name != StationName) {
            return new TimetableResponseContract();
        }

        var contract = new TimetableResponseContract {
            Station = (string)root.Attribute("station")!,
            Stops = root.Elements(StopName).Select(ParseStop).ToList()
        };

        return contract;
    }

    /// <summary>
    /// Parses a single stop element.
    /// </summary>
    private static StopDataContract ParseStop(XElement stopElement) {
        var tripInfo = stopElement.Element(TripInfoName) is { } tripInfoElement
            ? ParseTripInfo(tripInfoElement) : null;

        var arrival = stopElement.Element(ArrivalName) is { } arrivalElement
            ? ParseEvent(arrivalElement) : null;

        var departure = stopElement.Element(DepartureName) is { } departureElement
            ? ParseEvent(departureElement) : null;

        var messages = stopElement.Elements(MessageName).Select(ParseMessage).ToList();

        return new StopDataContract {
            Id = (string)stopElement.Attribute("id")!,
            Eva = (string)stopElement.Attribute("eva")!,
            TripInfo = tripInfo,
            Arrival = arrival,
            Departure = departure,
            Messages = messages.Count > 0 ? messages : null,
        };
    }

    /// <summary>
    /// Parses a trip label element.
    /// </summary>
    private static TripInfoContract ParseTripInfo(XElement element) => new() {
        Category = (string?)element.Attribute("c"),
        Number = (string?)element.Attribute("n"),
        Owner = (string?)element.Attribute("o"),
        TripType = (string?)element.Attribute("t"),
        FilterFlags = (string?)element.Attribute("f"),
    };

    /// <summary>
    /// Parses an arrival or departure event element.
    /// </summary>
    private static EventContract ParseEvent(XElement element) => new() {
        PlannedTime = (string?)element.Attribute("pt"),
        PlannedPlatform = (string?)element.Attribute("pp"),
        PlannedPath = (string?)element.Attribute("ppth"),
        PlannedStatus = (string?)element.Attribute("ps"),
        ChangedTime = (string?)element.Attribute("ct"),
        ChangedPlatform = (string?)element.Attribute("cp"),
        ChangedPath = (string?)element.Attribute("cpth"),
        ChangedStatus = (string?)element.Attribute("cs"),
        IsHidden = (string?)element.Attribute("hi"),
        Line = (string?)element.Attribute("l"),
        PlannedDistantEndpoint = (string?)element.Attribute("pde"),
        ChangedDistantEndpoint = (string?)element.Attribute("cde"),
        Wings = (string?)element.Attribute("wings"),
        Transition = (string?)element.Attribute("tra"),
    };

    /// <summary>
    /// Parses a message element.
    /// </summary>
    private static MessageContract ParseMessage(XElement element) => new() {
        Id = (string?)element.Attribute("id"),
        Type = (string?)element.Attribute("t"),
        Timestamp = (string?)element.Attribute("ts"),
        ValidFrom = (string?)element.Attribute("from"),
        ValidTo = (string?)element.Attribute("to"),
        Code = (string?)element.Attribute("c"),
        IsInternal = (string?)element.Attribute("int"),
        IsDeleted = (string?)element.Attribute("del"),
        ExternalCategory = (string?)element.Attribute("ec"),
        Priority = (string?)element.Attribute("priority"),
        Owner = (string?)element.Attribute("owner"),
        Category = (string?)element.Attribute("cat"),
        Text = element.Value,
    };
}
