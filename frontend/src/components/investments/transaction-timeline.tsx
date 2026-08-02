import { ArrowDownLeft, ArrowUpRight, Coins, Percent, Repeat, type LucideIcon } from "lucide-react";

import { ListSkeleton } from "@/components/ui-kit/skeletons";
import { useInvestmentsOverview } from "@/hooks/api/use-investments";
import { fmtINR, type InvestmentTxn } from "@/lib/investments-data";
import { cn } from "@/lib/utils";

const icons: Record<InvestmentTxn["kind"], LucideIcon> = {
  buy: ArrowDownLeft,
  sell: ArrowUpRight,
  sip: Repeat,
  dividend: Coins,
  interest: Percent,
};

/** Transaction history as a vertical timeline. */
export function TransactionTimeline() {
  const { data, isPending, isError, refetch, isFetching } = useInvestmentsOverview();

  if (isPending) return <ListSkeleton rows={5} />;
  if (isError || !data) {
    return (
      <div className="surface-tile px-4 py-6 text-center">
        <p className="text-sm font-medium">Unable to load transactions</p>
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

  return (
    <section className="surface-tile p-4">
      <ol className="relative space-y-4 before:absolute before:bottom-3 before:left-[15px] before:top-3 before:w-px before:bg-border">
        {data.transactions.map((txn) => {
          const Icon = icons[txn.kind];
          const credit = txn.amount >= 0;
          return (
            <li key={txn.id} className="relative grid grid-cols-[32px_minmax(0,1fr)_auto] items-start gap-3">
              <span className="z-10 grid size-8 place-items-center rounded-full border border-border bg-card text-muted-foreground">
                <Icon className="size-3.5" />
              </span>
              <div className="min-w-0 pt-0.5">
                <p className="truncate text-[14px] font-medium leading-snug">{txn.title}</p>
                <p className="truncate text-[11px] text-muted-foreground">
                  {txn.account} · {txn.date}
                </p>
              </div>
              <p
                className={cn(
                  "shrink-0 pt-0.5 text-[13px] font-semibold tabular-nums",
                  credit ? "text-success" : "text-foreground",
                )}
              >
                {credit ? "+" : "−"}
                {fmtINR(Math.abs(txn.amount))}
              </p>
            </li>
          );
        })}
      </ol>
    </section>
  );
}
