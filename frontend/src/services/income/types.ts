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

export type ClientResponseDto = ClientDto & {
  contactEmail?: string | null;
  contactPhone?: string | null;
  notes?: string | null;
};

export type ProjectDto = {
  id: string;
  clientId: string;
  clientName?: string;
  name: string;
  description?: string | null;
  status: number | string;
  startDate: string;
  endDate?: string | null;
  monthlyRevenue?: number | null;
  currencyCode: string;
};

export type ProjectListDto = {
  items: ProjectDto[];
  page: number;
  pageSize: number;
  totalCount: number;
};

export type InvoiceItemDto = {
  id: string;
  description: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
};

export type InvoiceDto = {
  id: string;
  clientId: string;
  clientName: string;
  projectId?: string | null;
  invoiceNumber: string;
  issueDate: string;
  dueDate: string;
  status: number | string;
  subTotal: number;
  amountPaid: number;
  outstandingAmount: number;
  currencyCode: string;
  notes?: string | null;
  items?: InvoiceItemDto[];
};

export type InvoiceListDto = {
  items: InvoiceDto[];
  page: number;
  pageSize: number;
  totalCount: number;
};

export type SalaryResponseDto = {
  id: string;
  memberName: string;
  employer: string;
  role: string;
  monthlyAmount: number;
  currencyCode: string;
  lastCreditedOn?: string | null;
  nextExpectedOn?: string | null;
  status: number | string;
  paymentId?: string | null;
  notes?: string | null;
};

export type DeveloperResponseDto = DeveloperDto & {
  notes?: string | null;
};

export type ExpenseResponseDto = ExpenseDto & {
  currencyCode?: string;
  notes?: string | null;
};

export type InvoicePaymentResponseDto = {
  id: string;
  invoiceId: string;
  amount: number;
  paidOn: string;
  method: number | string;
  reference?: string | null;
  notes?: string | null;
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
