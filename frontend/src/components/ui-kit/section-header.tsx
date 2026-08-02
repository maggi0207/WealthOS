import type { ReactNode } from "react";

import { cn } from "@/lib/utils";

/**
 * Section header — the rhythm marker of the app's design language.
 * Small uppercase eyebrow-weight title on the left, optional action on the right.
 */
export function SectionHeader({
  title,
  action,
  className,
}: {
  title: string;
  action?: ReactNode;
  className?: string;
}) {
  return (
    <div className={cn("mb-2.5 grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3 px-0.5", className)}>
      <h2 className="truncate text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
        {title}
      </h2>
      {action && (
        <div className="-my-2 shrink-0 text-xs font-medium text-primary [&>a]:inline-flex [&>a]:min-h-11 [&>a]:items-center [&>button]:inline-flex [&>button]:min-h-11 [&>button]:items-center">
          {action}
        </div>
      )}

    </div>
  );
}
