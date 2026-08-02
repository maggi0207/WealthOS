import {
  appreciation as mockAppreciation,
  equity as mockEquity,
  equityPct as mockEquityPct,
  homeLoan as mockHomeLoan,
  keyFacts as mockKeyFacts,
  photos as mockPhotos,
  propertyDetail as mockPropertyDetail,
  valueSeries as mockValueSeries,
} from "@/lib/property-data";
import type {
  PropertyDashboardDto,
  PropertyDetailView,
  PropertyDto,
  PropertyFactView,
  PropertyListDto,
  PropertyListItemDto,
  PropertyListItemView,
  PropertyListView,
  PropertyPhotoView,
  PropertySummaryDto,
  PropertySummaryView,
  PropertyTypeDto,
  PropertyStatusDto,
  PropertyValuePointView,
  PropertyValuationDto,
} from "@/services/properties/types";

const PROPERTY_TYPE_LABELS: Record<string, string> = {
  "0": "Residential",
  "1": "Commercial",
  "2": "Land",
  "3": "Residential Apartment",
  "4": "Villa",
  "5": "Plot",
  Residential: "Residential",
  Commercial: "Commercial",
  Land: "Land",
  Apartment: "Residential Apartment",
  Villa: "Villa",
  Plot: "Plot",
};

const PROPERTY_STATUS_LABELS: Record<string, string> = {
  "0": "Active",
  "1": "Under construction",
  "2": "Rented",
  "3": "For sale",
  "4": "Sold",
  "5": "Inactive",
  Active: "Active",
  UnderConstruction: "Under construction",
  Rented: "Rented",
  ForSale: "For sale",
  Sold: "Sold",
  Inactive: "Inactive",
};

function toNumber(value: unknown, fallback = 0): number {
  const n = typeof value === "number" ? value : Number(value);
  return Number.isFinite(n) ? n : fallback;
}

function toOptionalNumber(value: unknown): number | null {
  if (value === null || value === undefined || value === "") return null;
  const n = typeof value === "number" ? value : Number(value);
  return Number.isFinite(n) ? n : null;
}

function enumLabel(
  value: PropertyTypeDto | PropertyStatusDto | string | number | null | undefined,
  table: Record<string, string>,
  fallback: string,
): string {
  if (value === null || value === undefined) return fallback;
  return table[String(value)] ?? fallback;
}

function formatSqFt(value: number | null | undefined): string | null {
  if (value === null || value === undefined) return null;
  return `${Math.round(value).toLocaleString("en-IN")} sq ft`;
}

function formatPurchaseDate(dateOnly: string | null | undefined): {
  label: string;
  year: number | null;
} {
  if (!dateOnly) return { label: "—", year: null };
  const date = new Date(`${dateOnly}T00:00:00`);
  if (Number.isNaN(date.getTime())) {
    const yearMatch = /^(\d{4})/.exec(dateOnly);
    return {
      label: dateOnly,
      year: yearMatch ? Number(yearMatch[1]) : null,
    };
  }
  return {
    label: date.toLocaleDateString("en-IN", {
      day: "numeric",
      month: "short",
      year: "numeric",
    }),
    year: date.getFullYear(),
  };
}

function computeCagrPct(
  purchasePrice: number,
  currentValue: number,
  purchaseYear: number | null,
): number {
  if (!purchaseYear || purchasePrice <= 0 || currentValue <= 0) return 0;
  const years = Math.max(new Date().getFullYear() - purchaseYear, 1);
  const ratio = currentValue / purchasePrice;
  if (ratio <= 0) return 0;
  return Math.round((Math.pow(ratio, 1 / years) - 1) * 1000) / 10;
}

function buildAddressLines(dto: PropertyDto): string[] {
  const address = dto.address;
  if (!address) return [];
  const lines: string[] = [];
  if (address.line1) lines.push(address.line1);
  if (address.line2) lines.push(address.line2);
  const cityLine = [address.locality, address.city]
    .filter(Boolean)
    .join(", ");
  if (cityLine) {
    const withPin = address.postalCode
      ? `${cityLine} – ${address.postalCode}`
      : cityLine;
    lines.push(withPin);
  } else if (address.fullAddress) {
    return address.fullAddress.split(",").map((part) => part.trim());
  }
  return lines;
}

function buildOwnersLabel(dto: PropertyDto): string {
  if (dto.owners?.length) {
    return dto.owners
      .map((owner) => {
        const pct = toNumber(owner.ownershipPercentage);
        const share =
          pct >= 100 || dto.owners.length === 1
            ? "sole owner"
            : `${pct}%`;
        return `${owner.name} (${share})`;
      })
      .join(", ");
  }
  return dto.primaryOwnerName || "—";
}

