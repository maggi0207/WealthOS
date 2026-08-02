import { useState } from "react";
import { Briefcase, FileText, IndianRupee, Receipt, UserPlus, Users } from "lucide-react";
import type { LucideIcon } from "lucide-react";

import { ClientFormSheet } from "@/components/business/client-form-sheet";
import { DeveloperFormSheet } from "@/components/business/developer-form-sheet";
import { ExpenseFormSheet } from "@/components/business/expense-form-sheet";
import { InvoiceFormSheet } from "@/components/business/invoice-form-sheet";
import { PaymentFormSheet } from "@/components/business/payment-form-sheet";
import { ProjectFormSheet } from "@/components/business/project-form-sheet";
import { quickActions, type QuickActionId } from "@/lib/business-data";

const icons: Record<QuickActionId, LucideIcon> = {
  "add-client": Users,
  "add-project": Briefcase,
  "add-developer": UserPlus,
  "record-payment": IndianRupee,
  "record-expense": Receipt,
  "create-invoice": FileText,
};

/** Quick actions grid — opens create/edit form sheets. */
export function BusinessQuickActions() {
  const [active, setActive] = useState<QuickActionId | null>(null);

  return (
    <>
      <div className="grid grid-cols-2 gap-2.5 sm:grid-cols-3 lg:grid-cols-6">
        {quickActions.map((action) => {
          const Icon = icons[action.id];
          return (
            <button
              key={action.id}
              type="button"
              onClick={() => setActive(action.id)}
              className="surface-tile press min-h-[76px] p-3 text-left"
            >
              <span className="grid size-8 place-items-center rounded-lg bg-primary/10 text-primary">
                <Icon className="size-4" />
              </span>
              <p className="mt-2 truncate text-[13px] font-semibold">{action.label}</p>
              <p className="truncate text-[11px] text-muted-foreground">{action.hint}</p>
            </button>
          );
        })}
      </div>

      <ClientFormSheet
        open={active === "add-client"}
        onOpenChange={(open) => !open && setActive(null)}
        mode="create"
      />
      <ProjectFormSheet
        open={active === "add-project"}
        onOpenChange={(open) => !open && setActive(null)}
      />
      <DeveloperFormSheet
        open={active === "add-developer"}
        onOpenChange={(open) => !open && setActive(null)}
      />
      <PaymentFormSheet
        open={active === "record-payment"}
        onOpenChange={(open) => !open && setActive(null)}
      />
      <ExpenseFormSheet
        open={active === "record-expense"}
        onOpenChange={(open) => !open && setActive(null)}
      />
      <InvoiceFormSheet
        open={active === "create-invoice"}
        onOpenChange={(open) => !open && setActive(null)}
      />
    </>
  );
}
