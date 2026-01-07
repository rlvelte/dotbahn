using DotBahn.TimetableApi.Contracts;
using DotBahn.TimetableApi.Transformers;
using FluentAssertions;

namespace DotBahn.TimetableApi.Tests.Transformer;

public class StationTransformerTests {
    private readonly StationTransformer _transformer = new();

    [Fact]
    public void Transform_ShouldMapPropertiesCorrectly() {
        // Arrange
        var contract = new StationContract {
            Name = "Frankfurt(Main)Hbf",
            Eva = "8000105"
        };

        // Act
        var result = _transformer.Transform(contract);

        // Assert
        result.Name.Should().Be("Frankfurt(Main)Hbf");
        result.Eva.Should().Be("8000105");
    }
}
