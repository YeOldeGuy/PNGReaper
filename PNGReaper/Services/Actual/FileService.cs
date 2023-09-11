using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PNGReaper.Helpers;
using PNGReaper.Services.Abstract;

namespace PNGReaper.Services.Actual;

internal class FileService : IFileService
{
    private readonly string _baseDirectory;

    public FileService(AppConfig appConfig)
    {
        _baseDirectory = appConfig.ConfigurationsFolder;
    }

    public void CreateDirectory(StorageStrategy strategy)
    {
        var dir = GetStorageLocation(strategy);
        Directory.CreateDirectory(dir);
    }

    public string GetStorageLocation(StorageStrategy strategy = StorageStrategy.Local)
    {
        // This should be something like "C:/Users/bobke/AppData":
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        switch (strategy)
        {
            // Return something like "C:/Users/bobke/AppData/Local/<base dir>"
            case StorageStrategy.Local:
                var localPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    _baseDirectory);
                if (!Directory.Exists(localPath))
                    Directory.CreateDirectory(localPath);
                return localPath;

            // Return something like "C:/Users/bobke/AppData/Roaming/<base dir>" 
            case StorageStrategy.Roaming:
                var roamingPath = Path.Combine(appDataFolder, "Roaming", _baseDirectory);
                if (!Directory.Exists(roamingPath))
                    Directory.CreateDirectory(roamingPath);
                return roamingPath;

            // Return something like: "C:/Users/bobke/AppData/Local/Temp"
            case StorageStrategy.Temporary:
                return Path.GetTempPath();

            // Finally, return the empty string for no strategy
            case StorageStrategy.None:
            default:
                return string.Empty;
        }
    }

    public string? GetFilePath(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        var dir = GetStorageLocation(strategy);

        return filename == null ? null : Path.Combine(dir, filename);
    }

    public string GetTempFile(string? suffix = "", StorageStrategy strategy = StorageStrategy.Temporary)
    {
        suffix ??= "";

        var path = GetStorageLocation(strategy);
        var rand = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        if (!string.IsNullOrEmpty(suffix) && !suffix.StartsWith("."))
            suffix = "." + suffix;

        var temp = Path.Combine(path, $"{rand}{suffix}");
        // _logger.LogInformation("[FLS] Temp file name: {Temp}", temp);
        return temp;
    }

    public async Task<bool> FileExistsAsync(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename is null)
            return false;

        return await Task.Run(() => FileExists(filename, strategy));
    }

    public bool FileExists(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename == null)
            return false;

        if (Path.IsPathRooted(filename))
            return File.Exists(filename);

        var path = GetFilePath(filename, strategy);
        var exists = File.Exists(path);
        // _logger.LogInformation("[FLS] File {File} {Exist}", path, exists ? "exists" : "does not exist");
        return exists;
    }

    public string? ReadAllText(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename == null)
            return null;

        var path = GetFilePath(filename, strategy);
        if (path is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", filename);
            return null;

        var data = File.ReadAllText(path);
        // _logger.LogInformation("[FLS] Read {Count} bytes of data from {Path}", data.Length, path);
        return data;
    }

    public async Task<string?> ReadAllTextAsync(string? filename,
        StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename == null)
            return null;

        var path = GetFilePath(filename, strategy);
        if (path is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", filename);
            return null;

        using var reader = new StreamReader(path);
        var data = await reader.ReadToEndAsync();
        // _logger.LogInformation("Read [FLS] {Count} bytes of data from {Path}", data.Length, path);
        return data;
    }

    public async Task<byte[]?> ReadAllBytesAsync(string? filename,
        StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename == null)
            return null;

        return await Task.Run(() =>
        {
            var path = GetFilePath(filename, strategy);
            if (path is null)
                // _logger.LogWarning("[FLS] {File} generated a null path", filename);
                return null;

            var buf = File.ReadAllBytes(path);
            // _logger.LogInformation("[FLS] Read {Count} bytes of data from {Path}", buf.Length, path);
            return buf;
        });
    }

    public byte[]? ReadAllBytes(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename == null)
            return null;

        if (!FileExists(filename, strategy))
            return null;
        var path = GetFilePath(filename, strategy);
        if (path is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", filename);
            return null;

        var data = File.ReadAllBytes(path);
        // _logger.LogInformation("[FLS] Read {Count} bytes of data from {File}", data.Length, path);
        return data;
    }

    public bool WriteAllText(string? contents,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename is null || contents is null)
            return false;

        var path = GetFilePath(filename, strategy);
        if (path is null)
            return false;
        File.WriteAllText(path, contents, Encoding.UTF8);
        // _logger.LogInformation("[FLS] FileService.WriteAllText wrote {No} chars to {Name}", contents.Length, filename);
        return FileExists(path, strategy);
    }

    public async Task<bool> WriteAllTextAsync(string? contents,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename is null || contents is null)
            return false;

        return await Task.Run(() => WriteAllText(contents, filename, strategy));
    }

    public bool WriteAllBytes(byte[]? buffer,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local)
    {
        if (buffer is null || filename is null)
            return false;

        var path = GetFilePath(filename, strategy);
        if (path is null)
            return false;
        File.WriteAllBytes(path, buffer);
        return FileExists(path);
    }

    public async Task<bool> WriteAllBytesAsync(byte[]? buffer,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local)
    {
        if (buffer is null || filename is null)
            return false;

        var path = GetFilePath(filename, strategy);
        if (path is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", filename);
            return false;

        await using (var writer = new FileStream(path, FileMode.OpenOrCreate))
        {
            await writer.WriteAsync(buffer, 0, buffer.Length);
            writer.Flush();
            // _logger.LogInformation("[FLS] Wrote {Count} bytes to {Path}", buffer.Length, path);
        }

        return await FileExistsAsync(filename, strategy);
    }

    public async Task<bool> WriteStreamAsync(Stream? stream,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename is null || stream is null)
            return false;

        var path = GetFilePath(filename, strategy);
        if (path is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", filename);
            return false;

        await using (var outStream = new FileStream(path, FileMode.OpenOrCreate))
        {
            await stream.CopyToAsync(outStream);
            // _logger.LogInformation("[FLS] Wrote stream to {Path}", path);
        }

        return await FileExistsAsync(filename, strategy);
    }

    public async Task<bool> ExtractEmbeddedResourceAsync(string? resourceName,
        string? fileName,
        StorageStrategy strategy = StorageStrategy.Local)
    {
        if (resourceName is null || fileName is null)
            return false;
        var path = GetFilePath(fileName, strategy);
        if (path is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", fileName);
            return false;

        // _logger.LogInformation("[FLS] Attempting to extract {Resource} from executable", resourceName);
        await using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        {
            if (resource == null)
                return false;
            await using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
            await resource.CopyToAsync(file);

            // _logger.LogInformation("[FLS] Extracted {Length} bytes to {Path}", file.Length, path);
        }

        return await FileExistsAsync(fileName, strategy);
    }

    public bool Delete(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename is null)
            return false;
        var path = GetFilePath(filename, strategy);

        if (path is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", filename);
            return false;

        if (!File.Exists(path))
            // _logger.LogWarning("[FLS] Attempted to delete non-existent file, {File}", path);
            return false;

        // _logger.LogInformation("[FLS] Deleting {Path}", path);
        File.Delete(path);
        return !File.Exists(path);
    }

    public async Task<bool> DeleteAsync(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename == null)
            return false;
        var path = GetFilePath(filename, strategy);

        if (path is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", filename);
            return false;

        if (!File.Exists(path))
            // _logger.LogWarning("[FLS] Attempted to delete non-existent file, {File}", path);
            return false;

        // _logger.LogInformation("Deleting {Path}", path);
        File.Delete(path);
        return !await FileExistsAsync(filename, strategy);
    }

    public void Move(string? sourceFile,
        string? destinationFile,
        StorageStrategy sourceStrategy = StorageStrategy.None,
        StorageStrategy destinationStrategy = StorageStrategy.None,
        bool overwrite = false)
    {
        if (sourceFile is null || destinationFile is null)
            return;

#if NETCOREAPP
        var sourcePath = GetFilePath(sourceFile, sourceStrategy);
        var destinationPath = GetFilePath(destinationFile, destinationStrategy);
        if (sourcePath is null || destinationPath is null)
            return;
        File.Move(sourcePath, destinationPath, overwrite);
#else
        if (overwrite && File.Exists(GetFilePath(destinationFile, destinationStrategy)))
        {
            var f = GetFilePath(destinationFile, destinationStrategy);
            if (f != null)
                File.Delete(f);
        }

        var sFile = GetFilePath(sourceFile, sourceStrategy);
        var dFile = GetFilePath(destinationFile, destinationStrategy);

        // _logger.LogInformation("[FLS] Moving {Src} to {Dest}", sFile, dFile);

        if (sFile is null)
            return;
        if (dFile is null)
            return;
        File.Move(sFile, dFile);
#endif
    }

    public void Copy(string? sourceFile,
        string? destinationFile,
        StorageStrategy sourceStrategy = StorageStrategy.None,
        StorageStrategy destinationStrategy = StorageStrategy.None,
        bool overwrite = false)
    {
        if (sourceFile is null || destinationFile is null)
            return;

        var sourcePath = GetFilePath(sourceFile, sourceStrategy);
        var destinationPath = GetFilePath(destinationFile, destinationStrategy);
        if (sourcePath is null)
            // _logger.LogWarning("[FLS] File {Src} generated a null path", sourceFile);
            return;

        if (destinationPath is null)
            // _logger.LogWarning("[FLS] File {Src} generated a null path", destinationFile);
            return;

        // _logger.LogInformation("[FLS] Copying {Src} to {Dest}", sourcePath, destinationPath);
        File.Copy(sourcePath, destinationPath, overwrite);
    }


    public TimeSpan GetFileAge(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename is null)
            return TimeSpan.Zero;

        var filePath = GetFilePath(filename, strategy);
        if (filePath is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", filename);
            return TimeSpan.Zero;

        var delta = DateTime.Now - new FileInfo(filePath).LastWriteTime;
        // _logger.LogInformation("[FLS] File age of {Path}: {Delta}", filePath, delta);
        return delta;
    }


    public async Task<string?> DecompressFileAsync(string? filename,
        StorageStrategy strategy = StorageStrategy.None)
    {
        if (filename is null)
            return null;

        var actualFileName = strategy != StorageStrategy.None ? GetFilePath(filename, strategy) : filename;

        if (actualFileName is null)
            return string.Empty;
        var fileToDecompress = new FileInfo(actualFileName);

        var curFName = fileToDecompress.FullName;
        var extensionLength = fileToDecompress.Extension.Length;
        var newFileName = extensionLength > 0
            ? curFName.Remove(curFName.Length - fileToDecompress.Extension.Length)
            : curFName + ".tmp";

        // This newfangled syntax gets weird, doesn't it? Remember that
        // the "await" before each of these allows the system to do the
        // dispose asynchronously.
        await using var decompressedFileStream = File.Create(newFileName);
        await using var originalStream = fileToDecompress.OpenRead();
        await using var zipStream = new GZipStream(originalStream, CompressionMode.Decompress);

        // _logger.LogInformation("[FLS] Decompressing {Src} to {Dest}", actualFileName, newFileName);
        await zipStream.CopyToAsync(decompressedFileStream);

        return newFileName;
    }

    public async Task<bool> DecompressFileTo(string? source, string? destination)
    {
        // doesn't try to mess around with the strategy stuff, assuming that
        // read/write permissions were covered elsewhere

        if (source is null || destination is null)
            return false;
        var fileToDecompress = new FileInfo(source);

        await using var decompressedFileStream = File.Create(destination);
        await using var originalStream = fileToDecompress.OpenRead();
        await using var zipStream = new GZipStream(originalStream, CompressionMode.Decompress);
        // _logger.LogInformation("[FLS] Decompressing {Src} to {Dest}", source, destination);
        await zipStream.CopyToAsync(decompressedFileStream);

        return true;
    }

    public DateTime GetFileLastWriteTime(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename == null)
            return DateTime.Now;

        var path = GetFilePath(filename, strategy);
        if (path is null)
            return DateTime.MinValue;
        var fileInfo = new FileInfo(path);

        // _logger.LogInformation("[FLS] Write time of {Path}: {Date}", fileInfo.Name, fileInfo.LastWriteTime);
        return fileInfo.LastWriteTime;
    }

    public async Task<long> GetFileSizeAsync(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename is null)
            return 0;

        var size = await Task.Run(() => GetFileSize(filename, strategy));
        return size;
    }

    public long GetFileSize(string? filename, StorageStrategy strategy = StorageStrategy.Local)
    {
        if (filename is null)
            return 0;

        var path = GetFilePath(filename, strategy);
        if (path is null)
            // _logger.LogWarning("[FLS] {File} generated a null path", filename);
            return 0;

        var fileInfo = new FileInfo(path);
        // _logger.LogInformation("[FLS] Size of {Path}: {Bytes}", fileInfo.Name, fileInfo.Length);
        return fileInfo.Length;
    }
}