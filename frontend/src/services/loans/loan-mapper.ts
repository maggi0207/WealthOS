import {
  loanAccounts as mockAccounts,
  loanPayments as mockPayments,
  loanReminders as mockReminders,
  loansTotals as mockTotals,
  loansRepaidPct as mockRepaidPct,
  type LoanAccount,
  type LoanKind,
  type LoanPayment,
  type LoanReminder,
} from "@/lib/loans-data";
import type {
  LoanDto,
  LoanListDto,
  LoanListItemDto,
  LoanListView,
  LoanPaymentDto,
  LoanPaymentStatusDto,
  LoanReminderDto,
  LoanSummaryDto,
  LoanTotalsView,
  LoanTypeDto,
  UpcomingPaymentsDto,
} from "@/services/loans/types";

function toNumber(value: unknown, fallback = 0): number {
  const n = typeof value === "number" ? value : Number(value);
  return Number.isFinite(n) ? n : fallback;
}

function toDateOnly(value: string | null | undefined, fallback = ""): string {
  if (!value) return fallback;
  return value.length >= 10 ? value.slice(0, 10) : value;
}

function maskAccount(accountNumber: string | null | undefined): string {
  if (!accountNumber) return "•••• ----";
  const digits = accountNumber.replace(/\D/g, "");
  if (digits.length < 4) return `•••• ${digits || "----"}`;
  return `•••• ${digits.slice(-4)}`;
}

function mapLoanKind(type: LoanTypeDto): LoanKind {
  const key = String(type);
  if (key === "0" || key === "Home") return "home";
  if (key === "2" || key === "Jewel") return "jewel";
  if (key === "1" || key === "Personal") return "personal";
  if (key === "3" || key === "Vehicle") return "personal";
  if (key === "4" || key === "Education") return "personal";
  return "personal";
}

function mapPaymentStatus(
  status: LoanPaymentStatusDto,
): LoanPayment["status"] {
  const key = String(status);
  if (key === "1" || key === "Pending") return "pending";
  if (key === "2" || key === "Failed") return "failed";
  return "paid";
}

function formatDebtFreeBy(endDates: Array<string | null | undefined>): string {
  const parsed = endDates
    .filter(Boolean)
    .map((d) => new Date(`${toDateOnly(d)}T00:00:00`))
    .filter((d) => !Number.isNaN(d.getTime()));
  if (parsed.length === 0) return "—";
  const latest = parsed.reduce((a, b) => (a > b ? a : b));
  return latest.toLocaleDateString("en-IN", { month: "short", year: "numeric" });
}

function mapListItemToAccount(dto: LoanListItemDto): LoanAccount {
  return {
    id: String(dto.id),
    kind: mapLoanKind(dto.type),
    name: dto.name,
    lender: dto.lenderName,
    accountMask: "•••• ----",
    principal: toNumber(dto.principal),
    outstanding: toNumber(dto.outstandingBalance),
    emi: toNumber(dto.emiAmount),
    ratePct: toNumber(dto.interestRate),
    startedOn: "",
    closesOn: toDateOnly(dto.nextEmiDate),
    remainingMonths: toNumber(dto.remainingTenureMonths),
    nextEmiOn: toDateOnly(dto.nextEmiDate),
    autoDebit: false,
  };
}

function mapLoanDtoToAccount(dto: LoanDto): LoanAccount {
  return {
    id: String(dto.id),
    kind: mapLoanKind(dto.type),
    name: dto.name,
    lender: dto.lenderName,
    accountMask: maskAccount(dto.accountNumber),
    principal: toNumber(dto.principal),
    outstanding: toNumber(dto.outstandingBalance),
    emi: toNumber(dto.emiAmount),
    ratePct: toNumber(dto.interestRate),
    startedOn: toDateOnly(dto.startDate),
    closesOn: toDateOnly(dto.endDate),
    remainingMonths: toNumber(dto.remainingTenureMonths),
    nextEmiOn: toDateOnly(dto.nextEmiDate),
    autoDebit: Boolean(dto.autoDebit),
  };
}

export function mapLoanPayment(dto: LoanPaymentDto, loanId: string): LoanPayment {
  return {
    id: String(dto.id),
    loanId,
    paidOn: toDateOnly(dto.paidOn),
    amount: toNumber(dto.amount),
    principal: toNumber(dto.principalComponent),
    interest: toNumber(dto.interestComponent),
    status: mapPaymentStatus(dto.status),
    mode: dto.paymentMode || "—",
  };
}

