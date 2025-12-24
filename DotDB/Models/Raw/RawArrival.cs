namespace DotDB.Models.Raw;

/// <summary>
/// Raw attributes for arrival information
/// </summary>
public record RawArrival {
    /// <summary>
    /// Planned time (YYMMDDhhmm format)
    /// </summary>
    public string Pt { get; init; }
    
    /// <summary>
    /// Planned platform
    /// </summary>
    public string Pp { get; init; }
    
    /// <summary>
    /// Planned status (e.g., "p" for planned)
    /// </summary>
    public string Ps { get; init; }
    
    /// <summary>
    /// Changed time (YYMMDDhhmm format)
    /// </summary>
    public string Ct { get; init; }
    
    /// <summary>
    /// Changed platform
    /// </summary>
    public string Cp { get; init; } 
    
    /// <summary>
    /// Changed status (e.g., "c" for canceled)
    /// </summary>
    public string Cs { get; init; }    
    
    /// <summary>
    /// Hidden (0 or 1)
    /// </summary>
    public string Hi { get; init; }
    
    /// <summary>
    /// Line
    /// </summary>
    public string L { get; init; }
    
    /// <summary>
    /// Planned path (pipe-separated station list)
    /// </summary>
    public string Ppth { get; init; }
    
    /// <summary>
    /// Changed path (pipe-separated station list)
    /// </summary>
    public string Cpth { get; init; }
    
    /// <summary>
    /// Wing train information
    /// </summary>
    public string Wings { get; set; }
    
    /// <summary>
    /// Transition information
    /// </summary>
    public string Tra { get; set; }
    
    /// <summary>
    /// Planned distant endpoint
    /// </summary>
    public string Pde { get; init; }
    
    /// <summary>
    /// Changed distant endpoint
    /// </summary>
    public string Cde { get; init; }
}