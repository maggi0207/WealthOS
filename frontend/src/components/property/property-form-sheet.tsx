import { useEffect, useMemo, useState } from "react";
import { Loader2, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { Button } from "@/components/ui/button";
import {
  useCreateProperty,
  useDeleteProperty,
  useUpdateProperty,
} from "@/hooks/api/use-properties";
import { dashboardKeys } from "@/hooks/api/use-dashboard";
import {
  parseOptionalNumber,
  parseRequiredNumber,
  requiredText,
  toastMutationError,
} from "@/lib/form-utils";
import { useQueryClient } from "@tanstack/react-query";
import type {
  CreatePropertyRequestDto,
  PropertyDetailView,
  PropertyStatusDto,
  PropertyTypeDto,
  UpdatePropertyRequestDto,
} from "@/services/properties/types";

const TYPE_OPTIONS = [
  { value: "3", label: "Apartment" },
  { value: "0", label: "Residential" },
  { value: "4", label: "Villa" },
  { value: "1", label: "Commercial" },
  { value: "2", label: "Land" },
  { value: "5", label: "Plot" },
];

const STATUS_OPTIONS = [
  { value: "0", label: "Active" },
  { value: "2", label: "Rented" },
  { value: "1", label: "Under construction" },
  { value: "3", label: "For sale" },
  { value: "4", label: "Sold" },
  { value: "5", label: "Inactive" },
];

type FormState = {
  name: string;
  type: string;
  status: string;
  purchasePrice: string;
  currentMarketValue: string;
  purchaseDate: string;
  area: string;
  builtUpArea: string;
  floor: string;
  facing: string;
  bedrooms: string;
  bathrooms: string;
  parking: string;
  line1: string;
  locality: string;
  city: string;
  state: string;
  postalCode: string;
  googleMapsUrl: string;
  notes: string;
  description: string;
};

const emptyForm = (): FormState => ({
  name: "",
  type: "3",
  status: "0",
  purchasePrice: "",
  currentMarketValue: "",
  purchaseDate: "",
  area: "",
  builtUpArea: "",
  floor: "",
  facing: "",
  bedrooms: "",
  bathrooms: "",
  parking: "",
  line1: "",
  locality: "",
  city: "",
  state: "",
  postalCode: "",
  googleMapsUrl: "",
  notes: "",
  description: "",
});

function fromDetail(detail: PropertyDetailView): FormState {
  const typeVal =
    TYPE_OPTIONS.find((o) =>
      detail.typeLabel?.toLowerCase().includes(o.label.toLowerCase()),
    )?.value ?? "3";
  const statusVal =
    STATUS_OPTIONS.find((o) =>
      detail.statusLabel?.toLowerCase().includes(o.label.toLowerCase().split(" ")[0]!),
    )?.value ?? "0";

  return {
    name: detail.name ?? "",
    type: typeVal,
    status: statusVal,
    purchasePrice: String(detail.purchasePrice ?? ""),
    currentMarketValue: String(detail.currentValue ?? ""),
    purchaseDate: "",
    area: detail.area != null ? String(detail.area) : "",
    builtUpArea: detail.builtUpArea != null ? String(detail.builtUpArea) : "",
    floor: detail.floor ?? "",
    facing: detail.facing ?? "",
    bedrooms: detail.bedrooms != null ? String(detail.bedrooms) : "",
    bathrooms: detail.bathrooms != null ? String(detail.bathrooms) : "",
    parking: detail.parking != null ? String(detail.parking) : "",
    line1: detail.addressLines?.[0] ?? detail.address ?? "",
    locality: detail.locality ?? "",
    city: detail.city ?? "",
    state: detail.state ?? "",
    postalCode: detail.postalCode ?? "",
    googleMapsUrl: detail.googleMapsUrl ?? "",
    notes: "",
    description: detail.description ?? "",
  };
}

function buildPayload(form: FormState): CreatePropertyRequestDto | string {
  const nameErr = requiredText(form.name, "Property name");
  if (nameErr) return nameErr;
  const purchase = parseRequiredNumber(form.purchasePrice, "Purchase price");
  if (typeof purchase === "string") return purchase;
  const market = parseRequiredNumber(form.currentMarketValue, "Market value");
  if (typeof market === "string") return market;

  return {
    name: form.name.trim(),
    type: Number(form.type) as PropertyTypeDto,
    ownershipType: 0,
    purchaseDate: form.purchaseDate || null,
    purchasePrice: purchase,
    currentMarketValue: market,
    currencyCode: "INR",
    area: parseOptionalNumber(form.area),
    builtUpArea: parseOptionalNumber(form.builtUpArea),
    floor: form.floor.trim() || null,
    facing: form.facing.trim() || null,
    bedrooms: parseOptionalNumber(form.bedrooms),
    bathrooms: parseOptionalNumber(form.bathrooms),
    parking: parseOptionalNumber(form.parking),
    status: Number(form.status) as PropertyStatusDto,
    description: form.description.trim() || null,
    notes: form.notes.trim() || null,
    isRentalEnabled: Number(form.status) === 2,
    address: {
      line1: form.line1.trim() || null,
      locality: form.locality.trim() || null,
      city: form.city.trim() || null,
      state: form.state.trim() || null,
      postalCode: form.postalCode.trim() || null,
      country: "India",
      googleMapsUrl: form.googleMapsUrl.trim() || null,
    },
  };
}

export function PropertyFormSheet({
  open,
  onOpenChange,
  mode,
  propertyId,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  propertyId?: string;
  initial?: PropertyDetailView | null;
}) {
  const queryClient = useQueryClient();
  const createMutation = useCreateProperty();
  const updateMutation = useUpdateProperty();
  const deleteMutation = useDeleteProperty();
  const [form, setForm] = useState<FormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open) return;
    setError(null);
    setForm(mode === "edit" && initial ? fromDetail(initial) : emptyForm());
  }, [open, mode, initial]);

  const pending =
    createMutation.isPending || updateMutation.isPending || deleteMutation.isPending;

  const title = mode === "create" ? "Add property" : "Edit property";

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
        toast.success("Property added");
      } else if (propertyId) {
        const body: UpdatePropertyRequestDto = {
          ...payload,
          ownershipType: 0,
          status: payload.status ?? 0,
        };
        await updateMutation.mutateAsync({ id: propertyId, body });
        toast.success("Property updated");
      }
      void queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not save property");
    }
  }

  async function onDelete() {
    if (!propertyId) return;
    try {
      await deleteMutation.mutateAsync(propertyId);
      toast.success("Property deleted");
      void queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not delete property");
    }
  }

  const mapsHint = useMemo(
    () =>
      form.googleMapsUrl
        ? "Maps link will open in a new tab from the location card."
        : "Paste a Google Maps URL for this property.",
    [form.googleMapsUrl],
  );

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title={title}
      description="Manual entry — no broker integrations required."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() => setForm(mode === "edit" && initial ? fromDetail(initial) : emptyForm())}
      submitLabel={mode === "create" ? "Add property" : "Save changes"}
      deleteSlot={
        mode === "edit" && propertyId ? (
          <ConfirmDialog
            title="Delete property?"
            description="This removes the property from your portfolio. This cannot be undone."
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
                Delete property
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

      <Field id="prop-name" label="Name" value={form.name} onChange={(e) => set("name")(e.target.value)} />
      <div className="grid grid-cols-2 gap-2.5">
        <SelectField
          id="prop-type"
          label="Type"
          value={form.type}
          onChange={(e) => set("type")(e.target.value)}
          options={TYPE_OPTIONS}
        />
        <SelectField
          id="prop-status"
          label="Status"
          value={form.status}
          onChange={(e) => set("status")(e.target.value)}
          options={STATUS_OPTIONS}
        />
      </div>
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="prop-purchase"
          label="Purchase price (₹)"
          inputMode="decimal"
          value={form.purchasePrice}
          onChange={(e) => set("purchasePrice")(e.target.value)}
        />
        <Field
          id="prop-market"
          label="Market value (₹)"
          inputMode="decimal"
          value={form.currentMarketValue}
          onChange={(e) => set("currentMarketValue")(e.target.value)}
        />
      </div>
      <Field
        id="prop-date"
        label="Purchase date"
        type="date"
        value={form.purchaseDate}
        onChange={(e) => set("purchaseDate")(e.target.value)}
      />
      <div className="grid grid-cols-2 gap-2.5">
        <Field id="prop-area" label="Area (sq ft)" inputMode="decimal" value={form.area} onChange={(e) => set("area")(e.target.value)} />
        <Field id="prop-built" label="Built-up" inputMode="decimal" value={form.builtUpArea} onChange={(e) => set("builtUpArea")(e.target.value)} />
      </div>
      <div className="grid grid-cols-3 gap-2.5">
        <Field id="prop-floor" label="Floor" value={form.floor} onChange={(e) => set("floor")(e.target.value)} />
        <Field id="prop-beds" label="Beds" inputMode="numeric" value={form.bedrooms} onChange={(e) => set("bedrooms")(e.target.value)} />
        <Field id="prop-baths" label="Baths" inputMode="numeric" value={form.bathrooms} onChange={(e) => set("bathrooms")(e.target.value)} />
      </div>
      <Field id="prop-line1" label="Address line" value={form.line1} onChange={(e) => set("line1")(e.target.value)} />
      <div className="grid grid-cols-2 gap-2.5">
        <Field id="prop-locality" label="Locality" value={form.locality} onChange={(e) => set("locality")(e.target.value)} />
        <Field id="prop-city" label="City" value={form.city} onChange={(e) => set("city")(e.target.value)} />
      </div>
      <div className="grid grid-cols-2 gap-2.5">
        <Field id="prop-state" label="State" value={form.state} onChange={(e) => set("state")(e.target.value)} />
        <Field id="prop-pin" label="PIN" value={form.postalCode} onChange={(e) => set("postalCode")(e.target.value)} />
      </div>
      <Field
        id="prop-maps"
        label="Google Maps URL"
        value={form.googleMapsUrl}
        onChange={(e) => set("googleMapsUrl")(e.target.value)}
        hint={mapsHint}
      />
      <TextAreaField
        id="prop-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
      <p className="text-[11px] text-muted-foreground">
        Photo upload is a placeholder — attach media from Documents after saving.
      </p>
    </FormSheet>
  );
}
