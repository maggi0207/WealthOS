import { isMockApiMode } from "@/config/env";
import { BaseApiService } from "@/services/http/base-api-service";
import {
  mapIncomeOverview,
  mapMockIncomeOverview,
} from "@/services/income/income-mapper";
import type {
  AssignDeveloperRequestDto,
  CreateClientRequestDto,
  CreateDeveloperRequestDto,
  CreateExpenseRequestDto,
  CreateInvoiceRequestDto,
  CreateProjectRequestDto,
  RecordInvoicePaymentRequestDto,
  RecordSalaryRequestDto,
  UpdateClientRequestDto,
} from "@/services/income/requests";
import type {
  CashFlowDto,
  ClientListDto,
  ClientResponseDto,
  DeveloperListDto,
  DeveloperResponseDto,
  ExpenseListDto,
  ExpenseResponseDto,
  IncomeDashboardDto,
  IncomeOverviewView,
  InvoiceDto,
  InvoiceListDto,
  InvoicePaymentResponseDto,
  MonthlyIncomeTrendDto,
  PayrollListDto,
  ProfitLossDto,
  ProjectListDto,
  ProjectDto,
  SalaryResponseDto,
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

  async listClients(pageSize = 50): Promise<ClientListDto> {
    if (isMockApiMode()) {
      const overview = mapMockIncomeOverview();
      return {
        items: overview.clients.map((c) => ({
          id: c.id,
          name: c.name,
          engagement: c.engagement,
          status: c.status === "paused" ? 1 : 0,
          monthlyRevenue: c.monthlyRevenue,
          outstandingInvoice: c.outstandingInvoice,
          lastPaymentAmount: c.lastPaymentAmount,
          lastPaymentOn: c.lastPaymentOn,
          currencyCode: "INR",
        })),
        page: 1,
        pageSize,
        totalCount: overview.clients.length,
      };
    }
    return this.get<ClientListDto>(`/clients?pageSize=${pageSize}`);
  }

  async listInvoices(pageSize = 50): Promise<InvoiceListDto> {
    if (isMockApiMode()) {
      return { items: [], page: 1, pageSize, totalCount: 0 };
    }
    return this.get<InvoiceListDto>(`/invoices?pageSize=${pageSize}`);
  }

  async listProjects(pageSize = 50): Promise<ProjectListDto> {
    if (isMockApiMode()) {
      return { items: [], page: 1, pageSize, totalCount: 0 };
    }
    return this.get<ProjectListDto>(`/projects?pageSize=${pageSize}`);
  }

  async recordSalary(body: RecordSalaryRequestDto): Promise<SalaryResponseDto> {
    if (isMockApiMode()) {
      return {
        id: body.salaryId ?? "mock-salary",
        memberName: body.memberName,
        employer: body.employer,
        role: body.role,
        monthlyAmount: body.monthlyAmount,
        currencyCode: body.currencyCode,
        lastCreditedOn: body.paidOn,
        nextExpectedOn: body.nextExpectedOn,
        status: body.status,
      };
    }
    return this.post<SalaryResponseDto>("/income/salary", body);
  }

  async createClient(body: CreateClientRequestDto): Promise<ClientResponseDto> {
    if (isMockApiMode()) {
      return {
        id: `mock-${Date.now()}`,
        name: body.name,
        engagement: body.engagement,
        status: body.status,
        monthlyRevenue: body.monthlyRevenue,
        outstandingInvoice: 0,
        lastPaymentAmount: 0,
        currencyCode: body.currencyCode,
        contactEmail: body.contactEmail,
        contactPhone: body.contactPhone,
        notes: body.notes,
      };
    }
    return this.post<ClientResponseDto>("/clients", body);
  }

  async updateClient(
    id: string,
    body: UpdateClientRequestDto,
  ): Promise<ClientResponseDto> {
    if (isMockApiMode()) {
      return {
        id,
        name: body.name,
        engagement: body.engagement,
        status: body.status,
        monthlyRevenue: body.monthlyRevenue,
        outstandingInvoice: 0,
        lastPaymentAmount: 0,
        currencyCode: body.currencyCode,
        contactEmail: body.contactEmail,
        contactPhone: body.contactPhone,
        notes: body.notes,
      };
    }
    return this.put<ClientResponseDto>(`/clients/${id}`, body);
  }

  async deleteClient(id: string): Promise<void> {
    if (isMockApiMode()) return;
    await this.delete<unknown>(`/clients/${id}`);
  }

  async createProject(body: CreateProjectRequestDto): Promise<ProjectDto> {
    if (isMockApiMode()) {
      return {
        id: `mock-project-${Date.now()}`,
        clientId: body.clientId,
        name: body.name,
        description: body.description,
        status: body.status,
        startDate: body.startDate,
        endDate: body.endDate,
        monthlyRevenue: body.monthlyRevenue,
        currencyCode: body.currencyCode,
      };
    }
    return this.post<ProjectDto>("/projects", body);
  }

  async assignDeveloper(body: AssignDeveloperRequestDto): Promise<ProjectDto> {
    if (isMockApiMode()) {
      return {
        id: body.projectId,
        clientId: "",
        name: "",
        status: 0,
        startDate: body.assignedOn,
        currencyCode: "INR",
      };
    }
    return this.post<ProjectDto>("/projects/assign-developer", body);
  }

  async createDeveloper(
    body: CreateDeveloperRequestDto,
  ): Promise<DeveloperResponseDto> {
    if (isMockApiMode()) {
      return {
        id: `mock-dev-${Date.now()}`,
        name: body.name,
        role: body.role,
        monthlySalary: body.monthlySalary,
        primaryClientId: body.primaryClientId,
        isActive: true,
        notes: body.notes,
      };
    }
    return this.post<DeveloperResponseDto>("/developers", body);
  }

  async createInvoice(body: CreateInvoiceRequestDto): Promise<InvoiceDto> {
    if (isMockApiMode()) {
      const subTotal = body.items.reduce(
        (s, i) => s + i.quantity * i.unitPrice,
        0,
      );
      return {
        id: `mock-inv-${Date.now()}`,
        clientId: body.clientId,
        clientName: "",
        projectId: body.projectId,
        invoiceNumber: body.invoiceNumber,
        issueDate: body.issueDate,
        dueDate: body.dueDate,
        status: body.status,
        subTotal,
        amountPaid: 0,
        outstandingAmount: subTotal,
        currencyCode: body.currencyCode,
        notes: body.notes,
      };
    }
    return this.post<InvoiceDto>("/invoices", body);
  }

  async recordPayment(
    body: RecordInvoicePaymentRequestDto,
  ): Promise<InvoicePaymentResponseDto> {
    if (isMockApiMode()) {
      return {
        id: `mock-pay-${Date.now()}`,
        invoiceId: body.invoiceId,
        amount: body.amount,
        paidOn: body.paidOn,
        method: body.method,
        reference: body.reference,
        notes: body.notes,
      };
    }
    return this.post<InvoicePaymentResponseDto>("/payments", body);
  }

  async createExpense(body: CreateExpenseRequestDto): Promise<ExpenseResponseDto> {
    if (isMockApiMode()) {
      return {
        id: `mock-exp-${Date.now()}`,
        categoryId: body.categoryId ?? "",
        categoryName: body.categoryName,
        vendor: body.vendor,
        amount: body.amount,
        paidOn: body.paidOn,
        isRecurring: body.isRecurring,
        currencyCode: body.currencyCode,
        notes: body.notes,
      };
    }
    return this.post<ExpenseResponseDto>("/expenses", body);
  }
}

export const incomeService = new IncomeService();
