import { CalendarClock, Receipt, Wrench } from "lucide-react";

import { fmtINR, upkeep } from "@/lib/property-data";

/** Maintenance and property tax. */
export function UpkeepCard() {
  return (
    <div className="grid gap-3 sm:grid-cols-2">
      <section className="surface-tile p-4">
        <p className="flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
          <Wrench className="size-3.5" />
          Maintenance
        </p>
        <p className="mt-1.5 font-display text-xl font-semibold tabular-nums">
          {fmtINR(upkeep.maintenanceMonthly)}
          <span className="ml-1 text-[12px] font-medium text-muted-foreground">/ month</span>
        </p>
        <p className="mt-1 text-[12px] text-muted-foreground">Paid till {upkeep.maintenancePaidTill}</p>
        <p className="mt-2 border-t border-border/70 pt-2 text-[12px] text-muted-foreground">
          {upkeep.lastServiced}
        </p>
      </section>

      <section className="surface-tile p-4">
        <p className="flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
          <Receipt className="size-3.5" />
          Property tax
        </p>
        <p className="mt-1.5 font-display text-xl font-semibold tabular-nums">
          {fmtINR(upkeep.propertyTaxHalfYear)}
          <span className="ml-1 text-[12px] font-medium text-muted-foreground">/ half year</span>
        </p>
        <p className="mt-1 inline-flex items-center gap-1 text-[12px] font-medium text-warning">
          <CalendarClock className="size-3.5" />
          Due {upkeep.propertyTaxDue}
        </p>
        <p className="mt-2 border-t border-border/70 pt-2 text-[12px] text-muted-foreground">
          Annual upkeep {fmtINR(upkeep.annualUpkeep)}
        </p>
      </section>
    </div>
  );
}
