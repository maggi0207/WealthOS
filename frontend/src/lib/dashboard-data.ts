/**
 * Dashboard module mock data.
 * Pure frontend fixtures — no backend, no network.
 */

export type Trend = { label: string; value: number };

export const currency = "USD";

export const fmtCurrency = (value: number, opts?: Intl.NumberFormatOptions) =>
  new Intl.NumberFormat("en-US", {
    style: "currency",
    currency,
    maximumFractionDigits: 0,
    ...opts,
  }).format(value);

export const fmtCompact = (value: number) =>
  new Intl.NumberFormat("en-US", {
    style: "currency",
    currency,
    notation: "compact",
    maximumFractionDigits: 1,
  }).format(value);

export const fmtPct = (value: number) => `${value > 0 ? "+" : ""}${value.toFixed(1)}%`;

/* ------------------------------- KPI figures ------------------------------ */

export const kpis = {
  netWorth: { value: 2_486_400, changePct: 3.8, sparkline: [2.21, 2.24, 2.3, 2.28, 2.35, 2.41, 2.39, 2.44, 2.49] },
  assets: { value: 3_142_000, changePct: 2.6, sparkline: [2.86, 2.9, 2.94, 2.97, 3.0, 3.05, 3.08, 3.11, 3.14] },
  liabilities: { value: 655_600, changePct: -1.4, sparkline: [0.71, 0.7, 0.69, 0.68, 0.68, 0.67, 0.66, 0.66, 0.65] },
  monthlyIncome: { value: 24_800, changePct: 4.2, sparkline: [22.1, 22.6, 23.0, 23.4, 23.1, 24.0, 24.2, 24.5, 24.8] },
  monthlyExpenses: { value: 15_380, changePct: 1.9, sparkline: [14.2, 14.9, 15.6, 15.1, 14.8, 15.4, 15.0, 15.2, 15.38] },
  cashFlow: { value: 9_420, changePct: 8.7, sparkline: [7.9, 7.7, 7.4, 8.3, 8.3, 8.6, 9.2, 9.3, 9.42] },
};

/** Today's net-worth movement, surfaced on the home hero. */
export const netWorthToday = { amount: 12_480, changePct: 0.5 };

export const healthScore = {
  score: 78,
  grade: "Strong",
  changePts: 4,
  factors: [
    { label: "Savings rate", value: 86, weight: "High" },
    { label: "Debt-to-income", value: 72, weight: "High" },
    { label: "Emergency buffer", value: 64, weight: "Medium" },
    { label: "Diversification", value: 81, weight: "Medium" },
  ],
};

/* --------------------------------- Charts -------------------------------- */

export const netWorthTrend = [
  { month: "Jan", netWorth: 2_212_000, assets: 2_862_000, liabilities: 650_000 },
  { month: "Feb", netWorth: 2_240_000, assets: 2_901_000, liabilities: 661_000 },
  { month: "Mar", netWorth: 2_298_000, assets: 2_941_000, liabilities: 643_000 },
  { month: "Apr", netWorth: 2_281_000, assets: 2_965_000, liabilities: 684_000 },
  { month: "May", netWorth: 2_352_000, assets: 3_001_000, liabilities: 649_000 },
  { month: "Jun", netWorth: 2_408_000, assets: 3_048_000, liabilities: 640_000 },
  { month: "Jul", netWorth: 2_389_000, assets: 3_060_000, liabilities: 671_000 },
  { month: "Aug", netWorth: 2_441_000, assets: 3_105_000, liabilities: 664_000 },
  { month: "Sep", netWorth: 2_486_400, assets: 3_142_000, liabilities: 655_600 },
];

export const assetAllocation = [
  { name: "Equities", value: 1_010_000, key: "equities" },
  { name: "Real estate", value: 1_068_000, key: "realEstate" },
  { name: "Fixed income", value: 435_000, key: "fixedIncome" },
  { name: "Cash", value: 377_000, key: "cash" },
  { name: "Alternatives", value: 252_000, key: "alternatives" },
];

