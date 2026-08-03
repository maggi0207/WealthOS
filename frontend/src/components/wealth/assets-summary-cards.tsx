import { fmtCurrency, fmtCompact } from "@/lib/dashboard-data";
import { cn } from "@/lib/utils";

type SummaryProps = {
  totalAssets: number;
  totalLiabilities: number;
  netWorth: number;
  monthlyCashflow: number | null;
  currencyCode: string;
  loading?: boolean;
};

function SummarySkeleton() {
  return (
    <div className="grid grid-cols-2 gap-2.5 lg:grid-cols-4">
      {Array.from({ length: 4 }).map((_, i) => (
        <div key={i} className="surface-tile h-[96px] animate-pulse bg-muted/40" />
      ))}
    </div>
  );
}

function Card({
  label,
  value,
  tone,
}: {
  label: string;
  value: string;
  tone?: "positive" | "negative" | "neutral";
}) {
  return (
    <div className="surface-tile flex min-h-[96px] flex-col justify-between p-3.5 transition-colors hover:bg-muted/20">
      <p className="text-[11px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
        {label}
      </p>
      <p
        className={cn(
          "font-display text-[22px] font-semibold tabular-nums leading-none tracking-tight",
          tone === "positive" && "text-success",
          tone === "negative" && "text-destructive",
        )}
      >
        {value}
      </p>
    </div>
  );
}

/** Section 1 — summary KPI cards for the Assets page. */
export function AssetsSummaryCards({
  totalAssets,
  totalLiabilities,
  netWorth,
  monthlyCashflow,
  currencyCode,
  loading,
}: SummaryProps) {
  if (loading) return <SummarySkeleton />;

  return (
    <div className="grid grid-cols-2 gap-2.5 lg:grid-cols-4">
      <Card label="Total assets" value={fmtCompact(totalAssets, currencyCode)} />
      <Card label="Total liabilities" value={fmtCompact(totalLiabilities, currencyCode)} />
      <Card
        label="Net worth"
        value={fmtCurrency(netWorth, { currencyCode })}
        tone={netWorth >= 0 ? "positive" : "negative"}
      />
      <Card
        label="Monthly cashflow"
        value={
          monthlyCashflow == null
            ? "—"
            : fmtCurrency(monthlyCashflow, { currencyCode })
        }
        tone={
          monthlyCashflow == null
            ? "neutral"
            : monthlyCashflow >= 0
              ? "positive"
              : "negative"
        }
      />
    </div>
  );
}
