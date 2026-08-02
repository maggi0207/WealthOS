import { Plus } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

import { Sheet, SheetContent, SheetDescription, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { addInvestmentOptions } from "@/lib/investments-data";

/**
 * Sticky floating action that opens a bottom sheet of investment types.
 * Sits above the mobile bottom nav and the iOS home indicator.
 */
export function AddInvestmentFab() {
  const [open, setOpen] = useState(false);

  return (
    <>
      <div className="pointer-events-none sticky bottom-0 z-30 flex justify-end">
        <button
          type="button"
          onClick={() => setOpen(true)}
          className="press pointer-events-auto mb-[calc(5.5rem+env(safe-area-inset-bottom))] inline-flex h-12 items-center gap-2 rounded-full bg-primary px-5 text-[14px] font-semibold text-primary-foreground shadow-lg shadow-primary/25 md:mb-6"
        >
          <Plus className="size-5" />
          Add investment
        </button>
      </div>

      <Sheet open={open} onOpenChange={setOpen}>
        <SheetContent
          side="bottom"
          className="rounded-t-3xl border-border pb-[calc(1.25rem+env(safe-area-inset-bottom))]"
        >
          <SheetHeader className="text-left">
            <SheetTitle className="font-display text-lg">Add investment</SheetTitle>
            <SheetDescription>Pick what you want to track. Entry forms arrive with the editor.</SheetDescription>
          </SheetHeader>

          <div className="mt-4 grid grid-cols-2 gap-2.5">
            {addInvestmentOptions.map((option) => (
              <button
                key={option.id}
                type="button"
                onClick={() => {
                  setOpen(false);
                  toast.success(`${option.label} — coming with the investment editor`);
                }}
                className="surface-tile press min-h-[68px] p-3 text-left"
              >
                <p className="text-[14px] font-semibold">{option.label}</p>
                <p className="mt-0.5 text-[11px] leading-snug text-muted-foreground">{option.hint}</p>
              </button>
            ))}
          </div>
        </SheetContent>
      </Sheet>
    </>
  );
}
