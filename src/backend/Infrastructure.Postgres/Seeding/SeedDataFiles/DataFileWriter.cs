using System.Text.Json;

namespace Infrastructure.Postgres.Seeding.SeedDataFiles;

internal class DataFileWriter : IDisposable
{
    private readonly Utf8JsonWriter _writer;
    private readonly FileStream _fs;

    private static JsonWriterOptions _jsonOptions = new JsonWriterOptions
    {
        Indented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    internal DataFileWriter(string fileName)
    {
        _fs = File.Create(fileName);
        _writer = new Utf8JsonWriter(_fs, _jsonOptions);
    }

    internal void BeginWriteFile()
    {
        _writer.WriteStartArray();
    }

    internal void EndWriteFile()
    {
        _writer.WriteEndArray();
        _writer.Flush();
    }

    internal void WriteJsonObject(List<(string Name, JsonElement Value)> buffer)
    {
        _writer.WriteStartObject();
        foreach ((string name, JsonElement value) in buffer)
        {
            _writer.WritePropertyName(name);
            value.WriteTo(_writer);
        }
        _writer.WriteEndObject();
    }

    public void Dispose()
    {
        try
        {
            _writer?.Flush();
        }
        finally
        {
            _writer.Dispose();
            _fs.Dispose();
        }
    }
}
