import { CheckCircle2, Clock3, XCircle, type LucideIcon } from "lucide-react";

import { EmptyState } from "@/components/ui-kit/empty-state";
import { fmtDate, fmtINR, loanPayments, type LoanPayment } from "@/lib/loans-data";

const statusIcon: Record<LoanPayment["status"], LucideIcon> = {
  paid: CheckCircle2,
  pending: Clock3,
  failed: XCircle,
};

const statusTone: Record<LoanPayment["status"], string> = {
  paid: "bg-success/12 text-success",
  pending: "bg-warning/12 text-warning",
  failed: "bg-destructive/12 text-destructive",
};

/** Payment history for a loan (or all loans when loanId is omitted). */
export function PaymentHistory({ loanId }: { loanId?: string }) {
  const rows = loanId ? loanPayments.filter((p) => p.loanId === loanId) : loanPayments;

  if (rows.length === 0) {
    return (
      <EmptyState
        icon={Clock3}
        title="No payments yet"
        description="EMI payments for this loan will appear here once the first debit clears."
      />
    );
  }

  return (
    <ul className="surface-tile divide-y divide-border/50 overflow-hidden">
      {rows.map((payment) => {
        const Icon = statusIcon[payment.status];
        return (
          <li key={payment.id} className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
            <span className={`grid size-9 shrink-0 place-items-center rounded-xl ${statusTone[payment.status]}`}>
              <Icon className="size-4" />
            </span>
            <div className="min-w-0">
              <p className="truncate text-[14px] font-medium">{fmtDate(payment.paidOn)}</p>
              <p className="truncate text-[11px] text-muted-foreground">
                {payment.mode} · P {fmtINR(payment.principal)} · I {fmtINR(payment.interest)}
              </p>
            </div>
            <p className="shrink-0 text-right text-[13px] font-semibold tabular-nums">{fmtINR(payment.amount)}</p>
          </li>
        );
      })}
    </ul>
  );
}
