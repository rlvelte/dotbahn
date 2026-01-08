using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Enumerations;
using DotBahn.Clients.Timetables.Models;
using DotBahn.Clients.Timetables.Utilities;
using DotBahn.Modules.Shared.Enumerations;
using DotBahn.Modules.Shared.Transformer;

namespace DotBahn.Clients.Timetables.Transformers;

/// <summary>
/// Transformer for converting <see cref="MessageContract"/> to <see cref="Message"/>.
/// </summary>
public class MessageTransformer : ITransformer<MessageContract, Message> {
    /// <inheritdoc />
    public Message Transform(MessageContract contract) {
        return new Message {
            Id = contract.Id ?? string.Empty,
            Type = EnumExtensions.FromAssociatedValue(contract.Type, MessageType.Quality),
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
}
