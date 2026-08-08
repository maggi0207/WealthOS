/**
 * Investments module mock data (INR).
 * Pure frontend fixtures — no backend, no broker API. Connection states are
 * simulated: "connected" means "Connected (Mock)", "soon" means "Coming Soon".
 */

import { fmtINR, fmtINRShort, fmtPctSigned } from "@/lib/wealth-data";

export { fmtINR, fmtINRShort, fmtPctSigned };

/* ------------------------------- Portfolio ---------------------------------- */

export const portfolioSummary = {
  invested: 1_26_00_000,
  current: 1_47_20_000,
  todayChange: 62_400,
  todayChangePct: 0.42,
  xirr: 14.6,
};

export const portfolioReturn = portfolioSummary.current - portfolioSummary.invested;
export const portfolioReturnPct = (portfolioReturn / portfolioSummary.invested) * 100;

/* ---------------------------- Investment accounts --------------------------- */

export type AccountStatus = "connected" | "manual" | "soon" | "disconnected";

export type InvestmentAccount = {
  id: string;
  name: string;
  owner: string;
  kind: string;
  providerName?: string;
  providerKind?: number | string;
  providerId?: string;
  status: AccountStatus;
  lastSync: string;
  lastSyncedAt?: string | null;
  value: number;
  dayChangePct: number;
  holdings: number;
};

export const accounts: InvestmentAccount[] = [
  {
    id: "angel-magesh",
    name: "Angel One",
    owner: "Magesh",
    kind: "Broker · Stocks & MF",
    status: "connected",
    lastSync: "Synced 12 min ago",
    value: 58_40_000,
    dayChangePct: 0.61,
    holdings: 14,
  },
  {
    id: "angel-wife",
    name: "Angel One",
    owner: "Wife",
    kind: "Broker · Stocks & MF",
    status: "connected",
    lastSync: "Synced 38 min ago",
    value: 26_10_000,
    dayChangePct: -0.24,
    holdings: 9,
  },
  {
    id: "indiabonds",
    name: "IndiaBonds",
    owner: "Magesh",
    kind: "Corporate bonds",
    status: "soon",
    lastSync: "Integration coming soon",
    value: 24_50_000,
    dayChangePct: 0.05,
    holdings: 4,
  },
  {
    id: "manual",
    name: "Manual Investments",
    owner: "Household",
    kind: "SGB, FD & unlisted",
    status: "manual",
    lastSync: "Updated 2 days ago",
    value: 38_20_000,
    dayChangePct: 0.18,
    holdings: 6,
  },
];

export const accountsTotal = accounts.reduce((sum, a) => sum + a.value, 0);

export const statusLabel: Record<AccountStatus, string> = {
  connected: "Connected",
  manual: "Manual",
  soon: "Coming Soon",
  disconnected: "Disconnected",
};

/* -------------------------------- Allocation -------------------------------- */

export type InvestmentSlice = { name: string; value: number; color: string };

export const investmentAllocation: InvestmentSlice[] = [
  { name: "Stocks", value: 52_30_000, color: "var(--color-chart-1)" },
  { name: "Mutual Funds", value: 41_60_000, color: "var(--color-chart-2)" },
  { name: "Corporate Bonds", value: 24_50_000, color: "var(--color-chart-3)" },
  { name: "Gold ETFs", value: 21_80_000, color: "var(--color-chart-4)" },
  { name: "Cash", value: 7_00_000, color: "var(--color-chart-5)" },
];

export const investmentAllocationTotal = investmentAllocation.reduce((s, x) => s + x.value, 0);

/* --------------------------------- Holdings --------------------------------- */

export type Holding = {
  id: string;
  name: string;
  ticker: string;
  category: "Stocks" | "Mutual Funds" | "Corporate Bonds" | "Gold ETFs" | "Cash";
  accountId: string;
  value: number;
  invested: number;
  dayChange: number;
  dayChangePct: number;
};

