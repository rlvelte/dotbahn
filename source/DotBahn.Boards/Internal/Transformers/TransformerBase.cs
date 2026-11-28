using System.Globalization;
using DotBahn.Boards.Internal.Contracts;
using DotBahn.Boards.Models;
using DotBahn.Boards.Models.Entries;
using DotBahn.Boards.Models.Journeys;
using DotBahn.Boards.Models.Messages;
using DotBahn.Boards.Models.Operators;
using DotBahn.Common.Models;

namespace DotBahn.Boards.Internal.Transformers;

/// <summary>
/// Shared transformation helpers for arrival and departure boards.
/// </summary>
internal abstract class TransformerBase {
    private const string GermanLanguageCode = "deu";
    private const string EnglishLanguageCode = "eng";

    /// <summary>
    /// Lookup context shared by all board transformations.
    /// </summary>
    protected record TransformContext(IReadOnlyDictionary<string, StopPlace> StopPlaces, IReadOnlyDictionary<string, Administration> Administrations, IReadOnlyDictionary<int, Message> Messages);

    /// <summary>
    /// Transforms the shared board structure into the public domain model.
    /// </summary>
    protected static Board<TEntry> TransformBoard<TEntry, TContract>(IEnumerable<StopPlaceMultiLanguageContract> stopPlaceContracts, IEnumerable<StationMultiLanguageContract> stationContracts, IEnumerable<AdministrationExtendedMultiLanguageContract> administrationContracts, MessagesMultiLanguageContract? messagesContract, BoardMetaContract meta, IEnumerable<int> disruptionCommunications, IEnumerable<TContract> entries, Func<TContract, TransformContext, TEntry> transformEntry) where TEntry : BoardEntry {
        var ctx = BuildContext(stopPlaceContracts, stationContracts, administrationContracts, messagesContract);

        return new Board<TEntry> {
            ConsideredStopPlaces = (meta.ConsideredStopPlaces ?? [])
                .Select(eva => ctx.StopPlaces.GetValueOrDefault(eva))
                .Where(s => s != null)
                .Cast<StopPlace>()
                .ToList(),
            TruncatedTransportTypes = meta.TruncatedTransportTypes ?? [],
            Entries = entries.Select(e => transformEntry(e, ctx)).ToList(),
            StationMessages = ResolveMessages(disruptionCommunications, ctx.Messages).ToList()
        };
    }


    /// <summary>
    /// Builds a <see cref="ChangedValue{DateTime}"/> from scheduled and best-known time strings.
    /// </summary>
    protected static ChangedValue<DateTime> BuildTimeChanged(string timeSchedule, string time, string timeType) {
        var scheduled = ParseTime(timeSchedule);
        var actual = ParseTime(time);
        var hasUpdate = timeType is "PREVIEW" or "REAL" || actual != scheduled;

        return new ChangedValue<DateTime> {
            Original = scheduled,
            Updated = hasUpdate ? actual : null
        };
    }

    /// <summary>
    /// Builds a <see cref="ChangedRef{Platform}"/> from scheduled and current platform contracts.
    /// </summary>
    protected static ChangedRef<Platform> BuildPlatformChanged(PlatformContract? platformSchedule, PlatformContract? platform) {
        var scheduled = platformSchedule != null ? TransformPlatform(platformSchedule) : platform != null ? TransformPlatform(platform) : new Platform { Name = string.Empty };
        var actual = platform != null && platform.Name != scheduled.Name ? TransformPlatform(platform) : null;

        return new ChangedRef<Platform> {
            Original = scheduled,
            Updated = actual
        };
    }

    protected static Journey TransformJourneyInfo(string journeyId, JourneyInfoContract contract, TransformContext ctx) => new() {
        Id = journeyId,
        Origin = ResolveVisit(contract.Origin, ctx),
        Destination = ResolveVisit(contract.Destination, ctx),
        DifferingOrigin = ResolveStopPlace(contract.DifferingOrigin?.EvaNumber, ctx),
        DifferingDestination = ResolveStopPlace(contract.DifferingDestination?.EvaNumber, ctx),
        ScheduledStartTime = ParseTime(contract.ScheduledStartTime),
        TransportAtStart = TransformTransport(contract.TransportAtStart, ctx),
        IsCancelled = contract.JourneyCancelled,
        Type = contract.Type,
        HeaderAdministration = ctx.Administrations.GetValueOrDefault(contract.HeaderAdministrationId),
        HeaderJourneyNumber = contract.HeaderJourneyNumber
    };

