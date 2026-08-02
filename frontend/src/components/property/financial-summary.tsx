import { appreciation, fmtINR, fmtINRShort, propertyDetail, rental } from "@/lib/property-data";

const stats = [
  { label: "Current value", value: fmtINR(propertyDetail.currentValue), tone: "" },
  { label: "Purchase price", value: fmtINR(propertyDetail.purchasePrice), tone: "" },
  {
    label: "Appreciation",
    value: `${fmtINRShort(appreciation.absolute)} · ${appreciation.pct.toFixed(0)}%`,
    tone: "text-success",
  },
  { label: "Rental yield", value: `${rental.yieldPct}% · ${fmtINR(rental.monthlyRent)}/mo`, tone: "" },
];

/** Financial summary grid. */
export function FinancialSummary() {
  return (
    <section className="surface-tile grid grid-cols-2 divide-x divide-y divide-border/70 overflow-hidden">
      {stats.map((stat) => (
        <div key={stat.label} className="px-4 py-3">
          <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
            {stat.label}
          </p>
          <p className={`mt-1 font-display text-[15px] font-semibold tabular-nums ${stat.tone}`}>
            {stat.value}
          </p>
        </div>
      ))}
    </section>
  );
}