export const holdings: Holding[] = [
  { id: "h1", name: "HDFC Bank", ticker: "HDFCBANK", category: "Stocks", accountId: "angel-magesh", value: 12_40_000, invested: 9_10_000, dayChange: 9_800, dayChangePct: 0.79 },
  { id: "h2", name: "Infosys", ticker: "INFY", category: "Stocks", accountId: "angel-magesh", value: 9_80_000, invested: 7_60_000, dayChange: -6_200, dayChangePct: -0.63 },
  { id: "h3", name: "Tata Motors", ticker: "TATAMOTORS", category: "Stocks", accountId: "angel-wife", value: 7_20_000, invested: 5_40_000, dayChange: 14_100, dayChangePct: 1.99 },
  { id: "h4", name: "ITC", ticker: "ITC", category: "Stocks", accountId: "angel-wife", value: 6_10_000, invested: 5_20_000, dayChange: 2_300, dayChangePct: 0.38 },
  { id: "h5", name: "Reliance Industries", ticker: "RELIANCE", category: "Stocks", accountId: "angel-magesh", value: 16_80_000, invested: 12_90_000, dayChange: 21_400, dayChangePct: 1.29 },
  { id: "h6", name: "Nifty 50 Index Fund", ticker: "UTINIFTY", category: "Mutual Funds", accountId: "angel-magesh", value: 18_40_000, invested: 13_10_000, dayChange: 11_600, dayChangePct: 0.63 },
  { id: "h7", name: "Parag Parikh Flexi Cap", ticker: "PPFAS", category: "Mutual Funds", accountId: "angel-magesh", value: 14_10_000, invested: 9_80_000, dayChange: 7_900, dayChangePct: 0.56 },
  { id: "h8", name: "Mirae Emerging Bluechip", ticker: "MIRAE", category: "Mutual Funds", accountId: "angel-wife", value: 9_10_000, invested: 7_40_000, dayChange: -3_100, dayChangePct: -0.34 },
  { id: "h9", name: "HDFC Corporate Bond 8.4%", ticker: "HDFCCB28", category: "Corporate Bonds", accountId: "indiabonds", value: 14_50_000, invested: 13_20_000, dayChange: 600, dayChangePct: 0.04 },
  { id: "h10", name: "Muthoot Finance NCD 9.1%", ticker: "MUTHNCD", category: "Corporate Bonds", accountId: "indiabonds", value: 10_00_000, invested: 9_40_000, dayChange: 500, dayChangePct: 0.05 },
  { id: "h11", name: "Nippon Gold ETF", ticker: "GOLDBEES", category: "Gold ETFs", accountId: "manual", value: 13_20_000, invested: 9_60_000, dayChange: 8_400, dayChangePct: 0.64 },
  { id: "h12", name: "Sovereign Gold Bond 2031", ticker: "SGB31", category: "Gold ETFs", accountId: "manual", value: 8_60_000, invested: 6_40_000, dayChange: 3_700, dayChangePct: 0.43 },
  { id: "h13", name: "Liquid Fund — Idle Cash", ticker: "LIQUID", category: "Cash", accountId: "manual", value: 7_00_000, invested: 6_90_000, dayChange: 300, dayChangePct: 0.04 },
];

export const holdingCategories = [
  "All",
  "Stocks",
  "Mutual Funds",
  "Corporate Bonds",
  "Gold ETFs",
  "Cash",
] as const;

export type HoldingCategory = (typeof holdingCategories)[number];

/* ------------------------------- Performance -------------------------------- */

export type PerfRange = "1M" | "6M" | "1Y" | "All";

export const performanceSeries: Record<PerfRange, { label: string; value: number }[]> = {
  "1M": [
    { label: "W1", value: 143.2 },
    { label: "W2", value: 144.1 },
    { label: "W3", value: 145.8 },
    { label: "W4", value: 146.4 },
    { label: "Now", value: 147.2 },
  ],
  "6M": [
    { label: "Dec", value: 128.4 },
    { label: "Jan", value: 131.9 },
    { label: "Feb", value: 134.2 },
    { label: "Mar", value: 138.6 },
    { label: "Apr", value: 142.7 },
    { label: "May", value: 147.2 },
  ],
  "1Y": [
    { label: "Jun", value: 112.4 },
    { label: "Aug", value: 118.1 },
    { label: "Oct", value: 122.6 },
    { label: "Dec", value: 128.4 },
    { label: "Feb", value: 134.2 },
    { label: "Apr", value: 142.7 },
    { label: "Jun", value: 147.2 },
  ],
  All: [
    { label: "2021", value: 42.0 },
    { label: "2022", value: 61.5 },
    { label: "2023", value: 84.2 },
    { label: "2024", value: 108.9 },
    { label: "2025", value: 131.4 },
    { label: "2026", value: 147.2 },
  ],
};

