import { useEffect, useState } from "react";
import { toast } from "sonner";

import { Field } from "@/components/ui-kit/field";
import { TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { useCreateExpense } from "@/hooks/api/use-income";
import {
  parseRequiredNumber,
  requiredText,
  todayIsoDate,
  toastMutationError,
} from "@/lib/form-utils";
import type { CreateExpenseRequestDto } from "@/services/income/requests";

type FormState = {
  categoryName: string;
  vendor: string;
  amount: string;
  paidOn: string;
  isRecurring: boolean;
  notes: string;
};

const emptyForm = (): FormState => ({
  categoryName: "",
  vendor: "",
  amount: "",
  paidOn: todayIsoDate(),
  isRecurring: false,
  notes: "",
});

function buildPayload(form: FormState): CreateExpenseRequestDto | string {
  const categoryErr = requiredText(form.categoryName, "Category");
  if (categoryErr) return categoryErr;
  const vendorErr = requiredText(form.vendor, "Vendor");
  if (vendorErr) return vendorErr;
  const amount = parseRequiredNumber(form.amount, "Amount");
  if (typeof amount === "string") return amount;
  if (amount <= 0) return "Amount must be greater than zero";
  if (!form.paidOn) return "Paid on date is required";

  return {
    categoryName: form.categoryName.trim(),
    vendor: form.vendor.trim(),
    amount,
    currencyCode: "INR",
    paidOn: form.paidOn,
    isRecurring: form.isRecurring,
    notes: form.notes.trim() || null,
  };
}

export function ExpenseFormSheet({
  open,
  onOpenChange,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const createMutation = useCreateExpense();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(emptyForm());
  }, [open]);

  const pending = createMutation.isPending;
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
      await createMutation.mutateAsync(payload);
      toast.success("Expense recorded");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not record expense");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title="Record expense"
      description="Log a business expense — tools, cloud, office and more."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(emptyForm())}
      submitLabel="Record expense"
    >
      {error ? (
        <p role="alert" className="rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {error}
        </p>
      ) : null}

      <Field
        id="exp-category"
        label="Category"
        value={form.categoryName}
        onChange={(e) => set("categoryName")(e.target.value)}
        hint="e.g. Cloud & hosting"
      />
      <Field
        id="exp-vendor"
        label="Vendor"
        value={form.vendor}
        onChange={(e) => set("vendor")(e.target.value)}
      />
      <Field
        id="exp-amount"
        label="Amount (₹)"
        inputMode="decimal"
        value={form.amount}
        onChange={(e) => set("amount")(e.target.value)}
      />
      <Field
        id="exp-paid"
        label="Paid on"
        type="date"
        value={form.paidOn}
        onChange={(e) => set("paidOn")(e.target.value)}
      />
      <label className="flex min-h-11 items-center gap-2 text-sm">
        <input
          type="checkbox"
          checked={form.isRecurring}
          onChange={(e) => set("isRecurring")(e.target.checked)}
          className="size-4 rounded border-input"
        />
        Recurring expense
      </label>
      <TextAreaField
        id="exp-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
    </FormSheet>
  );
}
