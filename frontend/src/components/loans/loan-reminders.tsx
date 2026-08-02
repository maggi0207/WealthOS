import { BellRing, CalendarClock } from "lucide-react";
import { toast } from "sonner";

import { ListSkeleton } from "@/components/ui-kit/skeletons";
import { useUpcomingLoanPayments } from "@/hooks/api/use-loans";
import { fmtDate, fmtINR } from "@/lib/loans-data";

/** Upcoming EMI reminders with a mock notification toggle. */
export function LoanReminders() {
  const { data, isPending, isError, refetch, isFetching } =
    useUpcomingLoanPayments();

  if (isPending) {
    return <ListSkeleton rows={3} />;
  }

  if (isError || !data) {
    return (
      <div className="surface-tile px-4 py-6 text-center">
        <p className="text-sm font-medium">Unable to load upcoming EMIs</p>
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
    <ul className="surface-tile divide-y divide-border/50 overflow-hidden">
      {data.map((item) => (
        <li key={item.id} className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
          <span
            className={`grid size-9 shrink-0 place-items-center rounded-xl ${
              item.urgent ? "bg-warning/12 text-warning" : "bg-primary/10 text-primary"
            }`}
          >
            <CalendarClock className="size-4" />
          </span>
          <div className="min-w-0">
            <p className="truncate text-[14px] font-medium">{item.title}</p>
            <p className="truncate text-[11px] text-muted-foreground">
              {item.detail} · due {fmtDate(item.dueOn)}
            </p>
          </div>
          <div className="flex shrink-0 items-center gap-2">
            <span className="text-[13px] font-semibold tabular-nums">{fmtINR(item.amount)}</span>
            <button
              type="button"
              aria-label={`Remind me about ${item.title}`}
              onClick={() => toast.success(`Reminder set for ${item.title}`)}
              className="press grid size-11 place-items-center rounded-xl text-muted-foreground"
            >
              <BellRing className="size-4" />
            </button>
          </div>
        </li>
      ))}
    </ul>
  );
}
