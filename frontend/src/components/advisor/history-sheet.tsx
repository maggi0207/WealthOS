import { History, MessageSquare } from "lucide-react";

import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from "@/components/ui/sheet";
import { conversationHistory, historyBuckets, type Conversation } from "@/lib/advisor-data";

/** Conversation history grouped by Today / Yesterday / Earlier. */
export function HistorySheet({ onOpenConversation }: { onOpenConversation: (c: Conversation) => void }) {
  return (
    <Sheet>
      <SheetTrigger asChild>
        <button
          type="button"
          className="press inline-flex min-h-11 items-center gap-1.5 rounded-full border border-border/70 bg-secondary/50 px-3 text-[12.5px] font-semibold"
        >
          <History className="size-3.5 text-primary" />
          History
        </button>
      </SheetTrigger>
      <SheetContent side="right" className="w-[86vw] max-w-sm p-0">
        <SheetHeader className="border-b border-border/60 px-4 py-4">
          <SheetTitle className="font-display text-base">Conversations</SheetTitle>
        </SheetHeader>
        <div className="max-h-[calc(100dvh-4.5rem)] space-y-5 overflow-y-auto px-4 py-4 pb-safe">
          {historyBuckets.map((bucket) => {
            const items = conversationHistory.filter((c) => c.bucket === bucket);
            if (items.length === 0) return null;
            return (
              <section key={bucket}>
                <h3 className="mb-2 text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
                  {bucket}
                </h3>
                <ul className="space-y-2">
                  {items.map((conversation) => (
                    <li key={conversation.id}>
                      <button
                        type="button"
                        onClick={() => onOpenConversation(conversation)}
                        className="surface-tile press flex w-full items-start gap-3 p-3 text-left"
                      >
                        <span className="mt-0.5 grid size-8 shrink-0 place-items-center rounded-xl bg-primary/10 text-primary">
                          <MessageSquare className="size-4" />
                        </span>
                        <span className="min-w-0 flex-1">
                          <span className="block truncate text-[13.5px] font-semibold">{conversation.title}</span>
                          <span className="mt-0.5 block truncate text-[12px] text-muted-foreground">
                            {conversation.preview}
                          </span>
                        </span>
                        <span className="shrink-0 text-[10.5px] font-medium text-muted-foreground/80">
                          {conversation.time}
                        </span>
                      </button>
                    </li>
                  ))}
                </ul>
              </section>
            );
          })}
        </div>
      </SheetContent>
    </Sheet>
  );
}
