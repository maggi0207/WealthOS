import { advisorTopics } from "@/lib/advisor-data";

/** Horizontal, edge-to-edge quick topics. Tapping seeds a prompt. */
export function TopicChips({ onPick }: { onPick: (prompt: string) => void }) {
  return (
    <div className="bleed-gutter no-scrollbar overflow-x-auto">
      <div className="flex w-max gap-2 px-[max(var(--page-gutter),env(safe-area-inset-left))]">
        {advisorTopics.map((topic) => (
          <button
            key={topic.id}
            type="button"
            onClick={() => onPick(topic.prompt)}
            className="press inline-flex min-h-11 shrink-0 items-center gap-1.5 rounded-full border border-border/70 bg-secondary/50 px-3 text-[12.5px] font-semibold"
          >
            <topic.icon className="size-3.5 text-primary" />
            {topic.label}
          </button>
        ))}
      </div>
    </div>
  );
}
