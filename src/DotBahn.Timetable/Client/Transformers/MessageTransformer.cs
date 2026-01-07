using DotBahn.Core.Contracts;
using DotBahn.Core.Enumerations;
using DotBahn.Core.Models;
using DotBahn.Parsing.Base;

namespace DotBahn.Timetable.Client.Transformers;

/// <summary>
/// Transformer for converting <see cref="RawMessage"/> to <see cref="Message"/>.
/// </summary>
public class MessageTransformer : ITransformer<RawMessage, Message>
{
    private static readonly Dictionary<string, MessageType> MessageTypeMap = new()
    {
        { "q", MessageType.Quality },
        { "d", MessageType.Delay },
        { "i", MessageType.Info },
        { "r", MessageType.Disruption },
        { "u", MessageType.CauseOfDelay },
        { "f", MessageType.FreeText },
        { "c", MessageType.Connection },
        { "h", MessageType.Himmel }
    };

    /// <inheritdoc />
    public Message Transform(RawMessage raw)
    {
        return new Message
        {
            Id = raw.Id,
            Type = ParseMessageType(raw.Type),
            Code = raw.Code,
            Text = raw.Text,
            ValidFrom = !string.IsNullOrEmpty(raw.ValidFrom) 
                ? TransformerUtils.ParseApiTime(raw.ValidFrom) 
                : null,
            ValidTo = !string.IsNullOrEmpty(raw.ValidTo) 
                ? TransformerUtils.ParseApiTime(raw.ValidTo) 
                : null,
            Timestamp = !string.IsNullOrEmpty(raw.Timestamp) 
                ? TransformerUtils.ParseApiTime(raw.Timestamp) 
                : null,
            Priority = !string.IsNullOrEmpty(raw.Priority) 
                ? int.Parse(raw.Priority) 
                : null,
            IsInternal = raw.IsInternal == "1",
            IsDeleted = raw.IsDeleted == "1"
        };
    }

    private static MessageType ParseMessageType(string? type)
    {
        return type != null && MessageTypeMap.TryGetValue(type, out var messageType) 
            ? messageType 
            : MessageType.Info;
    }
}
