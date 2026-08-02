import { createFileRoute } from "@tanstack/react-router";
import { Download, PieChart } from "lucide-react";
import { toast } from "sonner";

import { DefaultErrorComponent } from "@/components/ui-kit/default-error-component";
import { SectionHeader } from "@/components/ui-kit/section-header";
import { HeroSkeleton, ListSkeleton } from "@/components/ui-kit/skeletons";
import { useReportExport, useReportSummary } from "@/hooks/api/use-reports";
import { fmtINR } from "@/lib/wealth-data";

export const Route = createFileRoute("/_shell/reports")({
  head: () => ({
    meta: [
      { title: "Reports — WealthOS" },
      { name: "description", content: "Custom financial reports and exports." },
      { property: "og:title", content: "Reports — WealthOS" },
      { property: "og:description", content: "Custom financial reports and exports." },
    ],
  }),
  errorComponent: DefaultErrorComponent,
  component: ReportsPage,
});

function ReportsPage() {
  const { data, isPending, isError, refetch, isFetching } = useReportSummary();
  const exportMutation = useReportExport();

  return (
    <div className="space-y-6">
      <h1 className="sr-only">Reports</h1>

      {isPending ? (
        <HeroSkeleton className="min-h-[10rem]" />
      ) : isError || !data ? (
        <section className="surface-hero p-4 sm:p-5">
          <p className="text-sm font-medium">Unable to load reports</p>
          <button
            type="button"
            onClick={() => void refetch()}
            disabled={isFetching}
            className="press mt-3 inline-flex min-h-11 items-center rounded-full bg-primary px-4 text-xs font-semibold text-primary-foreground"
          >
            Retry
          </button>
        </section>
      ) : (
        <section className="surface-hero p-4 sm:p-5">
          <div className="grid grid-cols-[minmax(0,1fr)_auto] items-start gap-3">
            <div className="min-w-0">
              <p className="text-[10px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
                {data.title}
              </p>
              <p className="mt-1 font-display text-fluid-2xl font-semibold tabular-nums">
                {fmtINR(data.netWorth)}
              </p>
              <p className="mt-1 text-[12px] text-muted-foreground">
                Generated{" "}
                {new Date(data.generatedAt).toLocaleString("en-IN", {
                  day: "numeric",
                  month: "short",
                  hour: "2-digit",
                  minute: "2-digit",
                })}
              </p>
            </div>
            <span className="grid size-11 shrink-0 place-items-center rounded-xl bg-primary/10 text-primary">
              <PieChart className="size-5" />
            </span>
          </div>
        </section>
      )}

      <section>
        <SectionHeader
          title="Key figures"
          action={
            <button
              type="button"
              className="press inline-flex min-h-11 items-center gap-1.5 text-[13px] font-semibold"
              onClick={() =>
                void exportMutation
                  .mutateAsync("pdf")
                  .then(() => toast.success("Export queued"))
                  .catch(() => toast.error("Export failed"))
              }
            >
              <Download className="size-4" />
              Export
            </button>
          }
        />
        {isPending || !data ? (
          <ListSkeleton rows={4} />
        ) : (
          <ul className="surface-tile divide-y divide-border/50 overflow-hidden">
            {data.sections.map((section) => (
              <li
                key={section.key}
                className="flex items-center justify-between gap-3 px-4 py-3"
              >
                <span className="text-[13px] text-muted-foreground">{section.label}</span>
                <span className="text-[14px] font-semibold tabular-nums">
                  {fmtINR(section.value)}
                </span>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}
