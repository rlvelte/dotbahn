using DotBahn.Modules.Shared.Transformer;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Models;
using DotBahn.TimetableApi.Transformers;
using FluentAssertions;
using NSubstitute;

namespace DotBahn.Tests.TimetableApi.Transformer;

public class StopTransformerTests {
    private readonly ITransformer<EventContract, EventInfo?> _eventTransformer;
    private readonly ITransformer<MessageContract, Message> _messageTransformer;
    private readonly StopTransformer _stopTransformer;

    public StopTransformerTests() {
        _eventTransformer = Substitute.For<ITransformer<EventContract, EventInfo?>>();
        _messageTransformer = Substitute.For<ITransformer<MessageContract, Message>>();
        _stopTransformer = new StopTransformer(_eventTransformer, _messageTransformer);
    }

    [Fact]
    public void Transform_ShouldMapBasicProperties() {
        // Arrange
        var contract = new StopDataContract {
            Id = "test_id",
            Eva = "8000105",
            TripLabel = new TripLabelContract {
                Category = "ICE",
                Number = "123",
                TripType = "p"
            }
        };

        // Act
        var result = _stopTransformer.Transform(contract);

        // Assert
        result.Id.Should().Be("test_id");
        result.Station.Eva.Should().Be("8000105");
        result.Train.Category.Should().Be("ICE");
        result.Train.Number.Should().Be("123");
    }

    [Fact]
    public void Transform_ShouldHandleArrivalAndDeparture() {
        // Arrange
        var arrivalContract = new EventContract { PlannedTime = "2301011200" };
        var departureContract = new EventContract { PlannedTime = "2301011210" };
        var contract = new StopDataContract {
            Arrival = arrivalContract,
            Departure = departureContract
        };

        var arrivalInfo = new EventInfo();
        var departureInfo = new EventInfo();

        _eventTransformer.Transform(arrivalContract).Returns(arrivalInfo);
        _eventTransformer.Transform(departureContract).Returns(departureInfo);

        // Act
        var result = _stopTransformer.Transform(contract);

        // Assert
        result.Arrival.Should().Be(arrivalInfo);
        result.Departure.Should().Be(departureInfo);
    }

    [Fact]
    public void Transform_ShouldHandleMessages() {
        // Arrange
        var messageContract = new MessageContract { Id = "m1" };
        var contract = new StopDataContract {
            Messages = [messageContract]
        };

        var message = new Message { Id = "m1" };
        _messageTransformer.Transform(messageContract).Returns(message);

        // Act
        var result = _stopTransformer.Transform(contract);

        // Assert
        result.Messages.Should().ContainSingle().Which.Should().Be(message);
    }
}
