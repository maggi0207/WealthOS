import { createFileRoute } from "@tanstack/react-router";

import { DocumentVault } from "@/components/property/document-vault";
import { EquityCard } from "@/components/property/equity-card";
import { FinancialSummary } from "@/components/property/financial-summary";
import { KeyFacts } from "@/components/property/key-facts";
import { LocationCard } from "@/components/property/location-card";
import { NearbyPlaces } from "@/components/property/nearby-places";
import { PropertyFab } from "@/components/property/property-fab";
import { PropertyGallery } from "@/components/property/property-gallery";
import { PropertyHero } from "@/components/property/property-hero";
import { PropertyInsights } from "@/components/property/property-insights";
import { PropertyLoanCard } from "@/components/property/loan-card";
import { PropertyScoreCard } from "@/components/property/property-score-card";
import { PropertyTimeline } from "@/components/property/property-timeline";
import { PropertyValueChart } from "@/components/property/value-chart";
import { UpkeepCard } from "@/components/property/upkeep-card";
import { SectionHeader } from "@/components/ui-kit/section-header";

export const Route = createFileRoute("/_shell/properties")({
  head: () => ({
    meta: [
      { title: "Ramana Flats, Adyar — Property Passport | WealthOS" },
      {
        name: "description",
        content:
          "Digital property passport for Ramana Flats Door No.3, Anna Avenue, Adyar — photos, value, score, documents, nearby places and timeline.",
      },
      { property: "og:title", content: "Ramana Flats, Adyar — Property Passport | WealthOS" },
      {
        property: "og:description",
        content:
          "Photos, market value, property score, documents, nearby places and timeline for Ramana Flats, Adyar.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: PropertyDetailsPage,
});

function PropertyDetailsPage() {
  return (
    <div className="space-y-6">
      <PropertyHero />

      <KeyFacts />

      <section>
        <SectionHeader title="Financial summary" />
        <FinancialSummary />
      </section>

      <section>
        <SectionHeader title="Location" />
        <LocationCard />
      </section>

      <section>
        <SectionHeader title="Property score" />
        <PropertyScoreCard />
      </section>

      <section>
        <SectionHeader title="AI insights" action={<span>Swipe →</span>} />
        <PropertyInsights />
      </section>

      <section>
        <SectionHeader title="Purchase vs current value" />
        <PropertyValueChart />
      </section>

      <section>
        <SectionHeader title="Ownership & equity" />
        <EquityCard />
      </section>

      <section>
        <SectionHeader title="Home loan" />
        <PropertyLoanCard />
      </section>

      <section>
        <SectionHeader title="Timeline" />
        <PropertyTimeline />
      </section>

      <section>
        <SectionHeader title="Documents" action={<span>6 items</span>} />
        <DocumentVault />
      </section>

      <section>
        <SectionHeader title="Nearby places" />
        <NearbyPlaces />
      </section>

      <section>
        <SectionHeader title="Property gallery" />
        <PropertyGallery />
      </section>

      <section>
        <SectionHeader title="Maintenance & tax" />
        <UpkeepCard />
      </section>

      <PropertyFab />
    </div>
  );
}
