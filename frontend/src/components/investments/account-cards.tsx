import { CircleDashed, Clock3, Plus, RefreshCw, ShieldCheck, PencilLine } from "lucide-react";
import { toast } from "sonner";

import { accounts, fmtINRShort, statusLabel, type AccountStatus } from "@/lib/investments-data";
import { cn } from "@/lib/utils";

const statusStyle: Record<AccountStatus, string> = {
  connected: "bg-success/12 text-success",
  manual: "bg-secondary text-muted-foreground",
  soon: "bg-amber-500/12 text-amber-500",
};

const statusIcon = {
  connected: ShieldCheck,
  manual: PencilLine,
  soon: Clock3,
} as const;

/** Investment accounts — snap-scrolling on phones, grid from sm up. */
export function AccountCards() {
  return (
    <div className="no-scrollbar bleed-gutter page-gutter flex snap-x snap-mandatory gap-3 overflow-x-auto pb-1 sm:mx-0 sm:grid sm:grid-cols-2 sm:px-0 lg:grid-cols-3">
      {accounts.map((account) => {
        const Icon = statusIcon[account.status];
        const up = account.dayChangePct >= 0;
        return (
          <article
            key={account.id}
            className="surface-tile press flex w-[76vw] max-w-[300px] shrink-0 snap-start flex-col p-4 sm:w-auto sm:max-w-none"
          >
            <div className="grid grid-cols-[minmax(0,1fr)_auto] items-start gap-2">
              <div className="min-w-0">
                <h3 className="truncate text-[15px] font-semibold">
                  {account.name} <span className="text-muted-foreground">({account.owner})</span>
                </h3>
                <p className="mt-0.5 truncate text-[11px] text-muted-foreground">{account.kind}</p>
              </div>
              <span
                className={cn(
                  "inline-flex shrink-0 items-center gap-1 rounded-full px-2 py-0.5 text-[10px] font-semibold",
                  statusStyle[account.status],
                )}
              >
                <Icon className="size-3" />
                {statusLabel[account.status]}
              </span>
            </div>

            <p className="mt-3 font-display text-xl font-semibold tabular-nums">{fmtINRShort(account.value)}</p>
            <p
              className={cn(
                "mt-0.5 text-[12px] font-medium tabular-nums",
                up ? "text-success" : "text-destructive",
              )}
            >
              {up ? "▲" : "▼"} {Math.abs(account.dayChangePct).toFixed(2)}% today · {account.holdings} holdings
            </p>

            <div className="mt-3 flex items-center justify-between gap-2 border-t border-border/60 pt-3">
              <span className="truncate text-[11px] text-muted-foreground">{account.lastSync}</span>
              <button
                type="button"
                disabled={account.status === "soon"}
                onClick={() =>
                  toast.success(
                    account.status === "manual"
                      ? `${account.name} — manual update coming with the editor`
                      : `${account.name} (${account.owner}) — mock sync complete`,
                  )
                }
                aria-label={`Sync ${account.name} ${account.owner}`}
                className="press grid size-11 shrink-0 place-items-center rounded-xl bg-secondary/70 text-muted-foreground disabled:opacity-40 md:size-9"
              >
                <RefreshCw className="size-4" />
              </button>
            </div>
          </article>
        );
      })}

      <button
        type="button"
        onClick={() => toast.success("Add account — broker linking is coming soon")}
        className="surface-tile press flex w-[62vw] max-w-[300px] shrink-0 snap-start flex-col items-center justify-center gap-2 border-dashed p-4 text-center sm:w-auto sm:max-w-none"
      >
        <span className="grid size-11 place-items-center rounded-2xl bg-primary/10 text-primary">
          <Plus className="size-5" />
        </span>
        <span className="text-[14px] font-semibold">Add account</span>
        <span className="inline-flex items-center gap-1 text-[11px] text-muted-foreground">
          <CircleDashed className="size-3" />
          Broker linking coming soon
        </span>
      </button>
    </div>
  );
}
