/**
 * Loans module mock data (INR).
 * Frontend-only fixtures shaped like a future API response: stable ids,
 * ISO-8601 dates, enum-ish unions and plain rupee numbers.
 */

import { fmtINR, fmtINRShort } from "@/lib/wealth-data";
import { fmtDate, fmtDateShort, type ISODate } from "@/lib/business-data";

export { fmtINR, fmtINRShort, fmtDate, fmtDateShort };
export type { ISODate };

export type LoanKind = "home" | "jewel" | "personal";

export type LoanAccount = {
  id: string;
  kind: LoanKind;
  name: string;
  lender: string;
  accountMask: string;
  principal: number;
  outstanding: number;
  emi: number;
  ratePct: number;
  startedOn: ISODate;
  closesOn: ISODate;
  remainingMonths: number;
  nextEmiOn: ISODate;
  autoDebit: boolean;
};

export const loanAccounts: LoanAccount[] = [
  {
    id: "loan-home-1",
    kind: "home",
    name: "Home loan — Ramana Flats",
    lender: "HDFC Bank",
    accountMask: "•••• 4821",
    principal: 62_00_000,
    outstanding: 38_45_000,
    emi: 52_400,
    ratePct: 8.6,
    startedOn: "2018-06-05",
    closesOn: "2033-05-05",
    remainingMonths: 82,
    nextEmiOn: "2026-08-05",
    autoDebit: true,
  },
  {
    id: "loan-jewel-1",
    kind: "jewel",
    name: "Jewel loan — gold pledge",
    lender: "Indian Overseas Bank",
    accountMask: "•••• 7710",
    principal: 6_50_000,
    outstanding: 4_10_000,
    emi: 18_900,
    ratePct: 9.4,
    startedOn: "2024-11-18",
    closesOn: "2027-05-18",
    remainingMonths: 22,
    nextEmiOn: "2026-08-18",
    autoDebit: false,
  },
  {
    id: "loan-personal-1",
    kind: "personal",
    name: "Personal loan — renovation",
    lender: "Axis Bank",
    accountMask: "•••• 2043",
    principal: 8_00_000,
    outstanding: 2_92_000,
    emi: 21_600,
    ratePct: 13.2,
    startedOn: "2023-03-12",
    closesOn: "2028-02-12",
    remainingMonths: 15,
    nextEmiOn: "2026-08-12",
    autoDebit: true,
  },
];

export const loansTotals = {
  outstanding: loanAccounts.reduce((s, l) => s + l.outstanding, 0),
  borrowed: loanAccounts.reduce((s, l) => s + l.principal, 0),
  monthlyEmi: loanAccounts.reduce((s, l) => s + l.emi, 0),
  debtFreeBy: "May 2033",
};

export const loansRepaidPct = Math.round(
  ((loansTotals.borrowed - loansTotals.outstanding) / loansTotals.borrowed) * 100,
);

export function loanPaidPct(loan: LoanAccount) {
  return Math.round(((loan.principal - loan.outstanding) / loan.principal) * 100);
}

export const loanKindLabel: Record<LoanKind, string> = {
  home: "Home loan",
  jewel: "Jewel loan",
  personal: "Personal loan",
};

/* ------------------------------ Amortization ------------------------------- */

export type AmortRow = {
  period: string; // "Aug 2026"
  emi: number;
  principal: number;
  interest: number;
  balance: number;
};

/** Straight-line amortization preview generated from the loan terms. */
export function amortizationPreview(loan: LoanAccount, months = 6): AmortRow[] {
  const monthlyRate = loan.ratePct / 100 / 12;
  let balance = loan.outstanding;
  const start = new Date(`${loan.nextEmiOn}T00:00:00`);
  const rows: AmortRow[] = [];

  for (let i = 0; i < months; i += 1) {
    const interest = Math.round(balance * monthlyRate);
    const principal = Math.max(0, Math.round(loan.emi - interest));
    balance = Math.max(0, balance - principal);
    const date = new Date(start.getFullYear(), start.getMonth() + i, 1);
    rows.push({
      period: date.toLocaleDateString("en-IN", { month: "short", year: "2-digit" }),
      emi: loan.emi,
      principal,
      interest,
      balance,
    });
  }
  return rows;
}

/* ----------------------------- Payment history ------------------------------ */

export type LoanPayment = {
  id: string;
  loanId: string;
  paidOn: ISODate;
  amount: number;
  principal: number;
  interest: number;
  status: "paid" | "pending" | "failed";
  mode: string;
};

