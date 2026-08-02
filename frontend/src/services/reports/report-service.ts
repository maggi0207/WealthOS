import { isMockApiMode } from "@/config/env";
import { BaseApiService } from "@/services/http/base-api-service";

export type ReportSummaryView = {
  title: string;
  generatedAt: string;
  netWorth: number;
  cashFlow: number;
  investmentValue: number;
  loanBalance: number;
  currencyCode: string;
  sections: Array<{ key: string; label: string; value: number }>;
};

type SummaryDto = {
  title?: string;
  generatedAt?: string;
  currencyCode?: string;
  netWorth?: number;
  cashFlow?: number;
  investmentValue?: number;
  loanBalance?: number;
  totalAssets?: number;
  totalLiabilities?: number;
};

type NetWorthDto = {
  title?: string;
  generatedAt?: string;
  currencyCode?: string;
  netWorth?: number;
  totalAssets?: number;
  totalLiabilities?: number;
};

type HealthDto = {
  title?: string;
  generatedAt?: string;
  score?: number;
  grade?: string;
};

class ReportService extends BaseApiService {
  protected readonly serviceName = "ReportService";

  async getSummary(): Promise<ReportSummaryView> {
    if (isMockApiMode()) {
      return {
        title: "Wealth summary",
        generatedAt: new Date().toISOString(),
        netWorth: 2_48_64_000,
        cashFlow: 3_70_000,
        investmentValue: 1_47_20_000,
        loanBalance: 45_47_000,
        currencyCode: "INR",
        sections: [
          { key: "networth", label: "Net worth", value: 2_48_64_000 },
          { key: "investments", label: "Investments", value: 1_47_20_000 },
          { key: "loans", label: "Loans", value: 45_47_000 },
          { key: "cashflow", label: "Monthly cash flow", value: 3_70_000 },
        ],
      };
    }

    const [summary, netWorth, health] = await Promise.all([
      this.get<SummaryDto>("/reports/summary").catch(() => ({}) as SummaryDto),
      this.get<NetWorthDto>("/reports/networth").catch(() => ({}) as NetWorthDto),
      this.get<HealthDto>("/reports/financial-health").catch(() => ({}) as HealthDto),
    ]);

    void health;

    const nw = Number(summary.netWorth ?? netWorth.netWorth ?? 0);
    return {
      title: summary.title || netWorth.title || "Wealth summary",
      generatedAt:
        summary.generatedAt || netWorth.generatedAt || new Date().toISOString(),
      netWorth: nw,
      cashFlow: Number(summary.cashFlow ?? 0),
      investmentValue: Number(summary.investmentValue ?? 0),
      loanBalance: Number(summary.loanBalance ?? 0),
      currencyCode: summary.currencyCode || netWorth.currencyCode || "INR",
      sections: [
        { key: "networth", label: "Net worth", value: nw },
        {
          key: "assets",
          label: "Assets",
          value: Number(summary.totalAssets ?? netWorth.totalAssets ?? 0),
        },
        {
          key: "liabilities",
          label: "Liabilities",
          value: Number(
            summary.totalLiabilities ?? netWorth.totalLiabilities ?? 0,
          ),
        },
        {
          key: "investments",
          label: "Investments",
          value: Number(summary.investmentValue ?? 0),
        },
      ],
    };
  }

  async requestExport(format: "pdf" | "csv" = "pdf"): Promise<{ id: string }> {
    if (isMockApiMode()) return { id: "mock-export" };
    return this.post<{ id: string }>("/reports/exports", { format });
  }
}

export const reportService = new ReportService();
