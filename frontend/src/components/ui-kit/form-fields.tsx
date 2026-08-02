import { forwardRef, type SelectHTMLAttributes, type TextareaHTMLAttributes } from "react";

import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { cn } from "@/lib/utils";

export const TextAreaField = forwardRef<
  HTMLTextAreaElement,
  TextareaHTMLAttributes<HTMLTextAreaElement> & {
    id: string;
    label: string;
    error?: string | null;
    hint?: string;
  }
>(function TextAreaField({ id, label, error, hint, className, ...props }, ref) {
  const describedBy = error ? `${id}-error` : hint ? `${id}-hint` : undefined;
  return (
    <div className="space-y-1.5">
      <Label htmlFor={id}>{label}</Label>
      <Textarea
        id={id}
        ref={ref}
        aria-invalid={error ? true : undefined}
        aria-describedby={describedBy}
        className={cn(
          "min-h-[88px] rounded-xl",
          error && "border-destructive focus-visible:ring-destructive",
          className,
        )}
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

export const SelectField = forwardRef<
  HTMLSelectElement,
  SelectHTMLAttributes<HTMLSelectElement> & {
    id: string;
    label: string;
    error?: string | null;
    hint?: string;
    options: { value: string; label: string }[];
  }
>(function SelectField({ id, label, error, hint, options, className, ...props }, ref) {
  const describedBy = error ? `${id}-error` : hint ? `${id}-hint` : undefined;
  return (
    <div className="space-y-1.5">
      <Label htmlFor={id}>{label}</Label>
      <select
        id={id}
        ref={ref}
        aria-invalid={error ? true : undefined}
        aria-describedby={describedBy}
        className={cn(
          "flex min-h-11 w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50",
          error && "border-destructive focus-visible:ring-destructive",
          className,
        )}
        {...props}
      >
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </select>
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