function buildKeyFacts(dto: PropertyDto): PropertyFactView[] {
  const facts: PropertyFactView[] = [];
  const builtUp = formatSqFt(toOptionalNumber(dto.builtUpArea));
  const area = formatSqFt(toOptionalNumber(dto.area));
  if (builtUp) facts.push({ label: "Built-up", value: builtUp });
  if (area) facts.push({ label: "Area", value: area });
  if (dto.floor) facts.push({ label: "Floor", value: dto.floor });
  if (dto.facing) facts.push({ label: "Facing", value: dto.facing });
  if (dto.bedrooms != null) {
    facts.push({ label: "Bedrooms", value: String(dto.bedrooms) });
  }
  if (dto.bathrooms != null) {
    facts.push({ label: "Bathrooms", value: String(dto.bathrooms) });
  }
  if (dto.parking != null) {
    facts.push({ label: "Parking", value: String(dto.parking) });
  }
  facts.push({
    label: "Type",
    value: enumLabel(dto.type, PROPERTY_TYPE_LABELS, "Property"),
  });
  facts.push({
    label: "Status",
    value: enumLabel(dto.status, PROPERTY_STATUS_LABELS, "Active"),
  });
  return facts;
}

function buildPhotos(dto: PropertyDto): PropertyPhotoView[] {
  const images = [...(dto.images ?? [])].sort(
    (a, b) => toNumber(a.sortOrder) - toNumber(b.sortOrder),
  );
  const mapped = images
    .filter((img) => Boolean(img.url))
    .map((img) => ({
      id: String(img.id),
      url: img.url as string,
      caption: img.caption || dto.name,
      category: img.category || "Exterior",
    }));

  // Preserve gallery UX when the API has no images yet.
  if (mapped.length === 0) {
    return mockPhotos.map((photo) => ({ ...photo }));
  }
  return mapped;
}

function toLakh(value: number): number {
  return Math.round((value / 100_000) * 10) / 10;
}

function buildValueSeries(
  dto: PropertyDto,
  valuations: PropertyValuationDto[],
): PropertyValuePointView[] {
  const purchasePrice = toNumber(dto.purchasePrice);
  const purchaseLakh = toLakh(purchasePrice);
  const sorted = [...valuations].sort((a, b) =>
    String(a.valuationDate).localeCompare(String(b.valuationDate)),
  );

  if (sorted.length === 0) {
    const { year } = formatPurchaseDate(dto.purchaseDate);
    const currentYear = String(new Date().getFullYear());
    return [
      {
        year: year ? String(year) : currentYear,
        purchase: purchaseLakh,
        market: purchaseLakh,
      },
      {
        year: currentYear,
        purchase: purchaseLakh,
        market: toLakh(toNumber(dto.currentMarketValue)),
      },
    ];
  }

  return sorted.map((entry) => {
    const { year } = formatPurchaseDate(entry.valuationDate);
    return {
      year: year ? String(year) : entry.valuationDate.slice(0, 4),
      purchase: purchaseLakh,
      market: toLakh(toNumber(entry.value)),
    };
  });
}

function mapListItem(dto: PropertyListItemDto): PropertyListItemView {
  return {
    id: String(dto.id),
    name: dto.name,
    typeLabel: enumLabel(dto.type, PROPERTY_TYPE_LABELS, "Property"),
    statusLabel: enumLabel(dto.status, PROPERTY_STATUS_LABELS, "Active"),
    primaryOwnerName: dto.primaryOwnerName || "—",
    city: dto.city || "",
    locality: dto.locality || "",
    purchasePrice: toNumber(dto.purchasePrice),
    currentMarketValue: toNumber(dto.currentMarketValue),
    currencyCode: dto.currencyCode || "INR",
    purchaseDate: dto.purchaseDate ?? null,
  };
}

/** Map GET /properties list payload → UI list view. */
export function mapPropertyListResponse(dto: PropertyListDto): PropertyListView {
  return {
    items: (dto.items ?? []).map(mapListItem),
    page: toNumber(dto.page, 1),
    pageSize: toNumber(dto.pageSize, 20),
    totalCount: toNumber(dto.totalCount),
    totalPages: toNumber(dto.totalPages),
  };
}

