import type { ReactNode } from "react";

import { cn } from "@/lib/utils";

/**
 * Sticky bottom action bar for mobile forms and detail views.
 * Sits above the bottom navigation and respects the iOS home indicator.
 * From md up it becomes an inline right-aligned action row.
 */
export function StickyActions({
  children,
  className,
}: {
  children: ReactNode;
  className?: string;
}) {
  return (
    <div
      className={cn(
        "fixed inset-x-0 bottom-14 z-30 flex items-center gap-2 border-t border-border/70 bg-background/95 px-4 py-3 pb-safe backdrop-blur-xl",
        "[&>*]:min-h-11 [&>*]:flex-1",
        "md:static md:inset-auto md:justify-end md:border-0 md:bg-transparent md:p-0 md:backdrop-blur-none md:[&>*]:flex-none",
        className,
      )}
    >
      {children}
    </div>
  );
}
