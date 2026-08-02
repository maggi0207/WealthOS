import { isMockApiMode } from "@/config/env";
import { BaseApiService } from "@/services/http/base-api-service";
import {
  mapIncomeOverview,
  mapMockIncomeOverview,
} from "@/services/income/income-mapper";
import type {
  CashFlowDto,
  ClientListDto,
  DeveloperListDto,
  ExpenseListDto,
  IncomeDashboardDto,
  IncomeOverviewView,
  MonthlyIncomeTrendDto,
  PayrollListDto,
  ProfitLossDto,
} from "@/services/income/types";

/**
 * Income & Business API — `/api/v1/income*`, clients, expenses, payroll, developers.
 * Transparent mock fallback when `VITE_API_MODE=mock`.
 */
class IncomeService extends BaseApiService {
  protected readonly serviceName = "IncomeService";

  async getOverview(period?: string): Promise<IncomeOverviewView> {
    if (isMockApiMode()) {
      return mapMockIncomeOverview();
    }

    const qs = period ? `?period=${encodeURIComponent(period)}` : "";
    const [
      cashFlow,
      pnl,
      dashboard,
      trend,
      clients,
      expenses,
      developers,
      payroll,
    ] = await Promise.all([
      this.get<CashFlowDto>(`/income/cashflow${qs}`),
      this.get<ProfitLossDto>(`/income/profit-loss${qs}`),
      this.get<IncomeDashboardDto>(`/income/dashboard${qs}`),
      this.get<MonthlyIncomeTrendDto>(`/income/monthly${qs}`),
      this.get<ClientListDto>("/clients?pageSize=50"),
      this.get<ExpenseListDto>("/expenses?pageSize=50"),
      this.get<DeveloperListDto>("/developers?pageSize=50"),
      this.get<PayrollListDto>("/payroll?pageSize=50"),
    ]);

    return mapIncomeOverview({
      cashFlow,
      pnl,
      dashboard,
      trend,
      clients,
      expenses,
      developers,
      payroll,
    });
  }
}

export const incomeService = new IncomeService();
