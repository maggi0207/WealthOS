/**
 * Property API DTOs (camelCase, matching ASP.NET serialization)
 * and UI view models consumed by the Property passport page.
 */

/* ---------------------------------- Enums --------------------------------- */

export type PropertyTypeDto =
  | 0
  | 1
  | 2
  | 3
  | 4
  | 5
  | "Residential"
  | "Commercial"
  | "Land"
  | "Apartment"
  | "Villa"
  | "Plot";

export type PropertyStatusDto =
  | 0
  | 1
  | 2
  | 3
  | 4
  | 5
  | "Active"
  | "UnderConstruction"
  | "Rented"
  | "ForSale"
  | "Sold"
  | "Inactive";

export type OwnershipTypeDto =
  | 0
  | 1
  | 2
  | 3
  | 4
  | "Sole"
  | "Joint"
  | "Inherited"
  | "Trust"
  | "Company";

/* ---------------------------------- DTOs ---------------------------------- */

export type PropertyAddressDto = {
  id: string;
  line1?: string | null;
  line2?: string | null;
  locality?: string | null;
  city?: string | null;
  state?: string | null;
  postalCode?: string | null;
  country?: string | null;
  fullAddress?: string | null;
  latitude?: number | null;
  longitude?: number | null;
  googleMapsUrl?: string | null;
};

export type PropertyOwnerDto = {
  id: string;
  name: string;
  ownershipPercentage: number;
  ownershipType: OwnershipTypeDto;
  isPrimary: boolean;
  linkedUserId?: string | null;
};

export type PropertyValuationDto = {
  id: string;
  valuationDate: string;
  value: number;
  currencyCode: string;
  source?: string | null;
  notes?: string | null;
};

export type PropertyLoanLinkDto = {
  id: string;
  loanId: string;
  notes?: string | null;
};

export type PropertyDocumentLinkDto = {
  id: string;
  documentId: string;
  notes?: string | null;
};

export type PropertyImageDto = {
  id: string;
  url?: string | null;
  caption?: string | null;
  category?: string | null;
  sortOrder: number;
  isPrimary: boolean;
};

export type PropertyNoteDto = {
  id: string;
  title: string;
  body: string;
};

export type PropertyDto = {
  id: string;
  name: string;
  type: PropertyTypeDto;
  ownershipType: OwnershipTypeDto;
  primaryOwnerName?: string | null;
  purchaseDate?: string | null;
  purchasePrice: number;
  currentMarketValue: number;
  appreciation: number;
  appreciationPercent?: number | null;
  currencyCode: string;
  area?: number | null;
  builtUpArea?: number | null;
  floor?: string | null;
  facing?: string | null;
  bedrooms?: number | null;
  bathrooms?: number | null;
  parking?: number | null;
  status: PropertyStatusDto;
  description?: string | null;
  notes?: string | null;
  isRentalEnabled: boolean;
  address?: PropertyAddressDto | null;
  owners: PropertyOwnerDto[];
  valuations: PropertyValuationDto[];
  loanLinks: PropertyLoanLinkDto[];
  documentLinks: PropertyDocumentLinkDto[];
  images: PropertyImageDto[];
  propertyNotes: PropertyNoteDto[];
  createdAt: string;
  updatedAt?: string | null;
};

export type PropertyListItemDto = {
  id: string;
  name: string;
  type: PropertyTypeDto;
  status: PropertyStatusDto;
  primaryOwnerName?: string | null;
  city?: string | null;
  locality?: string | null;
  purchasePrice: number;
  currentMarketValue: number;
  currencyCode: string;
  purchaseDate?: string | null;
};

