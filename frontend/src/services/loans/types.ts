/**
 * Loans API DTOs (camelCase, matching ASP.NET serialization)
 * and UI view models consumed by loans components.
 */

import type { LoanAccount, LoanKind, LoanPayment, LoanReminder } from "@/lib/loans-data";

export type { LoanAccount, LoanKind, LoanPayment, LoanReminder };

/* ---------------------------------- DTOs ---------------------------------- */

export type LoanTypeDto =
  | 0
  | 1
  | 2
  | 3
  | 4
  | 5
  | "Home"
  | "Personal"
  | "Jewel"
  | "Vehicle"
  | "Education"
  | "Other";

export type LoanStatusDto =
  | 0
  | 1
  | 2
  | 3
  | "Active"
  | "Closed"
  | "Defaulted"
  | "Restructured";

export type LoanPaymentStatusDto =
  | 0
  | 1
  | 2
  | "Paid"
  | "Pending"
  | "Failed";

export type LoanPaymentDto = {
  id: string;
  paidOn: string;
  amount: number;
  principalComponent: number;
  interestComponent: number;
  status: LoanPaymentStatusDto;
  paymentMode?: string | null;
  reference?: string | null;
  notes?: string | null;
  isPrepayment: boolean;
};

export type LoanReminderDto = {
  id: string;
  loanId: string;
  loanName: string;
  title: string;
  detail?: string | null;
  dueOn: string;
  amount: number;
  isUrgent: boolean;
};

export type LoanListItemDto = {
  id: string;
  name: string;
  type: LoanTypeDto;
  status: LoanStatusDto;
  lenderName: string;
  principal: number;
  outstandingBalance: number;
  emiAmount: number;
  interestRate: number;
  remainingTenureMonths: number;
  nextEmiDate?: string | null;
  currencyCode: string;
  loanProgressPercent: number;
  linkedPropertyId?: string | null;
};

export type LoanListDto = {
  items: LoanListItemDto[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type LoanDto = {
  id: string;
  name: string;
  type: LoanTypeDto;
  lenderName: string;
  accountNumber?: string | null;
  principal: number;
  outstandingBalance: number;
  interestRate: number;
  emiAmount: number;
  tenureMonths: number;
  remainingTenureMonths: number;
  startDate: string;
  endDate?: string | null;
  nextEmiDate?: string | null;
  status: LoanStatusDto;
  currencyCode: string;
  autoDebit: boolean;
  notes?: string | null;
  totalPrincipalPaid: number;
  totalInterestPaid: number;
  loanProgressPercent: number;
  emiProgressPercent: number;
  payments: LoanPaymentDto[];
  reminders: LoanReminderDto[];
  createdAt: string;
  updatedAt?: string | null;
};

export type LoanSummaryDto = {
  loanCount: number;
  totalLoanAmount: number;
  outstandingBalance: number;
  monthlyEmi: number;
  upcomingEmi: number;
  currencyCode: string;
  activeCount: number;
  closedCount: number;
};

export type UpcomingPaymentsDto = {
  items: LoanReminderDto[];
  totalUpcomingAmount: number;
  currencyCode: string;
};

export type LoanDashboardDto = {
  loan: LoanDto;
  totalPrincipalPaid: number;
  totalInterestPaid: number;
  loanProgressPercent: number;
  emiProgressPercent: number;
  paymentCount: number;
  reminderCount: number;
  generatedAt: string;
};

export type CreateLoanRequestDto = {
  name: string;
  type: LoanTypeDto;
  lenderName: string;
  principal: number;
  outstandingBalance: number;
  interestRate: number;
  emiAmount: number;
  tenureMonths: number;
  startDate: string;
  autoDebit?: boolean;
  accountNumber?: string;
  notes?: string;
};

export type UpdateLoanRequestDto = Partial<CreateLoanRequestDto> & {
  remainingTenureMonths?: number;
  nextEmiDate?: string;
  endDate?: string;
};

export type RecordLoanPaymentRequestDto = {
  paidOn: string;
  amount: number;
  principalComponent: number;
  interestComponent: number;
  paymentMode?: string;
  reference?: string;
  notes?: string;
  isPrepayment?: boolean;
};

export type LoanListQuery = {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: LoanStatusDto;
  type?: LoanTypeDto;
};

/* ------------------------------- View models ------------------------------ */

export type LoanTotalsView = {
  outstanding: number;
  borrowed: number;
  monthlyEmi: number;
  debtFreeBy: string;
  loanCount: number;
  repaidPct: number;
};

export type LoanListView = {
  accounts: LoanAccount[];
  totals: LoanTotalsView;
  page: number;
  pageSize: number;
  totalCount: number;
};
