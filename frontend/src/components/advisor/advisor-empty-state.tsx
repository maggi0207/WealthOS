import { Sprout } from "lucide-react";

import { suggestedPrompts } from "@/lib/advisor-data";
import { useAuth } from "@/lib/mock-auth";

/** First-run state: personalised greeting plus tappable starter prompts. */
export function AdvisorEmptyState({ onPick }: { onPick: (prompt: string) => void }) {
  const { user } = useAuth();
  const raw = (user?.name ?? "Magesh").split(" ")[0] ?? "Magesh";
  const firstName = raw.charAt(0).toUpperCase() + raw.slice(1);

  return (
    <div className="space-y-4">
      <div className="surface-hero relative overflow-hidden p-5">
        <span className="grid size-12 place-items-center rounded-2xl bg-primary/15 text-primary">
          <Sprout className="size-6" />
        </span>
        <h2 className="mt-3.5 font-display text-fluid-xl font-semibold leading-tight">
          Hello {firstName}, I'm your wealth advisor.
        </h2>
        <p className="mt-1.5 max-w-md text-[13.5px] leading-relaxed text-muted-foreground">
          I know your ₹2.86 Cr net worth, your Adyar flat, your home loan and every SIP you run. Ask me anything
          — I'll answer in plain numbers and take you straight to the right screen.
        </p>
      </div>

      <div className="grid gap-2 sm:grid-cols-2">
        {suggestedPrompts.map((prompt) => (
          <button
            key={prompt.id}
            type="button"
            onClick={() => onPick(prompt.text)}
            className="surface-tile press flex min-h-12 items-center gap-3 p-3.5 text-left"
          >
            <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-primary/10 text-primary">
              <prompt.icon className="size-4" />
            </span>
            <span className="min-w-0 flex-1 text-[13.5px] font-medium leading-snug">{prompt.text}</span>
          </button>
        ))}
      </div>
    </div>
  );
}