/** Map GET /properties/summary → UI summary. */
export function mapPropertySummaryResponse(
  dto: PropertySummaryDto,
): PropertySummaryView {
  return {
    propertyCount: toNumber(dto.propertyCount),
    totalPurchasePrice: toNumber(dto.totalPurchasePrice),
    totalMarketValue: toNumber(dto.totalMarketValue),
    totalAppreciation: toNumber(dto.totalAppreciation),
    totalAppreciationPercent:
      dto.totalAppreciationPercent === null ||
      dto.totalAppreciationPercent === undefined
        ? null
        : toNumber(dto.totalAppreciationPercent),
    currencyCode: dto.currencyCode || "INR",
    activeCount: toNumber(dto.activeCount),
    rentedCount: toNumber(dto.rentedCount),
  };
}

function mapPropertyDtoToDetail(
  dto: PropertyDto,
  equityEstimate?: number,
): PropertyDetailView {
  const purchasePrice = toNumber(dto.purchasePrice);
  const currentValue = toNumber(dto.currentMarketValue);
  const appreciationAbsolute =
    dto.appreciation !== undefined && dto.appreciation !== null
      ? toNumber(dto.appreciation)
      : currentValue - purchasePrice;
  const appreciationPct =
    dto.appreciationPercent !== undefined && dto.appreciationPercent !== null
      ? toNumber(dto.appreciationPercent)
      : purchasePrice > 0
        ? (appreciationAbsolute / purchasePrice) * 100
        : 0;

  const { label: purchaseDateLabel, year: purchaseYear } = formatPurchaseDate(
    dto.purchaseDate,
  );
  const primaryOwner =
    dto.owners?.find((o) => o.isPrimary) ?? dto.owners?.[0];
  const ownershipPct = primaryOwner
    ? toNumber(primaryOwner.ownershipPercentage, 100)
    : 100;

  const equity =
    equityEstimate !== undefined ? toNumber(equityEstimate) : currentValue;
  const loanOutstanding = Math.max(currentValue - equity, 0);
  const equityPct =
    currentValue > 0 ? Math.round((equity / currentValue) * 100) : 100;

  const address = dto.address;
  const fullAddress =
    address?.fullAddress ||
    buildAddressLines(dto).join(", ") ||
    dto.description ||
    "";

  return {
    id: String(dto.id),
    name: dto.name,
    doorNumber: dto.floor ? `Floor ${dto.floor}` : "",
    owner: dto.primaryOwnerName || primaryOwner?.name || "—",
    address: fullAddress,
    addressLines: buildAddressLines(dto),
    locality:
      [address?.line2, address?.locality].filter(Boolean).join(", ") ||
      address?.locality ||
      "",
    city: address?.city || "",
    postalCode: address?.postalCode || "",
    state: address?.state || "",
    purchaseYear,
    purchaseDateLabel,
    purchasePrice,
    currentValue,
    ownershipPct,
    ownersLabel: buildOwnersLabel(dto),
    typeLabel: enumLabel(dto.type, PROPERTY_TYPE_LABELS, "Property"),
    statusLabel: enumLabel(dto.status, PROPERTY_STATUS_LABELS, "Active"),
    currencyCode: dto.currencyCode || "INR",
    appreciationAbsolute,
    appreciationPct,
    cagrPct: computeCagrPct(purchasePrice, currentValue, purchaseYear),
    area: toOptionalNumber(dto.area),
    builtUpArea: toOptionalNumber(dto.builtUpArea),
    floor: dto.floor || "",
    facing: dto.facing || "",
    bedrooms: toOptionalNumber(dto.bedrooms),
    bathrooms: toOptionalNumber(dto.bathrooms),
    parking: toOptionalNumber(dto.parking),
    description: dto.description || "",
    isRentalEnabled: Boolean(dto.isRentalEnabled),
    googleMapsUrl: address?.googleMapsUrl ?? null,
    latitude: toOptionalNumber(address?.latitude),
    longitude: toOptionalNumber(address?.longitude),
    keyFacts: buildKeyFacts(dto),
    photos: buildPhotos(dto),
    valueSeries: buildValueSeries(dto, dto.valuations ?? []),
    equity,
    equityPct,
    loanOutstanding,
    valuationCount: dto.valuations?.length ?? 0,
    loanLinkCount: dto.loanLinks?.length ?? 0,
    documentLinkCount: dto.documentLinks?.length ?? 0,
  };
}

/** Map GET /properties/{id} → passport detail view. */
export function mapPropertyResponse(dto: PropertyDto): PropertyDetailView {
  return mapPropertyDtoToDetail(dto);
}

