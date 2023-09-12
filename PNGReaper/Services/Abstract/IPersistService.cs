using PNGReaper.Helpers;

namespace PNGReaper.Services.Abstract;

public interface IPersistService
{
    AppTheme Theme { get; set; }

    WindowPlacement StartPosition { get; set; }

    string LastFile { get; set; }
    bool Save();
}