    protected static Transport TransformTransport(TransportWithDirectionAndViaContract contract, TransformContext ctx) =>
        CreateTransport(
            contract,
            contract.Direction?.Texts ?? [],
            contract.Direction != null ? (contract.Direction.StopPlaces ?? []).Select(r => ResolveStopPlace(r.EvaNumber, ctx)).Where(s => s != null).Cast<StopPlace>().ToList() : [],
            (contract.Via ?? []).Select(v => ResolveVisit(v, ctx)).ToList(),
            contract.ReplacementTransport?.RealType,
            ctx);

    protected static IReadOnlyDictionary<string, int> TransformCodeshares(IEnumerable<CodeShareContract>? contracts) =>
        (contracts ?? []).ToDictionary(c => c.AirlineCode, c => c.Flightnumber);

    protected static TransportReference TransformTransportReference(TransportDestinationRefContract contract, TransformContext ctx) =>
        CreateTransportReference(contract, ctx,
            destination: ResolveVisit(contract.Destination, ctx),
            differingDestination: ResolveStopPlace(contract.DifferingDestination?.EvaNumber, ctx));

    protected static TransportReference TransformTransportReference(TransportOriginRefContract contract, TransformContext ctx) =>
        CreateTransportReference(contract, ctx,
            origin: ResolveVisit(contract.Origin, ctx),
            differingOrigin: ResolveStopPlace(contract.DifferingOrigin?.EvaNumber, ctx));

    protected static TransportReference TransformTransportReference(TransportDestinationPortionWorkingRefContract contract, TransformContext ctx) =>
        CreateTransportReference(contract, ctx,
            destination: ResolveVisit(contract.Destination, ctx),
            differingDestination: ResolveStopPlace(contract.DifferingDestination?.EvaNumber, ctx),
            separationAt: ResolveStopPlace(contract.SeparationAt?.EvaNumber, ctx));

    /// <summary>
    /// Resolves a list of message IDs to messages.
    /// </summary>
    protected static IReadOnlyList<Message> ResolveMessages(IEnumerable<int>? messageIds, IReadOnlyDictionary<int, Message> lookup) =>
        (messageIds ?? []).Select(lookup.GetValueOrDefault)
        .Where(m => m != null)
        .Cast<Message>()
        .ToList();

    protected static StopPlace? ResolveStopPlace(string? evaNumber, TransformContext ctx) =>
        !string.IsNullOrEmpty(evaNumber) && ctx.StopPlaces.TryGetValue(evaNumber, out var stopPlace) ? stopPlace : null;


    /// <summary>
    /// Builds the shared lookup context for all board transformations.
    /// </summary>
    private static TransformContext BuildContext(IEnumerable<StopPlaceMultiLanguageContract> stopPlaceContracts, IEnumerable<StationMultiLanguageContract> stationContracts, IEnumerable<AdministrationExtendedMultiLanguageContract> administrationContracts, MessagesMultiLanguageContract? messagesContract) {
        var stationNames = stationContracts.ToDictionary(s => s.Stada, c => SelectLocalized(c.LocalizedNames, n => n.Language, n => n.Name).Value);
        var stopPlaces = stopPlaceContracts.ToDictionary(s => s.EvaNumber, c => TransformStopPlace(c, stationNames));

        return new TransformContext(stopPlaces, administrationContracts.ToDictionary(a => a.AdministrationId, TransformAdministration), BuildMessageLookup(messagesContract));
    }

    /// <summary>
    /// Builds a message lookup dictionary from all message collections.
    /// </summary>
    private static IReadOnlyDictionary<int, Message> BuildMessageLookup(MessagesMultiLanguageContract? messages) {
        if (messages == null) {
            return new Dictionary<int, Message>();
        }

        var lookup = new Dictionary<int, Message>();

        foreach (var attribute in messages.Attributes ?? []) {
            lookup[attribute.MessageId] = TransformMessageAttribute(attribute);
        }

        foreach (var disruption in messages.Disruptions ?? []) {
            lookup[disruption.MessageId] = TransformMessageDisruption(disruption);
        }

        foreach (var note in messages.Notes ?? []) {
            lookup[note.MessageId] = TransformMessageNote(note);
        }

        foreach (var cause in messages.RisCauseCodes ?? []) {
            lookup[cause.MessageId] = TransformMessageRisCauseCode(cause);
        }

        foreach (var deviation in messages.RisQualityDeviations ?? []) {
            lookup[deviation.MessageId] = TransformMessageRisQualityDeviation(deviation);
        }

        return lookup;
    }

