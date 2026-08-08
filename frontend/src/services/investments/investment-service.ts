import { isMockApiMode } from "@/config/env";
import {
  accounts as mockAccounts,
  holdings as mockHoldings,
  investmentAllocation as mockAllocation,
  investmentAllocationTotal as mockAllocationTotal,
  portfolioReturn as mockReturn,
  portfolioReturnPct as mockReturnPct,
  portfolioSummary as mockSummary,
  transactions as mockTxns,
  type AccountStatus,
  type Holding,
  type InvestmentAccount,
  type InvestmentSlice,
  type InvestmentTxn,
} from "@/lib/investments-data";
import { BaseApiService } from "@/services/http/base-api-service";
import type {
  AddManualHoldingRequestDto,
  CreateInvestmentAccountRequestDto,
  HoldingDto,
  InvestmentAccountDto,
  InvestmentProviderListDto,
  RecordTransactionRequestDto,
  UpdateHoldingRequestDto,
  UpdateInvestmentAccountRequestDto,
} from "@/services/investments/types";

export type PortfolioView = {
  invested: number;
  current: number;
  todayChange: number;
  todayChangePct: number;
  xirr: number;
  overallReturn: number;
  overallReturnPct: number;
};

export type InvestmentsOverview = {
  portfolio: PortfolioView;
  accounts: InvestmentAccount[];
  holdings: Holding[];
  transactions: InvestmentTxn[];
  allocation: InvestmentSlice[];
  allocationTotal: number;
};

type PortfolioDto = {
  investedAmount: number;
  currentValue: number;
  todaysGain: number;
  todaysGainPercent: number;
  overallGain: number;
  absoluteReturnPercent: number;
  xirrPercent?: number | null;
};

type AccountDto = {
  id: string;
  name: string;
  ownerName?: string | null;
  providerId?: string | null;
  providerName?: string | null;
  providerKind?: number | string | null;
  kindLabel?: string | null;
  status: number | string;
  lastSyncedAt?: string | null;
  currentValue: number;
  dayChangePercent?: number;
  holdingCount?: number;
  holdingsCount?: number;
};

type AccountListDto = { items: AccountDto[] };

type HoldingDto = {
  id: string;
  accountId: string;
  name: string;
  symbol?: string | null;
  category: number | string;
  categoryName?: string | null;
  currentValue: number;
  investedAmount: number;
  dayChange?: number;
  dayChangePercent?: number;
};

type HoldingListDto = { items: HoldingDto[] };

type TxnDto = {
  id: string;
  type?: number | string;
  transactionType?: number | string;
  title?: string | null;
  description?: string | null;
  notes?: string | null;
  holdingName?: string | null;
  amount: number;
  tradeDate?: string;
  transactionDate?: string;
  accountName?: string | null;
};

type TxnListDto = { items: TxnDto[] };

type AllocationDto = {
  totalValue: number;
  slices: Array<{ categoryName: string; value: number; weightPercent: number }>;
};

const COLORS = [
  "var(--color-chart-1)",
  "var(--color-chart-2)",
  "var(--color-chart-3)",
  "var(--color-chart-4)",
  "var(--color-chart-5)",
];

function n(v: unknown, f = 0) {
  const x = typeof v === "number" ? v : Number(v);
  return Number.isFinite(x) ? x : f;
}

function mapStatus(s: number | string): AccountStatus {
  const k = String(s);
  if (k === "0" || k === "Manual") return "manual";
  if (k === "1" || k === "Connected") return "connected";
  if (k === "2" || k === "ComingSoon") return "soon";
  if (k === "3" || k === "Disconnected") return "disconnected";
  return "manual";
}

function mapHoldingCategory(c: number | string, name?: string | null): Holding["category"] {
  const raw = (name || String(c)).toLowerCase();
  if (raw.includes("mutual") || raw === "1") return "Mutual Funds";
  if (raw.includes("bond") || raw === "2") return "Corporate Bonds";
  if (raw.includes("gold") || raw === "3") return "Gold ETFs";
  if (raw.includes("cash") || raw.includes("fd") || raw === "4") return "Cash";
  return "Stocks";
}

function mapTxnKind(t: number | string): InvestmentTxn["kind"] {
  const k = String(t).toLowerCase();
  if (k.includes("sell") || k === "1") return "sell";
  if (k.includes("dividend") || k === "2") return "dividend";
  if (k.includes("sip") || k === "3") return "sip";
  if (k.includes("interest") || k === "4") return "interest";
  return "buy";
}

