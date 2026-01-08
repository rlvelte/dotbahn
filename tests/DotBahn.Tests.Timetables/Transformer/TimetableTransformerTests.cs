using DotBahn.Clients.Timetables.Contracts;
using DotBahn.Clients.Timetables.Models;
using DotBahn.Clients.Timetables.Transformers;
using DotBahn.Modules.Shared.Transformer;
using FluentAssertions;
using NSubstitute;

namespace DotBahn.Tests.Timetables.Transformer;

public class TimetableTransformerTests {
    private readonly TimetableTransformer _timetableTransformer;

    public TimetableTransformerTests() {
        var stopTransformer = Substitute.For<ITransformer<StopDataContract, TrainStop>>();
        _timetableTransformer = new TimetableTransformer(stopTransformer);
    }

    [Fact]
    public void Transform_ShouldMapStationNameAndStops() {
        // Arrange
        var stopTransformer = Substitute.For<ITransformer<StopDataContract, TrainStop>>();
        var transformer = new TimetableTransformer(stopTransformer);

        var stopContract1 = new StopDataContract { Id = "s1", Eva = "12345" };
        var stopContract2 = new StopDataContract { Id = "s2", Eva = "12345" };
        
        var contract = new TimetableResponseContract {
            Station = "Berlin Hbf",
            Stops = [stopContract1, stopContract2]
        };

        stopTransformer.Transform(stopContract1).Returns(new TrainStop { Id = "s1", Station = new StationInfo { Eva = "12345" } });
        stopTransformer.Transform(stopContract2).Returns(new TrainStop { Id = "s2", Station = new StationInfo { Eva = "12345" } });

        // Act
        var result = transformer.Transform(contract);

        // Assert
        result.Station.Should().Be("Berlin Hbf");
        result.Stops.Should().HaveCount(2);
        result.Stops.All(s => s.Station.Name == "Berlin Hbf").Should().BeTrue();
    }
}
