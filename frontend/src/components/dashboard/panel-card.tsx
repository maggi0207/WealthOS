import type { ReactNode } from "react";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { cn } from "@/lib/utils";

export function PanelCard({
  title,
  subtitle,
  actions,
  children,
  className,
  contentClassName,
}: {
  title: string;
  subtitle?: string;
  actions?: ReactNode;
  children: ReactNode;
  className?: string;
  contentClassName?: string;
}) {
  return (
    <Card className={cn("surface-panel gap-0 overflow-hidden py-0", className)}>
      <CardHeader className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3 border-b border-border/50 px-4 py-3 sm:px-5 sm:py-3.5">
        <div className="min-w-0">
          <CardTitle className="truncate font-display text-[0.95rem] font-semibold leading-tight">{title}</CardTitle>
          {subtitle && <p className="mt-0.5 truncate text-[11px] text-muted-foreground sm:text-xs">{subtitle}</p>}
        </div>
        {actions && <div className="flex shrink-0 items-center gap-2">{actions}</div>}
      </CardHeader>
      <CardContent className={cn("px-4 py-3.5 sm:px-5 sm:py-4", contentClassName)}>{children}</CardContent>
    </Card>
  );
}
