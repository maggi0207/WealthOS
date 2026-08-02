/**
 * Income & Business API DTOs and UI view models.
 */

import type {
  BusinessClient,
  BusinessExpense,
  CashFlowSummary,
  ClientStatus,
  Developer,
  IncomePoint,
  PayrollStatus,
  SalaryMember,
} from "@/lib/business-data";

export type {
  BusinessClient,
  BusinessExpense,
  CashFlowSummary,
  ClientStatus,
  Developer,
  IncomePoint,
  PayrollStatus,
  SalaryMember,
};

/* ---------------------------------- DTOs ---------------------------------- */

export type IncomeDashboardDto = {
  period: string;
  monthlyIncome: number;
  businessRevenue: number;
  salary: number;
  developerCost: number;
  businessExpenses: number;
  outstandingInvoices: number;
  netProfit: number;
  cashAvailable: number;
  savingsRatePercent: number;
  currencyCode: string;
};

export type CashFlowDto = {
  period: string;
  periodLabel: string;
  salaryIncome: number;
  businessRevenue: number;
  developerPayroll: number;
  businessExpenses: number;
  personalOutflow: number;
  netCashFlow: number;
  currencyCode: string;
};

export type ProfitLossDto = {
  period: string;
  businessRevenue: number;
  developerCost: number;
  businessExpenses: number;
  grossProfit: number;
  netProfit: number;
  salaryIncome: number;
  totalIncome: number;
  cashAvailable: number;
  savingsRatePercent: number;
  currencyCode: string;
};

export type MonthlyIncomePointDto = {
  label: string;
  period: string;
  salary: number;
  business: number;
};

export type MonthlyIncomeTrendDto = {
  points: MonthlyIncomePointDto[];
};

export type ClientDto = {
  id: string;
  name: string;
  engagement: string;
  status: number | string;
  monthlyRevenue: number;
  outstandingInvoice: number;
  lastPaymentAmount: number;
  lastPaymentOn?: string | null;
  currencyCode: string;
};

export type ClientListDto = {
  items: ClientDto[];
  page: number;
  pageSize: number;
  totalCount: number;
};

export type DeveloperDto = {
  id: string;
  name: string;
  role: string;
  monthlySalary: number;
  primaryClientId?: string | null;
  primaryClientName?: string | null;
  isActive: boolean;
};

export type DeveloperListDto = {
  items: DeveloperDto[];
  page: number;
  pageSize: number;
  totalCount: number;
};

export type PayrollDto = {
  id: string;
  developerId: string;
  developerName: string;
  amount: number;
  period: string;
  status: number | string;
  paidOn?: string | null;
  scheduledOn?: string | null;
};

export type PayrollListDto = {
  items: PayrollDto[];
  page: number;
  pageSize: number;
  totalCount: number;
};

export type ExpenseDto = {
  id: string;
  categoryId: string;
  categoryName: string;
  vendor: string;
  amount: number;
  paidOn: string;
  isRecurring: boolean;
  period?: string | null;
};

export type ExpenseListDto = {
  items: ExpenseDto[];
  page: number;
  pageSize: number;
  totalCount: number;
};

/* ------------------------------- View models ------------------------------ */

export type CashFlowView = CashFlowSummary & {
  businessProfit: number;
  totalIncome: number;
  savings: number;
  savingsRate: number;
  outstandingInvoices: number;
  activeClientCount: number;
  marginPct: number;
};

export type PnlView = {
  period: string;
  periodLabel: string;
  businessRevenue: number;
  developerCost: number;
  businessExpenses: number;
  grossProfit: number;
  netProfit: number;
  salaryIncome: number;
  totalIncome: number;
  cashAvailable: number;
  savingsRatePercent: number;
};

export type IncomeOverviewView = {
  cashFlow: CashFlowView;
  pnl: PnlView;
  trend: IncomePoint[];
  clients: BusinessClient[];
  expenses: BusinessExpense[];
  developers: Developer[];
  salaries: SalaryMember[];
  totalOutstanding: number;
  totalBusinessExpenses: number;
};
