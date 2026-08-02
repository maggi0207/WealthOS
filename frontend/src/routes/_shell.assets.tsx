import { createFileRoute, Link } from "@tanstack/react-router";

import { SectionHeader } from "@/components/ui-kit/section-header";
import { AddAssetFab } from "@/components/wealth/add-asset-fab";
import { AllocationDonut } from "@/components/wealth/allocation-donut";
import { AssetCardRail } from "@/components/wealth/asset-card-rail";
import { InvestmentCards } from "@/components/wealth/investment-cards";
import { LoanSummaryCard } from "@/components/wealth/loan-summary-card";
import { NetWorthHero } from "@/components/wealth/net-worth-hero";
import { PropertySummaryCard } from "@/components/wealth/property-summary-card";

export const Route = createFileRoute("/_shell/assets")({
  head: () => ({
    meta: [
      { title: "Wealth — WealthOS" },
      {
        name: "description",
        content: "Net worth, asset allocation, property, loans and investments in one mobile-first view.",
      },
      { property: "og:title", content: "Wealth — WealthOS" },
      {
        property: "og:description",
        content: "Net worth, asset allocation, property, loans and investments in one mobile-first view.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: WealthPage,
});

function WealthPage() {
  return (
    <div className="space-y-6">
      <h1 className="sr-only">Wealth</h1>

      <NetWorthHero />

      <section>
        <SectionHeader title="Allocation" />
        <AllocationDonut />
      </section>

      <section>
        <SectionHeader title="Your assets" action={<span>Swipe →</span>} />
        <AssetCardRail />
      </section>

      <section>
        <SectionHeader
          title="Property"
          action={
            <Link to="/properties" className="press -my-2 inline-flex min-h-11 items-center">
              View all
            </Link>
          }
        />
        <PropertySummaryCard />
      </section>

      <section>
        <SectionHeader
          title="Loans"
          action={
            <Link to="/loans" className="press -my-2 inline-flex min-h-11 items-center">
              View all
            </Link>
          }
        />
        <LoanSummaryCard />
      </section>

      <section>
        <SectionHeader
          title="Investments"
          action={
            <Link to="/investments" className="press -my-2 inline-flex min-h-11 items-center">
              View all
            </Link>
          }
        />
        <InvestmentCards />
      </section>

      <AddAssetFab />
    </div>
  );
}
