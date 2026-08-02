import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";

import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { useCreateInvoice, useIncomeOverview } from "@/hooks/api/use-income";
import {
  parseRequiredNumber,
  requiredText,
  todayIsoDate,
  toastMutationError,
} from "@/lib/form-utils";
import type { CreateInvoiceRequestDto, InvoiceStatusDto } from "@/services/income/requests";

const STATUS_OPTIONS = [
  { value: "0", label: "Draft" },
  { value: "1", label: "Sent" },
  { value: "2", label: "Partially paid" },
  { value: "3", label: "Paid" },
];

type FormState = {
  clientId: string;
  invoiceNumber: string;
  issueDate: string;
  dueDate: string;
  status: string;
  itemDescription: string;
  quantity: string;
  unitPrice: string;
  notes: string;
};

const emptyForm = (): FormState => ({
  clientId: "",
  invoiceNumber: "",
  issueDate: todayIsoDate(),
  dueDate: todayIsoDate(),
  status: "1",
  itemDescription: "Monthly retainer",
  quantity: "1",
  unitPrice: "",
  notes: "",
});

function buildPayload(form: FormState): CreateInvoiceRequestDto | string {
  if (!form.clientId) return "Client is required";
  const numberErr = requiredText(form.invoiceNumber, "Invoice number", 1);
  if (numberErr) return numberErr;
  if (!form.issueDate) return "Issue date is required";
  if (!form.dueDate) return "Due date is required";
  if (form.dueDate < form.issueDate) return "Due date must be on or after issue date";

  const descErr = requiredText(form.itemDescription, "Line item description", 1);
  if (descErr) return descErr;
  const qty = parseRequiredNumber(form.quantity, "Quantity");
  if (typeof qty === "string") return qty;
  if (qty <= 0) return "Quantity must be greater than zero";
  const price = parseRequiredNumber(form.unitPrice, "Unit price");
  if (typeof price === "string") return price;
  if (price < 0) return "Unit price cannot be negative";

  return {
    clientId: form.clientId,
    invoiceNumber: form.invoiceNumber.trim(),
    issueDate: form.issueDate,
    dueDate: form.dueDate,
    status: Number(form.status) as InvoiceStatusDto,
    currencyCode: "INR",
    notes: form.notes.trim() || null,
    items: [
      {
        description: form.itemDescription.trim(),
        quantity: qty,
        unitPrice: price,
      },
    ],
  };
}

export function InvoiceFormSheet({
  open,
  onOpenChange,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { data: overview } = useIncomeOverview();
  const createMutation = useCreateInvoice();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const clientOptions = useMemo(
    () =>
      (overview?.clients ?? []).map((c) => ({
        value: c.id,
        label: c.name,
      })),
    [overview?.clients],
  );

  useEffect(() => {
    if (!open) return;
    setError(null);
    const firstClient = clientOptions[0]?.value ?? "";
    setForm({ ...emptyForm(), clientId: firstClient });
  }, [open, clientOptions]);

  const pending = createMutation.isPending;
  const set =
    (key: keyof FormState) =>
    (value: string) =>
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
      await createMutation.mutateAsync(payload);
      toast.success("Invoice created");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not create invoice");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title="Create invoice"
      description="Bill a client with a single line item."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() =>
        setForm({ ...emptyForm(), clientId: clientOptions[0]?.value ?? "" })
      }
      submitLabel="Create invoice"
    >
      {error ? (
        <p role="alert" className="rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {error}
        </p>
      ) : null}

      {clientOptions.length === 0 ? (
        <p className="text-sm text-muted-foreground">Add a client first to create an invoice.</p>
      ) : (
        <>
          <SelectField
            id="inv-client"
            label="Client"
            value={form.clientId}
            onChange={(e) => set("clientId")(e.target.value)}
            options={clientOptions}
          />
          <Field
            id="inv-number"
            label="Invoice number"
            value={form.invoiceNumber}
            onChange={(e) => set("invoiceNumber")(e.target.value)}
          />
          <div className="grid grid-cols-2 gap-2.5">
            <Field
              id="inv-issue"
              label="Issue date"
              type="date"
              value={form.issueDate}
              onChange={(e) => set("issueDate")(e.target.value)}
            />
            <Field
              id="inv-due"
              label="Due date"
              type="date"
              value={form.dueDate}
              onChange={(e) => set("dueDate")(e.target.value)}
            />
          </div>
          <SelectField
            id="inv-status"
            label="Status"
            value={form.status}
            onChange={(e) => set("status")(e.target.value)}
            options={STATUS_OPTIONS}
          />
          <Field
            id="inv-item-desc"
            label="Line item"
            value={form.itemDescription}
            onChange={(e) => set("itemDescription")(e.target.value)}
          />
          <div className="grid grid-cols-2 gap-2.5">
            <Field
              id="inv-qty"
              label="Quantity"
              inputMode="decimal"
              value={form.quantity}
              onChange={(e) => set("quantity")(e.target.value)}
            />
            <Field
              id="inv-price"
              label="Unit price (₹)"
              inputMode="decimal"
              value={form.unitPrice}
              onChange={(e) => set("unitPrice")(e.target.value)}
            />
          </div>
          <TextAreaField
            id="inv-notes"
            label="Notes"
            value={form.notes}
            onChange={(e) => set("notes")(e.target.value)}
          />
        </>
      )}
    </FormSheet>
  );
}