export const loanPayments: LoanPayment[] = [
  { id: "pay-1", loanId: "loan-home-1", paidOn: "2026-07-05", amount: 52_400, principal: 24_850, interest: 27_550, status: "paid", mode: "Auto debit" },
  { id: "pay-2", loanId: "loan-personal-1", paidOn: "2026-07-12", amount: 21_600, principal: 18_400, interest: 3_200, status: "paid", mode: "Auto debit" },
  { id: "pay-3", loanId: "loan-jewel-1", paidOn: "2026-07-18", amount: 18_900, principal: 15_690, interest: 3_210, status: "paid", mode: "UPI" },
  { id: "pay-4", loanId: "loan-home-1", paidOn: "2026-06-05", amount: 52_400, principal: 24_670, interest: 27_730, status: "paid", mode: "Auto debit" },
  { id: "pay-5", loanId: "loan-personal-1", paidOn: "2026-06-12", amount: 21_600, principal: 18_210, interest: 3_390, status: "failed", mode: "Auto debit" },
  { id: "pay-6", loanId: "loan-jewel-1", paidOn: "2026-06-18", amount: 18_900, principal: 15_540, interest: 3_360, status: "paid", mode: "UPI" },
];

/* -------------------------------- Reminders --------------------------------- */

export type LoanReminder = {
  id: string;
  loanId: string;
  title: string;
  detail: string;
  dueOn: ISODate;
  amount: number;
  urgent: boolean;
};

export const loanReminders: LoanReminder[] = [
  { id: "rem-1", loanId: "loan-home-1", title: "Home loan EMI", detail: "Auto debit · HDFC Bank", dueOn: "2026-08-05", amount: 52_400, urgent: true },
  { id: "rem-2", loanId: "loan-personal-1", title: "Personal loan EMI", detail: "Auto debit · Axis Bank", dueOn: "2026-08-12", amount: 21_600, urgent: false },
  { id: "rem-3", loanId: "loan-jewel-1", title: "Jewel loan EMI", detail: "Manual · pay via UPI", dueOn: "2026-08-18", amount: 18_900, urgent: false },
];

/* ------------------------------- AI insights -------------------------------- */

export type LoanInsight = {
  id: string;
  tag: string;
  tone: "positive" | "caution" | "neutral";
  title: string;
  body: string;
  impact: string;
  action: string;
};

export const loanInsights: LoanInsight[] = [
  {
    id: "li-1",
    tag: "Prepay",
    tone: "positive",
    title: "Close the personal loan first",
    body: "At 13.2% it is your costliest debt. A ₹2.9 L lump sum clears it and frees ₹21,600 every month.",
    impact: "Saves ₹41,000 interest",
    action: "Simulate",
  },
  {
    id: "li-2",
    tag: "Refinance",
    tone: "neutral",
    title: "Home loan rate is 40 bps above market",
    body: "Comparable lenders are quoting 8.2%. A balance transfer on ₹38.45 L trims your EMI by ~₹1,150.",
    impact: "Saves ₹94,000 over tenure",
    action: "Compare lenders",
  },
  {
    id: "li-3",
    tag: "Watch",
    tone: "caution",
    title: "One auto debit failed in June",
    body: "The Axis personal loan debit bounced. Keep ₹25,000 buffer before the 12th to avoid penalty charges.",
    impact: "Avoids ₹1,200 penalty",
    action: "Set reminder",
  },
];

/* --------------------------- Prepayment simulator ---------------------------- */

export const prepaymentPresets = [1_00_000, 2_50_000, 5_00_000] as const;

/** Rough months-saved estimate for a lump-sum prepayment. */
export function simulatePrepayment(loan: LoanAccount, lumpSum: number) {
  const monthlyRate = loan.ratePct / 100 / 12;
  const balance = Math.max(0, loan.outstanding - lumpSum);
  const monthsFor = (b: number) => {
    if (b <= 0) return 0;
    const x = 1 - (b * monthlyRate) / loan.emi;
    if (x <= 0) return loan.remainingMonths;
    return Math.ceil(-Math.log(x) / Math.log(1 + monthlyRate));
  };
  const before = monthsFor(loan.outstanding);
  const after = monthsFor(balance);
  const interestSaved = Math.max(0, before * loan.emi - lumpSum - after * loan.emi);
  return { monthsSaved: Math.max(0, before - after), interestSaved, newBalance: balance };
}
