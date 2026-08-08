/** Static mock data used by the shell. Replaced per-module later. */

export const workspace = {
  name: "WealthOS",
  tagline: "Personal wealth operating system",
  currency: "USD",
};

export const portfolioSummary = {
  netWorth: 2_486_400,
  netWorthChangePct: 3.8,
  assets: 3_142_000,
  liabilities: 655_600,
  monthlyCashflow: 9_420,
  savingsRatePct: 34,
};

export const moduleStatus: Record<string, { records: number; lastSync: string }> = {
  "/assets": { records: 42, lastSync: "2 hours ago" },
  "/properties": { records: 3, lastSync: "yesterday" },
  "/loans": { records: 4, lastSync: "yesterday" },
  "/investments": { records: 18, lastSync: "12 minutes ago" },
  "/income": { records: 7, lastSync: "today" },
  "/expenses": { records: 214, lastSync: "today" },
  "/goals": { records: 5, lastSync: "3 days ago" },
  "/documents": { records: 63, lastSync: "last week" },
  "/reports": { records: 9, lastSync: "last week" },
  "/ai-advisor": { records: 0, lastSync: "—" },
  "/settings": { records: 1, lastSync: "live" },
  "/dashboard": { records: 0, lastSync: "just now" },
};

export const notifications = [
  { id: "n1", title: "Mortgage rate review due", meta: "Loans · in 6 days" },
  { id: "n2", title: "Q3 brokerage statement ready", meta: "Documents · 2h ago" },
  { id: "n3", title: "Emergency fund goal at 82%", meta: "Goals · today" },
];

export const formatCurrency = (value: number, currency = workspace.currency) =>
  new Intl.NumberFormat("en-US", {
    style: "currency",
    currency,
    maximumFractionDigits: 0,
  }).format(value);
