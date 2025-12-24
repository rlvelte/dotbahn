using DotDB.Models;
using DotDB.Models.Additional;
using DotDB.Models.Enumerations;
using DotDB.Models.Raw;

namespace DotDB.Client.Parser;

/// <summary>
/// Helper methods for parsing and transforming API data
/// </summary>
public static class TimetableTransformer
{
    /// <summary>
    /// Parses time string from API format (YYMMDDhhmm) to DateTime
    /// </summary>
    public static DateTime ParseApiTime(string timeStr)
    {
        if (string.IsNullOrEmpty(timeStr) || timeStr.Length != 10)
            throw new ArgumentException("Invalid time format", nameof(timeStr));

        int year = 2000 + int.Parse(timeStr.Substring(0, 2));
        int month = int.Parse(timeStr.Substring(2, 2));
        int day = int.Parse(timeStr.Substring(4, 2));
        int hour = int.Parse(timeStr.Substring(6, 2));
        int minute = int.Parse(timeStr.Substring(8, 2));

        return new DateTime(year, month, day, hour, minute);
    }

    /// <summary>
    /// Calculates delay in minutes between two dates
    /// </summary>
    public static int CalculateDelay(DateTime planned, DateTime actual)
    {
        return (int)Math.Round((actual - planned).TotalMinutes);
    }

    /// <summary>
    /// Parses path string (pipe-separated stations) to list
    /// </summary>
    public static List<string> ParsePath(string pathStr)
    {
        if (string.IsNullOrEmpty(pathStr))
            return new List<string>();

        return pathStr.Split('|')
                      .Where(s => !string.IsNullOrWhiteSpace(s))
                      .ToList();
    }

    /// <summary>
    /// Parses event status from string
    /// </summary>
    public static EventStatus ParseEventStatus(string status)
    {
        return status switch
        {
            "p" => EventStatus.Planned,
            "a" => EventStatus.Added,
            "c" => EventStatus.Cancelled,
            _ => EventStatus.Planned
        };
    }

    /// <summary>
    /// Parses message type from string
    /// </summary>
    public static MessageType ParseMessageType(string type)
    {
        return type switch
        {
            "q" => MessageType.Quality,
            "d" => MessageType.Delay,
            "i" => MessageType.Info,
            "r" => MessageType.Disruption,
            "u" => MessageType.CauseOfDelay,
            "f" => MessageType.FreeText,
            "c" => MessageType.Connection,
            "h" => MessageType.Himmel,
            _ => MessageType.Info
        };
    }

    /// <summary>
    /// Parses trip type from string
    /// </summary>
    public static TripType ParseTripType(string type)
    {
        return type switch
        {
            "p" => TripType.Passenger,
            "e" => TripType.Empty,
            "z" => TripType.Freight,
            _ => TripType.Passenger
        };
    }

    /// <summary>
    /// Transforms raw event data to managed EventInfo
    /// </summary>
    public static EventInfo TransformEvent(RawArrival rawEvent)
    {
        if (rawEvent == null || string.IsNullOrEmpty(rawEvent.Pt))
            return null;

        var plannedTime = ParseApiTime(rawEvent.Pt);
        DateTime? changedTime = !string.IsNullOrEmpty(rawEvent.Ct) 
            ? ParseApiTime(rawEvent.Ct) 
            : null;

        var plannedPath = ParsePath(rawEvent.Ppth);
        var changedPath = ParsePath(rawEvent.Cpth);

        return new EventInfo
        {
            Time = new TimeInfo
            {
                Planned = plannedTime,
                Changed = changedTime,
                Delay = changedTime.HasValue 
                    ? CalculateDelay(plannedTime, changedTime.Value) 
                    : null,
                IsDelayed = changedTime.HasValue && changedTime.Value > plannedTime
            },
            Platform = new PlatformInfo
            {
                Planned = rawEvent.Pp ?? string.Empty,
                Changed = rawEvent.Cp,
                HasChanged = !string.IsNullOrEmpty(rawEvent.Cp) && 
                             rawEvent.Cp != rawEvent.Pp
            },
            Path = new PathInfo
            {
                Planned = plannedPath,
                Changed = changedPath.Count > 0 ? changedPath : null,
                HasChanged = changedPath.Count > 0 && 
                             !plannedPath.SequenceEqual(changedPath)
            },
            Status = ParseEventStatus(rawEvent.Ps ?? "p"),
            IsCancelled = rawEvent.Cs == "c",
            IsHidden = rawEvent.Hi == "1",
            Line = rawEvent.L,
            DistantEndpoint = new DistantEndpoint
            {
                Planned = rawEvent.Pde,
                Changed = rawEvent.Cde
            }
        };
    }

