import {
  businessExpenses as mockExpenses,
  businessProfit as mockBusinessProfit,
  cashFlow as mockCashFlow,
  clients as mockClients,
  developers as mockDevelopers,
  incomeTrend as mockTrend,
  salaryMembers as mockSalaries,
  savings as mockSavings,
  savingsRate as mockSavingsRate,
  totalBusinessExpenses as mockTotalExpenses,
  totalIncome as mockTotalIncome,
  totalOutstanding as mockOutstanding,
  type BusinessClient,
  type BusinessExpense,
  type ClientStatus,
  type Developer,
  type IncomePoint,
  type PayrollStatus,
  type SalaryMember,
} from "@/lib/business-data";
import type {
  CashFlowDto,
  CashFlowView,
  ClientDto,
  ClientListDto,
  DeveloperDto,
  DeveloperListDto,
  ExpenseDto,
  ExpenseListDto,
  IncomeDashboardDto,
  IncomeOverviewView,
  MonthlyIncomeTrendDto,
  PayrollDto,
  PayrollListDto,
  PnlView,
  ProfitLossDto,
} from "@/services/income/types";

function toNumber(value: unknown, fallback = 0): number {
  const n = typeof value === "number" ? value : Number(value);
  return Number.isFinite(n) ? n : fallback;
}

function toDateOnly(value: string | null | undefined, fallback = ""): string {
  if (!value) return fallback;
  return value.length >= 10 ? value.slice(0, 10) : value;
}

function mapClientStatus(status: number | string): ClientStatus {
  const key = String(status);
  if (key === "1" || key === "Paused" || key === "paused") return "paused";
  return "active";
}

function mapPayrollStatus(status: number | string): PayrollStatus {
  const key = String(status);
  if (key === "1" || key === "Pending" || key === "pending") return "pending";
  if (key === "2" || key === "Scheduled" || key === "scheduled") return "scheduled";
  return "paid";
}

function periodLabelFromPeriod(period: string): string {
  if (!period || period.length < 7) return period || "—";
  const date = new Date(`${period}-01T00:00:00`);
  if (Number.isNaN(date.getTime())) return period;
  return date.toLocaleDateString("en-IN", { month: "long", year: "numeric" });
}

export function mapCashFlow(
  dto: CashFlowDto,
  dashboard?: IncomeDashboardDto | null,
  activeClientCount = 0,
): CashFlowView {
  const salaryIncome = toNumber(dto.salaryIncome);
  const businessRevenue = toNumber(dto.businessRevenue);
  const businessPayroll = toNumber(dto.developerPayroll);
  const businessExpenses = toNumber(dto.businessExpenses);
  const personalOutflow = toNumber(dto.personalOutflow);
  const businessProfit = businessRevenue - businessPayroll - businessExpenses;
  const totalIncome = salaryIncome + businessProfit;
  const savings = totalIncome - personalOutflow;
  const savingsRate =
    dashboard != null
      ? toNumber(dashboard.savingsRatePercent)
      : totalIncome > 0
        ? (savings / totalIncome) * 100
        : 0;
  const marginPct =
    businessRevenue > 0 ? Math.round((businessProfit / businessRevenue) * 100) : 0;

  return {
    periodLabel: dto.periodLabel || periodLabelFromPeriod(dto.period),
    period: dto.period,
    salaryIncome,
    businessRevenue,
    businessPayroll,
    businessExpenses,
    personalOutflow,
    businessProfit,
    totalIncome,
    savings,
    savingsRate,
    outstandingInvoices: toNumber(dashboard?.outstandingInvoices),
    activeClientCount,
    marginPct,
  };
}

export function mapPnl(dto: ProfitLossDto): PnlView {
  return {
    period: dto.period,
    periodLabel: periodLabelFromPeriod(dto.period),
    businessRevenue: toNumber(dto.businessRevenue),
    developerCost: toNumber(dto.developerCost),
    businessExpenses: toNumber(dto.businessExpenses),
    grossProfit: toNumber(dto.grossProfit),
    netProfit: toNumber(dto.netProfit),
    salaryIncome: toNumber(dto.salaryIncome),
    totalIncome: toNumber(dto.totalIncome),
    cashAvailable: toNumber(dto.cashAvailable),
    savingsRatePercent: toNumber(dto.savingsRatePercent),
  };
}

export function mapClient(dto: ClientDto): BusinessClient {
  return {
    id: String(dto.id),
    name: dto.name,
    engagement: dto.engagement,
    status: mapClientStatus(dto.status),
    monthlyRevenue: toNumber(dto.monthlyRevenue),
    outstandingInvoice: toNumber(dto.outstandingInvoice),
    lastPaymentAmount: toNumber(dto.lastPaymentAmount),
    lastPaymentOn: toDateOnly(dto.lastPaymentOn),
  };
}

