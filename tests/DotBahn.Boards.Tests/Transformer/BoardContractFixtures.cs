using DotBahn.Boards.Internal.Contracts;

namespace DotBahn.Boards.Tests.Transformer;

/// <summary>
/// Shared OpenBoards contract fixtures for transformer tests.
/// </summary>
internal static class BoardContractFixtures {
    public const string HannoverEva = "8000105";
    public const string WolfsburgEva = "8000096";
    public const string HannoverStada = "8000105";
    public const string UnknownEva = "8009999";

    public static List<StopPlaceMultiLanguageContract> StopPlaces() => [
        new() {
            EvaNumber = HannoverEva,
            Ifopt = "de:03241:8000105",
            Stada = HannoverStada,
            LocalizedNames = [
                new() { Language = "eng", Name = "Hannover Central Station", Symbol = "H", SymbolName = "Hannover" },
                new() { Language = "deu", Name = "Hannover Hbf", Symbol = "H", SymbolName = "Hannover" }
            ]
        },
        new() {
            EvaNumber = WolfsburgEva,
            LocalizedNames = [new() { Language = "deu", Name = "Wolfsburg Hbf" }]
        }
    ];

    public static List<StationMultiLanguageContract> Stations() => [
        new() {
            Stada = HannoverStada,
            LocalizedNames = [
                new() { Language = "eng", Name = "Hannover" },
                new() { Language = "deu", Name = "Hannover" }
            ]
        }
    ];

    public static List<AdministrationExtendedMultiLanguageContract> Administrations() => [
        new() {
            AdministrationId = "db-fahrdienst",
            OperatorCode = "DB",
            OperatorName = "Deutsche Bahn",
            TransportCompanies = [
                new() {
                    Language = "deu",
                    Name = "Deutsche Bahn AG",
                    ShortName = "DB",
                    Email = "info@db.de",
                    PhoneNumber = "+49 30 123456",
                    Website = "https://www.bahn.de",
                    WebsiteTicketing = "https://www.bahn.de/ticket",
                    WebsiteDigitalAssistant = "https://www.bahn.de/assistant"
                }
            ]
        }
    ];

    public static MessagesMultiLanguageContract Messages() => new() {
        Attributes = [
            new() {
                MessageId = 101,
                Code = "HELP",
                DisplayPriority = 1,
                DisplayPriorityDetail = 2,
                Descriptions = [
                    new() { Language = "deu", Text = "Barrierefreier Einstieg" },
                    new() { Language = "eng", Text = "Step-free boarding" }
                ]
            }
        ],
        Disruptions = [
            new() {
                MessageId = 102,
                Cause = "Construction",
                Effect = "Delay",
                DisplayPriority = 3,
                DisruptionId = "D-1",
                DisruptionCommunicationId = "DC-1",
                HasAlternatives = true,
                Descriptions = [
                    new() {
                        Language = "deu",
                        Text = "Bauarbeiten",
                        TextShort = "Bauarh.",
                        Links = [new() { Label = "Info", Url = "https://bahn.de/info" }],
                        Attachments = [new() { Label = "PDF", Url = "https://bahn.de/plan.pdf" }],
                        Images = [new() { Label = "Map", Url = "https://bahn.de/map.png" }]
                    }
                ]
            }
        ],
        Notes = [
            new() {
                MessageId = 103,
                Category = "GENERAL",
                Code = "BIKE",
                DisplayPriority = 5,
                DisruptionId = "D-2",
                DisruptionCommunicationId = "DC-2",
                Descriptions = [new() { Language = "deu", Text = "Fahrradmitnahme moeglich" }]
            }
        ],
        RisCauseCodes = [
            new() {
                MessageId = 104,
                Code = "1",
                Descriptions = [new() { Language = "deu", Text = "Bauarbeiten" }]
            }
        ],
        RisQualityDeviations = [
            new() {
                MessageId = 105,
                Code = "Q1",
                Descriptions = [new() { Language = "deu", Text = "Verspaetung" }]
            }
        ]
    };

    public static BoardMetaContract Meta(List<string>? considered = null, List<string>? truncated = null) => new() {
        ConsideredStopPlaces = considered ?? [HannoverEva, WolfsburgEva],
        TruncatedTransportTypes = truncated
    };

    public static JourneyInfoContract JourneyInfo() => new() {
        Origin = new() { EvaNumber = WolfsburgEva },
        Destination = new() { EvaNumber = HannoverEva, Cancelled = true },
        DifferingOrigin = new() { EvaNumber = WolfsburgEva },
        DifferingDestination = new() { EvaNumber = HannoverEva },
        ScheduledStartTime = "2026-11-23T09:45:00+01:00",
        TransportAtStart = new() {
            AdministrationId = "db-fahrdienst",
            Category = "ICE",
            CategoryInternal = "ICE",
            Type = "HIGH_SPEED",
            JourneyNumber = 701,
            OperationalJourneyNumber = "701-2026-11-23",
            JourneyDescription = "Ice 701"
        },
        JourneyCancelled = false,
        Type = "JOURNEY",
        HeaderAdministrationId = "db-fahrdienst",
        HeaderJourneyNumber = 701
    };

