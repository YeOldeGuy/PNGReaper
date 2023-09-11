using System;
using System.IO;
using System.Threading.Tasks;

namespace PNGReaper.Services.Abstract;

public enum StorageStrategy
{
    /// <summary>
    ///     The <c>C:\Users\{username}\AppData\Local\{appname}</c> directory.
    /// </summary>
    Local,

    /// <summary>
    ///     The <c>C:\Users\{username}\AppData\Roaming\{appname}</c> directory.
    /// </summary>
    Roaming,

    /// <summary>
    ///     The <c>C:\Users\{username}\AppData\Local\Temp</c> directory (normally).
    /// </summary>
    Temporary,

    /// <summary>
    ///     Use this if you already have a full path name and don't want to
    ///     prepend any path.
    /// </summary>
    None
}

/// <summary>
///     Provides a common service for accessing files. Remember, kids:
///     Anything that can be designed can be over-designed.
/// </summary>
public interface IFileService
{
    /// <summary>
    ///     Gets the full path name of the directory specified by the
    ///     <see cref="StorageStrategy" />.
    /// </summary>
    /// <param name="strategy">The <see cref="StorageStrategy" />.</param>
    /// <returns>A full path name for the location specified.</returns>
    string? GetStorageLocation(StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Fittingly, creates a directory, but is limited to creating only those that
    ///     can be reached via the <see cref="StorageStrategy" />.
    /// </summary>
    /// <param name="strategy">The directory location to create.</param>
    void CreateDirectory(StorageStrategy strategy);

    /// <summary>
    ///     Given the specified <see cref="StorageStrategy" />, return the path
    ///     name of the specified file in that location, irregardless of whether the
    ///     file exists or not.
    /// </summary>
    /// <param name="filename">
    ///     A filename with no directory.
    /// </param>
    /// <param name="strategy">
    ///     Location to place the specified file in.
    /// </param>
    /// <returns>
    ///     A fully qualified path name to the specified file.
    /// </returns>
    string? GetFilePath(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Generates a unique file name in the temporary directory. Does not
    ///     create the file. Uses the system's temp file generator, so it'll
    ///     be a classic 8.3 name.
    /// </summary>
    /// <param name="suffix">A file suffix to append, if desired.</param>
    /// <param name="strategy">Storage location of the temporary file.</param>
    /// <returns>Returns a fully qualified path name to the temporary file.</returns>
    string GetTempFile(string? suffix = "",
        StorageStrategy strategy = StorageStrategy.Temporary);

    /// <summary>
    ///     Asynchronously test for existence of a file in the specified
    ///     location. A fully-specified file name will ignore the strategy
    ///     specification.
    /// </summary>
    /// <param name="filename">
    ///     A file path name, rooted in the <paramref name="location" />.
    /// </param>
    /// <param name="location"></param>
    /// <returns>True if the file exists.</returns>
    Task<bool> FileExistsAsync(string? filename,
        StorageStrategy location = StorageStrategy.Local);

    /// <summary>
    ///     Test for existence of a file in the specified location. A fully-specified
    ///     file name will ignore the strategy specification.
    /// </summary>
    /// <returns>True if the file exists.</returns>
    bool FileExists(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Read all of the text in the specified file.
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="strategy">Location of the file.</param>
    /// <returns>Text of the file as a string.</returns>
    string? ReadAllText(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Asynchronously read all of the text in the specified file.
    /// </summary>
    /// <param name="filename">
    ///     A file path name, rooted in the <paramref name="strategy" /> location.
    /// </param>
    /// <param name="strategy">Location of the file.</param>
    /// <returns>Text of the file as a string.</returns>
    Task<string?> ReadAllTextAsync(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Asynchronously read all bytes from the specified file.
    /// </summary>
    /// <param name="filename">
    ///     A file path name, rooted in the <paramref name="strategy" /> location.
    /// </param>
    /// <param name="strategy">Location of the file.</param>
    /// <returns>An array of <see cref="byte" /> values, uninterpreted.</returns>
    Task<byte[]?> ReadAllBytesAsync(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Read all bytes from the specified file.
    /// </summary>
    /// <param name="filename">
    ///     A file path name, rooted in the <paramref name="strategy" /> location.
    /// </param>
    /// <param name="strategy">Location of the file.</param>
    /// <returns>An array of <see cref="byte" /> values, uninterpreted.</returns>
    byte[]? ReadAllBytes(string? filename, StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Write the buffer of raw bytes to the specified file.
    /// </summary>
    /// <param name="buffer">A buffer of raw bytes.</param>
    /// <param name="filename">Name of the file.</param>
    /// <param name="strategy">Location of the file.</param>
    /// <returns>True if the file exists after writing.</returns>
    bool WriteAllBytes(byte[]? buffer,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Asynchronously write the specified buffer of uninterpreted bytes to
    ///     the specified file.
    /// </summary>
    /// <param name="buffer">A buffer of raw bytes.</param>
    /// <param name="filename">Name of the file.</param>
    /// <param name="strategy">Location of the file.</param>
    /// <returns>True if the file exists after writing.</returns>
    Task<bool> WriteAllBytesAsync(byte[]? buffer,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Write the UTF-8 encoded string to the specified file.
    /// </summary>
    /// <param name="contents">The text to write</param>
    /// <param name="filename">Name of the file.</param>
    /// <param name="strategy">Location of the file.</param>
    /// <returns>True if the file exists after writing.</returns>
    bool WriteAllText(string? contents,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Asynchronously write the UTF-8 encoded string to the specified file.
    /// </summary>
    /// <param name="contents">The text to write</param>
    /// <param name="filename">Name of the file.</param>
    /// <param name="strategy">Location of the file.</param>
    /// <returns>True if the file exists after writing.</returns>
    Task<bool> WriteAllTextAsync(string? contents,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Asynchronously write the stream to a file.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="filename"></param>
    /// <param name="strategy"></param>
    /// <returns>True if the file exists after writing.</returns>
    Task<bool> WriteStreamAsync(Stream? stream,
        string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Locates an embedded resource, then copies it to the specified file.
    /// </summary>
    /// <param name="resourceName">
    ///     The resource name. This is normally the Assembly, followed
    ///     by the name of the file, with spaces replaced by underscores.<br />
    ///     Ex: <c>Wave2.GeoLite2-City_20220913.tar.gz</c>
    /// </param>
    /// <param name="fileName">The file name to write the resource to</param>
    /// <param name="strategy">The destination folder, normally Local</param>
    /// <returns></returns>
    Task<bool> ExtractEmbeddedResourceAsync(string? resourceName,
        string? fileName,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Deletes the specified file at the given location.
    /// </summary>
    /// <param name="filename">
    ///     The file to delete.
    /// </param>
    /// <param name="strategy">
    ///     The directory the file is located in.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the file was deleted, <see langword="false" />
    ///     otherwise.
    /// </returns>
    bool Delete(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    Task<bool> DeleteAsync(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Moves the specified file to another location. By default, the full
    ///     paths of the files should be specified.
    /// </summary>
    /// <param name="sourceFile">
    ///     The file to move.
    /// </param>
    /// <param name="destinationFile">
    ///     The file name of the new file after moving (can be the same as
    ///     <paramref name="sourceFile" />
    /// </param>
    /// <param name="sourceStrategy">
    ///     The directory the file to move is located in.
    /// </param>
    /// <param name="destinationStrategy">
    ///     The directory to move the file to.
    /// </param>
    /// <param name="overwrite">
    ///     If true, any existing file will be overwritten on the move.
    ///     Defaults to false (don't overwrite), which will throw an exception
    ///     if the file exists at the destination.
    /// </param>
    /// <remarks>
    ///     It is entirely possible and acceptable for the source and
    ///     destination to be the same; the system will figure it out.
    /// </remarks>
    /// <exception cref="IOException">
    ///     Thrown if the destination file already exists.
    /// </exception>
    void Move(string? sourceFile,
        string? destinationFile,
        StorageStrategy sourceStrategy = StorageStrategy.None,
        StorageStrategy destinationStrategy = StorageStrategy.None,
        bool overwrite = false);

    /// <summary>
    ///     Copies the specified file to another location. By default, the full
    ///     paths of the files should be specified.
    /// </summary>
    /// <param name="sourceFile">
    ///     The file to move.
    /// </param>
    /// <param name="destinationFile">
    ///     The file name of the new file after moving (can be the same as
    ///     <paramref name="sourceFile" />
    /// </param>
    /// <param name="sourceStrategy">
    ///     The directory the file to move is located in.
    /// </param>
    /// <param name="destinationStrategy">
    ///     The directory to move the file to.
    /// </param>
    /// <param name="overwrite">
    ///     If true, any existing file will be overwritten on the move.
    ///     Defaults to false (don't overwrite), which will throw an exception
    ///     if the file exists at the destination.
    /// </param>
    /// <remarks>
    ///     It is entirely possible and acceptable for the source and
    ///     destination to be the same; the system will figure it out.
    /// </remarks>
    /// <exception cref="IOException">
    ///     Thrown if the destination file already exists.
    /// </exception>
    void Copy(string? sourceFile,
        string? destinationFile,
        StorageStrategy sourceStrategy = StorageStrategy.None,
        StorageStrategy destinationStrategy = StorageStrategy.None,
        bool overwrite = false);

    /// <summary>
    ///     Returns the age of the file as a <see cref="TimeSpan" /> of the
    ///     latest write time subtracted from the current time.
    /// </summary>
    /// <param name="filename">The file name to check.</param>
    /// <param name="strategy">The location where the file is.</param>
    /// <returns>The age of the file, based on the LastWriteTime.</returns>
    TimeSpan GetFileAge(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Decompress (unzip) the specified file, returning the name of the
    ///     new file. Specify the full file path if you use the default
    ///     <paramref name="strategy" />. The new file name is the specified
    ///     path without the file extension. If the file has no extension, the
    ///     new file name will have ".tmp" added and you are required to
    ///     reevaluate your life choices.
    /// </summary>
    /// <remarks>
    ///     The old file will <b>not</b> be deleted.
    /// </remarks>
    /// <param name="filename">File to decompress</param>
    /// <param name="strategy">
    ///     One of the <see cref="StorageStrategy" /> values. Defaults to None.
    /// </param>
    /// <returns>
    ///     Full path name of decompressed file.
    /// </returns>
    Task<string?> DecompressFileAsync(string? filename,
        StorageStrategy strategy = StorageStrategy.None);

    Task<bool> DecompressFileTo(string? compressedSource, string decompressedDestination);


    /// <summary>
    ///     Gets the creation date of the specified file.
    /// </summary>
    /// <param name="filename">File name to get information for.</param>
    /// <param name="strategy">The location of the file in the file system.</param>
    /// <returns>Creation date as a <see cref="DateTime" /> value.</returns>
    DateTime GetFileLastWriteTime(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Gets the size of the specified file in bytes, asynchronously.
    /// </summary>
    /// <param name="filename">File name to get information for.</param>
    /// <param name="strategy">The location of the file in the file system.</param>
    /// <returns>Creation date as a <see cref="long" /> value.</returns>
    Task<long> GetFileSizeAsync(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);

    /// <summary>
    ///     Gets the size of the specified file in bytes.
    /// </summary>
    /// <param name="filename">File name to get information for.</param>
    /// <param name="strategy">The location of the file in the file system.</param>
    /// <returns>Creation date as a <see cref="long" /> value.</returns>
    long GetFileSize(string? filename,
        StorageStrategy strategy = StorageStrategy.Local);
}