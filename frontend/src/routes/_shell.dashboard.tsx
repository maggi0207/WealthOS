import { createFileRoute } from "@tanstack/react-router";
import { Activity, Banknote, Receipt } from "lucide-react";

import { GreetingHeader } from "@/components/home/greeting-header";
import { HealthScoreHero } from "@/components/home/health-score-hero";
import { NetWorthCard } from "@/components/home/net-worth-card";
import { AiRecommendationRail } from "@/components/home/ai-recommendation-rail";
import { QuickActions } from "@/components/home/quick-actions";
import { UpcomingList } from "@/components/home/upcoming-list";
import { ActivityList } from "@/components/home/activity-list";
import { CompactStat } from "@/components/ui-kit/compact-stat";
import { SectionHeader } from "@/components/ui-kit/section-header";
import { NetWorthTrendChart } from "@/components/dashboard/net-worth-trend-chart";
import { AssetAllocationChart } from "@/components/dashboard/asset-allocation-chart";
import { IncomeVsExpensesChart } from "@/components/dashboard/income-expenses-chart";
import { LoanBreakdownChart } from "@/components/dashboard/loan-breakdown-chart";
import { fmtCurrency, kpis } from "@/lib/dashboard-data";

export const Route = createFileRoute("/_shell/dashboard")({
  head: () => ({
    meta: [
      { title: "Home — WealthOS" },
      {
        name: "description",
        content:
          "Your health score, net worth, AI recommendations, upcoming payments and recent activity in one mobile-first view.",
      },
      { property: "og:title", content: "Home — WealthOS" },
      {
        property: "og:description",
        content: "Health score, net worth, AI recommendations and upcoming payments at a glance.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: HomePage,
});

function HomePage() {
  return (
    <div className="space-y-6">
      <GreetingHeader />

      <HealthScoreHero />

      <NetWorthCard />

      <section>
        <SectionHeader title="AI recommendations" />
        <AiRecommendationRail />
      </section>

      <section>
        <SectionHeader title="Quick actions" />
        <QuickActions />
      </section>

      <section>
        <SectionHeader title="This month" />
        <div className="grid grid-cols-3 gap-2.5">
          <CompactStat
            label="Income"
            value={fmtCurrency(kpis.monthlyIncome.value)}
            delta={kpis.monthlyIncome.changePct}
            icon={Banknote}
            tone="positive"
          />
          <CompactStat
            label="Expenses"
            value={fmtCurrency(kpis.monthlyExpenses.value)}
            delta={kpis.monthlyExpenses.changePct}
            icon={Receipt}
            tone="negative"
          />
          <CompactStat
            label="Cash flow"
            value={fmtCurrency(kpis.cashFlow.value)}
            delta={kpis.cashFlow.changePct}
            icon={Activity}
            tone="positive"
          />
        </div>
      </section>

      <UpcomingList />

      <ActivityList />

      <section>
        <SectionHeader title="Insights" />
        <div className="grid gap-3 sm:gap-4 xl:grid-cols-2">
          <NetWorthTrendChart />
          <AssetAllocationChart />
          <IncomeVsExpensesChart />
          <LoanBreakdownChart />
        </div>
      </section>
    </div>
  );
}