export function mapLoanReminder(dto: LoanReminderDto): LoanReminder {
  return {
    id: String(dto.id),
    loanId: String(dto.loanId),
    title: dto.title,
    detail: dto.detail || dto.loanName || "",
    dueOn: toDateOnly(dto.dueOn),
    amount: toNumber(dto.amount),
    urgent: Boolean(dto.isUrgent),
  };
}

export function mapLoanSummary(
  dto: LoanSummaryDto,
  endDates: Array<string | null | undefined> = [],
): LoanTotalsView {
  const borrowed = toNumber(dto.totalLoanAmount);
  const outstanding = toNumber(dto.outstandingBalance);
  const repaidPct =
    borrowed > 0 ? Math.round(((borrowed - outstanding) / borrowed) * 100) : 0;

  return {
    outstanding,
    borrowed,
    monthlyEmi: toNumber(dto.monthlyEmi),
    debtFreeBy: formatDebtFreeBy(endDates),
    loanCount: toNumber(dto.loanCount),
    repaidPct,
  };
}

export function mapLoanListResponse(
  list: LoanListDto,
  summary?: LoanSummaryDto | null,
): LoanListView {
  const accounts = (list.items ?? []).map(mapListItemToAccount);
  const totals = summary
    ? mapLoanSummary(
        summary,
        accounts.map((a) => a.closesOn || a.nextEmiOn),
      )
    : buildTotalsFromAccounts(accounts);

  return {
    accounts,
    totals,
    page: list.page,
    pageSize: list.pageSize,
    totalCount: list.totalCount,
  };
}

function buildTotalsFromAccounts(accounts: LoanAccount[]): LoanTotalsView {
  const outstanding = accounts.reduce((s, l) => s + l.outstanding, 0);
  const borrowed = accounts.reduce((s, l) => s + l.principal, 0);
  const monthlyEmi = accounts.reduce((s, l) => s + l.emi, 0);
  return {
    outstanding,
    borrowed,
    monthlyEmi,
    debtFreeBy: formatDebtFreeBy(accounts.map((a) => a.closesOn)),
    loanCount: accounts.length,
    repaidPct:
      borrowed > 0
        ? Math.round(((borrowed - outstanding) / borrowed) * 100)
        : 0,
  };
}

export function mapLoanDetail(dto: LoanDto): {
  account: LoanAccount;
  payments: LoanPayment[];
  reminders: LoanReminder[];
} {
  return {
    account: mapLoanDtoToAccount(dto),
    payments: (dto.payments ?? []).map((p) => mapLoanPayment(p, String(dto.id))),
    reminders: (dto.reminders ?? []).map(mapLoanReminder),
  };
}

export function mapUpcomingPayments(dto: UpcomingPaymentsDto): LoanReminder[] {
  return (dto.items ?? []).map(mapLoanReminder);
}

/* --------------------------------- Mock ----------------------------------- */

export function mapMockLoanList(): LoanListView {
  return {
    accounts: mockAccounts.map((a) => ({ ...a })),
    totals: {
      outstanding: mockTotals.outstanding,
      borrowed: mockTotals.borrowed,
      monthlyEmi: mockTotals.monthlyEmi,
      debtFreeBy: mockTotals.debtFreeBy,
      loanCount: mockAccounts.length,
      repaidPct: mockRepaidPct,
    },
    page: 1,
    pageSize: mockAccounts.length,
    totalCount: mockAccounts.length,
  };
}

export function mapMockLoanSummary(): LoanTotalsView {
  return mapMockLoanList().totals;
}

export function mapMockUpcomingPayments(): LoanReminder[] {
  return mockReminders.map((r) => ({ ...r }));
}

export function mapMockLoanDetail(id: string): {
  account: LoanAccount;
  payments: LoanPayment[];
  reminders: LoanReminder[];
} {
  const account =
    mockAccounts.find((a) => a.id === id) ?? mockAccounts[0]!;
  return {
    account: { ...account },
    payments: mockPayments
      .filter((p) => p.loanId === account.id)
      .map((p) => ({ ...p })),
    reminders: mockReminders
      .filter((r) => r.loanId === account.id)
      .map((r) => ({ ...r })),
  };
}

export function mapMockPayments(loanId?: string): LoanPayment[] {
  const rows = loanId
    ? mockPayments.filter((p) => p.loanId === loanId)
    : mockPayments;
  return rows.map((p) => ({ ...p }));
}
