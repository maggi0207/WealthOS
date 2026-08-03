import { useEffect, useState } from "react";
import { Loader2, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { Button } from "@/components/ui/button";
import {
  useCreateManualAsset,
  useDeleteManualAsset,
  useUpdateManualAsset,
} from "@/hooks/api/use-manual-assets";
import {
  parseOptionalNumber,
  parseRequiredNumber,
  requiredText,
  todayIsoDate,
  toastMutationError,
} from "@/lib/form-utils";
import {
  MANUAL_ASSET_TYPE_OPTIONS,
  type CreateManualAssetRequestDto,
  type ManualAssetDto,
} from "@/services/assets/types";

type FormState = {
  type: string;
  name: string;
  currentValue: string;
  purchaseValue: string;
  quantity: string;
  purchaseDate: string;
  institution: string;
  notes: string;
};

const emptyForm = (presetType?: string): FormState => ({
  type: presetType ?? "1",
  name: "",
  currentValue: "",
  purchaseValue: "",
  quantity: "",
  purchaseDate: todayIsoDate(),
  institution: "",
  notes: "",
});

function fromAsset(asset: ManualAssetDto): FormState {
  const typeVal =
    typeof asset.type === "number"
      ? String(asset.type)
      : (MANUAL_ASSET_TYPE_OPTIONS.find((o) => o.label === asset.typeLabel)?.value ?? "99");
  return {
    type: typeVal,
    name: asset.name,
    currentValue: String(asset.currentValue),
    purchaseValue: String(asset.purchaseValue),
    quantity: asset.quantity != null ? String(asset.quantity) : "",
    purchaseDate: asset.purchaseDate ?? "",
    institution: asset.institution ?? "",
    notes: asset.notes ?? "",
  };
}

function buildPayload(form: FormState): CreateManualAssetRequestDto | string {
  const nameErr = requiredText(form.name, "Asset name");
  if (nameErr) return nameErr;
  const current = parseRequiredNumber(form.currentValue, "Current value");
  if (typeof current === "string") return current;
  if (current < 0) return "Current value cannot be negative";
  const purchase = parseRequiredNumber(form.purchaseValue, "Purchase value");
  if (typeof purchase === "string") return purchase;
  if (purchase < 0) return "Purchase value cannot be negative";
  const quantity = parseOptionalNumber(form.quantity);
  if (form.quantity.trim() && quantity == null) return "Quantity must be a number";
  if (quantity != null && quantity <= 0) return "Quantity must be greater than zero";

  return {
    type: Number(form.type),
    name: form.name.trim(),
    currentValue: current,
    purchaseValue: purchase,
    quantity,
    purchaseDate: form.purchaseDate.trim() || null,
    institution: form.institution.trim() || null,
    notes: form.notes.trim() || null,
    currencyCode: "INR",
  };
}

export function ManualAssetFormSheet({
  open,
  onOpenChange,
  mode,
  asset,
  presetType,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  asset?: ManualAssetDto | null;
  presetType?: string;
}) {
  const createMutation = useCreateManualAsset();
  const updateMutation = useUpdateManualAsset();
  const deleteMutation = useDeleteManualAsset();
  const [form, setForm] = useState<FormState>(emptyForm(presetType));
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(mode === "edit" && asset ? fromAsset(asset) : emptyForm(presetType));
  }, [open, mode, asset, presetType]);

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
      if (mode === "edit" && asset) {
        await updateMutation.mutateAsync({ id: asset.id, body: payload });
        toast.success("Asset updated");
      } else {
        await createMutation.mutateAsync(payload);
        toast.success("Asset added");
      }
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not save asset");
    }
  }

  async function onDelete() {
    if (!asset) return;
    try {
      await deleteMutation.mutateAsync(asset.id);
      toast.success("Asset deleted");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not delete asset");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title={mode === "create" ? "Add asset" : "Edit asset"}
      description="Track cash, gold, vehicles and other holdings not managed elsewhere."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(mode === "edit" && asset ? fromAsset(asset) : emptyForm(presetType))}
      submitLabel={mode === "create" ? "Add asset" : "Save changes"}
      deleteSlot={
        mode === "edit" && asset ? (
          <ConfirmDialog
            title="Delete asset?"
            description="This removes the manual asset from your wealth summary."
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
                Delete asset
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

      <SelectField
        id="asset-type"
        label="Asset type"
        value={form.type}
        onChange={(e) => set("type")(e.target.value)}
        options={MANUAL_ASSET_TYPE_OPTIONS}
      />
      <Field
        id="asset-name"
        label="Asset name"
        value={form.name}
        onChange={(e) => set("name")(e.target.value)}
      />
      <div className="grid grid-cols-2 gap-3">
        <Field
          id="asset-current"
          label="Current value"
          inputMode="decimal"
          value={form.currentValue}
          onChange={(e) => set("currentValue")(e.target.value)}
        />
        <Field
          id="asset-purchase"
          label="Purchase value"
          inputMode="decimal"
          value={form.purchaseValue}
          onChange={(e) => set("purchaseValue")(e.target.value)}
        />
      </div>
      <div className="grid grid-cols-2 gap-3">
        <Field
          id="asset-qty"
          label="Quantity"
          inputMode="decimal"
          value={form.quantity}
          onChange={(e) => set("quantity")(e.target.value)}
          hint="Optional"
        />
        <Field
          id="asset-date"
          label="Purchase date"
          type="date"
          value={form.purchaseDate}
          onChange={(e) => set("purchaseDate")(e.target.value)}
        />
      </div>
      <Field
        id="asset-institution"
        label="Institution"
        value={form.institution}
        onChange={(e) => set("institution")(e.target.value)}
        hint="Optional — bank, dealer, or scheme"
      />
      <TextAreaField
        id="asset-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
      <p className="text-[11px] text-muted-foreground">
        Attachments will be available when document storage is linked to this asset.
      </p>
    </FormSheet>
  );
}