    /// <summary>
    /// Transforms raw departure to managed EventInfo
    /// </summary>
    public static EventInfo TransformEvent(RawDeparture rawEvent)
    {
        if (rawEvent == null || string.IsNullOrEmpty(rawEvent.Pt))
            return null;

        var plannedTime = ParseApiTime(rawEvent.Pt);
        DateTime? changedTime = !string.IsNullOrEmpty(rawEvent.Ct) 
            ? ParseApiTime(rawEvent.Ct) 
            : null;

        var plannedPath = ParsePath(rawEvent.Ppth);
        var changedPath = ParsePath(rawEvent.Cpth);

        return new EventInfo
        {
            Time = new TimeInfo
            {
                Planned = plannedTime,
                Changed = changedTime,
                Delay = changedTime.HasValue 
                    ? CalculateDelay(plannedTime, changedTime.Value) 
                    : null,
                IsDelayed = changedTime.HasValue && changedTime.Value > plannedTime
            },
            Platform = new PlatformInfo
            {
                Planned = rawEvent.Pp ?? string.Empty,
                Changed = rawEvent.Cp,
                HasChanged = !string.IsNullOrEmpty(rawEvent.Cp) && 
                             rawEvent.Cp != rawEvent.Pp
            },
            Path = new PathInfo
            {
                Planned = plannedPath,
                Changed = changedPath.Count > 0 ? changedPath : null,
                HasChanged = changedPath.Count > 0 && 
                             !plannedPath.SequenceEqual(changedPath)
            },
            Status = ParseEventStatus(rawEvent.Ps ?? "p"),
            IsCancelled = rawEvent.Cs == "c",
            IsHidden = rawEvent.Hi == "1",
            Line = rawEvent.L,
            DistantEndpoint = new DistantEndpoint
            {
                Planned = rawEvent.Pde,
                Changed = rawEvent.Cde
            }
        };
    }

    /// <summary>
    /// Transforms raw message to managed Message
    /// </summary>
    public static Message TransformMessage(RawMessage rawMsg)
    {
        return new Message
        {
            Id = rawMsg.Id,
            Type = ParseMessageType(rawMsg.T),
            Code = rawMsg.C,
            Text = rawMsg.Text,
            ValidFrom = !string.IsNullOrEmpty(rawMsg.From) 
                ? ParseApiTime(rawMsg.From) 
                : null,
            ValidTo = !string.IsNullOrEmpty(rawMsg.To) 
                ? ParseApiTime(rawMsg.To) 
                : null,
            Timestamp = !string.IsNullOrEmpty(rawMsg.Ts) 
                ? ParseApiTime(rawMsg.Ts) 
                : null,
            Priority = !string.IsNullOrEmpty(rawMsg.Priority) 
                ? int.Parse(rawMsg.Priority) 
                : null,
            IsInternal = rawMsg.Int == "1",
            IsDeleted = rawMsg.Del == "1"
        };
    }

    /// <summary>
    /// Transforms raw stop data to managed TrainStop
    /// </summary>
    public static TrainStop TransformStop(RawStopData rawStop, string stationName)
    {
        var arrival = TransformEvent(rawStop.Ar);
        var departure = TransformEvent(rawStop.Dp);
        var messages = rawStop.M?.Select(TransformMessage).ToList() 
                       ?? new List<Message>();

        bool hasDelay = (arrival?.Time.IsDelayed ?? false) || 
                        (departure?.Time.IsDelayed ?? false);

        bool hasPlatformChange = (arrival?.Platform.HasChanged ?? false) || 
                                 (departure?.Platform.HasChanged ?? false);

        bool hasRouteChange = (arrival?.Path.HasChanged ?? false) || 
                              (departure?.Path.HasChanged ?? false);

        bool isCancelled = (arrival?.IsCancelled ?? false) || 
                           (departure?.IsCancelled ?? false);

        return new TrainStop
        {
            Id = rawStop.Id,
            Station = new StationInfo
            {
                Eva = rawStop.Eva ?? string.Empty,
                Name = stationName
            },
            Train = new TrainInfo
            {
                Category = rawStop.Tl.C,
                Number = rawStop.Tl.N,
                Type = ParseTripType(rawStop.Tl.T),
                Owner = rawStop.Tl.O,
                FilterFlags = rawStop.Tl.F
            },
            Arrival = arrival,
            Departure = departure,
            Messages = messages,
            HasDelay = hasDelay,
            HasPlatformChange = hasPlatformChange,
            HasRouteChange = hasRouteChange,
            IsCancelled = isCancelled
        };
    }

    /// <summary>
    /// Transforms raw timetable to managed Timetable
    /// </summary>
    public static Timetable TransformTimetable(RawTimetable rawTimetable)
    {
        var stops = rawTimetable.Stops
                                .Select(stop => TransformStop(stop, rawTimetable.Station))
                                .ToList();

        return new Timetable
        {
            Station = rawTimetable.Station,
            Stops = stops,
            LastUpdated = DateTime.Now
        };
    }
}