function relativeSync(iso?: string | null): string {
  if (!iso) return "Not synced";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return "Not synced";
  const mins = Math.round((Date.now() - d.getTime()) / 60_000);
  if (mins < 60) return `Synced ${Math.max(mins, 1)} min ago`;
  if (mins < 1440) return `Synced ${Math.round(mins / 60)} hr ago`;
  return `Synced ${Math.round(mins / 1440)} days ago`;
}

function formatTxnDate(iso?: string | null): string {
  if (!iso) return "—";
  const d = new Date(iso.length <= 10 ? `${iso}T00:00:00` : iso);
  if (Number.isNaN(d.getTime())) return iso;
  return d.toLocaleDateString("en-IN", { day: "numeric", month: "short", year: "numeric" });
}

function mapMock(): InvestmentsOverview {
  return {
    portfolio: {
      invested: mockSummary.invested,
      current: mockSummary.current,
      todayChange: mockSummary.todayChange,
      todayChangePct: mockSummary.todayChangePct,
      xirr: mockSummary.xirr,
      overallReturn: mockReturn,
      overallReturnPct: mockReturnPct,
    },
    accounts: mockAccounts.map((a) => ({ ...a })),
    holdings: mockHoldings.map((h) => ({ ...h })),
    transactions: mockTxns.map((t) => ({ ...t })),
    allocation: mockAllocation.map((s) => ({ ...s })),
    allocationTotal: mockAllocationTotal,
  };
}

class InvestmentService extends BaseApiService {
  protected readonly serviceName = "InvestmentService";

  async getOverview(): Promise<InvestmentsOverview> {
    if (isMockApiMode()) return mapMock();

    const settled = await Promise.allSettled([
      this.get<PortfolioDto>("/investments/portfolio"),
      this.get<AccountListDto>("/investments/accounts?pageSize=50"),
      this.get<HoldingListDto>("/investments/holdings?pageSize=100"),
      this.get<TxnListDto>("/investments/transactions?pageSize=50"),
      this.get<AllocationDto>("/investments/allocation"),
    ]);

    const portfolio = settled[0].status === "fulfilled" ? settled[0].value : null;
    const accounts = settled[1].status === "fulfilled" ? settled[1].value : null;
    const holdings = settled[2].status === "fulfilled" ? settled[2].value : null;
    const txns = settled[3].status === "fulfilled" ? settled[3].value : null;
    const allocation = settled[4].status === "fulfilled" ? settled[4].value : null;

    const failed = settled.filter((r) => r.status === "rejected");
    if (failed.length === settled.length) {
      const reason = failed[0]?.status === "rejected" ? failed[0].reason : null;
      throw reason instanceof Error ? reason : new Error("Unable to load investments");
    }

    return {
      portfolio: {
        invested: n(portfolio?.investedAmount),
        current: n(portfolio?.currentValue),
        todayChange: n(portfolio?.todaysGain),
        todayChangePct: n(portfolio?.todaysGainPercent),
        xirr: n(portfolio?.xirrPercent, 0),
        overallReturn: n(portfolio?.overallGain),
        overallReturnPct: n(portfolio?.absoluteReturnPercent),
      },
      accounts: (accounts?.items ?? []).map((a) => ({
        id: String(a.id),
        name: a.name,
        owner: a.ownerName || "—",
        kind: a.kindLabel || a.providerName || "Investment",
        providerName: a.providerName || undefined,
        providerKind: a.providerKind ?? undefined,
        providerId: a.providerId ? String(a.providerId) : undefined,
        status: mapStatus(a.status),
        lastSync: relativeSync(a.lastSyncedAt),
        lastSyncedAt: a.lastSyncedAt ?? null,
        value: n(a.currentValue),
        dayChangePct: n(a.dayChangePercent),
        holdings: n(a.holdingsCount ?? a.holdingCount),
      })),
      holdings: (holdings?.items ?? []).map((h) => ({
        id: String(h.id),
        name: h.name,
        ticker: h.symbol || "",
        category: mapHoldingCategory(h.category, h.categoryName),
        accountId: String(h.accountId),
        value: n(h.currentValue),
        invested: n(h.investedAmount),
        dayChange: n(h.dayChange),
        dayChangePct: n(h.dayChangePercent),
      })),
      transactions: (txns?.items ?? []).map((t) => ({
        id: String(t.id),
        kind: mapTxnKind(t.transactionType ?? t.type ?? 0),
        title: t.title || t.holdingName || t.notes || t.description || "Transaction",
        account: t.accountName || "—",
        date: formatTxnDate(t.transactionDate ?? t.tradeDate),
        amount: n(t.amount),
      })),
      allocation: (allocation?.slices ?? []).map((s, i) => ({
        name: s.categoryName,
        value: n(s.value),
        color: COLORS[i % COLORS.length]!,
      })),
      allocationTotal: n(allocation?.totalValue),
    };
  }

