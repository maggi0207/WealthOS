import { Search, SearchX } from "lucide-react";
import { useEffect, useMemo, useState } from "react";

import { EmptyState } from "@/components/ui-kit/empty-state";
import { ListSkeleton } from "@/components/ui-kit/skeletons";
import { Input } from "@/components/ui/input";
import {
  accounts,
  fmtINR,
  fmtINRShort,
  holdingCategories,
  holdings,
  type HoldingCategory,
} from "@/lib/investments-data";
import { cn } from "@/lib/utils";

const accountName = (id: string) => {
  const a = accounts.find((x) => x.id === id);
  return a ? `${a.name} · ${a.owner}` : "Manual";
};

/** Searchable, filterable holdings list — card rows on mobile, table-like on desktop. */
export function HoldingsList() {
  const [query, setQuery] = useState("");
  const [category, setCategory] = useState<HoldingCategory>("All");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const id = window.setTimeout(() => setLoading(false), 520);
    return () => window.clearTimeout(id);
  }, []);

  const rows = useMemo(() => {
    const q = query.trim().toLowerCase();
    return holdings.filter(
      (h) =>
        (category === "All" || h.category === category) &&
        (q === "" || h.name.toLowerCase().includes(q) || h.ticker.toLowerCase().includes(q)),
    );
  }, [query, category]);

  return (
    <section className="space-y-3">
      <div className="relative">
        <Search className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search holdings or tickers"
          aria-label="Search holdings"
          className="h-11 rounded-xl pl-9"
        />
      </div>

      <div className="no-scrollbar bleed-gutter page-gutter flex gap-2 overflow-x-auto pb-0.5 sm:mx-0 sm:flex-wrap sm:px-0">
        {holdingCategories.map((c) => (
          <button
            key={c}
            type="button"
            onClick={() => setCategory(c)}
            aria-pressed={category === c}
            className={cn(
              "press min-h-11 shrink-0 rounded-full border px-3.5 text-[12px] font-semibold transition-colors",
              category === c
                ? "border-primary bg-primary text-primary-foreground"
                : "border-border bg-secondary/50 text-muted-foreground",
            )}
          >
            {c}
          </button>
        ))}
      </div>

      {loading ? (
        <ListSkeleton rows={5} />
      ) : rows.length === 0 ? (
        <EmptyState
          icon={SearchX}
          title="No holdings match"
          description="Try a different ticker, or clear the filters to see all investments."
          action={
            <button
              type="button"
              onClick={() => {
                setQuery("");
                setCategory("All");
              }}
              className="press inline-flex min-h-11 items-center rounded-full bg-primary px-5 text-sm font-semibold text-primary-foreground"
            >
              Clear filters
            </button>
          }
        />
      ) : (
        <ul className="surface-tile divide-y divide-border/50 overflow-hidden">
          {rows.map((h) => {
            const up = h.dayChange >= 0;
            const gain = h.value - h.invested;
            const gainPct = (gain / h.invested) * 100;
            return (
              <li key={h.id} className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
                <div className="min-w-0">
                  <p className="truncate text-[14px] font-medium">{h.name}</p>
                  <p className="truncate text-[11px] text-muted-foreground">
                    {h.ticker} · {accountName(h.accountId)}
                  </p>
                  <p className={cn("mt-0.5 text-[11px] font-medium tabular-nums", gain >= 0 ? "text-success" : "text-destructive")}>
                    {gain >= 0 ? "+" : "−"}
                    {fmtINRShort(Math.abs(gain))} ({gainPct.toFixed(1)}%) overall
                  </p>
                </div>
                <div className="shrink-0 text-right">
                  <p className="text-[14px] font-semibold tabular-nums">{fmtINRShort(h.value)}</p>
                  <p className={cn("text-[11px] font-medium tabular-nums", up ? "text-success" : "text-destructive")}>
                    {up ? "▲" : "▼"} {fmtINR(Math.abs(h.dayChange))}
                  </p>
                  <p className={cn("text-[10px] tabular-nums", up ? "text-success" : "text-destructive")}>
                    {up ? "+" : ""}
                    {h.dayChangePct.toFixed(2)}% today
                  </p>
                </div>
              </li>
            );
          })}
        </ul>
      )}
    </section>
  );
}
