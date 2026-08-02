import { useEffect, useMemo, useState } from "react";
import { Loader2, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { Button } from "@/components/ui/button";
import {
  useCreateInvestmentAccount,
  useDeleteInvestmentAccount,
  useInvestmentProviders,
  useUpdateInvestmentAccount,
} from "@/hooks/api/use-investments";
import {
  requiredText,
  toastMutationError,
} from "@/lib/form-utils";
import type { InvestmentAccount } from "@/lib/investments-data";
import type {
  CreateInvestmentAccountRequestDto,
  InvestmentAccountStatusDto,
  UpdateInvestmentAccountRequestDto,
} from "@/services/investments/types";

const STATUS_OPTIONS = [
  { value: "0", label: "Manual" },
  { value: "1", label: "Connected" },
  { value: "2", label: "Coming soon" },
  { value: "3", label: "Disconnected" },
];

type FormState = {
  name: string;
  ownerName: string;
  kindLabel: string;
  status: string;
  notes: string;
};

const emptyForm = (): FormState => ({
  name: "",
  ownerName: "",
  kindLabel: "Manual portfolio",
  status: "0",
  notes: "",
});

function fromAccount(account: InvestmentAccount, notes = ""): FormState {
  const statusVal =
    account.status === "connected"
      ? "1"
      : account.status === "soon"
        ? "2"
        : "0";
  return {
    name: account.name,
    ownerName: account.owner,
    kindLabel: account.kind,
    status: statusVal,
    notes,
  };
}

export function AccountFormSheet({
  open,
  onOpenChange,
  mode,
  account,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  account?: InvestmentAccount | null;
}) {
  const { data: providers } = useInvestmentProviders();
  const createMutation = useCreateInvestmentAccount();
  const updateMutation = useUpdateInvestmentAccount();
  const deleteMutation = useDeleteInvestmentAccount();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const manualProviderId = useMemo(() => {
    const items = providers?.items ?? [];
    const manual =
      items.find((p) => p.name.toLowerCase() === "manual") ??
      items.find((p) => String(p.kind).toLowerCase().includes("manual")) ??
      items[0];
    return manual?.id ?? "a1111111-1111-2222-3333-444444444401";
  }, [providers]);

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(mode === "edit" && account ? fromAccount(account) : emptyForm());
  }, [open, mode, account]);

  const pending =
    createMutation.isPending || updateMutation.isPending || deleteMutation.isPending;

  const set =
    (key: keyof FormState) =>
    (value: string) =>
      setForm((prev) => ({ ...prev, [key]: value }));

  function buildPayload():
    | CreateInvestmentAccountRequestDto
    | UpdateInvestmentAccountRequestDto
    | string {
    const nameErr = requiredText(form.name, "Account name");
    if (nameErr) return nameErr;
    const ownerErr = requiredText(form.ownerName, "Owner name");
    if (ownerErr) return ownerErr;

    return {
      name: form.name.trim(),
      ownerName: form.ownerName.trim(),
      kindLabel: form.kindLabel.trim() || "Manual",
      status: Number(form.status) as InvestmentAccountStatusDto,
      currencyCode: "INR",
      notes: form.notes.trim() || null,
    };
  }

  async function onSubmit() {
    const payload = buildPayload();
    if (typeof payload === "string") {
      setError(payload);
      toast.error(payload);
      return;
    }
    setError(null);
    try {
      if (mode === "create") {
        await createMutation.mutateAsync({
          ...payload,
          providerId: manualProviderId,
        });
        toast.success("Investment account added");
      } else if (account) {
        await updateMutation.mutateAsync({ id: account.id, body: payload });
        toast.success("Account updated");
      }
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not save account");
    }
  }

  async function onDelete() {
    if (!account) return;
    try {
      await deleteMutation.mutateAsync(account.id);
      toast.success("Account deleted");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not delete account");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title={mode === "create" ? "Add account" : "Edit account"}
      description="Manual investment account — broker sync is optional."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(mode === "edit" && account ? fromAccount(account) : emptyForm())}
      submitLabel={mode === "create" ? "Add account" : "Save changes"}
      deleteSlot={
        mode === "edit" && account ? (
          <ConfirmDialog
            title="Delete account?"
            description="This removes the account and its holdings from your portfolio."
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
                Delete account
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

      <Field id="acct-name" label="Account name" value={form.name} onChange={(e) => set("name")(e.target.value)} />
      <Field
        id="acct-owner"
        label="Owner"
        value={form.ownerName}
        onChange={(e) => set("ownerName")(e.target.value)}
      />
      <Field
        id="acct-kind"
        label="Kind / label"
        value={form.kindLabel}
        onChange={(e) => set("kindLabel")(e.target.value)}
        hint="e.g. Broker · Stocks & MF, SGB & FD"
      />
      <SelectField
        id="acct-status"
        label="Status"
        value={form.status}
        onChange={(e) => set("status")(e.target.value)}
        options={STATUS_OPTIONS}
      />
      <TextAreaField
        id="acct-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
    </FormSheet>
  );
}
