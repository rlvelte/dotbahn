using System.Xml.Linq;
using DotDB.Models.Raw;

namespace DotDB.Client.Parser;

/// <summary>
/// XML parser for Deutsche Bahn timetable responses
/// </summary>
public static class TimetableXmlParser
{
    /// <summary>
    /// Parses XML response to raw timetable structure
    /// </summary>
    public static RawTimetable ParseXml(string xmlText)
    {
        var doc = XDocument.Parse(xmlText);
        var root = doc.Root;

        var station = root.Attribute("station")?.Value ?? string.Empty;

        var stops = root.Elements("s")
                        .Select(ParseStopElement)
                        .ToList();

        return new RawTimetable
        {
            Station = station,
            Stops = stops
        };
    }

    private static RawStopData ParseStopElement(XElement stopEl)
    {
        var id = stopEl.Attribute("id")?.Value;
        var eva = stopEl.Attribute("eva")?.Value;

        var tlEl = stopEl.Element("tl");
        var tl = new RawTripLabel
        {
            F = tlEl?.Attribute("f")?.Value,
            T = tlEl?.Attribute("t")?.Value,
            O = tlEl?.Attribute("o")?.Value,
            C = tlEl?.Attribute("c")?.Value,
            N = tlEl?.Attribute("n")?.Value
        };

        var arEl = stopEl.Element("ar");
        RawArrival ar = null;
        if (arEl != null)
        {
            ar = new RawArrival
            {
                Pt = arEl.Attribute("pt")?.Value,
                Pp = arEl.Attribute("pp")?.Value,
                Ps = arEl.Attribute("ps")?.Value,
                Ct = arEl.Attribute("ct")?.Value,
                Cp = arEl.Attribute("cp")?.Value,
                Cs = arEl.Attribute("cs")?.Value,
                Hi = arEl.Attribute("hi")?.Value,
                L = arEl.Attribute("l")?.Value,
                Ppth = arEl.Attribute("ppth")?.Value,
                Cpth = arEl.Attribute("cpth")?.Value,
                Wings = arEl.Attribute("wings")?.Value,
                Tra = arEl.Attribute("tra")?.Value,
                Pde = arEl.Attribute("pde")?.Value,
                Cde = arEl.Attribute("cde")?.Value
            };
        }

        var dpEl = stopEl.Element("dp");
        RawDeparture dp = null;
        if (dpEl != null)
        {
            dp = new RawDeparture
            {
                Pt = dpEl.Attribute("pt")?.Value,
                Pp = dpEl.Attribute("pp")?.Value,
                Ps = dpEl.Attribute("ps")?.Value,
                Ct = dpEl.Attribute("ct")?.Value,
                Cp = dpEl.Attribute("cp")?.Value,
                Cs = dpEl.Attribute("cs")?.Value,
                Hi = dpEl.Attribute("hi")?.Value,
                L = dpEl.Attribute("l")?.Value,
                Ppth = dpEl.Attribute("ppth")?.Value,
                Cpth = dpEl.Attribute("cpth")?.Value,
                Wings = dpEl.Attribute("wings")?.Value,
                Tra = dpEl.Attribute("tra")?.Value,
                Pde = dpEl.Attribute("pde")?.Value,
                Cde = dpEl.Attribute("cde")?.Value
            };
        }

        var messages = stopEl.Elements("m")
                             .Select(msgEl => new RawMessage
                             {
                                 Id = msgEl.Attribute("id")?.Value,
                                 T = msgEl.Attribute("t")?.Value,
                                 From = msgEl.Attribute("from")?.Value,
                                 To = msgEl.Attribute("to")?.Value,
                                 C = msgEl.Attribute("c")?.Value,
                                 Int = msgEl.Attribute("int")?.Value,
                                 Del = msgEl.Attribute("del")?.Value,
                                 Ec = msgEl.Attribute("ec")?.Value,
                                 Ts = msgEl.Attribute("ts")?.Value,
                                 Priority = msgEl.Attribute("priority")?.Value,
                                 Owner = msgEl.Attribute("owner")?.Value,
                                 Cat = msgEl.Attribute("cat")?.Value,
                                 Text = msgEl.Value
                             })
                             .ToList();

        return new RawStopData
        {
            Id = id,
            Eva = eva,
            Tl = tl,
            Ar = ar,
            Dp = dp,
            M = messages.Count > 0 ? messages : null
        };
    }
}