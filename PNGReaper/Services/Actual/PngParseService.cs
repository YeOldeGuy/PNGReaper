using System;
using PNGReaper.Services.Abstract;

namespace PNGReaper.Services.Actual;

internal class PngParseService : IPngParseService
{
    private const string _negativePrompt = "Negative Prompt:";
    private const string _stepsPrompt    = "Steps:";
    private const string _seedPrompt     = "Seed:";
    private const string _samplerPrompt  = "Sampler:";
    private const string _cfgScalePrompt = "CFG Scale";
    private const string _modelPrompt    = "Model:";
    private const string _sizePrompt     = "Size:";
    private const string _hashPrompt     = "Model Hash:";

    public void SetText(string rawInput)
    {
        var header = CopyToNullByte(rawInput);
        RawData = rawInput[(header.Length + 1)..];
    }

    public string? Prompt
    {
        get
        {
            if (string.IsNullOrEmpty(RawData))
                return null;
            var index = RawData.IndexOf(_negativePrompt, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return string.Empty;

            var p = RawData[..index];
            return p.Trim();
        }
    }

    public string? NegativePrompt
    {
        get
        {
            if (string.IsNullOrEmpty(RawData))
                return null;

            var negIndex = RawData.IndexOf(_negativePrompt, StringComparison.OrdinalIgnoreCase) +
                         _negativePrompt.Length;
            var stepsIndex = RawData.IndexOf(_stepsPrompt, StringComparison.OrdinalIgnoreCase);

            if (negIndex < 0 || stepsIndex < 0 || stepsIndex <= negIndex) return string.Empty;

            var s = RawData[negIndex..stepsIndex];
            return s.Trim();
        }
    }

    public string RawData { get; private set; } = string.Empty;

    public string? Steps => GetNamedParameter(_stepsPrompt);
    public string? Seed => GetNamedParameter(_seedPrompt);
    public string? Sampler => GetNamedParameter(_samplerPrompt);
    public string? CFGScale => GetNamedParameter(_cfgScalePrompt);
    public string? Model => GetNamedParameter(_modelPrompt);
    public string? Size => GetNamedParameter(_sizePrompt);
    public string? ModelHash => GetNamedParameter(_hashPrompt);

    private static string CopyToNullByte(ReadOnlySpan<char> chars)
    {
        var index = chars.IndexOf('\0');
        if (index < 0)
            return string.Empty;
        var slice = chars[..index];
        return new string(slice);
    }

    private string? GetNamedParameter(string prompt)
    {
        if (string.IsNullOrEmpty(RawData))
            return null;

        var start = RawData.IndexOf(prompt, StringComparison.OrdinalIgnoreCase) + prompt.Length + 1;
        if (start < 0 || start > RawData.Length)
            return "";
        var end = RawData.IndexOf(',', start);
        return end < 0 ? string.Empty : RawData[start..end].Trim();
    }
}