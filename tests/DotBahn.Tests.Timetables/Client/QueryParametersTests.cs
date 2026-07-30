using DotBahn.Common.Utilities;

namespace DotBahn.Tests.Timetables.Client;

public class QueryParametersTests {
    public static TheoryData<string, IEnumerable<string>?, bool> AddCollectionEdgeCases => new() {
        { "BL—non_empty", ["a", "b"], true },
        { "C1—null", null, false },
        { "C2—empty", [], false },
    };

    [Theory]
    [MemberData(nameof(AddCollectionEdgeCases))]
    public void AddCollectionEdge(string _, IEnumerable<string>? values, bool expectedAdded) {
        var qp = QueryParameters.Create();
        qp.Add("key", values);
        var qs = qp.ToQueryString();

        if (expectedAdded) {
            Assert.Contains("key=", qs);
        } else {
            Assert.DoesNotContain("key=", qs);
        }
    }

    public static TheoryData<string, string?, bool> AddStringEdgeCases => new() {
        { "BL—valid", "val", true },
        { "C1—null", null, false },
        { "C2—empty", "", false },
        { "C3—whitespace", " ", false },
    };

    [Theory]
    [MemberData(nameof(AddStringEdgeCases))]
    public void AddStringEdge(string _, string? value, bool expectedAdded) {
        var qp = QueryParameters.Create();
        qp.Add("key", value);
        var qs = qp.ToQueryString();

        if (expectedAdded) {
            Assert.Contains("key=", qs);
        } else {
            Assert.DoesNotContain("key=", qs);
        }
    }
}
