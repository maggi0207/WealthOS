/**
 * Dashboard API DTOs (camelCase, matching ASP.NET serialization)
 * and UI view models consumed by home/dashboard components.
 */

/* ---------------------------------- DTOs ---------------------------------- */

export type HealthScoreFactorDto = {
  label: string;
  value: number;
  weight: string;
};

export type HealthScoreDto = {
  score: number;
  grade: string;
  changePoints: number;
  factors: HealthScoreFactorDto[];
};

export type RecentActivityDto = {
  id: string;
  title: string;
  detail: string;
  amount: number;
  direction: string;
  category: string;
  occurredAt: string;
};

export type QuickActionDto = {
  key: string;
  label: string;
  route: string;
  icon: string;
};

export type DashboardResponseDto = {
  netWorth: number;
  assetValue: number;
  liabilityValue: number;
  monthlyIncome: number;
  monthlyExpense: number;
  investmentValue: number;
  propertyValue: number;
  loanBalance: number;
  changePercent: number;
  currencyCode: string;
  healthScore: HealthScoreDto;
  recentActivities: RecentActivityDto[];
  quickActions: QuickActionDto[];
  generatedAt: string;
};

export type NetWorthResponseDto = {
  netWorth: number;
  assetValue: number;
  liabilityValue: number;
  changePercent: number;
  currencyCode: string;
};

export type DashboardHealthResponseDto = {
  status: string;
  providersReady: boolean;
  providerStatuses: Record<string, string>;
  checkedAt: string;
};

/* ------------------------------- View models ------------------------------ */

export type HealthScoreFactorView = {
  label: string;
  value: number;
  weight: string;
};

export type HealthScoreView = {
  score: number;
  grade: string;
  changePts: number;
  factors: HealthScoreFactorView[];
};

export type ActivityCategory =
  | "Income"
  | "Investment"
  | "Expense"
  | "Loan"
  | "Property"
  | string;

export type ActivityView = {
  id: string;
  title: string;
  detail: string;
  amount: number;
  direction: "in" | "out";
  category: ActivityCategory;
  time: string;
};

export type QuickActionView = {
  key: string;
  label: string;
  route: string;
  icon: string;
};

export type KpiMetric = {
  value: number;
  changePct?: number;
};

export type DashboardKpis = {
  netWorth: KpiMetric;
  assets: KpiMetric;
  liabilities: KpiMetric;
  monthlyIncome: KpiMetric;
  monthlyExpenses: KpiMetric;
  cashFlow: KpiMetric;
};

export type NetWorthTodayView = {
  amount: number;
  changePct: number;
};

export type DashboardSummaryView = {
  kpis: DashboardKpis;
  healthScore: HealthScoreView;
  recentActivities: ActivityView[];
  quickActions: QuickActionView[];
  currencyCode: string;
  generatedAt: string;
  /** Approximate period move shown on the net-worth hero. */
  netWorthToday: NetWorthTodayView;
};

export type NetWorthView = {
  netWorth: number;
  assetValue: number;
  liabilityValue: number;
  changePercent: number;
  currencyCode: string;
  netWorthToday: NetWorthTodayView;
};

export type DashboardHealthView = {
  status: string;
  providersReady: boolean;
  providerStatuses: Record<string, string>;
  checkedAt: string;
};
