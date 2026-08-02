import { useEffect, useMemo, useState } from "react";
import { Loader2, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { Field } from "@/components/ui-kit/field";
import { SelectField, TextAreaField } from "@/components/ui-kit/form-fields";
import { FormSheet } from "@/components/ui-kit/form-sheet";
import { Button } from "@/components/ui/button";
import {
  useAddManualHolding,
  useDeleteHolding,
  useInvestmentsOverview,
  useUpdateHolding,
} from "@/hooks/api/use-investments";
import {
  parseRequiredNumber,
  requiredText,
  toastMutationError,
} from "@/lib/form-utils";
import type { Holding } from "@/lib/investments-data";
import type {
  AddManualHoldingRequestDto,
  InvestmentCategoryDto,
  InvestmentTypeDto,
  UpdateHoldingRequestDto,
} from "@/services/investments/types";

const CATEGORY_OPTIONS = [
  { value: "0", label: "Stocks" },
  { value: "1", label: "Mutual Funds" },
  { value: "2", label: "Corporate Bonds" },
  { value: "3", label: "Gold ETFs" },
  { value: "4", label: "Cash / FD" },
  { value: "99", label: "Other" },
];

const TYPE_OPTIONS = [
  { value: "0", label: "Equity" },
  { value: "1", label: "Mutual fund" },
  { value: "2", label: "Bond" },
  { value: "3", label: "ETF" },
  { value: "4", label: "Gold" },
  { value: "5", label: "Fixed deposit" },
  { value: "7", label: "Cash" },
  { value: "99", label: "Other" },
];

export type HoldingPreset = "stock" | "mutual-fund" | "bond" | "gold" | "fd" | "manual";

const PRESET_MAP: Record<HoldingPreset, { category: string; type: string }> = {
  stock: { category: "0", type: "0" },
  "mutual-fund": { category: "1", type: "1" },
  bond: { category: "2", type: "2" },
  gold: { category: "3", type: "4" },
  fd: { category: "4", type: "5" },
  manual: { category: "99", type: "99" },
};

function categoryFromHolding(category: Holding["category"]): string {
  if (category === "Mutual Funds") return "1";
  if (category === "Corporate Bonds") return "2";
  if (category === "Gold ETFs") return "3";
  if (category === "Cash") return "4";
  return "0";
}

type FormState = {
  accountId: string;
  name: string;
  symbol: string;
  category: string;
  investmentType: string;
  quantity: string;
  investedAmount: string;
  currentValue: string;
  notes: string;
};

const emptyForm = (preset?: HoldingPreset, defaultAccountId = ""): FormState => {
  const p = preset ? PRESET_MAP[preset] : { category: "0", type: "0" };
  return {
    accountId: defaultAccountId,
    name: "",
    symbol: "",
    category: p.category,
    investmentType: p.type,
    quantity: "1",
    investedAmount: "",
    currentValue: "",
    notes: "",
  };
};

function fromHolding(holding: Holding): FormState {
  return {
    accountId: holding.accountId,
    name: holding.name,
    symbol: holding.ticker,
    category: categoryFromHolding(holding.category),
    investmentType: categoryFromHolding(holding.category) === "1" ? "1" : "0",
    quantity: "1",
    investedAmount: String(holding.invested),
    currentValue: String(holding.value),
    notes: "",
  };
}

export function HoldingFormSheet({
  open,
  onOpenChange,
  mode,
  holding,
  preset,
  defaultAccountId,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  mode: "create" | "edit";
  holding?: Holding | null;
  preset?: HoldingPreset;
  defaultAccountId?: string;
}) {
  const { data: overview } = useInvestmentsOverview();
  const createMutation = useAddManualHolding();
  const updateMutation = useUpdateHolding();
  const deleteMutation = useDeleteHolding();
  const [form, setForm] = useState<FormState>(emptyForm(preset, defaultAccountId));
  const [error, setError] = useState<string | null>(null);

  const accountOptions = useMemo(
    () =>
      (overview?.accounts ?? []).map((a) => ({
        value: a.id,
        label: `${a.name} (${a.owner})`,
      })),
    [overview?.accounts],
  );

  useEffect(() => {
    if (!open) return;
    setError(null);
    if (mode === "edit" && holding) {
      setForm(fromHolding(holding));
    } else {
      const firstAccount = overview?.accounts[0]?.id ?? "";
      setForm(emptyForm(preset, defaultAccountId || firstAccount));
    }
  }, [open, mode, holding, preset, defaultAccountId, overview?.accounts]);

  const pending =
    createMutation.isPending || updateMutation.isPending || deleteMutation.isPending;

  const set =
    (key: keyof FormState) =>
    (value: string) =>
      setForm((prev) => ({ ...prev, [key]: value }));

  function buildHoldingBody():
    | Omit<AddManualHoldingRequestDto, "accountId">
    | string {
    const nameErr = requiredText(form.name, "Holding name");
    if (nameErr) return nameErr;
    const qty = parseRequiredNumber(form.quantity, "Quantity");
    if (typeof qty === "string") return qty;
    const invested = parseRequiredNumber(form.investedAmount, "Invested amount");
    if (typeof invested === "string") return invested;
    const current = parseRequiredNumber(form.currentValue, "Current value");
    if (typeof current === "string") return current;
    if (qty <= 0) return "Quantity must be greater than zero";

    const averageCost = invested / qty;
    const currentPrice = current / qty;

    return {
      name: form.name.trim(),
      symbol: form.symbol.trim().toUpperCase() || form.name.trim().slice(0, 12).toUpperCase(),
      category: Number(form.category) as InvestmentCategoryDto,
      investmentType: Number(form.investmentType) as InvestmentTypeDto,
      quantity: qty,
      averageCost,
      investedAmount: invested,
      currentPrice,
      currentValue: current,
      dayChange: 0,
      dayChangePercent: 0,
      currencyCode: "INR",
      notes: form.notes.trim() || null,
    };
  }

  async function onSubmit() {
    if (mode === "create" && !form.accountId) {
      const msg = "Select an investment account";
      setError(msg);
      toast.error(msg);
      return;
    }
    const body = buildHoldingBody();
    if (typeof body === "string") {
      setError(body);
      toast.error(body);
      return;
    }
    setError(null);
    try {
      if (mode === "create") {
        await createMutation.mutateAsync({
          accountId: form.accountId,
          ...body,
        });
        toast.success("Holding added");
      } else if (holding) {
        const updateBody: UpdateHoldingRequestDto = body;
        await updateMutation.mutateAsync({ id: holding.id, body: updateBody });
        toast.success("Holding updated");
      }
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not save holding");
    }
  }

  async function onDelete() {
    if (!holding) return;
    try {
      await deleteMutation.mutateAsync(holding.id);
      toast.success("Holding deleted");
      onOpenChange(false);
    } catch (err) {
      toastMutationError(err, "Could not delete holding");
    }
  }

  return (
    <FormSheet
      open={open}
      onOpenChange={onOpenChange}
      title={mode === "create" ? "Add holding" : "Edit holding"}
      description="Manual holding — quantities and values you enter here."
      pending={pending}
      onSubmit={() => void onSubmit()}
      onReset={() =>
        setForm(
          mode === "edit" && holding
            ? fromHolding(holding)
            : emptyForm(preset, form.accountId),
        )
      }
      submitLabel={mode === "create" ? "Add holding" : "Save changes"}
      deleteSlot={
        mode === "edit" && holding ? (
          <ConfirmDialog
            title="Delete holding?"
            description="This removes the holding from your portfolio."
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
                Delete holding
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

      {mode === "create" ? (
        <SelectField
          id="hold-account"
          label="Account"
          value={form.accountId}
          onChange={(e) => set("accountId")(e.target.value)}
          options={
            accountOptions.length > 0
              ? accountOptions
              : [{ value: "", label: "No accounts — add one first" }]
          }
        />
      ) : null}

      <Field id="hold-name" label="Name" value={form.name} onChange={(e) => set("name")(e.target.value)} />
      <Field
        id="hold-symbol"
        label="Symbol / ticker"
        value={form.symbol}
        onChange={(e) => set("symbol")(e.target.value)}
      />
      <div className="grid grid-cols-2 gap-2.5">
        <SelectField
          id="hold-category"
          label="Category"
          value={form.category}
          onChange={(e) => set("category")(e.target.value)}
          options={CATEGORY_OPTIONS}
        />
        <SelectField
          id="hold-type"
          label="Instrument type"
          value={form.investmentType}
          onChange={(e) => set("investmentType")(e.target.value)}
          options={TYPE_OPTIONS}
        />
      </div>
      <Field
        id="hold-qty"
        label="Quantity"
        inputMode="decimal"
        value={form.quantity}
        onChange={(e) => set("quantity")(e.target.value)}
      />
      <div className="grid grid-cols-2 gap-2.5">
        <Field
          id="hold-invested"
          label="Invested (₹)"
          inputMode="decimal"
          value={form.investedAmount}
          onChange={(e) => set("investedAmount")(e.target.value)}
        />
        <Field
          id="hold-value"
          label="Current value (₹)"
          inputMode="decimal"
          value={form.currentValue}
          onChange={(e) => set("currentValue")(e.target.value)}
        />
      </div>
      <TextAreaField
        id="hold-notes"
        label="Notes"
        value={form.notes}
        onChange={(e) => set("notes")(e.target.value)}
      />
    </FormSheet>
  );
}
