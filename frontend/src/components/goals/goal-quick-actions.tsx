import { PlusCircle, Repeat, Shuffle, Wallet, type LucideIcon } from "lucide-react";
import { toast } from "sonner";

import { goalQuickActions } from "@/lib/goals-data";

const icons: Record<string, LucideIcon> = {
  "new-goal": PlusCircle,
  "add-funds": Wallet,
  "auto-invest": Repeat,
  rebalance: Shuffle,
};

/** Quick actions for the goals module — mock handlers. */
export function GoalQuickActions() {
  return (
    <div className="grid grid-cols-2 gap-2.5 sm:grid-cols-4">
      {goalQuickActions.map((action) => {
        const Icon = icons[action.id] ?? PlusCircle;
        return (
          <button
            key={action.id}
            type="button"
            onClick={() => toast.success(`${action.label} — coming with the goal editor`)}
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
