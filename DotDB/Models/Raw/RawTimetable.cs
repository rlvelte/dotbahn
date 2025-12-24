namespace DotDB.Models.Raw;

/// <summary>
/// Raw XML timetable response
/// </summary>
public class RawTimetable
{
    public string Station { get; set; }
    public List<RawStopData> Stops { get; set; }
}