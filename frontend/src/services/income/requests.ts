/**
 * Income & Business mutation request DTOs — mirror WealthOS.Application.Income.DTOs.Requests.
 */

export type ClientStatusDto = 0 | 1;

export type ProjectStatusDto = 0 | 1 | 2 | 3;

export type InvoiceStatusDto = 0 | 1 | 2 | 3 | 4 | 5;

export type PaymentMethodDto = 0 | 1 | 2 | 3 | 4;

export type SalaryStatusDto = 0 | 1 | 2;

export type PayrollStatusDto = 0 | 1 | 2;

export type CreateClientRequestDto = {
  name: string;
  engagement: string;
  status: ClientStatusDto;
  monthlyRevenue: number;
  currencyCode: string;
  contactEmail?: string | null;
  contactPhone?: string | null;
  notes?: string | null;
};

export type UpdateClientRequestDto = CreateClientRequestDto;

export type CreateProjectRequestDto = {
  clientId: string;
  name: string;
  description?: string | null;
  status: ProjectStatusDto;
  startDate: string;
  endDate?: string | null;
  monthlyRevenue?: number | null;
  currencyCode: string;
};

export type AssignDeveloperRequestDto = {
  projectId: string;
  developerId: string;
  assignedOn: string;
  roleOnProject?: string | null;
};

export type CreateDeveloperRequestDto = {
  name: string;
  role: string;
  monthlySalary: number;
  currencyCode: string;
  primaryClientId?: string | null;
  notes?: string | null;
};

export type CreateInvoiceItemRequestDto = {
  description: string;
  quantity: number;
  unitPrice: number;
};

export type CreateInvoiceRequestDto = {
  clientId: string;
  projectId?: string | null;
  invoiceNumber: string;
  issueDate: string;
  dueDate: string;
  status: InvoiceStatusDto;
  currencyCode: string;
  notes?: string | null;
  items: CreateInvoiceItemRequestDto[];
};

export type RecordInvoicePaymentRequestDto = {
  invoiceId: string;
  amount: number;
  paidOn: string;
  method: PaymentMethodDto;
  reference?: string | null;
  notes?: string | null;
};

export type CreateExpenseRequestDto = {
  categoryId?: string | null;
  categoryName: string;
  vendor: string;
  amount: number;
  currencyCode: string;
  paidOn: string;
  isRecurring: boolean;
  notes?: string | null;
};

export type RecordSalaryRequestDto = {
  salaryId?: string | null;
  memberName: string;
  employer: string;
  role: string;
  monthlyAmount: number;
  currencyCode: string;
  paidOn: string;
  period: string;
  status: SalaryStatusDto;
  nextExpectedOn?: string | null;
  notes?: string | null;
};

export type CreatePayrollRequestDto = {
  developerId: string;
  amount: number;
  period: string;
  status: PayrollStatusDto;
  paidOn?: string | null;
  scheduledOn?: string | null;
  notes?: string | null;
};
