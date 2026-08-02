import { createFileRoute, Link } from "@tanstack/react-router";

import { BusinessExpenses } from "@/components/business/business-expenses";
import { BusinessInsights } from "@/components/business/business-insights";
import { BusinessQuickActions } from "@/components/business/business-quick-actions";
import { CashFlowHero } from "@/components/business/cashflow-hero";
import { ClientCards } from "@/components/business/client-cards";
import { DeveloperPayroll } from "@/components/business/developer-payroll";
import { IncomeTrendChart } from "@/components/business/income-trend-chart";
import { PnlSummary } from "@/components/business/pnl-summary";
import { SalaryCards } from "@/components/business/salary-cards";
import { DefaultErrorComponent } from "@/components/ui-kit/default-error-component";
import { SectionHeader } from "@/components/ui-kit/section-header";
import { useIncomeOverview } from "@/hooks/api/use-income";
import { fmtINR } from "@/lib/business-data";

const description =
  "Track salary, client revenue, developer payroll, business expenses and monthly profit in one mobile-first view.";

export const Route = createFileRoute("/_shell/income")({
  head: () => ({
    meta: [
      { title: "Income & Business — WealthOS" },
      { name: "description", content: description },
      { property: "og:title", content: "Income & Business — WealthOS" },
      { property: "og:description", content: description },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  errorComponent: DefaultErrorComponent,
  component: IncomeBusinessPage,
});

function IncomeBusinessPage() {
  const { data } = useIncomeOverview();
  const periodLabel = data?.cashFlow.periodLabel ?? "";
  const outstanding = data?.totalOutstanding ?? 0;

  return (
    <div className="space-y-6">
      <h1 className="sr-only">Income & Business</h1>

      <CashFlowHero />

      <section>
        <SectionHeader title="Quick actions" />
        <BusinessQuickActions />
      </section>

      <section>
        <SectionHeader title="Salary" action={periodLabel ? <span>{periodLabel}</span> : undefined} />
        <SalaryCards />
      </section>

      <section>
        <SectionHeader
          title="Clients"
          action={<span className="tabular-nums">{fmtINR(outstanding)} due</span>}
        />
        <ClientCards />
      </section>

      <section>
        <SectionHeader title="Developer payroll" />
        <DeveloperPayroll />
      </section>

      <section>
        <SectionHeader title="Business expenses" />
        <BusinessExpenses />
      </section>

      <section>
        <SectionHeader title="Profit & loss" action={periodLabel ? <span>{periodLabel}</span> : undefined} />
        <PnlSummary />
      </section>

      <section>
        <SectionHeader title="Income trend" />
        <IncomeTrendChart />
      </section>

      <section>
        <SectionHeader
          title="AI business insights"
          action={
            <Link to="/ai-advisor" className="press -my-2 inline-flex min-h-11 items-center">
              Ask AI
            </Link>
          }
        />
        <BusinessInsights />
      </section>
    </div>
  );
}