    public static TransportWithDirectionAndViaContract Transport() => new() {
        AdministrationId = "db-fahrdienst",
        Category = "ICE",
        CategoryInternal = "ICE",
        Type = "HIGH_SPEED",
        JourneyNumber = 701,
        OperationalJourneyNumber = "701-2026-11-23",
        JourneyDescription = "Ice 701",
        Label = "ICE 701",
        Dfid = "dfid-1",
        Dtid = "dtid-1",
        Line = new() {
            Name = "ICE 701",
            Colour = new() {
                PrimaryColour = "#000000",
                PrimaryTextColour = "#FFFFFF",
                ChipVisualization = new() {
                    DifferingBoundaryColour = "#111111",
                    DifferingFillColour = "#222222",
                    DifferingTextColour = "#333333",
                    SecondaryFillColour = "#444444"
                },
                StrokeVisualization = new() {
                    DifferingBoundaryColour = "#555555",
                    SecondaryFillColour = "#666666"
                }
            }
        },
        Direction = new() {
            Texts = ["Wolfsburg Hbf"],
            StopPlaces = [new() { EvaNumber = WolfsburgEva }]
        },
        Via = [new() { EvaNumber = WolfsburgEva, Additional = true, DisplayPriority = 7 }],
        ReplacementTransport = new() { RealType = "BUS" }
    };

    public static ArrivalContract Arrival(string id = "a-1", string eva = HannoverEva) => new() {
        ArrivalId = id,
        EvaNumber = eva,
        JourneyId = "j-1",
        TimeSchedule = "2026-11-23T10:00:00+01:00",
        Time = "2026-11-23T10:05:00+01:00",
        TimeType = "REAL",
        PlatformSchedule = new() { Name = "2", Accessibility = new() { StepFreeAccess = true, BoardingAid = true } },
        Platform = new() { Name = "2" },
        Transport = Transport(),
        JourneyInfo = JourneyInfo(),
        Messages = [101, 102, 103, 104, 105, 999],
        Cancelled = false,
        Additional = true,
        OnDemand = false,
        NoPassengerChange = false,
        VehicleSequenceAvailable = true,
        PastDisruptions = true,
        Codeshares = [new() { AirlineCode = "LH", Flightnumber = 1234 }],
        ContinuationBy = [DestinationRef()],
        ReliefBy = [OriginRef()],
        ReliefFor = [OriginRef()],
        ReplacedBy = [OriginRef()],
        ReplacementFor = [OriginRef()],
        TravelsWith = [OriginRef()]
    };

    public static DepartureContract Departure(string id = "d-1", string eva = HannoverEva) => new() {
        DepartureId = id,
        EvaNumber = eva,
        JourneyId = "j-1",
        TimeSchedule = "2026-11-23T10:00:00+01:00",
        Time = "2026-11-23T10:05:00+01:00",
        TimeType = "REAL",
        PlatformSchedule = new() { Name = "2", Accessibility = new() { StepFreeAccess = true, BoardingAid = true } },
        Platform = new() { Name = "2" },
        Transport = Transport(),
        JourneyInfo = JourneyInfo(),
        Messages = [101, 102, 103, 104, 105, 999],
        Cancelled = false,
        Additional = true,
        OnDemand = false,
        NoPassengerChange = false,
        VehicleSequenceAvailable = true,
        FutureDisruptions = true,
        Codeshares = [new() { AirlineCode = "LH", Flightnumber = 1234 }],
        ContinuationFor = [OriginRef()],
        ReliefBy = [DestinationRef()],
        ReliefFor = [DestinationRef()],
        ReplacedBy = [DestinationRef()],
        ReplacementFor = [DestinationRef()],
        TravelsWith = [PortionRef()]
    };

    public static TransportOriginRefContract OriginRef(string journeyId = "ref-1") => new() {
        JourneyId = journeyId,
        AdministrationId = "db-fahrdienst",
        Category = "RB",
        CategoryInternal = "RB",
        Type = "REGIONAL",
        JourneyNumber = 4,
        OperationalJourneyNumber = "4-2026-11-23",
        JourneyDescription = "Rb 4",
        Label = "RB 4",
        Dfid = "dfid-2",
        Dtid = "dtid-2",
        Origin = new() { EvaNumber = WolfsburgEva },
        DifferingOrigin = new() { EvaNumber = HannoverEva }
    };

    public static TransportDestinationRefContract DestinationRef(string journeyId = "ref-2") => new() {
        JourneyId = journeyId,
        AdministrationId = "db-fahrdienst",
        Category = "RB",
        CategoryInternal = "RB",
        Type = "REGIONAL",
        JourneyNumber = 5,
        OperationalJourneyNumber = "5-2026-11-23",
        JourneyDescription = "Rb 5",
        Label = "RB 5",
        Dfid = "dfid-3",
        Dtid = "dtid-3",
        Destination = new() { EvaNumber = WolfsburgEva, Cancelled = true },
        DifferingDestination = new() { EvaNumber = HannoverEva }
    };

    public static TransportDestinationPortionWorkingRefContract PortionRef(string journeyId = "ref-3") => new() {
        JourneyId = journeyId,
        AdministrationId = "db-fahrdienst",
        Category = "RB",
        CategoryInternal = "RB",
        Type = "REGIONAL",
        JourneyNumber = 6,
        OperationalJourneyNumber = "6-2026-11-23",
        JourneyDescription = "Rb 6",
        Label = "RB 6",
        Dfid = "dfid-4",
        Dtid = "dtid-4",
        Destination = new() { EvaNumber = WolfsburgEva },
        DifferingDestination = new() { EvaNumber = HannoverEva },
        SeparationAt = new() { EvaNumber = HannoverEva }
    };
}
