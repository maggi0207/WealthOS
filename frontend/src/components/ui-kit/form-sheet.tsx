import type { ReactNode } from "react";
import { Loader2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";

/**
 * Bottom sheet shell for create/edit forms — matches existing FAB sheet chrome.
 */
export function FormSheet({
  open,
  onOpenChange,
  title,
  description,
  children,
  onSubmit,
  onCancel,
  onReset,
  submitLabel = "Save",
  pending,
  deleteSlot,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  title: string;
  description?: string;
  children: ReactNode;
  onSubmit: () => void;
  onCancel?: () => void;
  onReset?: () => void;
  submitLabel?: string;
  pending?: boolean;
  deleteSlot?: ReactNode;
}) {
  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent
        side="bottom"
        className="flex max-h-[92vh] flex-col gap-0 overflow-hidden rounded-t-3xl border-border p-0"
      >
        <SheetHeader className="shrink-0 space-y-1 border-b border-border/60 px-5 py-4 text-left">
          <SheetTitle className="font-display text-lg">{title}</SheetTitle>
          {description ? (
            <SheetDescription className="text-sm">{description}</SheetDescription>
          ) : null}
        </SheetHeader>

        <div className="min-h-0 flex-1 overflow-y-auto px-5 py-4">
          <div className="space-y-3.5">{children}</div>
        </div>

        <div className="shrink-0 space-y-2 border-t border-border/60 px-5 py-3 pb-[calc(0.75rem+env(safe-area-inset-bottom))]">
          <div className="flex gap-2">
            <Button
              type="button"
              variant="outline"
              className="min-h-11 flex-1 rounded-full"
              disabled={pending}
              onClick={() => {
                onCancel?.();
                onOpenChange(false);
              }}
            >
              Cancel
            </Button>
            {onReset ? (
              <Button
                type="button"
                variant="ghost"
                className="min-h-11 rounded-full px-4"
                disabled={pending}
                onClick={onReset}
              >
                Reset
              </Button>
            ) : null}
            <Button
              type="button"
              className="min-h-11 flex-1 rounded-full"
              disabled={pending}
              onClick={onSubmit}
            >
              {pending ? <Loader2 className="size-4 animate-spin" /> : null}
              {pending ? "Saving…" : submitLabel}
            </Button>
          </div>
          {deleteSlot}
        </div>
      </SheetContent>
    </Sheet>
  );
}
