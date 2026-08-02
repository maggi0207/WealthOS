/**
 * Wealth module mock data (INR).
 * Pure frontend fixtures — no backend, no network.
 */

export const fmtINR = (value: number, opts?: Intl.NumberFormatOptions) =>
  new Intl.NumberFormat("en-IN", {
    style: "currency",
    currency: "INR",
    maximumFractionDigits: 0,
    ...opts,
  }).format(value);

/** Indian short-scale formatting: ₹1.24 Cr, ₹18.5 L, ₹42,000. */
export const fmtINRShort = (value: number) => {
  const abs = Math.abs(value);
  const sign = value < 0 ? "-" : "";
  if (abs >= 1_00_00_000) return `${sign}₹${(abs / 1_00_00_000).toFixed(2)} Cr`;
  if (abs >= 1_00_000) return `${sign}₹${(abs / 1_00_000).toFixed(2)} L`;
  return fmtINR(value);
};

export const fmtPctSigned = (value: number) => `${value > 0 ? "+" : ""}${value.toFixed(1)}%`;

/* --------------------------------- Net worth -------------------------------- */

export const wealthSummary = {
  netWorth: 2_86_40_000,
  assets: 3_54_20_000,
  liabilities: 67_80_000,
  todayChange: 1_24_500,
  todayChangePct: 0.44,
  ytdChangePct: 14.2,
};

export const netWorthSeries = [
  { label: "Sep", value: 2.31 },
  { label: "Oct", value: 2.38 },
  { label: "Nov", value: 2.42 },
  { label: "Dec", value: 2.51 },
  { label: "Jan", value: 2.58 },
  { label: "Feb", value: 2.64 },
  { label: "Mar", value: 2.69 },
  { label: "Apr", value: 2.78 },
  { label: "May", value: 2.86 },
];

/* -------------------------------- Allocation -------------------------------- */

export type AllocationSlice = {
  name: string;
  value: number;
  color: string;
};

export const allocation: AllocationSlice[] = [
  { name: "Property", value: 1_92_00_000, color: "var(--color-chart-1)" },
  { name: "Stocks", value: 84_50_000, color: "var(--color-chart-2)" },
  { name: "Gold", value: 38_20_000, color: "var(--color-chart-3)" },
  { name: "Corporate Bond", value: 24_50_000, color: "var(--color-chart-4)" },
  { name: "Cash", value: 15_00_000, color: "var(--color-chart-5)" },
];

export const allocationTotal = allocation.reduce((sum, slice) => sum + slice.value, 0);

/* ------------------------------- Asset cards -------------------------------- */

export type AssetCard = {
  id: string;
  name: string;
  category: string;
  value: number;
  invested: number;
  spark: number[];
};

export const assetCards: AssetCard[] = [
  {
    id: "adyar-flat",
    name: "Adyar Flat",
    category: "Property",
    value: 1_42_00_000,
    invested: 96_00_000,
    spark: [96, 102, 108, 116, 124, 131, 136, 142],
  },
  {
    id: "equity-portfolio",
    name: "Equity Portfolio",
    category: "Stocks",
    value: 84_50_000,
    invested: 61_20_000,
    spark: [61, 64, 68, 66, 72, 78, 81, 84.5],
  },
  {
    id: "sovereign-gold",
    name: "Sovereign Gold Bonds",
    category: "Gold",
    value: 38_20_000,
    invested: 27_40_000,
    spark: [27.4, 28.6, 30.1, 31.5, 33.4, 35.2, 36.8, 38.2],
  },
  {
    id: "corp-bond",
    name: "HDFC Corporate Bond",
    category: "Corporate Bond",
    value: 24_50_000,
    invested: 22_00_000,
    spark: [22, 22.4, 22.8, 23.1, 23.5, 23.9, 24.2, 24.5],
  },
  {
    id: "cash-liquid",
    name: "Savings & Liquid Funds",
    category: "Cash",
    value: 15_00_000,
    invested: 15_40_000,
    spark: [15.4, 15.9, 16.2, 15.8, 15.5, 15.2, 15.1, 15.0],
  },
];

/* ------------------------------ Property card ------------------------------- */

export const property = {
  name: "Adyar Flat",
  locality: "Adyar, Chennai",
  carpetArea: "970 sq ft",
  uds: "540 sq ft UDS",
  purchaseYear: 2018,
  purchasePrice: 96_00_000,
  estimatedValue: 1_42_00_000,
  loanBalance: 38_40_000,
  monthlyEmi: 42_600,
  rentalYield: 2.9,
  monthlyRent: 34_000,
};

/* -------------------------------- Loans ------------------------------------- */

export type LoanRow = {
  id: string;
  name: string;
  outstanding: number;
  principal: number;
  emi: number;
  rate: number;
  closesIn: string;
};

export const loans: LoanRow[] = [
  { id: "home", name: "Home loan · Adyar", outstanding: 38_40_000, principal: 72_00_000, emi: 42_600, rate: 8.4, closesIn: "Jun 2033" },
  { id: "car", name: "Car loan", outstanding: 6_20_000, principal: 12_00_000, emi: 18_400, rate: 9.1, closesIn: "Mar 2029" },
  { id: "personal", name: "Personal loan", outstanding: 3_20_000, principal: 8_00_000, emi: 14_200, rate: 11.5, closesIn: "Nov 2027" },
  { id: "education", name: "Education loan", outstanding: 20_00_000, principal: 24_00_000, emi: 21_800, rate: 7.6, closesIn: "Aug 2035" },
];

export const loanSummary = {
  outstanding: loans.reduce((s, l) => s + l.outstanding, 0),
  borrowed: loans.reduce((s, l) => s + l.principal, 0),
  monthlyEmi: loans.reduce((s, l) => s + l.emi, 0),
  debtFreeBy: "Aug 2035",
};

export const loanRepaidPct = Math.round(
  ((loanSummary.borrowed - loanSummary.outstanding) / loanSummary.borrowed) * 100,
);

/* ----------------------------- Investments ---------------------------------- */

export type InvestmentRow = {
  id: string;
  name: string;
  sleeve: string;
  value: number;
  xirr: number;
  weight: number;
};

export const investments: InvestmentRow[] = [
  { id: "index", name: "Nifty 50 Index Fund", sleeve: "Equity", value: 38_40_000, xirr: 16.4, weight: 26 },
  { id: "flexi", name: "Flexi Cap Fund", sleeve: "Equity", value: 26_10_000, xirr: 18.2, weight: 18 },
  { id: "direct", name: "Direct Equity", sleeve: "Equity", value: 20_00_000, xirr: 12.1, weight: 14 },
  { id: "sgb", name: "Sovereign Gold Bonds", sleeve: "Gold", value: 38_20_000, xirr: 11.8, weight: 26 },
  { id: "bond", name: "HDFC Corporate Bond", sleeve: "Debt", value: 24_50_000, xirr: 7.4, weight: 16 },
];

export const investmentSummary = {
  invested: 1_26_00_000,
  current: 1_47_20_000,
  xirr: 14.6,
  monthlySip: 1_10_000,
};