  async getProviders(): Promise<InvestmentProviderListDto> {
    if (isMockApiMode()) {
      return {
        items: [
          {
            id: "a1111111-1111-2222-3333-444444444401",
            kind: 0,
            name: "Manual",
            isEnabled: true,
            supportsSync: false,
          },
          {
            id: "a1111111-1111-2222-3333-444444444402",
            kind: 1,
            name: "Angel One",
            isEnabled: true,
            supportsSync: true,
          },
        ],
      };
    }
    return this.get<InvestmentProviderListDto>("/investments/providers");
  }

  async getPerformance(range: string): Promise<{
    points: Array<{ label: string; value: number }>;
    absoluteReturnPercent: number;
  }> {
    if (isMockApiMode()) {
      return { points: [], absoluteReturnPercent: 0 };
    }
    const dto = await this.get<{
      points?: Array<{ label: string; value: number }>;
      absoluteReturnPercent?: number;
    }>(`/investments/performance?range=${encodeURIComponent(range)}`);
    return {
      points: (dto.points ?? []).map((p) => ({
        label: p.label,
        value: n(p.value),
      })),
      absoluteReturnPercent: n(dto.absoluteReturnPercent),
    };
  }

  async createAccount(body: CreateInvestmentAccountRequestDto): Promise<InvestmentAccountDto> {
    if (isMockApiMode()) {
      return {
        id: crypto.randomUUID(),
        providerId: body.providerId,
        name: body.name,
        ownerName: body.ownerName,
        kindLabel: body.kindLabel,
        status: body.status ?? 0,
      };
    }
    return this.post<InvestmentAccountDto>("/investments/accounts", body);
  }

  async updateAccount(
    id: string,
    body: UpdateInvestmentAccountRequestDto,
  ): Promise<InvestmentAccountDto> {
    if (isMockApiMode()) {
      return { id, providerId: "", ...body, status: body.status ?? 0 };
    }
    return this.put<InvestmentAccountDto>(`/investments/accounts/${id}`, body);
  }

  async deleteAccount(id: string): Promise<void> {
    if (isMockApiMode()) return;
    await this.delete<unknown>(`/investments/accounts/${id}`);
  }

  async addManualHolding(body: AddManualHoldingRequestDto): Promise<HoldingDto> {
    if (isMockApiMode()) {
      return {
        id: crypto.randomUUID(),
        accountId: body.accountId,
        name: body.name,
        symbol: body.symbol,
        category: body.category,
        investmentType: body.investmentType,
        quantity: body.quantity,
        averageCost: body.averageCost,
        investedAmount: body.investedAmount,
        currentPrice: body.currentPrice,
        currentValue: body.currentValue,
      };
    }
    return this.post<HoldingDto>("/investments/manual-holding", body);
  }

  async updateHolding(id: string, body: UpdateHoldingRequestDto): Promise<HoldingDto> {
    if (isMockApiMode()) {
      return {
        id,
        accountId: "",
        name: body.name,
        symbol: body.symbol,
        category: body.category,
        investmentType: body.investmentType,
        quantity: body.quantity,
        averageCost: body.averageCost,
        investedAmount: body.investedAmount,
        currentPrice: body.currentPrice,
        currentValue: body.currentValue,
      };
    }
    return this.put<HoldingDto>(`/investments/holdings/${id}`, body);
  }

  async deleteHolding(id: string): Promise<void> {
    if (isMockApiMode()) return;
    await this.delete<unknown>(`/investments/holdings/${id}`);
  }

  async recordTransaction(body: RecordTransactionRequestDto): Promise<unknown> {
    if (isMockApiMode()) return {};
    return this.post<unknown>("/investments/transactions", body);
  }

  async connectProvider(accountId: string): Promise<void> {
    if (isMockApiMode()) return;
    await this.post<unknown>(`/investments/providers/${accountId}/connect`, {});
  }

  async syncProvider(
    accountId: string,
    target: "portfolio" | "holdings" | "transactions" = "holdings",
  ): Promise<void> {
    if (isMockApiMode()) return;
    await this.post<unknown>(`/investments/providers/${accountId}/sync?target=${target}`, {});
  }

  async disconnectProvider(accountId: string): Promise<void> {
    if (isMockApiMode()) return;
    await this.post<unknown>(`/investments/providers/${accountId}/disconnect`, {});
  }
}

export const investmentService = new InvestmentService();