export function mapExpense(dto: ExpenseDto): BusinessExpense {
  return {
    id: String(dto.id),
    category: dto.categoryName,
    vendor: dto.vendor,
    amount: toNumber(dto.amount),
    recurring: Boolean(dto.isRecurring),
    paidOn: toDateOnly(dto.paidOn),
  };
}

export function mapDeveloperFromPayroll(
  payroll: PayrollDto,
  developer?: DeveloperDto | null,
): Developer {
  return {
    id: String(payroll.developerId || developer?.id || payroll.id),
    name: payroll.developerName || developer?.name || "Developer",
    role: developer?.role || "Developer",
    clientId: developer?.primaryClientId
      ? String(developer.primaryClientId)
      : "",
    monthlySalary: toNumber(payroll.amount || developer?.monthlySalary),
    status: mapPayrollStatus(payroll.status),
    nextPaymentOn: toDateOnly(
      payroll.scheduledOn || payroll.paidOn,
      new Date().toISOString().slice(0, 10),
    ),
  };
}

export function mapDeveloper(dto: DeveloperDto): Developer {
  return {
    id: String(dto.id),
    name: dto.name,
    role: dto.role,
    clientId: dto.primaryClientId ? String(dto.primaryClientId) : "",
    monthlySalary: toNumber(dto.monthlySalary),
    status: dto.isActive ? "scheduled" : "pending",
    nextPaymentOn: new Date().toISOString().slice(0, 10),
  };
}

export function mapTrend(dto: MonthlyIncomeTrendDto): IncomePoint[] {
  return (dto.points ?? []).map((p) => ({
    label: p.label,
    period: p.period,
    // Backend stores rupees; chart fixtures use ₹ thousands.
    salary: Math.round(toNumber(p.salary) / 1000),
    business: Math.round(toNumber(p.business) / 1000),
  }));
}

export function mapIncomeOverview(args: {
  cashFlow: CashFlowDto;
  pnl: ProfitLossDto;
  dashboard: IncomeDashboardDto;
  trend: MonthlyIncomeTrendDto;
  clients: ClientListDto;
  expenses: ExpenseListDto;
  developers: DeveloperListDto;
  payroll: PayrollListDto;
}): IncomeOverviewView {
  const clients = (args.clients.items ?? []).map(mapClient);
  const expenses = (args.expenses.items ?? []).map(mapExpense);
  const developerById = new Map(
    (args.developers.items ?? []).map((d) => [String(d.id), d]),
  );
  const developers =
    (args.payroll.items ?? []).length > 0
      ? (args.payroll.items ?? []).map((p) =>
          mapDeveloperFromPayroll(p, developerById.get(String(p.developerId))),
        )
      : (args.developers.items ?? []).map(mapDeveloper);

  const activeClientCount = clients.filter((c) => c.status === "active").length;
  const cashFlow = mapCashFlow(args.cashFlow, args.dashboard, activeClientCount);

  return {
    cashFlow,
    pnl: mapPnl(args.pnl),
    trend: mapTrend(args.trend),
    clients,
    expenses,
    developers,
    salaries: mockSalaries.map((s) => ({ ...s })),
    totalOutstanding: clients.reduce((s, c) => s + c.outstandingInvoice, 0),
    totalBusinessExpenses: expenses.reduce((s, e) => s + e.amount, 0),
  };
}

export function mapMockIncomeOverview(): IncomeOverviewView {
  return {
    cashFlow: {
      ...mockCashFlow,
      businessProfit: mockBusinessProfit,
      totalIncome: mockTotalIncome,
      savings: mockSavings,
      savingsRate: mockSavingsRate,
      outstandingInvoices: mockOutstanding,
      activeClientCount: mockClients.filter((c) => c.status === "active").length,
      marginPct:
        mockCashFlow.businessRevenue > 0
          ? Math.round((mockBusinessProfit / mockCashFlow.businessRevenue) * 100)
          : 0,
    },
    pnl: {
      period: mockCashFlow.period,
      periodLabel: mockCashFlow.periodLabel,
      businessRevenue: mockCashFlow.businessRevenue,
      developerCost: mockCashFlow.businessPayroll,
      businessExpenses: mockCashFlow.businessExpenses,
      grossProfit: mockBusinessProfit,
      netProfit: mockBusinessProfit,
      salaryIncome: mockCashFlow.salaryIncome,
      totalIncome: mockTotalIncome,
      cashAvailable: mockSavings,
      savingsRatePercent: mockSavingsRate,
    },
    trend: mockTrend.map((p) => ({ ...p })),
    clients: mockClients.map((c) => ({ ...c })),
    expenses: mockExpenses.map((e) => ({ ...e })),
    developers: mockDevelopers.map((d) => ({ ...d })),
    salaries: mockSalaries.map((s) => ({ ...s })),
    totalOutstanding: mockOutstanding,
    totalBusinessExpenses: mockTotalExpenses,
  };
}
