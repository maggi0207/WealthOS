import { CalendarClock, Coins, Landmark, Repeat, type LucideIcon } from "lucide-react";

import { fmtINR, reminders, type Reminder } from "@/lib/investments-data";

const icons: Record<Reminder["kind"], LucideIcon> = {
  sip: Repeat,
  maturity: Landmark,
  dividend: Coins,
};

const kindLabel: Record<Reminder["kind"], string> = {
  sip: "SIP",
  maturity: "Maturity",
  dividend: "Dividend",
};

/** Upcoming SIPs, bond maturities and dividend credits. */
export function UpcomingReminders() {
  return (
    <ul className="surface-tile divide-y divide-border/50 overflow-hidden">
      {reminders.map((item) => {
        const Icon = icons[item.kind];
        return (
          <li key={item.id} className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
            <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-primary/10 text-primary">
              <Icon className="size-4" />
            </span>
            <div className="min-w-0">
              <p className="truncate text-[14px] font-medium">{item.title}</p>
              <p className="truncate text-[11px] text-muted-foreground">
                {kindLabel[item.kind]} · {item.detail}
              </p>
            </div>
            <div className="shrink-0 text-right">
              <p className="text-[13px] font-semibold tabular-nums">{fmtINR(item.amount)}</p>
              <p className="inline-flex items-center gap-1 text-[11px] text-muted-foreground">
                <CalendarClock className="size-3" />
                {item.due}
              </p>
            </div>
          </li>
        );
      })}
    </ul>
  );
}
