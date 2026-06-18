using DotBahn.Timetables.Internal.Contracts;
using DotBahn.Timetables.Internal.Transformers;
using DotBahn.Timetables.Models.Enumerations;

namespace DotBahn.Tests.Timetables.Transformer;

public class TimetableTransformerMessageTests {
    private readonly TimetableTransformer _transformer = new();

    private static TimetableResponseContract WithStop(StopDataContract stop) => new() {
        Station = "Test",
        Stops = [stop]
    };

    [Fact]
    public void Transform_InternalMessage_SetsInternalTextOnly() {
        var contract = WithStop(new StopDataContract {
            Id = "s1",
            Messages = [new MessageContract {
                Id = "m1",
                Type = "h",
                Timestamp = "2501191200",
                IsInternal = "1",
                Text = "internal info"
            }]
        });

        var stop = _transformer.Transform(contract).Stops.First();

        var msg = Assert.Single(stop.Messages);
        Assert.Equal("internal info", msg.InternalText);
        Assert.Null(msg.ExternalText);
    }

    [Fact]
    public void Transform_ExternalMessage_SetsExternalTextOnly() {
        var contract = WithStop(new StopDataContract {
            Id = "s1",
            Messages = [new MessageContract {
                Id = "m1",
                Type = "h",
                Timestamp = "2501191200",
                IsInternal = "0",
                Text = "public info"
            }]
        });

        var stop = _transformer.Transform(contract).Stops.First();

        var msg = Assert.Single(stop.Messages);
        Assert.Null(msg.InternalText);
        Assert.Equal("public info", msg.ExternalText);
    }

    [Fact]
    public void Transform_DeletedMessage_SetsIsDeletedTrue() {
        var contract = WithStop(new StopDataContract {
            Id = "s1",
            Messages = [new MessageContract {
                Id = "m1",
                Type = "h",
                Timestamp = "2501191200",
                IsDeleted = "1"
            }]
        });

        var stop = _transformer.Transform(contract).Stops.First();

        Assert.True(Assert.Single(stop.Messages).IsDeleted);
    }

    [Fact]
    public void Transform_NotDeletedMessage_SetsIsDeletedFalse() {
        var contract = WithStop(new StopDataContract {
            Id = "s1",
            Messages = [new MessageContract {
                Id = "m1",
                Type = "h",
                Timestamp = "2501191200",
                IsDeleted = "0"
            }]
        });

        var stop = _transformer.Transform(contract).Stops.First();

        Assert.False(Assert.Single(stop.Messages).IsDeleted);
    }

    [Fact]
    public void Transform_MessageWithValidityRange_SetsValidFromAndTo() {
        var contract = WithStop(new StopDataContract {
            Id = "s1",
            Messages = [new MessageContract {
                Id = "m1",
                Type = "h",
                Timestamp = "2501191200",
                ValidFrom = "2501190800",
                ValidTo   = "2501192200"
            }]
        });

        var stop = _transformer.Transform(contract).Stops.First();

        var msg = Assert.Single(stop.Messages);
        Assert.Equal(new DateTime(2025, 1, 19, 8, 0, 0), msg.ValidFrom);
        Assert.Equal(new DateTime(2025, 1, 19, 22, 0, 0), msg.ValidTo);
    }

    [Fact]
    public void Transform_StopWithoutMessages_ReturnsEmptyMessageList() {
        var contract = WithStop(new StopDataContract { Id = "s1" });

        var stop = _transformer.Transform(contract).Stops.First();

        Assert.Empty(stop.Messages);
    }

    [Fact]
    public void Transform_MessageType_ParsedCorrectly() {
        var contract = WithStop(new StopDataContract {
            Id = "s1",
            Messages = [new MessageContract {
                Id = "m1",
                Type = "q",
                Timestamp = "2501191200"
            }]
        });

        var msg = _transformer.Transform(contract).Stops.First().Messages.First();

        Assert.Equal(MessageType.QualityChange, msg.Type);
    }
}
