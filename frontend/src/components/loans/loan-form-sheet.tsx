import { useEffect, useState } from "react";
import { Loader2, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { useQueryClient } from "@tanstack/react-query";

import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { Button } from "@/components/ui/button";
import { dashboardKeys } from "@/hooks/api/use-dashboard";
import {
  useCreateLoan,
  useDeleteLoan,
  useUpdateLoan,
} from "@/hooks/api/use-loans";
import {
  parseRequiredNumber,
  requiredText,
  toastMutationError,
} from "@/lib/form-utils";
import type { LoanAccount } from "@/lib/loans-data";
import type { CreateLoanRequestDto, LoanTypeDto, UpdateLoanRequestDto } from "@/services/loans/types";

const TYPE_OPTIONS = [
  { value: "0", label: "Home" },
  { value: "2", label: "Jewel" },
  { value: "1", label: "Personal" },
  { value: "3", label: "Vehicle" },
  { value: "4", label: "Education" },
  { value: "5", label: "Other" },
];

type FormState = {
  name: string;
  type: string;
  lenderName: string;
  principal: string;
  outstandingBalance: string;
  interestRate: string;
  emiAmount: string;
  tenureMonths: string;
  remainingTenureMonths: string;
  startDate: string;
  nextEmiDate: string;
  accountNumber: string;
  autoDebit: boolean;
  notes: string;
};

const emptyForm = (): FormState => ({
  name: "",
  type: "0",
  lenderName: "",
  principal: "",
  outstandingBalance: "",
  interestRate: "",
  emiAmount: "",
  tenureMonths: "",
  remainingTenureMonths: "",
  startDate: "",
  nextEmiDate: "",
  accountNumber: "",
  autoDebit: true,
  notes: "",
});

function kindToType(kind: LoanAccount["kind"]): string {
  if (kind === "home") return "0";
  if (kind === "personal") return "1";
  if (kind === "jewel") return "2";
  return "5";
}

function fromAccount(loan: LoanAccount): FormState {
  return {
    name: loan.name,
    type: kindToType(loan.kind),
    lenderName: loan.lender,
    principal: String(loan.principal),
    outstandingBalance: String(loan.outstanding),
    interestRate: String(loan.ratePct),
    emiAmount: String(loan.emi),
    tenureMonths: String(loan.remainingMonths + 12),
    remainingTenureMonths: String(loan.remainingMonths),
    startDate: loan.startedOn,
    nextEmiDate: loan.nextEmiOn,
    accountNumber: loan.accountMask,
    autoDebit: loan.autoDebit,
    notes: "",
  };
}

function buildPayload(form: FormState): CreateLoanRequestDto | string {
  const nameErr = requiredText(form.name, "Loan name");
  if (nameErr) return nameErr;
  const lenderErr = requiredText(form.lenderName, "Lender");
  if (lenderErr) return lenderErr;
  if (!form.startDate) return "Start date is required";

  const principal = parseRequiredNumber(form.principal, "Principal");
  if (typeof principal === "string") return principal;
  const outstanding = parseRequiredNumber(form.outstandingBalance, "Outstanding");
  if (typeof outstanding === "string") return outstanding;
  const rate = parseRequiredNumber(form.interestRate, "Interest rate");
  if (typeof rate === "string") return rate;
  const emi = parseRequiredNumber(form.emiAmount, "EMI");
  if (typeof emi === "string") return emi;
  const tenure = parseRequiredNumber(form.tenureMonths, "Tenure");
  if (typeof tenure === "string") return tenure;
  const remainingRaw = form.remainingTenureMonths.trim()
    ? parseRequiredNumber(form.remainingTenureMonths, "Remaining tenure")
    : tenure;
  if (typeof remainingRaw === "string") return remainingRaw;

  return {
    name: form.name.trim(),
    type: Number(form.type) as LoanTypeDto,
    lenderName: form.lenderName.trim(),
    principal,
    outstandingBalance: outstanding,
    interestRate: rate,
    emiAmount: emi,
    tenureMonths: Math.round(tenure),
    remainingTenureMonths: Math.round(remainingRaw),
    startDate: form.startDate,
    nextEmiDate: form.nextEmiDate || undefined,
    autoDebit: form.autoDebit,
    accountNumber: form.accountNumber.trim() || undefined,
    notes: form.notes.trim() || undefined,
    currencyCode: "INR",
    interestType: 0,
    paymentFrequency: 0,
    status: 0,
  };
}

export function LoanFormSheet({
  open,
  onOpenChange,
  mode,
  loan,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  loan?: LoanAccount | null;
}) {
  const queryClient = useQueryClient();
  const createMutation = useCreateLoan();
  const updateMutation = useUpdateLoan();
  const deleteMutation = useDeleteLoan();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(mode === "edit" && loan ? fromAccount(loan) : emptyForm());
  }, [open, mode, loan]);

  const pending =
    createMutation.isPending || updateMutation.isPending || deleteMutation.isPending;

  const set =
    (key: keyof FormState) =>
    (value: string | boolean) =>
      setForm((prev) => ({ ...prev, [key]: value }));

  async function onSubmit() {
    const payload = buildPayload(form);
    if (typeof payload === "string") {
      setError(payload);
      toast.error(payload);
      return;
    }
    setError(null);
    try {
      if (mode === "create") {
        await createMutation.mutateAsync(payload);
        toast.success("Loan added");
      } else if (loan) {
        const body: UpdateLoanRequestDto = {
          ...payload,
          remainingTenureMonths: payload.remainingTenureMonths,
          nextEmiDate: payload.nextEmiDate,
        };
        await updateMutation.mutateAsync({ id: loan.id, body });
        toast.success("Loan updated");
      }
      void queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not save loan");
    }
  }

  async function onDelete() {
    if (!loan) return;
    try {
      await deleteMutation.mutateAsync(loan.id);
      toast.success("Loan deleted");
      void queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not delete loan");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title={mode === "create" ? "Add loan" : "Edit loan"}
      description="Track EMI, rate and outstanding balance manually."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(mode === "edit" && loan ? fromAccount(loan) : emptyForm())}
      submitLabel={mode === "create" ? "Add loan" : "Save changes"}
      deleteSlot={
        mode === "edit" && loan ? (
          <ConfirmDialog
            title="Delete loan?"
            description="This removes the loan and its payment history from your workspace."
            confirmLabel="Delete"
            destructive
            onConfirm={() => void onDelete()}
            trigger={
              <Button
                type="button"
                variant="ghost"
                className="min-h-11 w-full rounded-full text-destructive hover:bg-destructive/10 hover:text-destructive"
                disabled={pending}
              >
                {deleteMutation.isPending ? (
                  <Loader2 className="size-4 animate-spin" />
                ) : (
                  <Trash2 className="size-4" />
                )}
                Delete loan
              </Button>
            }
          />
        ) : null
      }
    >
      {error ? (
        <p role="alert" className="rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {error}
        </p>
      ) : null}

      <Field id="loan-name" label="Name" value={form.name} onChange={(e) => set("name")(e.target.value)} />
      <div className="grid grid-cols-2 gap-2.5">
        <SelectField
          id="loan-type"
          label="Type"
          value={form.type}
          onChange={(e) => set("type")(e.target.value)}
          options={TYPE_OPTIONS}
        />
        <Field
          id="loan-lender"
          label="Lender"
          value={form.lenderName}
          onChange={(e) => set("lenderName")(e.target.value)}
        />
      </div>
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="loan-principal"
          label="Principal (₹)"
          inputMode="decimal"
          value={form.principal}
          onChange={(e) => set("principal")(e.target.value)}
        />
        <Field
          id="loan-outstanding"
          label="Outstanding (₹)"
          inputMode="decimal"
          value={form.outstandingBalance}
          onChange={(e) => set("outstandingBalance")(e.target.value)}
        />
      </div>
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="loan-rate"
          label="Interest rate %"
          inputMode="decimal"
          value={form.interestRate}
          onChange={(e) => set("interestRate")(e.target.value)}
        />
        <Field
          id="loan-emi"
          label="EMI (₹)"
          inputMode="decimal"
          value={form.emiAmount}
          onChange={(e) => set("emiAmount")(e.target.value)}
        />
      </div>
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="loan-tenure"
          label="Tenure (months)"
          inputMode="numeric"
          value={form.tenureMonths}
          onChange={(e) => set("tenureMonths")(e.target.value)}
        />
        <Field
          id="loan-remaining"
          label="Remaining months"
          inputMode="numeric"
          value={form.remainingTenureMonths}
          onChange={(e) => set("remainingTenureMonths")(e.target.value)}
        />
      </div>
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="loan-start"
          label="Start date"
          type="date"
          value={form.startDate}
          onChange={(e) => set("startDate")(e.target.value)}
        />
        <Field
          id="loan-next"
          label="Next EMI"
          type="date"
          value={form.nextEmiDate}
          onChange={(e) => set("nextEmiDate")(e.target.value)}
        />
      </div>
      <Field
        id="loan-account"
        label="Account number"
        value={form.accountNumber}
        onChange={(e) => set("accountNumber")(e.target.value)}
      />
      <label className="flex min-h-11 items-center gap-2 text-sm">
        <input
          type="checkbox"
          checked={form.autoDebit}
          onChange={(e) => set("autoDebit")(e.target.checked)}
          className="size-4 rounded border-input"
        />
        Auto-debit enabled
      </label>
      <TextAreaField
        id="loan-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
    </FormSheet>
  );
}