    private static StopPlace ResolveStopPlaceOrDefault(string evaNumber, TransformContext ctx) =>
        ResolveStopPlace(evaNumber, ctx) ?? new StopPlace { EvaNumber = evaNumber, Name = evaNumber };

    private static Visit ResolveVisit(StopPlaceRefWithCancelContract reference, TransformContext ctx) => new() {
        StopPlace = ResolveStopPlaceOrDefault(reference.EvaNumber, ctx),
        Cancelled = reference.Cancelled
    };

    private static Visit ResolveVisit(StopPlaceRefViaContract reference, TransformContext ctx) => new() {
        StopPlace = ResolveStopPlaceOrDefault(reference.EvaNumber, ctx),
        Additional = reference.Additional,
        Cancelled = reference.Cancelled,
        DisplayPriority = reference.DisplayPriority
    };

    private static DateTime ParseTime(string value) => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

    private static StopPlace TransformStopPlace(StopPlaceMultiLanguageContract contract, IReadOnlyDictionary<string, string> stationNames) {
        var localized = SelectLocalized(contract.LocalizedNames, n => n.Language, n => (n.Name, n.Symbol, n.SymbolName));
        return new StopPlace {
            EvaNumber = contract.EvaNumber,
            Name = localized.Value.Name,
            Symbol = localized.Value.Symbol,
            SymbolName = localized.Value.SymbolName,
            Dhid = contract.Ifopt,
            StadaId = contract.Stada,
            StationName = !string.IsNullOrEmpty(contract.Stada) && stationNames.TryGetValue(contract.Stada, out var name) ? name : null
        };
    }

    private static Administration TransformAdministration(AdministrationExtendedMultiLanguageContract contract) => new() {
        Id = contract.AdministrationId,
        OperatorCode = contract.OperatorCode,
        OperatorName = contract.OperatorName,
        TransportCompanies = (contract.TransportCompanies ?? []).Select(TransformTransportCompany).ToList()
    };

    private static TransportCompany TransformTransportCompany(TransportCompanyMultiLanguageContract contract) => new() {
        Language = contract.Language,
        Name = contract.Name,
        ShortName = contract.ShortName,
        Email = contract.Email,
        PhoneNumber = contract.PhoneNumber,
        Website = contract.Website,
        WebsiteTicketing = contract.WebsiteTicketing,
        WebsiteDigitalAssistant = contract.WebsiteDigitalAssistant
    };

    private static Platform TransformPlatform(PlatformContract contract) => new() {
        Name = contract.Name,
        Ifopt = contract.Ifopt,
        StepFreeAccess = contract.Accessibility?.StepFreeAccess ?? false,
        BoardingAid = contract.Accessibility?.BoardingAid ?? false,
        TactileGuidingStrips = contract.Accessibility?.TactileGuidingStrips ?? false,
        TactileHandrailLabel = contract.Accessibility?.TactileHandrailLabel ?? false,
        TactilePlatformAccess = contract.Accessibility?.TactilePlatformAccess ?? false,
        AudibleSignalsAvailable = contract.Accessibility?.AudibleSignalsAvailable ?? false,
        AutomaticDoor = contract.Accessibility?.AutomaticDoor ?? false,
        PassengerInformationDisplay = contract.Accessibility?.PassengerInformationDisplay ?? false,
        PlatformSign = contract.Accessibility?.PlatformSign ?? false,
        StairsMarking = contract.Accessibility?.StairsMarking ?? false,
        StandardPlatformHeight = contract.Accessibility?.StandardPlatformHeight ?? false
    };

    private static Line TransformLine(LineInfoContract contract) => new() {
        Name = contract.Name,
        PrimaryColour = contract.Colour?.PrimaryColour,
        PrimaryTextColour = contract.Colour?.PrimaryTextColour,
        ChipDifferingBoundaryColour = contract.Colour?.ChipVisualization?.DifferingBoundaryColour,
        ChipDifferingFillColour = contract.Colour?.ChipVisualization?.DifferingFillColour,
        ChipDifferingTextColour = contract.Colour?.ChipVisualization?.DifferingTextColour,
        ChipSecondaryFillColour = contract.Colour?.ChipVisualization?.SecondaryFillColour,
        StrokeDifferingBoundaryColour = contract.Colour?.StrokeVisualization?.DifferingBoundaryColour,
        StrokeSecondaryFillColour = contract.Colour?.StrokeVisualization?.SecondaryFillColour
    };


