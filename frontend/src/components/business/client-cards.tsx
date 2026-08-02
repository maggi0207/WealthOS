import { useState } from "react";
import { Briefcase, PauseCircle, Pencil } from "lucide-react";

import { ClientFormSheet } from "@/components/business/client-form-sheet";
import { EmptyState } from "@/components/ui-kit/empty-state";
import { ListSkeleton } from "@/components/ui-kit/skeletons";
import { useIncomeOverview } from "@/hooks/api/use-income";
import { fmtDate, fmtINR, fmtINRShort, type BusinessClient, type ClientStatus } from "@/lib/business-data";
import { cn } from "@/lib/utils";

const statusStyle: Record<ClientStatus, string> = {
  active: "bg-success/12 text-success",
  paused: "bg-amber-500/12 text-amber-500",
};

/** Client cards — revenue, status, outstanding invoice and last payment. */
export function ClientCards() {
  const { data, isPending, isError, refetch, isFetching } = useIncomeOverview();
  const [formOpen, setFormOpen] = useState(false);
  const [editClient, setEditClient] = useState<BusinessClient | null>(null);

  function openCreate() {
    setEditClient(null);
    setFormOpen(true);
  }

  function openEdit(client: BusinessClient) {
    setEditClient(client);
    setFormOpen(true);
  }

  if (isPending) return <ListSkeleton rows={3} />;

  if (isError || !data) {
    return (
      <div className="surface-tile px-4 py-6 text-center">
        <p className="text-sm font-medium">Unable to load clients</p>
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

  const clients = data.clients;

  if (clients.length === 0) {
    return (
      <>
        <EmptyState
          icon={Briefcase}
          title="No clients yet"
          description="Add your first client to track monthly revenue, invoices and payments."
          action={
            <button
              type="button"
              onClick={openCreate}
              className="press inline-flex min-h-11 items-center rounded-xl bg-primary px-4 text-[13px] font-semibold text-primary-foreground"
            >
              Add client
            </button>
          }
        />
        <ClientFormSheet
          open={formOpen}
          onOpenChange={setFormOpen}
          mode="create"
        />
      </>
    );
  }

  return (
    <>
      <div className="no-scrollbar bleed-gutter page-gutter flex snap-x snap-mandatory gap-3 overflow-x-auto pb-1 sm:mx-0 sm:grid sm:grid-cols-2 sm:px-0 xl:grid-cols-3">
        {clients.map((client) => (
          <article
            key={client.id}
            className="surface-tile press flex w-[78vw] max-w-[310px] shrink-0 snap-start flex-col p-4 sm:w-auto sm:max-w-none"
          >
            <div className="grid grid-cols-[minmax(0,1fr)_auto] items-start gap-2">
              <div className="min-w-0">
                <h3 className="truncate text-[15px] font-semibold">{client.name}</h3>
                <p className="mt-0.5 truncate text-[11px] text-muted-foreground">{client.engagement}</p>
              </div>
              <span
                className={cn(
                  "inline-flex shrink-0 items-center gap-1 rounded-full px-2 py-0.5 text-[10px] font-semibold capitalize",
                  statusStyle[client.status],
                )}
              >
                {client.status === "paused" && <PauseCircle className="size-3" />}
                {client.status}
              </span>
            </div>

            <p className="mt-3 font-display text-xl font-semibold tabular-nums">
              {client.monthlyRevenue > 0 ? fmtINRShort(client.monthlyRevenue) : "—"}
              <span className="ml-1 text-[11px] font-medium text-muted-foreground">/ month</span>
            </p>

            <dl className="mt-3 grid grid-cols-2 gap-2 border-t border-border/60 pt-3 text-[11px]">
              <div className="min-w-0">
                <dt className="text-muted-foreground">Outstanding</dt>
                <dd
                  className={cn(
                    "mt-0.5 truncate font-semibold tabular-nums",
                    client.outstandingInvoice > 0 ? "text-amber-500" : "text-success",
                  )}
                >
                  {client.outstandingInvoice > 0 ? fmtINR(client.outstandingInvoice) : "Cleared"}
                </dd>
              </div>
              <div className="min-w-0">
                <dt className="text-muted-foreground">Last payment</dt>
                <dd className="mt-0.5 truncate font-medium tabular-nums">
                  {fmtINRShort(client.lastPaymentAmount)}
                  {client.lastPaymentOn ? ` · ${fmtDate(client.lastPaymentOn)}` : ""}
                </dd>
              </div>
            </dl>

            <button
              type="button"
              onClick={() => openEdit(client)}
              className="press mt-3 inline-flex min-h-11 items-center justify-center gap-1.5 rounded-xl bg-secondary text-[12px] font-semibold"
            >
              <Pencil className="size-3.5" />
              Edit client
            </button>
          </article>
        ))}
      </div>

      <ClientFormSheet
        open={formOpen}
        onOpenChange={setFormOpen}
        mode={editClient ? "edit" : "create"}
        client={editClient}
      />
    </>
  );
}