/**
 * Map GET /properties/{id}/dashboard → passport detail view.
 * Used by the Property details page for equity + related counts.
 */
export function mapPropertyDashboardResponse(
  dto: PropertyDashboardDto,
): PropertyDetailView {
  const detail = mapPropertyDtoToDetail(
    dto.property,
    toNumber(dto.equityEstimate),
  );
  return {
    ...detail,
    appreciationAbsolute:
      dto.appreciation !== undefined
        ? toNumber(dto.appreciation)
        : detail.appreciationAbsolute,
    appreciationPct:
      dto.appreciationPercent !== undefined && dto.appreciationPercent !== null
        ? toNumber(dto.appreciationPercent)
        : detail.appreciationPct,
    valuationCount: toNumber(dto.valuationCount, detail.valuationCount),
    loanLinkCount: toNumber(dto.loanLinkCount, detail.loanLinkCount),
    documentLinkCount: toNumber(
      dto.documentLinkCount,
      detail.documentLinkCount,
    ),
  };
}

/** Build list view from local fixtures (VITE_API_MODE=mock). */
export function mapMockPropertyList(): PropertyListView {
  const detail = mapMockPropertyDetail();
  return {
    items: [
      {
        id: detail.id,
        name: detail.name,
        typeLabel: detail.typeLabel,
        statusLabel: detail.statusLabel,
        primaryOwnerName: detail.owner,
        city: detail.city,
        locality: detail.locality,
        purchasePrice: detail.purchasePrice,
        currentMarketValue: detail.currentValue,
        currencyCode: detail.currencyCode,
        purchaseDate: null,
      },
    ],
    page: 1,
    pageSize: 20,
    totalCount: 1,
    totalPages: 1,
  };
}

/** Build summary view from local fixtures (VITE_API_MODE=mock). */
export function mapMockPropertySummary(): PropertySummaryView {
  const detail = mapMockPropertyDetail();
  return {
    propertyCount: 1,
    totalPurchasePrice: detail.purchasePrice,
    totalMarketValue: detail.currentValue,
    totalAppreciation: detail.appreciationAbsolute,
    totalAppreciationPercent: detail.appreciationPct,
    currencyCode: "INR",
    activeCount: 1,
    rentedCount: 0,
  };
}

/** Build passport detail from `@/lib/property-data` (VITE_API_MODE=mock). */
export function mapMockPropertyDetail(): PropertyDetailView {
  return {
    id: mockPropertyDetail.id,
    name: mockPropertyDetail.name,
    doorNumber: mockPropertyDetail.doorNumber,
    owner: mockPropertyDetail.owner,
    address: mockPropertyDetail.address,
    addressLines: [...mockPropertyDetail.addressLines],
    locality: mockPropertyDetail.locality,
    city: mockPropertyDetail.city,
    postalCode: "600020",
    state: "Tamil Nadu",
    purchaseYear: mockPropertyDetail.purchaseYear,
    purchaseDateLabel: mockPropertyDetail.purchaseDate,
    purchasePrice: mockPropertyDetail.purchasePrice,
    currentValue: mockPropertyDetail.currentValue,
    ownershipPct: mockPropertyDetail.ownershipPct,
    ownersLabel: mockPropertyDetail.owners,
    typeLabel: mockPropertyDetail.type,
    statusLabel: "Active",
    currencyCode: "INR",
    appreciationAbsolute: mockAppreciation.absolute,
    appreciationPct: mockAppreciation.pct,
    cagrPct: mockAppreciation.cagrPct,
    area: 795,
    builtUpArea: 970,
    floor: "Ground Floor",
    facing: "",
    bedrooms: null,
    bathrooms: null,
    parking: null,
    description: mockPropertyDetail.address,
    isRentalEnabled: true,
    googleMapsUrl: null,
    latitude: null,
    longitude: null,
    keyFacts: mockKeyFacts.map((fact) => ({ ...fact })),
    photos: mockPhotos.map((photo) => ({ ...photo })),
    valueSeries: mockValueSeries.map((point) => ({ ...point })),
    equity: mockEquity,
    equityPct: mockEquityPct,
    loanOutstanding: mockHomeLoan.outstanding,
    valuationCount: mockValueSeries.length,
    loanLinkCount: 1,
    documentLinkCount: 6,
  };
}

/** Mock dashboard = same as mock detail (equity already included). */
export function mapMockPropertyDashboard(): PropertyDetailView {
  return mapMockPropertyDetail();
}