    private static Transport TransformTransport(TransportContract contract, TransformContext ctx) =>
        CreateTransport(contract, [], [], [], null, ctx);

    private static Transport CreateTransport(
        string administrationId, string category, string categoryInternal, string type,
        int journeyNumber, string operationalNumber, string description, string? label,
        LineInfoContract? line, string? dfid, string? dtid,
        IReadOnlyList<string> direction, IReadOnlyList<StopPlace> directionStopPlaces,
        IReadOnlyList<Visit> via, string? replacementTransport, TransformContext ctx) => new() {
            Category = category,
            InternalCategory = categoryInternal,
            Type = type,
            Number = journeyNumber.ToString(CultureInfo.InvariantCulture),
            OperationalNumber = operationalNumber,
            Description = description,
            Label = label,
            Line = line != null ? TransformLine(line) : null,
            Dfid = dfid,
            Dtid = dtid,
            Direction = direction,
            DirectionStopPlaces = directionStopPlaces,
            Via = via,
            ReplacementTransport = replacementTransport,
            Administration = ctx.Administrations.GetValueOrDefault(administrationId) ?? new Administration {
                Id = administrationId,
                OperatorCode = string.Empty,
                OperatorName = string.Empty
            }
        };

    private static Transport CreateTransport(TransportWithDirectionAndViaContract c, IReadOnlyList<string> direction, IReadOnlyList<StopPlace> directionStopPlaces, IReadOnlyList<Visit> via, string? replacementTransport, TransformContext ctx) =>
        CreateTransport(c.AdministrationId, c.Category, c.CategoryInternal, c.Type, c.JourneyNumber, c.OperationalJourneyNumber, c.JourneyDescription, c.Label, c.Line, c.Dfid, c.Dtid, direction, directionStopPlaces, via, replacementTransport, ctx);

    private static Transport CreateTransport(TransportContract c, IReadOnlyList<string> direction, IReadOnlyList<StopPlace> directionStopPlaces, IReadOnlyList<Visit> via, string? replacementTransport, TransformContext ctx) =>
        CreateTransport(c.AdministrationId, c.Category, c.CategoryInternal, c.Type, c.JourneyNumber, c.OperationalJourneyNumber, c.JourneyDescription, c.Label, c.Line, c.Dfid, c.Dtid, direction, directionStopPlaces, via, replacementTransport, ctx);

    private static TransportReference CreateTransportReference(
        string administrationId, string category, string categoryInternal, string type,
        string journeyId, int journeyNumber, string operationalNumber, string description,
        string? label, LineInfoContract? line, string? dfid, string? dtid, TransformContext ctx,
        Visit? origin = null, Visit? destination = null,
        StopPlace? differingOrigin = null, StopPlace? differingDestination = null, StopPlace? separationAt = null) => new() {
            JourneyId = journeyId,
            Category = category,
            InternalCategory = categoryInternal,
            Type = type,
            Number = journeyNumber.ToString(CultureInfo.InvariantCulture),
            OperationalNumber = operationalNumber,
            Description = description,
            Label = label,
            Line = line != null ? TransformLine(line) : null,
            Dfid = dfid,
            Dtid = dtid,
            Administration = ctx.Administrations.GetValueOrDefault(administrationId),
            Origin = origin,
            Destination = destination,
            DifferingOrigin = differingOrigin,
            DifferingDestination = differingDestination,
            SeparationAt = separationAt
        };

    private static TransportReference CreateTransportReference(TransportDestinationRefContract c, TransformContext ctx, Visit? destination, StopPlace? differingDestination) =>
        CreateTransportReference(c.AdministrationId, c.Category, c.CategoryInternal, c.Type, c.JourneyId, c.JourneyNumber, c.OperationalJourneyNumber, c.JourneyDescription, c.Label, c.Line, c.Dfid, c.Dtid, ctx, destination: destination, differingDestination: differingDestination);

    private static TransportReference CreateTransportReference(TransportOriginRefContract c, TransformContext ctx, Visit? origin, StopPlace? differingOrigin) =>
        CreateTransportReference(c.AdministrationId, c.Category, c.CategoryInternal, c.Type, c.JourneyId, c.JourneyNumber, c.OperationalJourneyNumber, c.JourneyDescription, c.Label, c.Line, c.Dfid, c.Dtid, ctx, origin: origin, differingOrigin: differingOrigin);

