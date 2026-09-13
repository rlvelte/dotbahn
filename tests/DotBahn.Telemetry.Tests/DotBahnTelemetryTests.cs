using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using DotBahn.Common.Parsing;
using DotBahn.Common.Telemetry;
using DotBahn.Common.Tests;
using DotBahn.Common.Transformer;
using DotBahn.Stations;
using DotBahn.Stations.Internal.Contracts;
using DotBahn.Stations.Models;
using Moq;

namespace DotBahn.Telemetry.Tests;

[Collection("Telemetry")]
public class DotBahnTelemetryTests : ClientTestBase {
    private readonly Mock<IParser<StationsResponseContract>> _parserMock = new();
    private readonly Mock<ITransformer<IEnumerable<Station>, StationsResponseContract>> _transformerMock = new();
    private readonly StationClient _client;

    public DotBahnTelemetryTests() {
        HttpClient.BaseAddress = new Uri("https://apis.deutschebahn.com/");
        _client = new StationClient(HttpClient, AuthorizationMock.Object, _parserMock.Object, _transformerMock.Object);
    }

    [Fact]
    public void StartRequest_WithoutListener_ReturnsNull() {
        var activity = DotBahnTelemetry.StartRequest("stations", "/stations", null);

        Assert.Null(activity);
    }

    [Fact]
    public void StartRequest_WithListener_CreatesActivityWithTags() {
        using var _ = CreateListener();

        using var activity = DotBahnTelemetry.StartRequest("stations", "/stations?limit=1", "apis.deutschebahn.com");

        Assert.NotNull(activity);
        Assert.Equal(DotBahnTelemetry.RequestActivityName, activity.OperationName);
        Assert.Equal(ActivityKind.Client, activity.Kind);
        Assert.Equal("stations", activity.GetTagItem(DotBahnTelemetry.ApiTag));
        Assert.Equal("/stations?limit=1", activity.GetTagItem(DotBahnTelemetry.EndpointTag));
        Assert.Equal("apis.deutschebahn.com", activity.GetTagItem(DotBahnTelemetry.ServerAddressTag));
    }

    [Fact]
    public void StartRequest_WithUnknownApi_OmitsApiTag() {
        using var _ = CreateListener();

        using var activity = DotBahnTelemetry.StartRequest(null, "/stations", null);

        Assert.NotNull(activity);
        Assert.Null(activity.GetTagItem(DotBahnTelemetry.ApiTag));
        Assert.Null(activity.GetTagItem(DotBahnTelemetry.ServerAddressTag));
    }

    [Fact]
    public async Task GetStationsAsync_WithListener_EmitsSuccessfulRequestTelemetry() {
        SetupEmptyResponse();
        HttpHandler.RespondWith(HttpStatusCode.OK, "{}", "application/json");
        var histogramValues = new List<double>();
        var counterValues = new List<long>();
        using var meterListener = CreateMeterListener(histogramValues, counterValues);

        var result = await _client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        Assert.Empty(result);
        var request = Assert.Single(HttpHandler.SentRequests);
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Contains("/stations", request.RequestUri?.ToString());
        Assert.Contains("application/json", request.Headers.Accept.Select(h => h.MediaType));
        var measurement = Assert.Single(histogramValues);
        Assert.True(measurement >= 0);
        Assert.Single(counterValues);
    }

    [Fact]
    public async Task GetStationsAsync_OnHttpError_EmitsErrorTelemetry() {
        SetupEmptyResponse();
        HttpHandler.RespondWith(HttpStatusCode.InternalServerError, "{}", "application/json");
        Activity? captured = null;

        using var listener = new ActivityListener();
        listener.ShouldListenTo = source => source.Name == DotBahnTelemetry.SourceName;
        listener.Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded;
        listener.ActivityStarted = activity => captured = activity;
        ActivitySource.AddActivityListener(listener);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken));

        Assert.NotNull(captured);
        Assert.Equal(ActivityStatusCode.Error, captured.Status);
        Assert.Equal("HttpRequestException", captured.GetTagItem(DotBahnTelemetry.ErrorTypeTag));
        Assert.Equal("stations", captured.GetTagItem(DotBahnTelemetry.ApiTag));
    }

    [Fact]
    public async Task GetStationsAsync_WithoutListener_DoesNotThrow() {
        SetupEmptyResponse();
        HttpHandler.RespondWith(HttpStatusCode.OK, "{}", "application/json");

        var result = await _client.GetStationsAsync(new StationQuery(), TestContext.Current.CancellationToken);

        Assert.Empty(result);
    }

    private void SetupEmptyResponse() {
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns(new StationsResponseContract());
        _transformerMock.Setup(t => t.Transform(It.IsAny<StationsResponseContract>())).Returns([]);
    }

    private static ActivityListener CreateListener() {
        var listener = new ActivityListener {
            ShouldListenTo = source => source.Name == DotBahnTelemetry.SourceName,
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);
        return listener;
    }

    private static MeterListener CreateMeterListener(List<double> histogramValues, List<long> counterValues) {
        var listener = new MeterListener {
            InstrumentPublished = (instrument, meterListener) => {
                if (instrument.Meter.Name == DotBahnTelemetry.MeterName) {
                    meterListener.EnableMeasurementEvents(instrument, null);
                }
            }
        };

        listener.SetMeasurementEventCallback<double>((_, measurement, _, _) => histogramValues.Add(measurement));
        listener.SetMeasurementEventCallback<long>((_, measurement, _, _) => counterValues.Add(measurement));
        listener.Start();

        return listener;
    }
}
