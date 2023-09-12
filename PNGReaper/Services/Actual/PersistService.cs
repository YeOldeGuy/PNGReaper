using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;
using PNGReaper.Helpers;
using PNGReaper.Services.Abstract;
using Prism.Mvvm;

namespace PNGReaper.Services.Actual;

internal class PersistService : BindableBase, IPersistService
{
    private readonly IFileService                         _fileService;
    private readonly SemaphoreSlim                        _saveSemaphore;
    private readonly ConcurrentDictionary<string, string> _settings;
    private readonly string                               _storageName;

    public PersistService(IFileService fileService)
    {
        _fileService = fileService;

        _saveSemaphore = new SemaphoreSlim(1, 1);
        _settings      = new ConcurrentDictionary<string, string>();
        _storageName   = "pngreaper.json";

        if (fileService.FileExists(_storageName))
        {
            var json = fileService.ReadAllText(_storageName);
            if (json is not null)
                _settings = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(json)!;
        }
    }

    public bool Save()
    {
        var obtained = _saveSemaphore.Wait(TimeSpan.FromSeconds(3));
        if (obtained)
            try
            {
                var json = JsonConvert.SerializeObject(_settings);
                _fileService.WriteAllText(json, _storageName);
                return true;
            }
            finally
            {
                _saveSemaphore.Release();
            }

        return false;
    }

    public AppTheme Theme
    {
        get => Read(AppTheme.Default);
        set => Write(value);
    }

    public WindowPlacement StartPosition
    {
        get => Read(new WindowPlacement());
        set => Write(value);
    }

    public string LastFile
    {
        get => Read("");
        set => Write(value);
    }

    private T Read<T>(T defaultValue, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
            throw new ArgumentNullException(nameof(propertyName));

        if (_settings.TryGetValue(propertyName, out var setting))
        {
            if (string.IsNullOrEmpty(setting))
                return defaultValue;
            var rep = JsonConvert.DeserializeObject<T>(setting);
            return rep == null ? defaultValue : rep;
        }

        var json = JsonConvert.SerializeObject(defaultValue);
        _settings.TryAdd(propertyName, json);
        return defaultValue;
    }

    private void Write<T>(T value, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
            throw new ArgumentNullException(nameof(propertyName));

        var json = JsonConvert.SerializeObject(value);
        if (_settings.ContainsKey(propertyName))
        {
            var oldJson = _settings[propertyName];
            if (string.Equals(json, oldJson))
                return; // no change means no need to save
            _settings[propertyName] = json;
            RaisePropertyChanged(propertyName);
        }
        else
        {
            _settings.TryAdd(propertyName, json);
        }
    }
}