import { ArrowDownLeft, ArrowUpRight, Coins, Percent, Repeat, type LucideIcon } from "lucide-react";

import { fmtINR, transactions, type InvestmentTxn } from "@/lib/investments-data";
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
  return (
    <section className="surface-tile p-4">
      <ol className="relative space-y-4 before:absolute before:bottom-3 before:left-[15px] before:top-3 before:w-px before:bg-border">
        {transactions.map((txn) => {
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
