import {
  healthScore as mockHealthScore,
  kpis as mockKpis,
  netWorthToday as mockNetWorthToday,
  recentActivity as mockRecentActivity,
} from "@/lib/dashboard-data";
import type {
  ActivityView,
  DashboardHealthResponseDto,
  DashboardHealthView,
  DashboardKpis,
  DashboardResponseDto,
  DashboardSummaryView,
  HealthScoreDto,
  HealthScoreView,
  NetWorthResponseDto,
  NetWorthTodayView,
  NetWorthView,
  QuickActionDto,
  QuickActionView,
  RecentActivityDto,
} from "@/services/dashboard/types";

const ACTIVITY_CATEGORIES = new Set([
  "Income",
  "Investment",
  "Expense",
  "Loan",
  "Property",
]);

function toNumber(value: unknown, fallback = 0): number {
  const n = typeof value === "number" ? value : Number(value);
  return Number.isFinite(n) ? n : fallback;
}

function formatRelativeActivityTime(occurredAt: string): string {
  const date = new Date(occurredAt);
  if (Number.isNaN(date.getTime())) return occurredAt;

  const now = new Date();
  const startOfToday = new Date(now.getFullYear(), now.getMonth(), now.getDate());
  const startOfOccurred = new Date(date.getFullYear(), date.getMonth(), date.getDate());
  const dayDiff = Math.round(
    (startOfToday.getTime() - startOfOccurred.getTime()) / 86_400_000,
  );

  if (dayDiff === 0) {
    return `Today, ${date.toLocaleTimeString("en-US", {
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    })}`;
  }
  if (dayDiff === 1) return "Yesterday";
  if (dayDiff > 1 && dayDiff < 7) return `${dayDiff} days ago`;

  return date.toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
  });
}

function mapHealthScore(dto: HealthScoreDto): HealthScoreView {
  return {
    score: toNumber(dto.score),
    grade: dto.grade || "—",
    changePts: toNumber(dto.changePoints),
    factors: (dto.factors ?? []).map((factor) => ({
      label: factor.label,
      value: toNumber(factor.value),
      weight: factor.weight,
    })),
  };
}

function mapActivity(dto: RecentActivityDto): ActivityView {
  const direction = dto.direction === "out" ? "out" : "in";
  const category = ACTIVITY_CATEGORIES.has(dto.category)
    ? dto.category
    : dto.category || "Expense";

  return {
    id: String(dto.id),
    title: dto.title,
    detail: dto.detail,
    amount: toNumber(dto.amount),
    direction,
    category,
    time: formatRelativeActivityTime(dto.occurredAt),
  };
}

function mapQuickAction(dto: QuickActionDto): QuickActionView {
  return {
    key: dto.key,
    label: dto.label,
    route: dto.route,
    icon: dto.icon,
  };
}

function buildNetWorthToday(
  netWorth: number,
  changePercent: number,
): NetWorthTodayView {
  const changePct = toNumber(changePercent);
  const amount = Math.round(toNumber(netWorth) * (changePct / 100));
  return { amount, changePct };
}

function buildKpisFromDashboard(dto: DashboardResponseDto): DashboardKpis {
  const income = toNumber(dto.monthlyIncome);
  const expenses = toNumber(dto.monthlyExpense);
  return {
    netWorth: {
      value: toNumber(dto.netWorth),
      changePct: toNumber(dto.changePercent),
    },
    assets: { value: toNumber(dto.assetValue) },
    liabilities: { value: toNumber(dto.liabilityValue) },
    monthlyIncome: { value: income },
    monthlyExpenses: { value: expenses },
    cashFlow: { value: income - expenses },
  };
}

