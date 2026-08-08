using System.Globalization;
using System.Security.Cryptography;

namespace WealthOS.Infrastructure.Investments.Providers;

/// <summary>RFC 6238 TOTP (6 digits / 30s) for Angel One SmartAPI login.</summary>
internal static class AngelOneTotp
{
    public static string Generate(string base32Secret, DateTimeOffset utcNow)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(base32Secret);

        var key = DecodeBase32(base32Secret.Trim().Replace(" ", string.Empty, StringComparison.Ordinal));
        var counter = (ulong)Math.Floor(utcNow.ToUnixTimeSeconds() / 30d);
        var counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(counterBytes);
        }

        var hash = HMACSHA1.HashData(key, counterBytes);
        var offset = hash[^1] & 0x0F;
        var binary =
            ((hash[offset] & 0x7F) << 24)
            | ((hash[offset + 1] & 0xFF) << 16)
            | ((hash[offset + 2] & 0xFF) << 8)
            | (hash[offset + 3] & 0xFF);

        var otp = binary % 1_000_000;
        return otp.ToString("D6", CultureInfo.InvariantCulture);
    }

    private static byte[] DecodeBase32(string input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var cleaned = input.TrimEnd('=').ToUpperInvariant();
        var bits = 0;
        var value = 0;
        var output = new List<byte>(cleaned.Length * 5 / 8);

        foreach (var c in cleaned)
        {
            var idx = alphabet.IndexOf(c);
            if (idx < 0)
            {
                throw new FormatException("Invalid Base32 TOTP secret.");
            }

            value = (value << 5) | idx;
            bits += 5;
            if (bits >= 8)
            {
                output.Add((byte)((value >> (bits - 8)) & 0xFF));
                bits -= 8;
            }
        }

        return output.ToArray();
    }
}
