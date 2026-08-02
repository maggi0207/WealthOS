import { TileSkeleton } from "@/components/ui-kit/skeletons";
import { usePrimaryProperty } from "@/hooks/api/use-properties";
import { fmtINR, fmtINRShort, rental } from "@/lib/property-data";

/** Financial summary grid. */
export function FinancialSummary() {
  const { data, isPending, isError, refetch, isFetching } = usePrimaryProperty();

  if (isPending) {
    return <TileSkeleton className="h-28" />;
  }

  if (isError || !data) {
    return (
      <section className="surface-tile px-4 py-5 text-center">
        <p className="text-sm font-medium">Unable to load financial summary</p>
        <button
          type="button"
          onClick={() => void refetch()}
          disabled={isFetching}
          className="press mt-3 inline-flex min-h-11 items-center rounded-full bg-primary px-4 text-xs font-semibold text-primary-foreground"
        >
          Retry
        </button>
      </section>
    );
  }

  const stats = [
    { label: "Current value", value: fmtINR(data.currentValue), tone: "" },
    { label: "Purchase price", value: fmtINR(data.purchasePrice), tone: "" },
    {
      label: "Appreciation",
      value: `${fmtINRShort(data.appreciationAbsolute)} · ${data.appreciationPct.toFixed(0)}%`,
      tone: "text-success",
    },
    {
      label: "Rental yield",
      value: `${rental.yieldPct}% · ${fmtINR(rental.monthlyRent)}/mo`,
      tone: "",
    },
  ];

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
