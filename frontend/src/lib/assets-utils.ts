/**
 * Assets module helpers — merges derived (property/investment) and manual assets
 * into allocation and list views without duplicating source-of-truth records.
 */

import { fmtCurrency } from "@/lib/dashboard-data";
import type { ManualAssetDto, ManualAssetTypeDto } from "@/services/assets/types";
import type { Holding } from "@/lib/investments-data";
import type { PropertyListItemView } from "@/services/properties/types";

export type AssetSource = "property" | "investment" | "manual";

export type AllocationCategory =
  | "Real Estate"
  | "Equity"
  | "Mutual Funds"
  | "Gold"
  | "Debt"
  | "Cash"
  | "Vehicles"
  | "Crypto"
  | "Other";

export type UnifiedAsset = {
  id: string;
  source: AssetSource;
  sourceId: string;
  name: string;
  category: AllocationCategory;
  categoryLabel: string;
  currentValue: number;
  purchaseValue: number;
  gainLoss: number;
  gainLossPercent: number | null;
  updatedAt: string | null;
  href?: string;
};

export type AllocationSlice = {
  category: AllocationCategory;
  value: number;
  percent: number;
};

export type AssetActivity = {
  id: string;
  title: string;
  detail: string;
  occurredAt: string;
  amount: number;
};

const MANUAL_TYPE_TO_CATEGORY: Record<number, AllocationCategory> = {
  0: "Gold",
  1: "Cash",
  2: "Cash",
  3: "Debt",
  4: "Vehicles",
  5: "Gold",
  6: "Debt",
  7: "Debt",
  8: "Debt",
  9: "Crypto",
  10: "Other",
  99: "Other",
};

function parseManualType(type: ManualAssetTypeDto): number {
  if (typeof type === "number") return type;
  const map: Record<string, number> = {
    PhysicalGold: 0,
    Cash: 1,
    BankBalance: 2,
    FixedDeposit: 3,
    Vehicle: 4,
    Jewellery: 5,
    Ppf: 6,
    Epf: 7,
    Nps: 8,
    Crypto: 9,
    Collectibles: 10,
    Other: 99,
  };
  return map[type] ?? 99;
}

function holdingCategory(category: Holding["category"]): AllocationCategory {
  switch (category) {
    case "Stocks":
      return "Equity";
    case "Mutual Funds":
      return "Mutual Funds";
    case "Corporate Bonds":
      return "Debt";
    case "Gold ETFs":
      return "Gold";
    case "Cash":
      return "Cash";
    default:
      return "Other";
  }
}

export function mapPropertyAssets(items: PropertyListItemView[]): UnifiedAsset[] {
  return items.map((p) => {
    const gain = p.currentMarketValue - p.purchasePrice;
    const pct = p.purchasePrice > 0 ? (gain / p.purchasePrice) * 100 : null;
    return {
      id: `property:${p.id}`,
      source: "property" as const,
      sourceId: p.id,
      name: p.name,
      category: "Real Estate" as const,
      categoryLabel: "Real Estate",
      currentValue: p.currentMarketValue,
      purchaseValue: p.purchasePrice,
      gainLoss: gain,
      gainLossPercent: pct,
      updatedAt: p.purchaseDate ?? null,
      href: `/properties/${p.id}`,
    };
  });
}

export function mapInvestmentAssets(holdings: Holding[]): UnifiedAsset[] {
  return holdings.map((h) => {
    const gain = h.value - h.invested;
    const pct = h.invested > 0 ? (gain / h.invested) * 100 : null;
    const category = holdingCategory(h.category);
    return {
      id: `investment:${h.id}`,
      source: "investment" as const,
      sourceId: h.id,
      name: h.name,
      category,
      categoryLabel: h.category,
      currentValue: h.value,
      purchaseValue: h.invested,
      gainLoss: gain,
      gainLossPercent: pct,
      updatedAt: null,
      href: "/investments",
    };
  });
}

export function mapManualAssets(items: ManualAssetDto[]): UnifiedAsset[] {
  return items.map((a) => {
    const typeNum = parseManualType(a.type);
    const category = MANUAL_TYPE_TO_CATEGORY[typeNum] ?? "Other";
    return {
      id: `manual:${a.id}`,
      source: "manual" as const,
      sourceId: a.id,
      name: a.name,
      category,
      categoryLabel: a.typeLabel || category,
      currentValue: a.currentValue,
      purchaseValue: a.purchaseValue,
      gainLoss: a.gainLoss,
      gainLossPercent: a.gainLossPercent ?? null,
      updatedAt: a.updatedAt ?? a.createdAt,
    };
  });
}

export function buildAllocation(assets: UnifiedAsset[]): AllocationSlice[] {
  const totals = new Map<AllocationCategory, number>();
  for (const asset of assets) {
    totals.set(asset.category, (totals.get(asset.category) ?? 0) + asset.currentValue);
  }
  const grand = [...totals.values()].reduce((s, v) => s + v, 0);
  const order: AllocationCategory[] = [
    "Real Estate",
    "Equity",
    "Mutual Funds",
    "Gold",
    "Debt",
    "Cash",
    "Vehicles",
    "Crypto",
    "Other",
  ];
  return order
    .filter((c) => (totals.get(c) ?? 0) > 0)
    .map((category) => {
      const value = totals.get(category) ?? 0;
      return {
        category,
        value,
        percent: grand > 0 ? (value / grand) * 100 : 0,
      };
    });
}

export function buildAssetActivity(args: {
  properties: PropertyListItemView[];
  holdings: Holding[];
  manuals: ManualAssetDto[];
}): AssetActivity[] {
  const activities: AssetActivity[] = [];

  for (const p of args.properties) {
    activities.push({
      id: `prop-act-${p.id}`,
      title: "Property tracked",
      detail: p.name,
      occurredAt: p.purchaseDate ? `${p.purchaseDate}T00:00:00Z` : new Date().toISOString(),
      amount: p.currentMarketValue,
    });
  }

  for (const h of args.holdings) {
    activities.push({
      id: `inv-act-${h.id}`,
      title: "Investment holding",
      detail: h.name,
      occurredAt: new Date().toISOString(),
      amount: h.value,
    });
  }

  for (const m of args.manuals) {
    const edited = Boolean(m.updatedAt);
    activities.push({
      id: `man-act-${m.id}`,
      title: edited ? "Manual asset updated" : "Manual asset added",
      detail: m.name,
      occurredAt: m.updatedAt ?? m.createdAt,
      amount: m.currentValue,
    });
  }

  return activities
    .sort((a, b) => new Date(b.occurredAt).getTime() - new Date(a.occurredAt).getTime())
    .slice(0, 12);
}

export function formatAssetMoney(value: number, currencyCode = "INR"): string {
  return fmtCurrency(value, { currencyCode });
}

export function formatRelativeDate(iso: string | null): string {
  if (!iso) return "—";
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) return "—";
  return date.toLocaleDateString("en-IN", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
}
