using DotBahn.Core.Contracts;
using DotBahn.Core.Enumerations;
using DotBahn.Core.Models;
using DotBahn.Parsing.Base;
using DotBahn.TimetableApi.Utilities;

namespace DotBahn.TimetableApi.Transformers;

/// <summary>
/// Transformer for converting <see cref="RawMessage"/> to <see cref="Message"/>.
/// </summary>
public class MessageTransformer : ITransformer<RawMessage, Message> {
    private static readonly Dictionary<string, MessageType> MessageTypeMap = new() {
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
    public Message Transform(RawMessage contract) {
        return new Message {
            Id = contract.Id ?? string.Empty,
            Type = ParseMessageType(contract.Type),
            Code = contract.Code,
            Text = contract.Text,
            ValidFrom = !string.IsNullOrEmpty(contract.ValidFrom) ? TransformerUtils.ParseApiTime(contract.ValidFrom) : null,
            ValidTo = !string.IsNullOrEmpty(contract.ValidTo) ? TransformerUtils.ParseApiTime(contract.ValidTo) : null,
            Timestamp = !string.IsNullOrEmpty(contract.Timestamp) ? TransformerUtils.ParseApiTime(contract.Timestamp) : null,
            Priority = !string.IsNullOrEmpty(contract.Priority) ? int.Parse(contract.Priority) : null,
            IsInternal = contract.IsInternal == "1",
            IsDeleted = contract.IsDeleted == "1"
        };
    }

    private static MessageType ParseMessageType(string? type) =>
        type != null && MessageTypeMap.TryGetValue(type, out var messageType) ? messageType : MessageType.Info;
}
