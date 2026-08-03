import { Plus } from "lucide-react";
import { useState } from "react";

import { HoldingFormSheet } from "@/components/investments/holding-form-sheet";
import { PropertyFormSheet } from "@/components/property/property-form-sheet";
import { ManualAssetFormSheet } from "@/components/wealth/manual-asset-form-sheet";
import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from "@/components/ui/sheet";

type PickerOption = {
  id: string;
  label: string;
  hint: string;
  kind: "property" | "investment" | "manual";
  manualType?: string;
};

const OPTIONS: PickerOption[] = [
  { id: "property", label: "Property", hint: "Opens property form", kind: "property" },
  { id: "investment", label: "Investment", hint: "Opens investment form", kind: "investment" },
  { id: "gold", label: "Physical Gold", hint: "Bullion or coins", kind: "manual", manualType: "0" },
  { id: "cash", label: "Cash", hint: "Physical cash on hand", kind: "manual", manualType: "1" },
  { id: "bank", label: "Bank Account", hint: "Savings or current", kind: "manual", manualType: "2" },
  { id: "fd", label: "Fixed Deposit", hint: "Bank or NBFC FD", kind: "manual", manualType: "3" },
  { id: "vehicle", label: "Vehicle", hint: "Car, bike, or other", kind: "manual", manualType: "4" },
  { id: "jewellery", label: "Jewellery", hint: "Gold / diamond jewellery", kind: "manual", manualType: "5" },
  { id: "retirement", label: "PPF / EPF / NPS", hint: "Retirement accounts", kind: "manual", manualType: "6" },
  { id: "crypto", label: "Crypto", hint: "Digital assets", kind: "manual", manualType: "9" },
  { id: "other", label: "Other", hint: "Collectibles and more", kind: "manual", manualType: "99" },
];

/**
 * Sticky FAB — type selector that routes to Property, Investment, or Manual asset forms.
 */
export function AddAssetFab({ label = "Add asset" }: { label?: string }) {
  const [pickerOpen, setPickerOpen] = useState(false);
  const [propertyOpen, setPropertyOpen] = useState(false);
  const [investmentOpen, setInvestmentOpen] = useState(false);
  const [manualOpen, setManualOpen] = useState(false);
  const [manualType, setManualType] = useState("1");

  function onPick(option: PickerOption) {
    setPickerOpen(false);
    if (option.kind === "property") {
      setPropertyOpen(true);
      return;
    }
    if (option.kind === "investment") {
      setInvestmentOpen(true);
      return;
    }
    setManualType(option.manualType ?? "99");
    setManualOpen(true);
  }

  return (
    <>
      <div className="pointer-events-none sticky bottom-0 z-30 flex justify-end">
        <button
          type="button"
          onClick={() => setPickerOpen(true)}
          className="press pointer-events-auto mb-[calc(5.5rem+env(safe-area-inset-bottom))] inline-flex h-12 items-center gap-2 rounded-full bg-primary px-5 text-[14px] font-semibold text-primary-foreground shadow-lg shadow-primary/25 md:mb-6"
        >
          <Plus className="size-5" />
          {label}
        </button>
      </div>

      <Sheet open={pickerOpen} onOpenChange={setPickerOpen}>
        <SheetContent
          side="bottom"
          className="rounded-t-3xl border-border pb-[calc(1.25rem+env(safe-area-inset-bottom))]"
        >
          <SheetHeader className="text-left">
            <SheetTitle className="font-display text-lg">Add asset</SheetTitle>
            <SheetDescription>
              Property and investments open their existing forms. Everything else is a manual asset.
            </SheetDescription>
          </SheetHeader>

          <div className="mt-4 grid max-h-[60vh] grid-cols-2 gap-2.5 overflow-y-auto pr-1">
            {OPTIONS.map((option) => (
              <button
                key={option.id}
                type="button"
                onClick={() => onPick(option)}
                className="surface-tile press min-h-[68px] p-3 text-left"
              >
                <p className="text-[14px] font-semibold">{option.label}</p>
                <p className="mt-0.5 text-[11px] leading-snug text-muted-foreground">{option.hint}</p>
              </button>
            ))}
          </div>
        </SheetContent>
      </Sheet>

      <PropertyFormSheet open={propertyOpen} onOpenChange={setPropertyOpen} mode="create" />
      <HoldingFormSheet open={investmentOpen} onOpenChange={setInvestmentOpen} mode="create" preset="manual" />
      <ManualAssetFormSheet
        open={manualOpen}
        onOpenChange={setManualOpen}
        mode="create"
        presetType={manualType}
      />
    </>
  );
}
