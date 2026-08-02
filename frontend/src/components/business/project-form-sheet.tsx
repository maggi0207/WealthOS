import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";

import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { useCreateProject, useIncomeOverview } from "@/hooks/api/use-income";
import {
  parseOptionalNumber,
  requiredText,
  todayIsoDate,
  toastMutationError,
} from "@/lib/form-utils";
import type { CreateProjectRequestDto, ProjectStatusDto } from "@/services/income/requests";

const STATUS_OPTIONS = [
  { value: "0", label: "Active" },
  { value: "1", label: "On hold" },
  { value: "2", label: "Completed" },
  { value: "3", label: "Cancelled" },
];

type FormState = {
  clientId: string;
  name: string;
  description: string;
  status: string;
  startDate: string;
  monthlyRevenue: string;
};

const emptyForm = (): FormState => ({
  clientId: "",
  name: "",
  description: "",
  status: "0",
  startDate: todayIsoDate(),
  monthlyRevenue: "",
});

function buildPayload(form: FormState): CreateProjectRequestDto | string {
  if (!form.clientId) return "Client is required";
  const nameErr = requiredText(form.name, "Project name");
  if (nameErr) return nameErr;
  if (!form.startDate) return "Start date is required";

  return {
    clientId: form.clientId,
    name: form.name.trim(),
    description: form.description.trim() || null,
    status: Number(form.status) as ProjectStatusDto,
    startDate: form.startDate,
    monthlyRevenue: parseOptionalNumber(form.monthlyRevenue),
    currencyCode: "INR",
  };
}

export function ProjectFormSheet({
  open,
  onOpenChange,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const { data: overview } = useIncomeOverview();
  const createMutation = useCreateProject();
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
      toast.success("Project created");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not create project");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title="Add project"
      description="Create a project under an existing client."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() =>
        setForm({ ...emptyForm(), clientId: clientOptions[0]?.value ?? "" })
      }
      submitLabel="Add project"
    >
      {error ? (
        <p role="alert" className="rounded-xl bg-destructive/10 px-3 py-2 text-sm text-destructive">
          {error}
        </p>
      ) : null}

      {clientOptions.length === 0 ? (
        <p className="text-sm text-muted-foreground">Add a client first to create a project.</p>
      ) : (
        <>
          <SelectField
            id="project-client"
            label="Client"
            value={form.clientId}
            onChange={(e) => set("clientId")(e.target.value)}
            options={clientOptions}
          />
          <Field
            id="project-name"
            label="Project name"
            value={form.name}
            onChange={(e) => set("name")(e.target.value)}
          />
          <div className="grid grid-cols-2 gap-2.5">
            <SelectField
              id="project-status"
              label="Status"
              value={form.status}
              onChange={(e) => set("status")(e.target.value)}
              options={STATUS_OPTIONS}
            />
            <Field
              id="project-start"
              label="Start date"
              type="date"
              value={form.startDate}
              onChange={(e) => set("startDate")(e.target.value)}
            />
          </div>
          <Field
            id="project-revenue"
            label="Monthly revenue (₹)"
            inputMode="decimal"
            value={form.monthlyRevenue}
            onChange={(e) => set("monthlyRevenue")(e.target.value)}
            hint="Optional"
          />
          <TextAreaField
            id="project-desc"
            label="Description"
            value={form.description}
            onChange={(e) => set("description")(e.target.value)}
          />
        </>
      )}
    </FormSheet>
  );
}