    private static TransportReference CreateTransportReference(TransportDestinationPortionWorkingRefContract c, TransformContext ctx, Visit? destination, StopPlace? differingDestination, StopPlace? separationAt) =>
        CreateTransportReference(c.AdministrationId, c.Category, c.CategoryInternal, c.Type, c.JourneyId, c.JourneyNumber, c.OperationalJourneyNumber, c.JourneyDescription, c.Label, c.Line, c.Dfid, c.Dtid, ctx, destination: destination, differingDestination: differingDestination, separationAt: separationAt);

    private static Message TransformMessageAttribute(MessageAttributeMultiLanguageContract contract) => new() {
        Type = MessageType.Attribute,
        Text = SelectLocalized(contract.Descriptions, d => d.Language, d => d.Text).Value,
        Code = contract.Code,
        Priority = contract.DisplayPriority,
        DetailedPriority = contract.DisplayPriorityDetail
    };

    private static Message TransformMessageDisruption(MessageDisruptionCommunicationMultiLanguageContract contract) {
        var localized = SelectLocalized(contract.Descriptions, d => d.Language, d => d.Text);
        return new Message {
            Type = MessageType.Disruption,
            Text = localized.Value,
            ShortText = localized.Item?.TextShort,
            Cause = contract.Cause,
            Effect = contract.Effect,
            Priority = contract.DisplayPriority,
            DisruptionId = contract.DisruptionId,
            DisruptionCommunicationId = contract.DisruptionCommunicationId,
            HasAlternatives = contract.HasAlternatives ?? false,
            Resources = BuildMessageResources(localized.Item?.Links, localized.Item?.Attachments, localized.Item?.Images)
        };
    }

    private static Message TransformMessageNote(MessageNoteMultiLanguageContract contract) {
        var localized = SelectLocalized(contract.Descriptions, d => d.Language, d => d.Text);
        return new Message {
            Type = MessageType.Note,
            Text = localized.Value,
            ShortText = localized.Item?.TextShort,
            Category = contract.Category,
            Code = contract.Code,
            Priority = contract.DisplayPriority,
            DisruptionId = contract.DisruptionId,
            DisruptionCommunicationId = contract.DisruptionCommunicationId,
            Resources = BuildMessageResources(localized.Item?.Links, localized.Item?.Attachments, localized.Item?.Images)
        };
    }

    private static Message TransformMessageRisCauseCode(MessageRisCauseCodeMultiLanguageContract contract) => new() {
        Type = MessageType.RisCauseCode,
        Text = SelectLocalized(contract.Descriptions, d => d.Language, d => d.Text).Value,
        Code = contract.Code
    };

    private static Message TransformMessageRisQualityDeviation(MessageRisQualityDeviationMultiLanguageContract contract) => new() {
        Type = MessageType.RisQualityDeviation,
        Text = SelectLocalized(contract.Descriptions, d => d.Language, d => d.Text).Value,
        Code = contract.Code
    };

    private static IReadOnlyList<MessageResource> BuildMessageResources(IEnumerable<DisruptionCommunicationLinkContract>? links, IEnumerable<DisruptionCommunicationAttachmentContract>? attachments, IEnumerable<DisruptionCommunicationImageContract>? images) =>
        (links ?? []).Select(c => CreateMessageResource(c.Label, c.Url, MessageResourceType.Link))
        .Concat((attachments ?? []).Select(c => CreateMessageResource(c.Label, c.Url, MessageResourceType.Attachment)))
        .Concat((images ?? []).Select(c => CreateMessageResource(c.Label, c.Url, MessageResourceType.Image)))
        .ToList();

    private static MessageResource CreateMessageResource(string label, string url, MessageResourceType type) => new() {
        Type = type,
        Label = label,
        Url = url
    };

    private static (T Item, TValue Value) SelectLocalized<T, TValue>(IEnumerable<T>? items, Func<T, string> languageSelector, Func<T, TValue> valueSelector) {
        var list = items?.ToList() ?? [];
        var selected = list.FirstOrDefault(x => languageSelector(x).Equals(GermanLanguageCode, StringComparison.OrdinalIgnoreCase)) ?? list.FirstOrDefault(x => languageSelector(x).Equals(EnglishLanguageCode, StringComparison.OrdinalIgnoreCase)) ?? list.FirstOrDefault();

        return selected == null ? (default!, default!) : (selected, valueSelector(selected));
    }
}
