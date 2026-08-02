import { ArrowUp } from "lucide-react";
import { forwardRef, type FormEvent } from "react";

/** Sticky composer that clears the mobile bottom nav and the iOS home indicator. */
export const AdvisorComposer = forwardRef<
  HTMLTextAreaElement,
  {
    value: string;
    onChange: (value: string) => void;
    onSubmit: () => void;
    disabled?: boolean;
  }
>(function AdvisorComposer({ value, onChange, onSubmit, disabled }, ref) {
  const submit = (event: FormEvent) => {
    event.preventDefault();
    if (!value.trim() || disabled) return;
    onSubmit();
  };

  return (
    <div className="bleed-gutter sticky bottom-0 z-30 border-t border-border/60 bg-background/92 backdrop-blur-xl">
      <form
        onSubmit={submit}
        className="page-gutter flex items-end gap-2 pb-[calc(5rem+env(safe-area-inset-bottom))] pt-2.5 md:pb-3"
      >
        <textarea
          ref={ref}
          rows={1}
          value={value}
          onChange={(event) => onChange(event.target.value)}
          onKeyDown={(event) => {
            if (event.key === "Enter" && !event.shiftKey) {
              event.preventDefault();
              submit(event);
            }
          }}
          placeholder="Ask about your money…"
          className="max-h-32 min-h-11 flex-1 resize-none rounded-2xl border border-border/70 bg-secondary/40 px-3.5 py-3 text-[14px] leading-snug outline-none placeholder:text-muted-foreground/70 focus-visible:border-primary/60"
        />
        <button
          type="submit"
          disabled={!value.trim() || disabled}
          aria-label="Send message"
          className="press grid size-11 shrink-0 place-items-center rounded-2xl bg-primary text-primary-foreground disabled:opacity-40"
        >
          <ArrowUp className="size-5" />
        </button>
      </form>
    </div>
  );
});
