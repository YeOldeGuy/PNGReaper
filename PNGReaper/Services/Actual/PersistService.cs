using PNGReaper.Helpers;
using PNGReaper.Services.Abstract;

namespace PNGReaper.Services.Actual;

internal class PersistService : IPersistService
{
    public AppTheme Theme { get; set; }

    public WindowPlacement StartPosition { get; set; }
}