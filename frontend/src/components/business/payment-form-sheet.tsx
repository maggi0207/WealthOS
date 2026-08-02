import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";

import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { useIncomeInvoices, useRecordPayment } from "@/hooks/api/use-income";
import {
  parseRequiredNumber,
  todayIsoDate,
  toastMutationError,
} from "@/lib/form-utils";
import type { PaymentMethodDto, RecordInvoicePaymentRequestDto } from "@/services/income/requests";

const METHOD_OPTIONS = [
  { value: "0", label: "Bank transfer" },
  { value: "1", label: "UPI" },
  { value: "2", label: "Cheque" },
  { value: "3", label: "Cash" },
  { value: "4", label: "Other" },
];

type FormState = {
  invoiceId: string;
  amount: string;
  paidOn: string;
  method: string;
  reference: string;
  notes: string;
};

const emptyForm = (): FormState => ({
  invoiceId: "",
  amount: "",
  paidOn: todayIsoDate(),
  method: "0",
  reference: "",
  notes: "",
});

function buildPayload(form: FormState): RecordInvoicePaymentRequestDto | string {
  if (!form.invoiceId) return "Invoice is required";
  const amount = parseRequiredNumber(form.amount, "Amount");
  if (typeof amount === "string") return amount;
  if (amount <= 0) return "Amount must be greater than zero";
  if (!form.paidOn) return "Paid on date is required";

  return {
    invoiceId: form.invoiceId,
    amount,
    paidOn: form.paidOn,
    method: Number(form.method) as PaymentMethodDto,
    reference: form.reference.trim() || null,
    notes: form.notes.trim() || null,
  };
}

/** Record a client payment against an outstanding invoice. */
export function PaymentFormSheet({
  open,
  onOpenChange,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { data: invoiceList } = useIncomeInvoices();
  const recordMutation = useRecordPayment();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const invoiceOptions = useMemo(() => {
    const items = invoiceList?.items ?? [];
    return items
      .filter((inv) => inv.outstandingAmount > 0)
      .map((inv) => ({
        value: inv.id,
        label: `${inv.invoiceNumber} · ${inv.clientName} (₹${inv.outstandingAmount.toLocaleString("en-IN")} due)`,
      }));
  }, [invoiceList?.items]);

  useEffect(() => {
    if (!open) return;
    setError(null);
    const first = invoiceOptions[0]?.value ?? "";
    const firstOutstanding =
      invoiceList?.items.find((i) => i.id === first)?.outstandingAmount ?? 0;
    setForm({
      ...emptyForm(),
      invoiceId: first,
      amount: firstOutstanding > 0 ? String(firstOutstanding) : "",
    });
  }, [open, invoiceOptions, invoiceList?.items]);

  const pending = recordMutation.isPending;
  const set =
    (key: keyof FormState) =>
    (value: string) =>
      setForm((prev) => ({ ...prev, [key]: value }));

  function onInvoiceChange(invoiceId: string) {
    const outstanding =
      invoiceList?.items.find((i) => i.id === invoiceId)?.outstandingAmount ?? 0;
    setForm((prev) => ({
      ...prev,
      invoiceId,
      amount: outstanding > 0 ? String(outstanding) : prev.amount,
    }));
  }

  async function onSubmit() {
    const payload = buildPayload(form);
    if (typeof payload === "string") {
      setError(payload);
      toast.error(payload);
      return;
    }
    setError(null);
    try {
      await recordMutation.mutateAsync(payload);
      toast.success("Payment recorded");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not record payment");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title="Record payment"
      description="Log a client payment against an invoice."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => {
        const first = invoiceOptions[0]?.value ?? "";
        const outstanding =
          invoiceList?.items.find((i) => i.id === first)?.outstandingAmount ?? 0;
        setForm({
          ...emptyForm(),
          invoiceId: first,
          amount: outstanding > 0 ? String(outstanding) : "",
        });
      }}
      submitLabel="Record payment"
    >
      {error ? (
        <p role="alert" className="rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {error}
        </p>
      ) : null}

      {invoiceOptions.length === 0 ? (
        <p className="text-sm text-muted-foreground">
          No outstanding invoices. Create an invoice first.
        </p>
      ) : (
        <>
          <SelectField
            id="pay-invoice"
            label="Invoice"
            value={form.invoiceId}
            onChange={(e) => onInvoiceChange(e.target.value)}
            options={invoiceOptions}
          />
          <Field
            id="pay-amount"
            label="Amount (₹)"
            inputMode="decimal"
            value={form.amount}
            onChange={(e) => set("amount")(e.target.value)}
          />
          <div className="grid grid-cols-2 gap-2.5">
            <Field
              id="pay-date"
              label="Paid on"
              type="date"
              value={form.paidOn}
              onChange={(e) => set("paidOn")(e.target.value)}
            />
            <SelectField
              id="pay-method"
              label="Method"
              value={form.method}
              onChange={(e) => set("method")(e.target.value)}
              options={METHOD_OPTIONS}
            />
          </div>
          <Field
            id="pay-ref"
            label="Reference"
            value={form.reference}
            onChange={(e) => set("reference")(e.target.value)}
            hint="UTR, cheque no., etc."
          />
          <TextAreaField
            id="pay-notes"
            label="Notes"
            value={form.notes}
            onChange={(e) => set("notes")(e.target.value)}
          />
        </>
      )}
    </FormSheet>
  );
}
