/**
 * Income & Business module mock data (INR).
 * Pure frontend fixtures — no backend. The shapes below are intentionally
 * backend-shaped: stable string ids, ISO-8601 date strings, enum-ish unions and
 * numeric minor-unit-free rupee amounts, so swapping the constants for API
 * responses later is a drop-in change.
 */

import { fmtINR, fmtINRShort, fmtPctSigned } from "@/lib/wealth-data";

export { fmtINR, fmtINRShort, fmtPctSigned };

export type ISODate = string; // "2026-07-31"

export function fmtDate(iso: ISODate): string {
  return new Date(`${iso}T00:00:00`).toLocaleDateString("en-IN", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
}

export function fmtDateShort(iso: ISODate): string {
  return new Date(`${iso}T00:00:00`).toLocaleDateString("en-IN", { day: "numeric", month: "short" });
}

/* ------------------------------- Cash flow ---------------------------------- */

export type CashFlowSummary = {
  periodLabel: string;
  period: string; // "2026-07"
  salaryIncome: number;
  businessRevenue: number;
  businessPayroll: number;
  businessExpenses: number;
  personalOutflow: number;
};

export const cashFlow: CashFlowSummary = {
  periodLabel: "July 2026",
  period: "2026-07",
  salaryIncome: 3_85_000,
  businessRevenue: 6_40_000,
  businessPayroll: 3_10_000,
  businessExpenses: 78_500,
  personalOutflow: 2_46_000,
};

export const businessProfit = cashFlow.businessRevenue - cashFlow.businessPayroll - cashFlow.businessExpenses;
export const totalIncome = cashFlow.salaryIncome + businessProfit;
export const savings = totalIncome - cashFlow.personalOutflow;
export const savingsRate = (savings / totalIncome) * 100;

/* -------------------------------- Salaries ---------------------------------- */

export type SalaryMember = {
  id: string;
  memberName: string;
  employer: string;
  role: string;
  monthlySalary: number;
  lastCreditedOn: ISODate;
  nextExpectedOn: ISODate;
  status: "active" | "upcoming";
};

export const salaryMembers: SalaryMember[] = [
  {
    id: "sal-magesh",
    memberName: "Magesh",
    employer: "Zoho Corporation",
    role: "Engineering Manager",
    monthlySalary: 2_45_000,
    lastCreditedOn: "2026-07-31",
    nextExpectedOn: "2026-08-31",
    status: "active",
  },
  {
    id: "sal-wife",
    memberName: "Wife",
    employer: "Freshworks",
    role: "Senior Analyst",
    monthlySalary: 1_40_000,
    lastCreditedOn: "2026-07-30",
    nextExpectedOn: "2026-08-30",
    status: "active",
  },
];

/* --------------------------------- Clients ---------------------------------- */

export type ClientStatus = "active" | "paused";

export type BusinessClient = {
  id: string;
  name: string;
  engagement: string;
  status: ClientStatus;
  monthlyRevenue: number;
  outstandingInvoice: number;
  lastPaymentAmount: number;
  lastPaymentOn: ISODate;
};

export const clients: BusinessClient[] = [
  {
    id: "cl-northbridge",
    name: "Northbridge Retail",
    engagement: "Retainer · Web platform",
    status: "active",
    monthlyRevenue: 2_75_000,
    outstandingInvoice: 2_75_000,
    lastPaymentAmount: 2_75_000,
    lastPaymentOn: "2026-07-08",
  },
  {
    id: "cl-lumen",
    name: "Lumen Health",
    engagement: "Retainer · Mobile app",
    status: "active",
    monthlyRevenue: 2_10_000,
    outstandingInvoice: 0,
    lastPaymentAmount: 2_10_000,
    lastPaymentOn: "2026-07-05",
  },
  {
    id: "cl-arka",
    name: "Arka Logistics",
    engagement: "Time & material · Dashboard",
    status: "active",
    monthlyRevenue: 1_55_000,
    outstandingInvoice: 78_000,
    lastPaymentAmount: 77_000,
    lastPaymentOn: "2026-06-28",
  },
  {
    id: "cl-vetri",
    name: "Vetri Motors",
    engagement: "Support · Paused since May",
    status: "paused",
    monthlyRevenue: 0,
    outstandingInvoice: 45_000,
    lastPaymentAmount: 90_000,
    lastPaymentOn: "2026-05-12",
  },
];

export const totalOutstanding = clients.reduce((sum, c) => sum + c.outstandingInvoice, 0);

/* ----------------------------- Developer payroll ---------------------------- */

export type PayrollStatus = "paid" | "pending" | "scheduled";

export type Developer = {
  id: string;
  name: string;
  role: string;
  clientId: string;
  monthlySalary: number;
  status: PayrollStatus;
  nextPaymentOn: ISODate;
};

export const developers: Developer[] = [
  {
    id: "dev-arun",
    name: "Arun Prakash",
    role: "Full-stack developer",
    clientId: "cl-northbridge",
    monthlySalary: 95_000,
    status: "paid",
    nextPaymentOn: "2026-08-05",
  },
  {
    id: "dev-divya",
    name: "Divya R",
    role: "React Native developer",
    clientId: "cl-lumen",
    monthlySalary: 85_000,
    status: "paid",
    nextPaymentOn: "2026-08-05",
  },
  {
    id: "dev-karthik",
    name: "Karthik S",
    role: "Backend developer",
    clientId: "cl-arka",
    monthlySalary: 78_000,
    status: "pending",
    nextPaymentOn: "2026-08-02",
  },
  {
    id: "dev-sneha",
    name: "Sneha M",
    role: "QA engineer",
    clientId: "cl-northbridge",
    monthlySalary: 52_000,
    status: "scheduled",
    nextPaymentOn: "2026-08-05",
  },
];

export const payrollStatusLabel: Record<PayrollStatus, string> = {
  paid: "Paid",
  pending: "Pending",
  scheduled: "Scheduled",
};

export function clientName(clientId: string): string {
  return clients.find((c) => c.id === clientId)?.name ?? "Unassigned";
}

/* ----------------------------- Business expenses ---------------------------- */

export type BusinessExpense = {
  id: string;
  category: string;
  vendor: string;
  amount: number;
  recurring: boolean;
  paidOn: ISODate;
};

export const businessExpenses: BusinessExpense[] = [
  { id: "exp-cloud", category: "Cloud & hosting", vendor: "AWS", amount: 28_400, recurring: true, paidOn: "2026-07-03" },
  { id: "exp-tools", category: "Software tools", vendor: "Figma, GitHub, Slack", amount: 16_200, recurring: true, paidOn: "2026-07-04" },
  { id: "exp-office", category: "Co-working", vendor: "IndiQube Adyar", amount: 22_000, recurring: true, paidOn: "2026-07-01" },
  { id: "exp-ca", category: "Professional fees", vendor: "CA & compliance", amount: 8_500, recurring: false, paidOn: "2026-07-14" },
  { id: "exp-misc", category: "Travel & misc", vendor: "Client visits", amount: 3_400, recurring: false, paidOn: "2026-07-19" },
];

export const totalBusinessExpenses = businessExpenses.reduce((sum, e) => sum + e.amount, 0);

/* ------------------------------ Income trend -------------------------------- */

export type IncomePoint = {
  label: string;
  period: string;
  salary: number; // ₹ thousand
  business: number; // ₹ thousand
};

export const incomeTrend: IncomePoint[] = [
  { label: "Feb", period: "2026-02", salary: 385, business: 470 },
  { label: "Mar", period: "2026-03", salary: 385, business: 520 },
  { label: "Apr", period: "2026-04", salary: 385, business: 505 },
  { label: "May", period: "2026-05", salary: 385, business: 560 },
  { label: "Jun", period: "2026-06", salary: 385, business: 595 },
  { label: "Jul", period: "2026-07", salary: 385, business: 640 },
];

/* ---------------------------- AI business insights -------------------------- */

export type BusinessInsight = {
  id: string;
  tag: string;
  tone: "positive" | "caution" | "neutral";
  title: string;
  body: string;
  impact: string;
  action: string;
};

export const businessInsights: BusinessInsight[] = [
  {
    id: "ins-outstanding",
    tag: "Collections",
    tone: "caution",
    title: "₹3.98 L in invoices is overdue",
    body: "Northbridge and Arka both crossed 30 days. Chasing them clears 62% of this month's payroll.",
    impact: "Frees ₹3.98 L cash",
    action: "Send reminders",
  },
  {
    id: "ins-margin",
    tag: "Margin",
    tone: "positive",
    title: "Business margin improved to 39%",
    body: "Revenue grew 7.6% while payroll stayed flat. Holding this pace adds ₹6.2 L a year.",
    impact: "+₹6.2 L / year",
    action: "See P&L",
  },
  {
    id: "ins-concentration",
    tag: "Risk",
    tone: "caution",
    title: "43% of revenue sits with one client",
    body: "Northbridge dominates the book. One more mid-sized retainer would balance the concentration risk.",
    impact: "Reduce single-client risk",
    action: "Plan pipeline",
  },
  {
    id: "ins-savings",
    tag: "Savings",
    tone: "positive",
    title: "Savings rate is at 60%",
    body: "Salary covers household outflow entirely, so business profit is fully investable this month.",
    impact: "₹3.7 L investable",
    action: "Route to SIP",
  },
];

/* ------------------------------ Quick actions ------------------------------- */

export type QuickActionId =
  | "add-client"
  | "add-developer"
  | "record-payment"
  | "record-expense"
  | "create-invoice";

export const quickActions: { id: QuickActionId; label: string; hint: string }[] = [
  { id: "add-client", label: "Add client", hint: "New retainer or project" },
  { id: "add-developer", label: "Add developer", hint: "Assign to a client" },
  { id: "record-payment", label: "Record payment", hint: "Client payment received" },
  { id: "record-expense", label: "Record expense", hint: "Tools, cloud, office" },
  { id: "create-invoice", label: "Create invoice", hint: "Bill a client" },
];
