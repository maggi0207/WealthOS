import { Building2, FileText, Landmark, ShieldPlus, TrendingUp, UserRound, type LucideIcon } from "lucide-react";

import { categoryCount, docCategories, type DocCategory } from "@/lib/documents-data";

const icons: Record<DocCategory, LucideIcon> = {
  property: Building2,
  loans: Landmark,
  investments: TrendingUp,
  identity: UserRound,
  insurance: ShieldPlus,
  tax: FileText,
};

/** Category grid — the entry point into the vault. */
export function CategoryGrid({ onSelect }: { onSelect?: (category: DocCategory) => void }) {
  return (
    <div className="grid grid-cols-2 gap-2.5 sm:grid-cols-3 lg:grid-cols-6">
      {docCategories.map((category) => {
        const Icon = icons[category.id];
        return (
          <button
            key={category.id}
            type="button"
            onClick={() => onSelect?.(category.id)}
            className="surface-tile press min-h-[76px] p-3 text-left"
          >
            <span className="grid size-8 place-items-center rounded-lg bg-primary/10 text-primary">
              <Icon className="size-4" />
            </span>
            <p className="mt-2 truncate text-[13px] font-semibold">{category.label}</p>
            <p className="truncate text-[11px] tabular-nums text-muted-foreground">
              {categoryCount(category.id)} files
            </p>
          </button>
        );
      })}
    </div>
  );
}
