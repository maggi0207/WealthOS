import { createFileRoute } from "@tanstack/react-router";
import { useMemo } from "react";

import { SectionHeader } from "@/components/ui-kit/section-header";
import { AddAssetFab } from "@/components/wealth/add-asset-fab";
import { AssetsActivity } from "@/components/wealth/assets-activity";
import { AssetsAllocation } from "@/components/wealth/assets-allocation";
import { AssetsList } from "@/components/wealth/assets-list";
import { AssetsSummaryCards } from "@/components/wealth/assets-summary-cards";
import { useDashboard } from "@/hooks/api/use-dashboard";
import { useIncomeOverview } from "@/hooks/api/use-income";
import { useInvestmentsOverview } from "@/hooks/api/use-investments";
import { useLoanSummary } from "@/hooks/api/use-loans";
import { useManualAssets } from "@/hooks/api/use-manual-assets";
import { useProperties } from "@/hooks/api/use-properties";
import {
  buildAllocation,
  buildAssetActivity,
  mapInvestmentAssets,
  mapManualAssets,
  mapPropertyAssets,
} from "@/lib/assets-utils";

export const Route = createFileRoute("/_shell/assets")({
  head: () => ({
    meta: [
      { title: "Assets — WealthOS" },
      {
        name: "description",
        content:
          "Central wealth summary — derived property and investment assets plus manual holdings.",
      },
      { property: "og:title", content: "Assets — WealthOS" },
      {
        property: "og:description",
        content:
          "Central wealth summary — derived property and investment assets plus manual holdings.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: AssetsPage,
});

function AssetsPage() {
  const dashboard = useDashboard();
  const properties = useProperties({ page: 1, pageSize: 100 });
  const investments = useInvestmentsOverview();
  const loans = useLoanSummary();
  const manuals = useManualAssets({ page: 1, pageSize: 100 });
  const income = useIncomeOverview();

  const loading =
    dashboard.isLoading ||
    properties.isLoading ||
    investments.isLoading ||
    manuals.isLoading ||
    loans.isLoading;

  const error =
    dashboard.isError ||
    properties.isError ||
    investments.isError ||
    manuals.isError ||
    loans.isError;

  const currencyCode = dashboard.data?.currencyCode ?? "INR";

  const unified = useMemo(() => {
    const propertyAssets = mapPropertyAssets(properties.data?.items ?? []);
    const investmentAssets = mapInvestmentAssets(investments.data?.holdings ?? []);
    const manualAssets = mapManualAssets(manuals.data?.items ?? []);
    return [...propertyAssets, ...investmentAssets, ...manualAssets].sort(
      (a, b) => b.currentValue - a.currentValue,
    );
  }, [properties.data?.items, investments.data?.holdings, manuals.data?.items]);

  const allocation = useMemo(() => buildAllocation(unified), [unified]);

  const activities = useMemo(
    () =>
      buildAssetActivity({
        properties: properties.data?.items ?? [],
        holdings: investments.data?.holdings ?? [],
        manuals: manuals.data?.items ?? [],
      }),
    [properties.data?.items, investments.data?.holdings, manuals.data?.items],
  );

  const totalAssets =
    dashboard.data?.assetValue ??
    unified.reduce((sum, asset) => sum + asset.currentValue, 0);
  const totalLiabilities =
    dashboard.data?.liabilityValue ?? loans.data?.outstandingBalance ?? 0;
  const netWorth = dashboard.data?.netWorth ?? totalAssets - totalLiabilities;

  const monthlyCashflow = (() => {
    if (dashboard.data) {
      return dashboard.data.monthlyIncome - dashboard.data.monthlyExpense;
    }
    const cf = income.data?.cashFlow;
    if (!cf) return null;
    return cf.totalIncome - (cf.personalOutflow ?? 0);
  })();

  return (
    <div className="space-y-6">
      <h1 className="sr-only">Assets</h1>

      {error ? (
        <div
          role="alert"
          className="rounded-2xl border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm text-destructive"
        >
          Unable to load your wealth summary. Refresh and try again.
        </div>
      ) : null}

      <section>
        <SectionHeader title="Summary" />
        <AssetsSummaryCards
          totalAssets={totalAssets}
          totalLiabilities={totalLiabilities}
          netWorth={netWorth}
          monthlyCashflow={monthlyCashflow}
          currencyCode={currencyCode}
          loading={loading}
        />
      </section>

      <section>
        <SectionHeader title="Asset allocation" />
        <AssetsAllocation slices={allocation} currencyCode={currencyCode} loading={loading} />
      </section>

      <section>
        <SectionHeader title="All assets" action={<span>{unified.length} items</span>} />
        <AssetsList assets={unified} currencyCode={currencyCode} loading={loading} />
      </section>

      <section>
        <SectionHeader title="Recent activity" />
        <AssetsActivity activities={activities} currencyCode={currencyCode} loading={loading} />
      </section>

      <AddAssetFab />
    </div>
  );
}
