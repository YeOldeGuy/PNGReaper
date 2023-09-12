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

            var nIndex = RawData.IndexOf(_negativePrompt, StringComparison.OrdinalIgnoreCase) +
                         _negativePrompt.Length;
            var sIndex = RawData.IndexOf(_stepsPrompt, StringComparison.OrdinalIgnoreCase);
            var s = RawData[nIndex..sIndex];
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
        return RawData.Substring(start, end - start).Trim();
    }
}