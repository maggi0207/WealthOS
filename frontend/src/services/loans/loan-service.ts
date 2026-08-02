import { isMockApiMode } from "@/config/env";
import { BaseApiService } from "@/services/http/base-api-service";
import {
  mapLoanDetail,
  mapLoanListResponse,
  mapLoanSummary,
  mapMockLoanDetail,
  mapMockLoanList,
  mapMockLoanSummary,
  mapMockPayments,
  mapMockUpcomingPayments,
  mapUpcomingPayments,
} from "@/services/loans/loan-mapper";
import type {
  CreateLoanRequestDto,
  LoanAccount,
  LoanDashboardDto,
  LoanDto,
  LoanListDto,
  LoanListQuery,
  LoanListView,
  LoanPayment,
  LoanPaymentDto,
  LoanReminder,
  LoanSummaryDto,
  LoanTotalsView,
  RecordLoanPaymentRequestDto,
  UpcomingPaymentsDto,
  UpdateLoanRequestDto,
} from "@/services/loans/types";

function buildListQuery(params: LoanListQuery = {}): string {
  const search = new URLSearchParams();
  if (params.page != null) search.set("page", String(params.page));
  if (params.pageSize != null) search.set("pageSize", String(params.pageSize));
  if (params.search) search.set("search", params.search);
  if (params.status != null) search.set("status", String(params.status));
  if (params.type != null) search.set("type", String(params.type));
  const qs = search.toString();
  return qs ? `?${qs}` : "";
}

/**
 * Loans API service — `/api/v{version}/loans*`.
 * Transparent mock fallback when `VITE_API_MODE=mock`.
 */
class LoanService extends BaseApiService {
  protected readonly serviceName = "LoanService";

  async list(params: LoanListQuery = {}): Promise<LoanListView> {
    if (isMockApiMode()) {
      return mapMockLoanList();
    }
    const [list, summary] = await Promise.all([
      this.get<LoanListDto>(`/loans${buildListQuery({ pageSize: 50, ...params })}`),
      this.get<LoanSummaryDto>("/loans/summary"),
    ]);
    return mapLoanListResponse(list, summary);
  }

  async getSummary(): Promise<LoanTotalsView> {
    if (isMockApiMode()) {
      return mapMockLoanSummary();
    }
    const [summary, list] = await Promise.all([
      this.get<LoanSummaryDto>("/loans/summary"),
      this.get<LoanListDto>("/loans?pageSize=50"),
    ]);
    return mapLoanSummary(
      summary,
      (list.items ?? []).map((i) => i.nextEmiDate),
    );
  }

  async getById(id: string): Promise<{
    account: LoanAccount;
    payments: LoanPayment[];
    reminders: LoanReminder[];
  }> {
    if (isMockApiMode()) {
      return mapMockLoanDetail(id);
    }
    const dto = await this.get<LoanDto>(`/loans/${id}`);
    return mapLoanDetail(dto);
  }

  async getDashboard(id: string): Promise<{
    account: LoanAccount;
    payments: LoanPayment[];
    reminders: LoanReminder[];
  }> {
    if (isMockApiMode()) {
      return mapMockLoanDetail(id);
    }
    const dto = await this.get<LoanDashboardDto>(`/loans/${id}/dashboard`);
    return mapLoanDetail(dto.loan);
  }

  async getUpcoming(days = 45): Promise<LoanReminder[]> {
    if (isMockApiMode()) {
      return mapMockUpcomingPayments();
    }
    const dto = await this.get<UpcomingPaymentsDto>(
      `/loans/upcoming?days=${days}`,
    );
    return mapUpcomingPayments(dto);
  }

  async getPayments(loanId: string): Promise<LoanPayment[]> {
    if (isMockApiMode()) {
      return mapMockPayments(loanId);
    }
    const detail = await this.getById(loanId);
    return detail.payments;
  }

  async create(body: CreateLoanRequestDto): Promise<LoanAccount> {
    if (isMockApiMode()) {
      return mapMockLoanList().accounts[0]!;
    }
    const dto = await this.post<LoanDto>("/loans", body);
    return mapLoanDetail(dto).account;
  }

  async update(id: string, body: UpdateLoanRequestDto): Promise<LoanAccount> {
    if (isMockApiMode()) {
      return mapMockLoanDetail(id).account;
    }
    const dto = await this.put<LoanDto>(`/loans/${id}`, body);
    return mapLoanDetail(dto).account;
  }

  async remove(id: string): Promise<void> {
    if (isMockApiMode()) {
      return;
    }
    await this.delete<unknown>(`/loans/${id}`);
  }

  async recordPayment(
    id: string,
    body: RecordLoanPaymentRequestDto,
  ): Promise<LoanPayment> {
    if (isMockApiMode()) {
      return mapMockPayments(id)[0]!;
    }
    const dto = await this.post<LoanPaymentDto>(`/loans/${id}/payments`, body);
    return {
      id: String(dto.id),
      loanId: id,
      paidOn: dto.paidOn?.slice(0, 10) ?? "",
      amount: dto.amount,
      principal: dto.principalComponent,
      interest: dto.interestComponent,
      status: "paid",
      mode: dto.paymentMode || "—",
    };
  }
}

export const loanService = new LoanService();
