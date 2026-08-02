import { useEffect, useState } from "react";
import { Loader2, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { Button } from "@/components/ui/button";
import {
  useCreateClient,
  useDeleteClient,
  useUpdateClient,
} from "@/hooks/api/use-income";
import type { BusinessClient } from "@/lib/business-data";
import {
  parseRequiredNumber,
  requiredText,
  toastMutationError,
} from "@/lib/form-utils";
import type {
  ClientStatusDto,
  CreateClientRequestDto,
  UpdateClientRequestDto,
} from "@/services/income/requests";

const STATUS_OPTIONS = [
  { value: "0", label: "Active" },
  { value: "1", label: "Paused" },
];

type FormState = {
  name: string;
  engagement: string;
  status: string;
  monthlyRevenue: string;
  contactEmail: string;
  contactPhone: string;
  notes: string;
};

const emptyForm = (): FormState => ({
  name: "",
  engagement: "",
  status: "0",
  monthlyRevenue: "",
  contactEmail: "",
  contactPhone: "",
  notes: "",
});

function fromClient(client: BusinessClient): FormState {
  return {
    name: client.name,
    engagement: client.engagement,
    status: client.status === "paused" ? "1" : "0",
    monthlyRevenue: String(client.monthlyRevenue),
    contactEmail: "",
    contactPhone: "",
    notes: "",
  };
}

function buildPayload(form: FormState): CreateClientRequestDto | string {
  const nameErr = requiredText(form.name, "Client name");
  if (nameErr) return nameErr;
  const engagementErr = requiredText(form.engagement, "Engagement");
  if (engagementErr) return engagementErr;
  const revenue = parseRequiredNumber(form.monthlyRevenue, "Monthly revenue");
  if (typeof revenue === "string") return revenue;
  if (revenue < 0) return "Monthly revenue cannot be negative";

  return {
    name: form.name.trim(),
    engagement: form.engagement.trim(),
    status: Number(form.status) as ClientStatusDto,
    monthlyRevenue: revenue,
    currencyCode: "INR",
    contactEmail: form.contactEmail.trim() || null,
    contactPhone: form.contactPhone.trim() || null,
    notes: form.notes.trim() || null,
  };
}

export function ClientFormSheet({
  open,
  onOpenChange,
  mode,
  client,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  client?: BusinessClient | null;
}) {
  const createMutation = useCreateClient();
  const updateMutation = useUpdateClient();
  const deleteMutation = useDeleteClient();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(mode === "edit" && client ? fromClient(client) : emptyForm());
  }, [open, mode, client]);

  const pending =
    createMutation.isPending || updateMutation.isPending || deleteMutation.isPending;

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
      if (mode === "create") {
        await createMutation.mutateAsync(payload);
        toast.success("Client added");
      } else if (client) {
        const body: UpdateClientRequestDto = payload;
        await updateMutation.mutateAsync({ id: client.id, body });
        toast.success("Client updated");
      }
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not save client");
    }
  }

  async function onDelete() {
    if (!client) return;
    try {
      await deleteMutation.mutateAsync(client.id);
      toast.success("Client deleted");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not delete client");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title={mode === "create" ? "Add client" : "Edit client"}
      description="Track retainer or project revenue for a business client."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(mode === "edit" && client ? fromClient(client) : emptyForm())}
      submitLabel={mode === "create" ? "Add client" : "Save changes"}
      deleteSlot={
        mode === "edit" && client ? (
          <ConfirmDialog
            title="Delete client?"
            description="This removes the client and related project data. This cannot be undone."
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
                Delete client
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

      <Field
        id="client-name"
        label="Client name"
        value={form.name}
        onChange={(e) => set("name")(e.target.value)}
      />
      <Field
        id="client-engagement"
        label="Engagement"
        value={form.engagement}
        onChange={(e) => set("engagement")(e.target.value)}
        hint="e.g. Retainer · Web platform"
      />
      <div className="grid grid-cols-2 gap-2.5">
        <SelectField
          id="client-status"
          label="Status"
          value={form.status}
          onChange={(e) => set("status")(e.target.value)}
          options={STATUS_OPTIONS}
        />
        <Field
          id="client-revenue"
          label="Monthly revenue (₹)"
          inputMode="decimal"
          value={form.monthlyRevenue}
          onChange={(e) => set("monthlyRevenue")(e.target.value)}
        />
      </div>
      <Field
        id="client-email"
        label="Contact email"
        type="email"
        value={form.contactEmail}
        onChange={(e) => set("contactEmail")(e.target.value)}
      />
      <Field
        id="client-phone"
        label="Contact phone"
        value={form.contactPhone}
        onChange={(e) => set("contactPhone")(e.target.value)}
      />
      <TextAreaField
        id="client-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
    </FormSheet>
  );
}
