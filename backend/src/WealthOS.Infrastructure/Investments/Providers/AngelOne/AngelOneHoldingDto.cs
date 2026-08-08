using System.Text.Json;
using System.Text.Json.Serialization;
using WealthOS.Domain.Investments.Enums;

namespace WealthOS.Infrastructure.Investments.Providers;

/// <summary>Normalized Angel One holding row used for upsert.</summary>
public sealed class AngelOneHoldingDto
{
    public string TradingSymbol { get; init; } = string.Empty;
    public string Exchange { get; init; } = string.Empty;
    public string? Isin { get; init; }
    public string? SymbolToken { get; init; }
    public decimal Quantity { get; init; }
    public decimal AveragePrice { get; init; }
    public decimal Ltp { get; init; }
    public decimal Close { get; init; }
    public decimal ProfitAndLoss { get; init; }
    public decimal PnlPercentage { get; init; }

    public string MatchKey =>
        !string.IsNullOrWhiteSpace(Isin)
            ? Isin.Trim().ToUpperInvariant()
            : TradingSymbol.Trim().ToUpperInvariant();

    public string DisplayName
    {
        get
        {
            var symbol = TradingSymbol.Trim();
            if (symbol.EndsWith("-EQ", StringComparison.OrdinalIgnoreCase))
            {
                return symbol[..^3];
            }

            return string.IsNullOrWhiteSpace(symbol) ? MatchKey : symbol;
        }
    }

    public decimal InvestedAmount => RoundMoney(AveragePrice * Quantity);

    public decimal CurrentValue => RoundMoney(Ltp * Quantity);

    public decimal DayChange => RoundMoney((Ltp - Close) * Quantity);

    public decimal DayChangePercent =>
        Close == 0m ? 0m : RoundPercent((Ltp - Close) / Close * 100m);

    public InvestmentCategory Category => InferCategory(TradingSymbol, Exchange);

    public InvestmentType InvestmentType =>
        Category == InvestmentCategory.MutualFunds ? InvestmentType.MutualFund : InvestmentType.Equity;

    public string SyncNotes =>
        $"angelone|isin={Isin}|token={SymbolToken}|exchange={Exchange}";

    public static IReadOnlyList<AngelOneHoldingDto> ParseAllHoldingsPayload(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
        {
            return [];
        }

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (root.TryGetProperty("status", out var status) &&
            status.ValueKind == JsonValueKind.False)
        {
            var message = root.TryGetProperty("message", out var msg) ? msg.GetString() : "Angel One holdings failed.";
            throw new InvalidOperationException(message ?? "Angel One holdings failed.");
        }

        JsonElement data = root;
        if (root.TryGetProperty("data", out var dataNode))
        {
            data = dataNode;
        }

        if (data.ValueKind == JsonValueKind.Null || data.ValueKind == JsonValueKind.Undefined)
        {
            return [];
        }

        IEnumerable<JsonElement> rows;
        if (data.ValueKind == JsonValueKind.Array)
        {
            rows = data.EnumerateArray();
        }
        else if (data.TryGetProperty("holdings", out var holdings) && holdings.ValueKind == JsonValueKind.Array)
        {
            rows = holdings.EnumerateArray();
        }
        else
        {
            return [];
        }

        return rows
            .Select(ParseRow)
            .Where(h => h.Quantity > 0m && !string.IsNullOrWhiteSpace(h.MatchKey))
            .ToList();
    }

    private static AngelOneHoldingDto ParseRow(JsonElement row) =>
        new()
        {
            TradingSymbol = ReadString(row, "tradingsymbol"),
            Exchange = ReadString(row, "exchange"),
            Isin = NullIfEmpty(ReadString(row, "isin")),
            SymbolToken = NullIfEmpty(ReadString(row, "symboltoken")),
            Quantity = ReadDecimal(row, "quantity"),
            AveragePrice = ReadDecimal(row, "averageprice"),
            Ltp = ReadDecimal(row, "ltp"),
            Close = ReadDecimal(row, "close"),
            ProfitAndLoss = ReadDecimal(row, "profitandloss"),
            PnlPercentage = ReadDecimal(row, "pnlpercentage"),
        };

    private static InvestmentCategory InferCategory(string tradingSymbol, string exchange)
    {
        var symbol = tradingSymbol.ToUpperInvariant();
        var ex = exchange.ToUpperInvariant();
        if (symbol.Contains("ETF", StringComparison.Ordinal) || symbol.Contains("GOLD", StringComparison.Ordinal))
        {
            return symbol.Contains("GOLD", StringComparison.Ordinal)
                ? InvestmentCategory.GoldEtfs
                : InvestmentCategory.Stocks;
        }

        if (ex is "BSE" or "NSE" || symbol.EndsWith("-EQ", StringComparison.Ordinal))
        {
            return InvestmentCategory.Stocks;
        }

        if (ex.Contains("MF", StringComparison.Ordinal) || symbol.Contains("MF", StringComparison.Ordinal))
        {
            return InvestmentCategory.MutualFunds;
        }

        return InvestmentCategory.Stocks;
    }

    private static string ReadString(JsonElement row, string name)
    {
        if (!row.TryGetProperty(name, out var value))
        {
            return string.Empty;
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.ToString(),
            _ => string.Empty,
        };
    }

    private static decimal ReadDecimal(JsonElement row, string name)
    {
        if (!row.TryGetProperty(name, out var value))
        {
            return 0m;
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number => value.TryGetDecimal(out var d) ? d : 0m,
            JsonValueKind.String => decimal.TryParse(value.GetString(), out var parsed) ? parsed : 0m,
            _ => 0m,
        };
    }

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static decimal RoundMoney(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static decimal RoundPercent(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);
}

internal sealed class AngelOneApiEnvelope
{
    [JsonPropertyName("status")]
    public bool Status { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errorcode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}

internal sealed class AngelOneLoginData
{
    [JsonPropertyName("jwtToken")]
    public string? JwtToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("feedToken")]
    public string? FeedToken { get; set; }
}
