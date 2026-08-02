/** Investment API DTOs — aligned with WealthOS.Application.Investments */

export type InvestmentAccountStatusDto = 0 | 1 | 2 | 3;

export type InvestmentCategoryDto = 0 | 1 | 2 | 3 | 4 | 99;

export type InvestmentTypeDto = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 99;

export type InvestmentTransactionTypeDto = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 99;

export type InvestmentProviderDto = {
  id: string;
  kind: number | string;
  name: string;
  description?: string | null;
  isEnabled: boolean;
  supportsSync: boolean;
};

export type InvestmentProviderListDto = {
  items: InvestmentProviderDto[];
  totalCount?: number;
};

export type InvestmentAccountDto = {
  id: string;
  providerId: string;
  providerName?: string;
  name: string;
  ownerName: string;
  kindLabel: string;
  status: InvestmentAccountStatusDto | string;
  notes?: string | null;
  currencyCode?: string;
};

export type HoldingDto = {
  id: string;
  accountId: string;
  name: string;
  symbol: string;
  category: InvestmentCategoryDto | string;
  investmentType: InvestmentTypeDto | string;
  quantity: number;
  averageCost: number;
  investedAmount: number;
  currentPrice: number;
  currentValue: number;
  dayChange?: number;
  dayChangePercent?: number;
  notes?: string | null;
  currencyCode?: string;
};

export type CreateInvestmentAccountRequestDto = {
  providerId: string;
  name: string;
  ownerName: string;
  kindLabel: string;
  status?: InvestmentAccountStatusDto;
  currencyCode?: string;
  notes?: string | null;
  externalAccountReference?: string | null;
};

export type UpdateInvestmentAccountRequestDto = Omit<
  CreateInvestmentAccountRequestDto,
  "providerId"
>;

export type AddManualHoldingRequestDto = {
  accountId: string;
  name: string;
  symbol: string;
  category: InvestmentCategoryDto;
  investmentType: InvestmentTypeDto;
  quantity: number;
  averageCost: number;
  investedAmount: number;
  currentPrice: number;
  currentValue: number;
  dayChange?: number;
  dayChangePercent?: number;
  currencyCode?: string;
  notes?: string | null;
};

export type UpdateHoldingRequestDto = Omit<AddManualHoldingRequestDto, "accountId">;

export type RecordTransactionRequestDto = {
  accountId: string;
  holdingId?: string | null;
  transactionType: InvestmentTransactionTypeDto;
  quantity: number;
  price: number;
  amount: number;
  fees?: number;
  transactionDate: string;
  currencyCode?: string;
  notes?: string | null;
  externalReference?: string | null;
};
