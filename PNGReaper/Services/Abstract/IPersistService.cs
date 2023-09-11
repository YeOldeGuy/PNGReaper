using PNGReaper.Helpers;

namespace PNGReaper.Services.Abstract;

internal interface IPersistService
{
    AppTheme Theme { get; set; }

    WindowPlacement StartPosition { get; set; }
}