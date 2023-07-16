using System.Net.Http;

/// <summary>
/// Shark用HttpClient
/// </summary>
public static class RASHttpClient
{
    /// <summary>
    /// instance
    /// </summary>
    public static readonly HttpClient instance = new HttpClient();
}