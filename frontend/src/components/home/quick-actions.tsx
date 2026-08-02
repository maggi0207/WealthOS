import { Link } from "@tanstack/react-router";
import { Banknote, Coins, Landmark, Receipt, type LucideIcon } from "lucide-react";

type Action = { label: string; to: string; icon: LucideIcon };

const actions: Action[] = [
  { label: "Add expense", to: "/expenses", icon: Receipt },
  { label: "Add asset", to: "/assets", icon: Coins },
  { label: "Add income", to: "/income", icon: Banknote },
  { label: "Record payment", to: "/loans", icon: Landmark },
];

/** Four thumb-sized shortcuts to the most common money actions. */
export function QuickActions() {
  return (
    <div className="grid grid-cols-4 gap-2.5">
      {actions.map((action) => (
        <Link
          key={action.to + action.label}
          to={action.to}
          className="surface-tile press flex min-h-[5.25rem] flex-col items-center justify-center gap-2 px-1.5 py-3 text-center"
        >
          <span className="grid size-9 place-items-center rounded-xl bg-primary/12 text-primary">
            <action.icon className="size-4" />
          </span>
          <span className="w-full text-[11px] font-medium leading-tight text-foreground">{action.label}</span>
        </Link>
      ))}
    </div>
  );
}
