using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Koyashiro.PngChunkUtil;
using PNGReaper.Services.Abstract;
using Prism.Commands;
using Prism.Mvvm;

namespace PNGReaper.ViewModels;

internal class ShellViewModel : BindableBase
{
    private readonly IPersistService  _persistService;
    private readonly IPngParseService _pngParseService;
    private readonly IFileService _fileService;
    private          string?          _cfg;

    private DelegateCommand? _CopyNegPrompt;
    private DelegateCommand? _CopyPrompt;
    private DelegateCommand? _CopySeed;

    private string? _imageFile;
    private string? _model;
    private string? _modelHash;
    private string? _negativePrompt;
    private string? _prompt;
    private string? _raw;
    private string? _sampler;
    private string? _seed;
    private string? _size;
    private string? _steps;

    public ShellViewModel(IPngParseService pngParseService,
        IPersistService persistService,
        IFileService fileService)
    {
        _pngParseService = pngParseService;
        _persistService  = persistService;
        _fileService = fileService;

        ImageFile = string.IsNullOrEmpty(_persistService.LastFile) 
            ? @"C:\Users\kewal\StableDiffusion\webui\outputs\txt2img-images\2023-09-09\00121-1846211428.png" 
            : _persistService.LastFile;
    }

    public string? ImageFile
    {
        get => _imageFile;
        set
        {
            if (_fileService.FileExists(value))
            {
                SetProperty(ref _imageFile, value);
                ParsePNG(value);

                Raw = _pngParseService.RawData;
                Prompt = _pngParseService.Prompt;
                NegativePrompt = _pngParseService.NegativePrompt;
                Seed = _pngParseService.Seed;
                Size = _pngParseService.Size;
                Model = _pngParseService.Model;
                ModelHash = _pngParseService.ModelHash;
                CFG = _pngParseService.CFGScale;
                Sampler = _pngParseService.Sampler;
                Steps = _pngParseService.Steps;
            }
            else
            {
                Raw = string.Empty;
                Prompt = string.Empty;
                NegativePrompt = string.Empty;
                Seed = string.Empty;
                Size = string.Empty;
                Model = string.Empty;
                ModelHash = string.Empty;
                CFG = string.Empty;
                Sampler = string.Empty;
                Steps = string.Empty;
            }
        }
    }

    public string? Raw
    {
        get => _raw;
        set => SetProperty(ref _raw, value);
    }

    public string? Prompt
    {
        get => _prompt;
        set
        {
            if (SetProperty(ref _prompt, value)) RaisePropertyChanged(nameof(CopyPrompt));
        }
    }

    public string? NegativePrompt
    {
        get => _negativePrompt;
        set
        {
            if (SetProperty(ref _negativePrompt, value)) RaisePropertyChanged(nameof(CopyNegPrompt));
        }
    }

    public string? Steps
    {
        get => _steps;
        set => SetProperty(ref _steps, value);
    }

    public string? Sampler
    {
        get => _sampler;
        set => SetProperty(ref _sampler, value);
    }

    public string? Seed
    {
        get => _seed;
        set
        {
            if (SetProperty(ref _seed, value)) RaisePropertyChanged(nameof(CopySeed));
        }
    }

    public string? CFG
    {
        get => _cfg;
        set => SetProperty(ref _cfg, value);
    }

    public string? Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    public string? ModelHash
    {
        get => _modelHash;
        set => SetProperty(ref _modelHash, value);
    }

    public string? Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    public DelegateCommand CopyPrompt =>
        _CopyPrompt ??= new DelegateCommand(() => { Clipboard.SetText(Prompt ?? ""); });

    public DelegateCommand CopyNegPrompt =>
        _CopyNegPrompt ??= new DelegateCommand(() => { Clipboard.SetText(NegativePrompt ?? ""); });

    public DelegateCommand CopySeed =>
        _CopySeed ??= new DelegateCommand(() => { Clipboard.SetText(Seed ?? ""); });

    private void ParsePNG(string? filename)
    {
        if (string.IsNullOrEmpty(filename)) 
            return;

        var chunks = PngReader.ReadBytes(File.ReadAllBytes(filename)).ToList();
        var rawText = chunks.FirstOrDefault(c => c.ChunkType!.Equals("tEXt", StringComparison.Ordinal));
        var allText = Encoding.ASCII.GetString(rawText.ChunkDataBytes);

        _persistService.LastFile = filename;

        _pngParseService.SetText(allText);
        _persistService.Save();
    }
}