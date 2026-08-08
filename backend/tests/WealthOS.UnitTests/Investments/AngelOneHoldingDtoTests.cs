using FluentAssertions;
using WealthOS.Domain.Investments.Enums;
using WealthOS.Infrastructure.Investments.Providers;

namespace WealthOS.UnitTests.Investments;

public sealed class AngelOneHoldingDtoTests
{
    [Fact]
    public void ParseAllHoldingsPayload_ShouldMapGetAllHoldingShape()
    {
        const string json = """
            {
              "status": true,
              "message": "SUCCESS",
              "errorcode": "",
              "data": {
                "holdings": [
                  {
                    "tradingsymbol": "TATASTEEL-EQ",
                    "exchange": "NSE",
                    "isin": "INE081A01020",
                    "quantity": 2,
                    "averageprice": 111.87,
                    "ltp": 130.15,
                    "close": 129.6,
                    "symboltoken": "3499",
                    "profitandloss": 37,
                    "pnlpercentage": 16.34
                  }
                ],
                "totalholding": {
                  "totalholdingvalue": 260.3,
                  "totalinvvalue": 223.74,
                  "totalprofitandloss": 36.56,
                  "totalpnlpercentage": 16.34
                }
              }
            }
            """;

        var holdings = AngelOneHoldingDto.ParseAllHoldingsPayload(json);

        holdings.Should().HaveCount(1);
        var row = holdings[0];
        row.MatchKey.Should().Be("INE081A01020");
        row.DisplayName.Should().Be("TATASTEEL");
        row.InvestedAmount.Should().Be(223.74m);
        row.CurrentValue.Should().Be(260.30m);
        row.DayChange.Should().Be(1.10m);
        row.Category.Should().Be(InvestmentCategory.Stocks);
        row.SyncNotes.Should().StartWith("angelone|");
    }

    [Fact]
    public void ParseAllHoldingsPayload_ShouldAcceptLegacyArrayData()
    {
        const string json = """
            {
              "status": true,
              "data": [
                {
                  "tradingsymbol": "SBIN-EQ",
                  "exchange": "NSE",
                  "isin": "INE062A01020",
                  "quantity": 8,
                  "averageprice": 573.1,
                  "ltp": 579.05,
                  "close": 570.5
                }
              ]
            }
            """;

        var holdings = AngelOneHoldingDto.ParseAllHoldingsPayload(json);
        holdings.Should().HaveCount(1);
        holdings[0].Quantity.Should().Be(8m);
    }
}

public sealed class AngelOneTotpTests
{
    [Fact]
    public void Generate_ShouldReturnSixDigits()
    {
        // Well-known test vector secret "GEZDGNBVGY3TQOJQ" = "12345678901234567890" in base32 (RFC 6238 sample uses ascii key; we only assert format here).
        var code = AngelOneTotp.Generate("JBSWY3DPEHPK3PXP", DateTimeOffset.UnixEpoch.AddSeconds(30));
        code.Should().HaveLength(6);
        code.Should().MatchRegex("^[0-9]{6}$");
    }
}
