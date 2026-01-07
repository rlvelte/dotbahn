using DotBahn.Parsing.Base;
using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Models;
using DotBahn.TimetableApi.Transformers;
using FluentAssertions;
using NSubstitute;

namespace DotBahn.TimetableApi.Tests.Transformer;

public class TimetableTransformerTests {
    private readonly TimetableTransformer _timetableTransformer;

    public TimetableTransformerTests() {
        var eventTransformer = Substitute.For<ITransformer<EventContract, EventInfo?>>();
        var messageTransformer = Substitute.For<ITransformer<MessageContract, Message>>();
        var stopTransformer = new StopTransformer(eventTransformer, messageTransformer);
        
        _timetableTransformer = new TimetableTransformer(stopTransformer);
    }

    [Fact]
    public void Transform_ShouldMapStationNameAndStops() {
        // Arrange
        var contract = new TimetableResponseContract {
            Station = "Berlin Hbf",
            Stops = [
                new StopDataContract { Id = "s1", Eva = "12345" },
                new StopDataContract { Id = "s2", Eva = "12345" }
            ]
        };

        // Act
        var result = _timetableTransformer.Transform(contract);

        // Assert
        result.Station.Should().Be("Berlin Hbf");
        result.Stops.Should().HaveCount(2);
        result.Stops.All(s => s.Station.Name == "Berlin Hbf").Should().BeTrue();
    }
}
