import { ArrowDownRight, ArrowUpRight } from "lucide-react";

import { HeroSkeleton } from "@/components/ui-kit/skeletons";
import { useInvestmentsOverview } from "@/hooks/api/use-investments";
import { useCountUp } from "@/hooks/use-count-up";
import { fmtINR, fmtINRShort, fmtPctSigned } from "@/lib/investments-data";
import { cn } from "@/lib/utils";

/** Portfolio summary hero — invested, current value, day move, return and XIRR. */
export function PortfolioHero() {
  const { data, isPending, isError, refetch, isFetching } = useInvestmentsOverview();
  const p = data?.portfolio;
  const animated = useCountUp(p?.current ?? 0, 1100);
  const up = (p?.todayChange ?? 0) >= 0;

  if (isPending) return <HeroSkeleton className="min-h-[14rem]" />;

  if (isError || !p) {
    return (
      <section className="surface-hero overflow-hidden p-4 sm:p-5">
        <p className="text-sm font-medium">Unable to load portfolio</p>
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

  return (
    <section className="surface-hero overflow-hidden">
      <div className="px-4 pt-4 sm:px-5 sm:pt-5">
        <p className="text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
          Current value
        </p>
        <p className="mt-1 truncate font-display text-[1.75rem] font-semibold leading-none tabular-nums sm:text-4xl">
          {fmtINR(Math.round(animated))}
        </p>
        <p className="mt-2 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs">
          <span
            className={cn(
              "inline-flex items-center gap-0.5 rounded-full px-2 py-0.5 font-semibold tabular-nums",
              up ? "bg-success/12 text-success" : "bg-destructive/12 text-destructive",
            )}
          >
            {up ? <ArrowUpRight className="size-3.5" /> : <ArrowDownRight className="size-3.5" />}
            {fmtINR(Math.abs(p.todayChange))} ({Math.abs(p.todayChangePct)}%)
          </span>
          <span className="text-muted-foreground">today</span>
        </p>
      </div>

      <dl className="mt-4 grid grid-cols-2 divide-x divide-y divide-border/70 border-t border-border/70">
        <Cell label="Total invested" value={fmtINRShort(p.invested)} />
        <Cell
          label="Overall return"
          value={fmtINRShort(p.overallReturn)}
          sub={fmtPctSigned(p.overallReturnPct)}
          tone="positive"
        />
        <Cell label="XIRR" value={`${p.xirr}%`} sub="Placeholder" />
        <Cell label="Accounts" value={String(data.accounts.length)} sub="connected + manual" />
      </dl>
    </section>
  );
}

function Cell({
  label,
  value,
  sub,
  tone,
}: {
  label: string;
  value: string;
  sub?: string;
  tone?: "positive";
}) {
  return (
    <div className="px-4 py-3 sm:px-5">
      <dt className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
        {label}
      </dt>
      <dd
        className={cn(
          "mt-1 font-display text-[1.05rem] font-semibold tabular-nums sm:text-lg",
          tone === "positive" && "text-success",
        )}
      >
        {value}
      </dd>
      {sub ? <p className="mt-0.5 text-[11px] text-muted-foreground">{sub}</p> : null}
    </div>
  );
}