/** Map GET /dashboard payload → UI summary. */
export function mapDashboardResponse(
  dto: DashboardResponseDto,
): DashboardSummaryView {
  const netWorth = toNumber(dto.netWorth);
  const changePercent = toNumber(dto.changePercent);

  return {
    kpis: buildKpisFromDashboard(dto),
    healthScore: mapHealthScore(dto.healthScore ?? { score: 0, grade: "", changePoints: 0, factors: [] }),
    recentActivities: (dto.recentActivities ?? []).map(mapActivity),
    quickActions: (dto.quickActions ?? []).map(mapQuickAction),
    currencyCode: dto.currencyCode || "USD",
    generatedAt: dto.generatedAt,
    netWorthToday: buildNetWorthToday(netWorth, changePercent),
  };
}

/** Map GET /dashboard/net-worth payload → UI net-worth slice. */
export function mapNetWorthResponse(dto: NetWorthResponseDto): NetWorthView {
  const netWorth = toNumber(dto.netWorth);
  const changePercent = toNumber(dto.changePercent);

  return {
    netWorth,
    assetValue: toNumber(dto.assetValue),
    liabilityValue: toNumber(dto.liabilityValue),
    changePercent,
    currencyCode: dto.currencyCode || "USD",
    netWorthToday: buildNetWorthToday(netWorth, changePercent),
  };
}

/** Map GET /dashboard/activities payload → UI activity list. */
export function mapActivitiesResponse(
  dtos: RecentActivityDto[] | null | undefined,
): ActivityView[] {
  return (dtos ?? []).map(mapActivity);
}

/** Map GET /dashboard/health payload → module readiness view. */
export function mapDashboardHealthResponse(
  dto: DashboardHealthResponseDto,
): DashboardHealthView {
  return {
    status: dto.status,
    providersReady: Boolean(dto.providersReady),
    providerStatuses: dto.providerStatuses ?? {},
    checkedAt: dto.checkedAt,
  };
}

/** Build summary view from local fixtures (VITE_API_MODE=mock). */
export function mapMockDashboardSummary(): DashboardSummaryView {
  return {
    kpis: {
      netWorth: {
        value: mockKpis.netWorth.value,
        changePct: mockKpis.netWorth.changePct,
      },
      assets: {
        value: mockKpis.assets.value,
        changePct: mockKpis.assets.changePct,
      },
      liabilities: {
        value: mockKpis.liabilities.value,
        changePct: mockKpis.liabilities.changePct,
      },
      monthlyIncome: {
        value: mockKpis.monthlyIncome.value,
        changePct: mockKpis.monthlyIncome.changePct,
      },
      monthlyExpenses: {
        value: mockKpis.monthlyExpenses.value,
        changePct: mockKpis.monthlyExpenses.changePct,
      },
      cashFlow: {
        value: mockKpis.cashFlow.value,
        changePct: mockKpis.cashFlow.changePct,
      },
    },
    healthScore: {
      score: mockHealthScore.score,
      grade: mockHealthScore.grade,
      changePts: mockHealthScore.changePts,
      factors: mockHealthScore.factors.map((f) => ({ ...f })),
    },
    recentActivities: mockRecentActivity.map((item) => ({ ...item })),
    quickActions: [],
    currencyCode: "USD",
    generatedAt: new Date().toISOString(),
    netWorthToday: { ...mockNetWorthToday },
  };
}

export function mapMockNetWorth(): NetWorthView {
  const summary = mapMockDashboardSummary();
  return {
    netWorth: summary.kpis.netWorth.value,
    assetValue: summary.kpis.assets.value,
    liabilityValue: summary.kpis.liabilities.value,
    changePercent: summary.kpis.netWorth.changePct ?? 0,
    currencyCode: summary.currencyCode,
    netWorthToday: summary.netWorthToday,
  };
}

export function mapMockActivities(limit = 10): ActivityView[] {
  return mapMockDashboardSummary().recentActivities.slice(0, limit);
}

export function mapMockHealthScore(): HealthScoreView {
  return mapMockDashboardSummary().healthScore;
}

export function mapMockDashboardHealth(): DashboardHealthView {
  return {
    status: "Healthy",
    providersReady: true,
    providerStatuses: {
      property: "Healthy",
      loan: "Healthy",
      investment: "Healthy",
      income: "Healthy",
      document: "Healthy",
    },
    checkedAt: new Date().toISOString(),
  };
}
