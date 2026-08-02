import { clientName, developers, fmtDateShort, fmtINR, payrollStatusLabel, type PayrollStatus } from "@/lib/business-data";
import { cn } from "@/lib/utils";

const statusStyle: Record<PayrollStatus, string> = {
  paid: "bg-success/12 text-success",
  pending: "bg-amber-500/12 text-amber-500",
  scheduled: "bg-secondary text-muted-foreground",
};

/** Developer payroll — assigned client, salary, payment status and next payment. */
export function DeveloperPayroll() {
  const monthly = developers.reduce((sum, d) => sum + d.monthlySalary, 0);

  return (
    <section className="surface-tile overflow-hidden">
      <ul className="divide-y divide-border/50">
        {developers.map((dev) => (
          <li key={dev.id} className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
            <div className="min-w-0">
              <p className="truncate text-[14px] font-semibold">{dev.name}</p>
              <p className="truncate text-[11px] text-muted-foreground">
                {dev.role} · {clientName(dev.clientId)}
              </p>
              <p className="mt-1 flex flex-wrap items-center gap-x-2 gap-y-1 text-[11px] text-muted-foreground">
                <span
                  className={cn("rounded-full px-2 py-0.5 text-[10px] font-semibold", statusStyle[dev.status])}
                >
                  {payrollStatusLabel[dev.status]}
                </span>
                <span className="tabular-nums">Next {fmtDateShort(dev.nextPaymentOn)}</span>
              </p>
            </div>
            <p className="shrink-0 font-display text-[15px] font-semibold tabular-nums">{fmtINR(dev.monthlySalary)}</p>
          </li>
        ))}
      </ul>
      <div className="flex items-center justify-between gap-3 border-t border-border/60 bg-secondary/30 px-4 py-3">
        <span className="text-[11px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
          Monthly payroll
        </span>
        <span className="font-display text-[15px] font-semibold tabular-nums">{fmtINR(monthly)}</span>
      </div>
    </section>
  );
}
