namespace PNGReaper.Services.Abstract;

public interface IPngParseService
{
    void SetText(string rawInput);
    
    string? Prompt { get; }
    string? NegativePrompt { get; }
    
    string? Steps { get; }
    string? Seed { get; }
    string? Sampler { get; }
    string? CFGScale { get; }
    string? Model { get; }
    string? Size { get; }
    string? ModelHash { get; }
    
    string RawData { get; }
}