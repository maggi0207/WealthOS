import { createFileRoute, Link } from "@tanstack/react-router";

import { AccountCards } from "@/components/investments/account-cards";
import { AddInvestmentFab } from "@/components/investments/add-investment-fab";
import { HoldingsList } from "@/components/investments/holdings-list";
import { InvestmentAllocationDonut } from "@/components/investments/investment-allocation";
import { InvestmentInsights } from "@/components/investments/investment-insights";
import { PerformanceChart } from "@/components/investments/performance-chart";
import { PortfolioHero } from "@/components/investments/portfolio-hero";
import { TransactionTimeline } from "@/components/investments/transaction-timeline";
import { UpcomingReminders } from "@/components/investments/upcoming-reminders";
import { DefaultErrorComponent } from "@/components/ui-kit/default-error-component";
import { SectionHeader } from "@/components/ui-kit/section-header";

export const Route = createFileRoute("/_shell/investments")({
  head: () => ({
    meta: [
      { title: "Investments — WealthOS" },
      {
        name: "description",
        content: "Track investment accounts, holdings, allocation, SIPs and performance in one mobile-first view.",
      },
      { property: "og:title", content: "Investments — WealthOS" },
      {
        property: "og:description",
        content: "Track investment accounts, holdings, allocation, SIPs and performance in one mobile-first view.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  errorComponent: DefaultErrorComponent,
  component: InvestmentsPage,
});

function InvestmentsPage() {
  return (
    <div className="space-y-6">
      <h1 className="sr-only">Investments</h1>

      <PortfolioHero />

      <section>
        <SectionHeader title="Accounts" action={<span>Swipe →</span>} />
        <AccountCards />
      </section>

      <section>
        <SectionHeader title="Asset allocation" />
        <InvestmentAllocationDonut />
      </section>

      <section>
        <SectionHeader title="Performance" />
        <PerformanceChart />
      </section>

      <section>
        <SectionHeader title="Holdings" />
        <HoldingsList />
      </section>

      <section>
        <SectionHeader
          title="Upcoming"
          action={
            <Link to="/goals" className="press -my-2 inline-flex min-h-11 items-center">
              View all
            </Link>
          }
        />
        <UpcomingReminders />
      </section>

      <section>
        <SectionHeader
          title="AI insights"
          action={
            <Link to="/ai-advisor" className="press -my-2 inline-flex min-h-11 items-center">
              Ask AI
            </Link>
          }
        />
        <InvestmentInsights />
      </section>

      <section>
        <SectionHeader title="Transactions" />
        <TransactionTimeline />
      </section>

      <AddInvestmentFab />
    </div>
  );
}