export type PropertyListDto = {
  items: PropertyListItemDto[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type PropertySummaryDto = {
  propertyCount: number;
  totalPurchasePrice: number;
  totalMarketValue: number;
  totalAppreciation: number;
  totalAppreciationPercent?: number | null;
  currencyCode: string;
  activeCount: number;
  rentedCount: number;
};

/** GET /api/v1/properties/{id}/dashboard — used by the Property passport page. */
export type PropertyDashboardDto = {
  property: PropertyDto;
  equityEstimate: number;
  appreciation: number;
  appreciationPercent?: number | null;
  valuationCount: number;
  loanLinkCount: number;
  documentLinkCount: number;
  imageCount: number;
  noteCount: number;
  generatedAt: string;
};

export type PropertyAddressRequestDto = {
  line1?: string | null;
  line2?: string | null;
  locality?: string | null;
  city?: string | null;
  state?: string | null;
  postalCode?: string | null;
  country?: string | null;
  fullAddress?: string | null;
  latitude?: number | null;
  longitude?: number | null;
  googleMapsUrl?: string | null;
};

export type PropertyOwnerRequestDto = {
  name: string;
  ownershipPercentage?: number;
  ownershipType?: OwnershipTypeDto;
  isPrimary?: boolean;
  linkedUserId?: string | null;
};

export type CreatePropertyRequestDto = {
  name: string;
  type: PropertyTypeDto;
  ownershipType?: OwnershipTypeDto;
  purchaseDate?: string | null;
  purchasePrice: number;
  currentMarketValue: number;
  currencyCode?: string;
  area?: number | null;
  builtUpArea?: number | null;
  floor?: string | null;
  facing?: string | null;
  bedrooms?: number | null;
  bathrooms?: number | null;
  parking?: number | null;
  status?: PropertyStatusDto;
  description?: string | null;
  notes?: string | null;
  isRentalEnabled?: boolean;
  address?: PropertyAddressRequestDto | null;
  owners?: PropertyOwnerRequestDto[] | null;
};

export type UpdatePropertyRequestDto = CreatePropertyRequestDto & {
  ownershipType: OwnershipTypeDto;
  status: PropertyStatusDto;
};

export type PropertyListQuery = {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: PropertyStatusDto;
  type?: PropertyTypeDto;
};

/* ------------------------------- View models ------------------------------ */

export type PropertyFactView = { label: string; value: string };

export type PropertyPhotoView = {
  id: string;
  url: string;
  caption: string;
  category: string;
};

export type PropertyValuePointView = {
  year: string;
  purchase: number;
  market: number;
};

export type PropertyListItemView = {
  id: string;
  name: string;
  typeLabel: string;
  statusLabel: string;
  primaryOwnerName: string;
  city: string;
  locality: string;
  purchasePrice: number;
  currentMarketValue: number;
  currencyCode: string;
  purchaseDate: string | null;
};

export type PropertyListView = {
  items: PropertyListItemView[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type PropertySummaryView = {
  propertyCount: number;
  totalPurchasePrice: number;
  totalMarketValue: number;
  totalAppreciation: number;
  totalAppreciationPercent: number | null;
  currencyCode: string;
  activeCount: number;
  rentedCount: number;
};

/** Passport / detail view consumed by Property UI components. */
export type PropertyDetailView = {
  id: string;
  name: string;
  doorNumber: string;
  owner: string;
  address: string;
  addressLines: string[];
  locality: string;
  city: string;
  postalCode: string;
  state: string;
  purchaseYear: number | null;
  purchaseDateLabel: string;
  purchasePrice: number;
  currentValue: number;
  ownershipPct: number;
  ownersLabel: string;
  typeLabel: string;
  statusLabel: string;
  currencyCode: string;
  appreciationAbsolute: number;
  appreciationPct: number;
  cagrPct: number;
  area: number | null;
  builtUpArea: number | null;
  floor: string;
  facing: string;
  bedrooms: number | null;
  bathrooms: number | null;
  parking: number | null;
  description: string;
  isRentalEnabled: boolean;
  googleMapsUrl: string | null;
  latitude: number | null;
  longitude: number | null;
  keyFacts: PropertyFactView[];
  photos: PropertyPhotoView[];
  valueSeries: PropertyValuePointView[];
  equity: number;
  equityPct: number;
  loanOutstanding: number;
  valuationCount: number;
  loanLinkCount: number;
  documentLinkCount: number;
};
