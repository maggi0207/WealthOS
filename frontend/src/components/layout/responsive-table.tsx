import type { ReactNode } from "react";

import { cn } from "@/lib/utils";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

export type ResponsiveColumn<T> = {
  /** Stable id for the column. */
  key: string;
  header: string;
  cell: (row: T) => ReactNode;
  /** Rendered as the card title on mobile instead of a label/value row. */
  primary?: boolean;
  /** Right-aligned in the table and shown as the card's trailing value. */
  align?: "left" | "right";
  /** Omitted from the mobile card view. */
  hideOnMobile?: boolean;
  className?: string;
};

/**
 * Mobile-first data list: stacked cards on small screens, a real table from md up.
 * Never scrolls horizontally on phones.
 */
export function ResponsiveTable<T>({
  data,
  columns,
  getRowKey,
  onRowClick,
  emptyMessage = "Nothing to show yet.",
  className,
}: {
  data: T[];
  columns: ResponsiveColumn<T>[];
  getRowKey: (row: T, index: number) => string;
  onRowClick?: (row: T) => void;
  emptyMessage?: string;
  className?: string;
}) {
  if (data.length === 0) {
    return (
      <p className="rounded-xl border border-dashed border-border/70 p-6 text-center text-sm text-muted-foreground">
        {emptyMessage}
      </p>
    );
  }

  const primaryCol = columns.find((c) => c.primary) ?? columns[0];
  if (!primaryCol) return null;
  const cardCols = columns.filter((c) => c !== primaryCol && !c.hideOnMobile);


  return (
    <div className={cn("w-full min-w-0", className)}>
      {/* Mobile: cards */}
      <ul className="grid gap-3 md:hidden">
        {data.map((row, index) => (
          <li key={getRowKey(row, index)}>
            <div
              role={onRowClick ? "button" : undefined}
              tabIndex={onRowClick ? 0 : undefined}
              onClick={onRowClick ? () => onRowClick(row) : undefined}
              onKeyDown={
                onRowClick
                  ? (event) => {
                      if (event.key === "Enter" || event.key === " ") {
                        event.preventDefault();
                        onRowClick(row);
                      }
                    }
                  : undefined
              }
              className={cn(
                "surface-panel min-h-11 space-y-3 p-4",
                onRowClick && "cursor-pointer active:opacity-80",
              )}
            >
              <div className="min-w-0 text-sm font-semibold">{primaryCol.cell(row)}</div>
              <dl className="grid gap-2">
                {cardCols.map((col) => (
                  <div key={col.key} className="flex items-start justify-between gap-3">
                    <dt className="shrink-0 text-xs uppercase tracking-wide text-muted-foreground">
                      {col.header}
                    </dt>
                    <dd className="min-w-0 text-right text-sm">{col.cell(row)}</dd>
                  </div>
                ))}
              </dl>
            </div>
          </li>
        ))}
      </ul>

      {/* md+: table */}
      <div className="hidden md:block">
        <Table>
          <TableHeader>
            <TableRow>
              {columns.map((col) => (
                <TableHead
                  key={col.key}
                  className={cn(col.align === "right" && "text-right", col.className)}
                >
                  {col.header}
                </TableHead>
              ))}
            </TableRow>
          </TableHeader>
          <TableBody>
            {data.map((row, index) => (
              <TableRow
                key={getRowKey(row, index)}
                onClick={onRowClick ? () => onRowClick(row) : undefined}
                className={cn(onRowClick && "cursor-pointer")}
              >
                {columns.map((col) => (
                  <TableCell
                    key={col.key}
                    className={cn(col.align === "right" && "text-right", col.className)}
                  >
                    {col.cell(row)}
                  </TableCell>
                ))}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