export const incomeVsExpenses = [
  { month: "Apr", income: 23_400, expenses: 15_100 },
  { month: "May", income: 23_100, expenses: 14_800 },
  { month: "Jun", income: 24_000, expenses: 15_400 },
  { month: "Jul", income: 24_200, expenses: 15_000 },
  { month: "Aug", income: 24_500, expenses: 15_200 },
  { month: "Sep", income: 24_800, expenses: 15_380 },
];

export const loanBreakdown = [
  { name: "Home mortgage", outstanding: 412_000, ratePct: 4.1, emi: 2_410 },
  { name: "Rental property", outstanding: 168_000, ratePct: 5.2, emi: 1_180 },
  { name: "Car loan", outstanding: 41_600, ratePct: 6.4, emi: 620 },
  { name: "Education loan", outstanding: 34_000, ratePct: 3.5, emi: 340 },
];

/* --------------------------------- Panels -------------------------------- */

export type Activity = {
  id: string;
  title: string;
  detail: string;
  amount: number;
  direction: "in" | "out";
  category: "Income" | "Investment" | "Expense" | "Loan" | "Property";
  time: string;
};

export const recentActivity: Activity[] = [
  { id: "a1", title: "Salary credited", detail: "Meridian Capital · Payroll", amount: 18_400, direction: "in", category: "Income", time: "Today, 09:12" },
  { id: "a2", title: "SIP executed", detail: "Global Index Fund · monthly", amount: 2_000, direction: "out", category: "Investment", time: "Today, 06:30" },
  { id: "a3", title: "Mortgage EMI", detail: "Home mortgage · auto-debit", amount: 2_410, direction: "out", category: "Loan", time: "Yesterday" },
  { id: "a4", title: "Rent received", detail: "Harbour View apartment", amount: 3_150, direction: "in", category: "Property", time: "Yesterday" },
  { id: "a5", title: "Dividend payout", detail: "Blue-chip equity basket", amount: 940, direction: "in", category: "Investment", time: "2 days ago" },
  { id: "a6", title: "Card settlement", detail: "Travel & dining", amount: 1_265, direction: "out", category: "Expense", time: "3 days ago" },
];

export type Task = {
  id: string;
  title: string;
  detail: string;
  amount?: number;
  due: string;
  dueInDays: number;
  type: "EMI" | "SIP" | "Bond" | "Insurance";
};

export const upcomingTasks: Task[] = [
  { id: "t1", title: "Home mortgage EMI", detail: "Auto-debit · Meridian Bank", amount: 2_410, due: "Oct 3", dueInDays: 2, type: "EMI" },
  { id: "t2", title: "Global Index SIP", detail: "Monthly systematic plan", amount: 2_000, due: "Oct 5", dueInDays: 4, type: "SIP" },
  { id: "t3", title: "Treasury bond maturity", detail: "5-yr sovereign · 6.8% coupon", amount: 50_000, due: "Oct 12", dueInDays: 11, type: "Bond" },
  { id: "t4", title: "Car loan EMI", detail: "Auto-debit · Northline Auto", amount: 620, due: "Oct 15", dueInDays: 14, type: "EMI" },
  { id: "t5", title: "Term insurance premium", detail: "Annual renewal", amount: 1_480, due: "Oct 22", dueInDays: 21, type: "Insurance" },
];

export type Insight = {
  id: string;
  title: string;
  body: string;
  impact: string;
  tone: "opportunity" | "risk" | "optimisation";
};

export const aiInsights: Insight[] = [
  {
    id: "i1",
    title: "Refinance the rental mortgage",
    body: "Your 5.2% rental loan is 1.1pt above current market. Refinancing could free up cashflow immediately.",
    impact: "+$1,850 / yr",
    tone: "opportunity",
  },
  {
    id: "i2",
    title: "Cash drag on the portfolio",
    body: "12% sits in cash versus your 7% target. Deploying the excess into fixed income lifts blended yield.",
    impact: "+$6,200 / yr",
    tone: "optimisation",
  },
  {
    id: "i3",
    title: "Emergency buffer below target",
    body: "Liquid reserves cover 3.4 months of expenses. Six months is the recommended floor for your profile.",
    impact: "Risk: medium",
    tone: "risk",
  },
];
