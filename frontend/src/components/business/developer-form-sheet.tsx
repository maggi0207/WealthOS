import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";

import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { useCreateDeveloper, useIncomeOverview } from "@/hooks/api/use-income";
import {
  parseRequiredNumber,
  requiredText,
  toastMutationError,
} from "@/lib/form-utils";
import type { CreateDeveloperRequestDto } from "@/services/income/requests";

type FormState = {
  name: string;
  role: string;
  monthlySalary: string;
  primaryClientId: string;
  notes: string;
};

const emptyForm = (): FormState => ({
  name: "",
  role: "",
  monthlySalary: "",
  primaryClientId: "",
  notes: "",
});

function buildPayload(form: FormState): CreateDeveloperRequestDto | string {
  const nameErr = requiredText(form.name, "Name");
  if (nameErr) return nameErr;
  const roleErr = requiredText(form.role, "Role");
  if (roleErr) return roleErr;
  const salary = parseRequiredNumber(form.monthlySalary, "Monthly salary");
  if (typeof salary === "string") return salary;
  if (salary <= 0) return "Monthly salary must be greater than zero";

  return {
    name: form.name.trim(),
    role: form.role.trim(),
    monthlySalary: salary,
    currencyCode: "INR",
    primaryClientId: form.primaryClientId || null,
    notes: form.notes.trim() || null,
  };
}

export function DeveloperFormSheet({
  open,
  onOpenChange,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { data: overview } = useIncomeOverview();
  const createMutation = useCreateDeveloper();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const clientOptions = useMemo(
    () => [
      { value: "", label: "Unassigned" },
      ...(overview?.clients ?? []).map((c) => ({
        value: c.id,
        label: c.name,
      })),
    ],
    [overview?.clients],
  );

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(emptyForm());
  }, [open]);

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
      toast.success("Developer added");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not add developer");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title="Add developer"
      description="Add a team member for payroll tracking."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(emptyForm())}
      submitLabel="Add developer"
    >
      {error ? (
        <p role="alert" className="rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {error}
        </p>
      ) : null}

      <Field
        id="dev-name"
        label="Name"
        value={form.name}
        onChange={(e) => set("name")(e.target.value)}
      />
      <Field
        id="dev-role"
        label="Role"
        value={form.role}
        onChange={(e) => set("role")(e.target.value)}
      />
      <Field
        id="dev-salary"
        label="Monthly salary (₹)"
        inputMode="decimal"
        value={form.monthlySalary}
        onChange={(e) => set("monthlySalary")(e.target.value)}
      />
      <SelectField
        id="dev-client"
        label="Primary client"
        value={form.primaryClientId}
        onChange={(e) => set("primaryClientId")(e.target.value)}
        options={clientOptions}
      />
      <TextAreaField
        id="dev-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
    </FormSheet>
  );
}
