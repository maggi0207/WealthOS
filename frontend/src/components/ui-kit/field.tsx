import { forwardRef, type InputHTMLAttributes } from "react";

import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";

/**
 * Labelled input with inline validation messaging.
 * Errors are announced (`role="alert"`) and wired via `aria-describedby`.
 */
export const Field = forwardRef<
  HTMLInputElement,
  InputHTMLAttributes<HTMLInputElement> & { id: string; label: string; error?: string | null | undefined; hint?: string | undefined }
>(function Field({ id, label, error, hint, className, ...props }, ref) {
  const describedBy = error ? `${id}-error` : hint ? `${id}-hint` : undefined;

  return (
    <div className="space-y-1.5">
      <Label htmlFor={id}>{label}</Label>
      <Input
        id={id}
        ref={ref}
        aria-invalid={error ? true : undefined}
        aria-describedby={describedBy}
        className={cn("min-h-11", error && "border-destructive focus-visible:border-destructive", className)}
        {...props}
      />
      {error ? (
        <p id={`${id}-error`} role="alert" className="text-[12px] font-medium text-destructive">
          {error}
        </p>
      ) : hint ? (
        <p id={`${id}-hint`} className="text-[12px] text-muted-foreground">
          {hint}
        </p>
      ) : null}
    </div>
  );
});