export const perfRanges: PerfRange[] = ["1M", "6M", "1Y", "All"];

/* -------------------------------- Reminders --------------------------------- */

export type Reminder = {
  id: string;
  kind: "sip" | "maturity" | "dividend";
  title: string;
  detail: string;
  due: string;
  amount: number;
};

export const reminders: Reminder[] = [
  { id: "r1", kind: "sip", title: "Nifty 50 Index SIP", detail: "Angel One (Magesh)", due: "5 Aug", amount: 40_000 },
  { id: "r2", kind: "sip", title: "Flexi Cap SIP", detail: "Angel One (Magesh)", due: "7 Aug", amount: 35_000 },
  { id: "r3", kind: "sip", title: "Emerging Bluechip SIP", detail: "Angel One (Wife)", due: "10 Aug", amount: 25_000 },
  { id: "r4", kind: "dividend", title: "ITC dividend", detail: "₹6.25 / share credit", due: "18 Aug", amount: 18_750 },
  { id: "r5", kind: "maturity", title: "Muthoot NCD maturity", detail: "9.1% · IndiaBonds", due: "24 Sep", amount: 10_00_000 },
];

/* -------------------------------- AI insights ------------------------------- */

export type InvestmentInsight = {
  id: string;
  tag: string;
  tone: "positive" | "caution" | "neutral";
  title: string;
  body: string;
  impact: string;
  action: string;
};

export const investmentInsights: InvestmentInsight[] = [
  {
    id: "i1",
    tag: "Concentration",
    tone: "caution",
    title: "Stocks are 36% of the portfolio",
    body: "Reliance and HDFC Bank alone make up 20% of investments. A partial trim into your index fund would smooth drawdowns.",
    impact: "Risk ↓ ~12%",
    action: "Rebalance plan",
  },
  {
    id: "i2",
    tag: "Cash drag",
    tone: "neutral",
    title: "₹7.00 L sitting in liquid funds",
    body: "That's 4.8% of investable assets earning 6.1%. Laddering ₹4 L into your bond sleeve could add ~₹12,000 a year.",
    impact: "+₹12,000 / yr",
    action: "Move to bonds",
  },
  {
    id: "i3",
    tag: "Tax",
    tone: "positive",
    title: "₹1.10 L of LTCG headroom left",
    body: "Harvesting long-term gains before March keeps your equity gains tax-free this financial year.",
    impact: "Save ₹11,000",
    action: "Plan harvest",
  },
];

/* ----------------------------- Transactions --------------------------------- */

export type InvestmentTxn = {
  id: string;
  kind: "buy" | "sell" | "sip" | "dividend" | "interest";
  title: string;
  account: string;
  date: string;
  amount: number;
};

export const transactions: InvestmentTxn[] = [
  { id: "t1", kind: "sip", title: "Nifty 50 Index Fund SIP", account: "Angel One (Magesh)", date: "5 Jul 2026", amount: -40_000 },
  { id: "t2", kind: "buy", title: "Bought 40 · Reliance", account: "Angel One (Magesh)", date: "2 Jul 2026", amount: -1_18_400 },
  { id: "t3", kind: "dividend", title: "Infosys dividend", account: "Angel One (Magesh)", date: "28 Jun 2026", amount: 9_400 },
  { id: "t4", kind: "interest", title: "HDFC Corporate Bond coupon", account: "IndiaBonds", date: "24 Jun 2026", amount: 60_900 },
  { id: "t5", kind: "sell", title: "Sold 150 · ITC", account: "Angel One (Wife)", date: "19 Jun 2026", amount: 71_250 },
  { id: "t6", kind: "buy", title: "Gold ETF accumulation", account: "Manual Investments", date: "11 Jun 2026", amount: -50_000 },
];

/* --------------------------- Add investment sheet --------------------------- */

export const addInvestmentOptions = [
  { id: "stock", label: "Stock", hint: "Listed equity · NSE / BSE" },
  { id: "mutual-fund", label: "Mutual Fund", hint: "Lumpsum or new SIP" },
  { id: "bond", label: "Bond", hint: "Corporate bond or NCD" },
  { id: "gold", label: "Gold", hint: "ETF, SGB or digital gold" },
  { id: "fd", label: "Fixed Deposit", hint: "Bank or corporate FD" },
  { id: "manual", label: "Manual Entry", hint: "Anything else you track" },
] as const;
