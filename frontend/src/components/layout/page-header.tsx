import type { ReactNode } from "react";

export function PageHeader({
  title,
  description,
  actions,
}: {
  title: string;
  description?: string;
  actions?: ReactNode;
}) {
  return (
    <header className="grid gap-2.5 sm:flex sm:items-center sm:justify-between sm:gap-4">
      <div className="min-w-0">
        <h1 className="truncate font-display text-fluid-2xl font-semibold">{title}</h1>
        {description && (
          <p className="mt-0.5 line-clamp-2 text-[13px] leading-snug text-muted-foreground sm:mt-1 sm:text-sm sm:line-clamp-none">
            {description}
          </p>
        )}
      </div>
      {actions && (
        <div className="flex min-w-0 shrink-0 flex-wrap items-center gap-2 [&>*]:min-h-10 sm:[&>*]:min-h-9">
          {actions}
        </div>
      )}
    </header>
  );
}
