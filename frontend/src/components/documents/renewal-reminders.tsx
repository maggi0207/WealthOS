import { BellRing, CalendarClock, ShieldCheck } from "lucide-react";
import { toast } from "sonner";

import { EmptyState } from "@/components/ui-kit/empty-state";
import { fmtDate, renewals } from "@/lib/documents-data";

/** Expiry and renewal reminders across the vault. */
export function RenewalReminders() {
  if (renewals.length === 0) {
    return (
      <EmptyState
        icon={ShieldCheck}
        title="Nothing expiring"
        description="Every document in your vault is current. We'll nudge you 30 days before any expiry."
      />
    );
  }

  return (
    <ul className="surface-tile divide-y divide-border/50 overflow-hidden">
      {renewals.map((item) => {
        const overdue = item.status === "expired";
        return (
          <li key={item.id} className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
            <span
              className={`grid size-9 shrink-0 place-items-center rounded-xl ${
                overdue ? "bg-destructive/12 text-destructive" : "bg-warning/12 text-warning"
              }`}
            >
              <CalendarClock className="size-4" />
            </span>
            <div className="min-w-0">
              <p className="truncate text-[14px] font-medium">{item.name}</p>
              <p className="truncate text-[11px] text-muted-foreground">
                {overdue ? "Expired" : "Expires"} {fmtDate(item.expiresOn)}
              </p>
            </div>
            <button
              type="button"
              aria-label={`Set renewal reminder for ${item.name}`}
              onClick={() => toast.success(`Renewal reminder set for ${item.name}`)}
              className="press grid size-11 shrink-0 place-items-center rounded-xl text-muted-foreground"
            >
              <BellRing className="size-4" />
            </button>
          </li>
        );
      })}
    </ul>
  );
}
