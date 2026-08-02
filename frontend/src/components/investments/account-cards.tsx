import { CircleDashed, Clock3, Plus, RefreshCw, ShieldCheck, PencilLine } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

import { AccountFormSheet } from "@/components/investments/account-form-sheet";
import { ListSkeleton } from "@/components/ui-kit/skeletons";
import { useInvestmentsOverview } from "@/hooks/api/use-investments";
import { fmtINRShort, statusLabel, type AccountStatus, type InvestmentAccount } from "@/lib/investments-data";
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
  const { data, isPending, isError, refetch, isFetching } = useInvestmentsOverview();
  const [createOpen, setCreateOpen] = useState(false);
  const [editAccount, setEditAccount] = useState<InvestmentAccount | null>(null);

  if (isPending) return <ListSkeleton rows={3} />;
  if (isError || !data) {
    return (
      <div className="surface-tile px-4 py-6 text-center">
        <p className="text-sm font-medium">Unable to load accounts</p>
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
    <>
      <div className="no-scrollbar bleed-gutter page-gutter flex snap-x snap-mandatory gap-3 overflow-x-auto pb-1 sm:mx-0 sm:grid sm:grid-cols-2 sm:px-0 lg:grid-cols-3">
        {data.accounts.map((account) => {
          const Icon = statusIcon[account.status];
          const up = account.dayChangePct >= 0;
          return (
            <article
              key={account.id}
              className="surface-tile press flex w-[76vw] max-w-[300px] shrink-0 snap-start flex-col p-4 sm:w-auto sm:max-w-none"
            >
              <button
                type="button"
                onClick={() => setEditAccount(account)}
                className="text-left"
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
              </button>

              <div className="mt-3 flex items-center justify-between gap-2 border-t border-border/60 pt-3">
                <span className="truncate text-[11px] text-muted-foreground">{account.lastSync}</span>
                <button
                  type="button"
                  disabled={account.status === "soon"}
                  onClick={() =>
                    toast.success(
                      account.status === "manual"
                        ? `${account.name} — sync not required for manual accounts`
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
          onClick={() => setCreateOpen(true)}
          className="surface-tile press flex w-[62vw] max-w-[300px] shrink-0 snap-start flex-col items-center justify-center gap-2 border-dashed p-4 text-center sm:w-auto sm:max-w-none"
        >
          <span className="grid size-11 place-items-center rounded-2xl bg-primary/10 text-primary">
            <Plus className="size-5" />
          </span>
          <span className="text-[14px] font-semibold">Add account</span>
          <span className="inline-flex items-center gap-1 text-[11px] text-muted-foreground">
            <CircleDashed className="size-3" />
            Manual entry supported
          </span>
        </button>
      </div>

      <AccountFormSheet open={createOpen} onOpenChange={setCreateOpen} mode="create" />
      <AccountFormSheet
        open={Boolean(editAccount)}
        onOpenChange={(open) => {
          if (!open) setEditAccount(null);
        }}
        mode="edit"
        account={editAccount}
      />
    </>
  );
}
