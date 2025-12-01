using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Postgres.Seeding.Shared;

/// <summary>
/// Генрация GUID детерминированого на основе значений: namespace (Guid) и name (string)
/// по стандарту UUIDv5 (RFC 4122)
/// </summary>
public static class Uuid5
{
    // ВАЖНО: нужно зафиксировать namespace один раз и больше не менять
    public static readonly Guid SeedNamespace = new("6ba7b810-9dad-11d1-80b4-00c04fd430c8"); // пример: DNS

    private static readonly ReadOnlyMemory<byte> SeedNamespaceBytes = GuidToBigEndianBytes(SeedNamespace);

    private static byte[] GuidToBigEndianBytes(Guid seedNamespace)
    {
        Span<byte> nsBig = stackalloc byte[16];
        WriteGuidBigEndian(seedNamespace, nsBig);

        return nsBig.ToArray();
    }

    /// <summary>
    /// Создаёт детерминированный UUIDv5 (RFC 4122) на основе указанного <paramref name="name"/>,
    /// используя заранее подготовленный namespace <see cref="SeedNamespace"/>.
    /// Работает быстрее, чем <see cref="Create"/> за счёт отсутствия повторного преобразования namespace.
    /// </summary>
    /// <param name="name">Уникальная строка внутри фиксированного namespace.</param>
    /// <returns>UUID версии 5, детерминированно рассчитанный из имени и SeedNamespace.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если указанная строка пуста или null.</exception>
    public static Guid CreateFast(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));

        byte[] nameBytes = Encoding.UTF8.GetBytes(name);

        using var sha1 = SHA1.Create();
        sha1.TransformBlock(SeedNamespaceBytes.ToArray(), 0, 16, null, 0);
        sha1.TransformFinalBlock(nameBytes, 0, nameBytes.Length);

        return PostProcess(sha1.Hash!);
    }


    /// <summary>
    /// Создаёт детерминированный UUIDv5 (RFC 4122) на основе указанного <paramref name="name"/> 
    /// и переданного пространства имён <paramref name="ns"/>.
    /// Результат является стабильным и идентичным на всех платформах при одинаковых входных данных.
    /// </summary>
    /// <param name="ns">Пространство имён UUID (namespace), определяющее домен уникальности.</param>
    /// <param name="name">Уникальная строка внутри указанного пространства имён.</param>
    /// <returns>UUID версии 5, детерминированно рассчитанный из сочетания <paramref name="ns"/> и <paramref name="name"/>.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если указанная строка пуста или равна null.</exception>
    public static Guid Create(Guid ns, string name)

    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));

        Span<byte> nsBig = stackalloc byte[16];
        WriteGuidBigEndian(ns, nsBig);

        byte[] nameBytes = Encoding.UTF8.GetBytes(name);

        using var sha1 = SHA1.Create();
        sha1.TransformBlock(nsBig.ToArray(), 0, 16, null, 0);
        sha1.TransformFinalBlock(nameBytes, 0, nameBytes.Length);

        return PostProcess(sha1.Hash!);
    }

    private static Guid PostProcess(byte[] hash)            // 20 байт
    {
        Span<byte> uuid = stackalloc byte[16];
        hash.AsSpan(0, 16).CopyTo(uuid);

        // version = 5 (0101)
        uuid[6] = (byte)((uuid[6] & 0x0F) | 0x50);
        // variant = RFC 4122 (10xxxxxx)
        uuid[8] = (byte)((uuid[8] & 0x3F) | 0x80);

        return ReadGuidBigEndian(uuid);
    }

    // Guid → big-endian (network order) 16 байт
    private static void WriteGuidBigEndian(Guid g, Span<byte> dest)
    {
        byte[] b = g.ToByteArray();
        dest[0] = b[3]; dest[1] = b[2]; dest[2] = b[1]; dest[3] = b[0];
        dest[4] = b[5]; dest[5] = b[4];
        dest[6] = b[7]; dest[7] = b[6];
        dest[8] = b[8]; dest[9] = b[9]; dest[10] = b[10]; dest[11] = b[11];
        dest[12] = b[12]; dest[13] = b[13]; dest[14] = b[14]; dest[15] = b[15];
    }

    // big-endian 16 байт → Guid (внутренний little-endian для полей Data1..3)
    private static Guid ReadGuidBigEndian(ReadOnlySpan<byte> src)
    {
        Span<byte> b = stackalloc byte[16];
        b[0] = src[3]; b[1] = src[2]; b[2] = src[1]; b[3] = src[0];
        b[4] = src[5]; b[5] = src[4];
        b[6] = src[7]; b[7] = src[6];
        b[8] = src[8]; b[9] = src[9]; b[10] = src[10]; b[11] = src[11];
        b[12] = src[12]; b[13] = src[13]; b[14] = src[14]; b[15] = src[15];
        return new Guid(b);
    }
}

