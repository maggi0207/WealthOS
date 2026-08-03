/** Manual asset API DTOs matching ASP.NET camelCase serialization. */

export type ManualAssetTypeDto =
  | 0
  | 1
  | 2
  | 3
  | 4
  | 5
  | 6
  | 7
  | 8
  | 9
  | 10
  | 99
  | "PhysicalGold"
  | "Cash"
  | "BankBalance"
  | "FixedDeposit"
  | "Vehicle"
  | "Jewellery"
  | "Ppf"
  | "Epf"
  | "Nps"
  | "Crypto"
  | "Collectibles"
  | "Other";

export type ManualAssetDto = {
  id: string;
  type: ManualAssetTypeDto;
  typeLabel: string;
  name: string;
  purchaseValue: number;
  currentValue: number;
  gainLoss: number;
  gainLossPercent?: number | null;
  quantity?: number | null;
  institution?: string | null;
  purchaseDate?: string | null;
  notes?: string | null;
  currencyCode: string;
  createdAt: string;
  updatedAt?: string | null;
};

export type ManualAssetListDto = {
  items: ManualAssetDto[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  totalCurrentValue: number;
  currencyCode: string;
};

export type CreateManualAssetRequestDto = {
  type: number;
  name: string;
  purchaseValue: number;
  currentValue: number;
  quantity?: number | null;
  institution?: string | null;
  purchaseDate?: string | null;
  notes?: string | null;
  currencyCode: string;
};

export type UpdateManualAssetRequestDto = CreateManualAssetRequestDto;

export const MANUAL_ASSET_TYPE_OPTIONS: { value: string; label: string }[] = [
  { value: "0", label: "Physical Gold" },
  { value: "1", label: "Cash" },
  { value: "2", label: "Bank Balance" },
  { value: "3", label: "Fixed Deposit" },
  { value: "4", label: "Vehicle" },
  { value: "5", label: "Jewellery" },
  { value: "6", label: "PPF" },
  { value: "7", label: "EPF" },
  { value: "8", label: "NPS" },
  { value: "9", label: "Crypto" },
  { value: "10", label: "Collectibles" },
  { value: "99", label: "Other" },
];
