import { HeroSkeleton } from "@/components/ui-kit/skeletons";
import { useIncomeOverview } from "@/hooks/api/use-income";
import { fmtINR } from "@/lib/business-data";
import { cn } from "@/lib/utils";

/** Monthly P&L: revenue − payroll − expenses = net profit. */
export function PnlSummary() {
  const { data, isPending, isError, refetch, isFetching } = useIncomeOverview();

  if (isPending) return <HeroSkeleton className="min-h-[12rem]" />;

  if (isError || !data) {
    return (
      <div className="surface-tile px-4 py-6 text-center">
        <p className="text-sm font-medium">Unable to load P&amp;L</p>
        <button
          type="button"
          onClick={() => void refetch()}
          disabled={isFetching}
          className="press mt-3 inline-flex min-h-11 items-center rounded-full bg-primary px-4 text-xs font-semibold text-primary-foreground"
        >
          Retry
        </button>
      </div>
    );
  }

  const { cashFlow } = data;
  const businessProfit = cashFlow.businessProfit;
  const margin =
    cashFlow.businessRevenue > 0
      ? (businessProfit / cashFlow.businessRevenue) * 100
      : 0;

  const rows = [
    { label: "Revenue", value: cashFlow.businessRevenue, sign: "+" as const },
    { label: "Developer payroll", value: cashFlow.businessPayroll, sign: "−" as const },
    { label: "Business expenses", value: cashFlow.businessExpenses, sign: "−" as const },
  ];

  return (
    <section className="surface-tile overflow-hidden">
      <ul className="divide-y divide-border/50">
        {rows.map((row) => (
          <li key={row.label} className="flex items-center justify-between gap-3 px-4 py-3">
            <span className="truncate text-[13px] text-muted-foreground">{row.label}</span>
            <span
              className={cn(
                "shrink-0 text-[14px] font-semibold tabular-nums",
                row.sign === "+" ? "text-success" : "text-foreground",
              )}
            >
              {row.sign} {fmtINR(row.value)}
            </span>
          </li>
        ))}
      </ul>

      <div className="border-t border-border/60 bg-secondary/30 px-4 py-3.5">
        <div className="flex items-center justify-between gap-3">
          <span className="text-[11px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
            Net profit
          </span>
          <span className="font-display text-xl font-semibold tabular-nums text-success">{fmtINR(businessProfit)}</span>
        </div>
        <div className="mt-2.5 h-1.5 w-full overflow-hidden rounded-full bg-border/60">
          <div
            className="h-full rounded-full bg-success transition-[width] duration-700"
            style={{ width: `${Math.min(margin, 100).toFixed(1)}%` }}
          />
        </div>
        <p className="mt-1.5 text-[11px] text-muted-foreground tabular-nums">
          {margin.toFixed(1)}% margin on {fmtINR(cashFlow.businessRevenue)} revenue
        </p>
      </div>
    </section>
  );
}
