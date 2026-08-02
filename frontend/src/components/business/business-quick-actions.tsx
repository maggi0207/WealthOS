import { FileText, IndianRupee, Receipt, UserPlus, Users } from "lucide-react";
import type { LucideIcon } from "lucide-react";
import { toast } from "sonner";

import { quickActions, type QuickActionId } from "@/lib/business-data";

const icons: Record<QuickActionId, LucideIcon> = {
  "add-client": Users,
  "add-developer": UserPlus,
  "record-payment": IndianRupee,
  "record-expense": Receipt,
  "create-invoice": FileText,
};

/** Quick actions grid — mock handlers until the editors ship. */
export function BusinessQuickActions() {
  return (
    <div className="grid grid-cols-2 gap-2.5 sm:grid-cols-3 lg:grid-cols-5">
      {quickActions.map((action) => {
        const Icon = icons[action.id];
        return (
          <button
            key={action.id}
            type="button"
            onClick={() => toast.success(`${action.label} — coming with the business editor`)}
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
  );
}
