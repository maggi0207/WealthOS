import { Plus } from "lucide-react";
import { useState } from "react";

import { LoanFormSheet } from "@/components/loans/loan-form-sheet";
import type { LoanAccount } from "@/lib/loans-data";

/** Sticky FAB for adding loans; edit opens via selected loan actions. */
export function AddLoanFab({
  editLoan,
  onEditConsumed,
}: {
  editLoan?: LoanAccount | null;
  onEditConsumed?: () => void;
}) {
  const [createOpen, setCreateOpen] = useState(false);

  return (
    <>
      <div className="pointer-events-none sticky bottom-0 z-30 flex justify-end">
        <button
          type="button"
          onClick={() => setCreateOpen(true)}
          className="press pointer-events-auto mb-[calc(5.5rem+env(safe-area-inset-bottom))] inline-flex h-12 items-center gap-2 rounded-full bg-primary px-5 text-[14px] font-semibold text-primary-foreground shadow-lg shadow-primary/25 md:mb-6"
        >
          <Plus className="size-5" />
          Add loan
        </button>
      </div>

      <LoanFormSheet
        open={createOpen}
        onOpenChange={setCreateOpen}
        mode="create"
      />

      <LoanFormSheet
        open={Boolean(editLoan)}
        onOpenChange={(open) => {
          if (!open) onEditConsumed?.();
        }}
        mode="edit"
        loan={editLoan}
      />
    </>
  );
}
