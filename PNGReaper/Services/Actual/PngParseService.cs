using System;
using PNGReaper.Services.Abstract;

namespace PNGReaper.Services.Actual;

internal class PngParseService : IPngParseService
{
    private       string _input          = string.Empty;
    private const string _negativePrompt = "Negative Prompt:";
    private const string _stepsPrompt    = "Steps:";
    private const string _seedPrompt     = "Seed:";
    private const string _samplerPrompt  = "Sampler:";
    private const string _cfgScalePrompt = "CFG Scale";
    private const string _modelPrompt    = "Model:";
    private const string _sizePrompt     = "Size:";
    private const string _hashPrompt     = "Model Hash:";
    
    private static string CopyToNullByte(ReadOnlySpan<char> chars)
    {
        var index = chars.IndexOf('\0');
        var slice = chars[..index];
        return new string(slice);
    }
    
    private string? GetNamedParameter(string prompt)
    {
        if (string.IsNullOrEmpty(_input))
            return null;
        
        var start = _input.IndexOf(prompt, StringComparison.OrdinalIgnoreCase) + prompt.Length + 1;
        if (start < 0 || start > _input.Length)
            return "";
        var end = _input.IndexOf(',', start);
        return _input.Substring(start, end - start).Trim();
    }

    public void SetText(string rawInput)
    {
        var header = CopyToNullByte(rawInput);
        _input = rawInput[(header.Length + 1)..];
    }

    public string? Prompt
    {
        get
        {
            if (string.IsNullOrEmpty(_input))
                return null;
            var index = _input.IndexOf(_negativePrompt, StringComparison.OrdinalIgnoreCase);
            var p = _input[..index];
            return p.Trim();
        }
    }

    public string? NegativePrompt
    {
        get
        {
            if (string.IsNullOrEmpty(_input))
                return null;
            
            var nIndex = _input.IndexOf(_negativePrompt, StringComparison.OrdinalIgnoreCase);
            var sIndex = _input.IndexOf(_stepsPrompt, StringComparison.OrdinalIgnoreCase);
            var s = _input[nIndex..sIndex];
            return s.Trim();
        }
    }

    public string RawData => _input;

    public string? Steps => GetNamedParameter(_stepsPrompt);
    public string? Seed => GetNamedParameter(_seedPrompt);
    public string? Sampler => GetNamedParameter(_samplerPrompt);
    public string? CFGScale => GetNamedParameter(_cfgScalePrompt);
    public string? Model => GetNamedParameter(_modelPrompt);
    public string? Size => GetNamedParameter(_sizePrompt);
    public string? ModelHash => GetNamedParameter(_hashPrompt);
}