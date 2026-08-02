import { createFileRoute } from "@tanstack/react-router";
import { PenSquare } from "lucide-react";
import { useCallback, useEffect, useRef, useState } from "react";

import { AdvisorComposer } from "@/components/advisor/advisor-composer";
import { AdvisorEmptyState } from "@/components/advisor/advisor-empty-state";
import { ChatMessageRow, TypingIndicator } from "@/components/advisor/chat-message";
import { HistorySheet } from "@/components/advisor/history-sheet";
import { InsightRail } from "@/components/advisor/insight-rail";
import { TopicChips } from "@/components/advisor/topic-chips";
import { SectionHeader } from "@/components/ui-kit/section-header";
import {
  makeUserMessage,
  mockAdvisorReply,
  type ChatMessage,
  type Conversation,
} from "@/lib/advisor-data";

export const Route = createFileRoute("/_shell/ai-advisor")({
  head: () => ({
    meta: [
      { title: "AI Advisor — WealthOS" },
      {
        name: "description",
        content: "Chat with your personal wealth advisor about property, loans, investments, taxes and goals.",
      },
      { property: "og:title", content: "AI Advisor — WealthOS" },
      {
        property: "og:description",
        content: "Chat with your personal wealth advisor about property, loans, investments, taxes and goals.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  component: AiAdvisorPage,
});

function AiAdvisorPage() {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [input, setInput] = useState("");
  const [typing, setTyping] = useState(false);
  const inputRef = useRef<HTMLTextAreaElement>(null);
  const endRef = useRef<HTMLDivElement>(null);
  const timers = useRef<ReturnType<typeof setTimeout>[]>([]);

  useEffect(() => {
    inputRef.current?.focus();
    return () => timers.current.forEach(clearTimeout);
  }, []);

  useEffect(() => {
    endRef.current?.scrollIntoView({ behavior: "smooth", block: "end" });
  }, [messages, typing]);

  const ask = useCallback((text: string) => {
    const question = text.trim();
    if (!question) return;
    setMessages((prev) => [...prev, makeUserMessage(question)]);
    setInput("");
    setTyping(true);
    const timer = setTimeout(() => {
      setMessages((prev) => [...prev, mockAdvisorReply(question)]);
      setTyping(false);
      inputRef.current?.focus();
    }, 900);
    timers.current.push(timer);
  }, []);

  const openConversation = useCallback((conversation: Conversation) => {
    setTyping(false);
    setMessages(conversation.messages);
  }, []);

  const reset = useCallback(() => {
    setTyping(false);
    setMessages([]);
    setInput("");
    inputRef.current?.focus();
  }, []);

  const started = messages.length > 0;

  return (
    <div className="flex min-h-[calc(100dvh-8rem)] flex-col">
      <header className="grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3">
        <div className="min-w-0">
          <p className="text-[11px] font-medium uppercase tracking-[0.14em] text-muted-foreground">AI Advisor</p>
          <h1 className="mt-0.5 truncate font-display text-fluid-xl font-semibold leading-tight">Ask anything</h1>
        </div>
        <div className="flex shrink-0 items-center gap-2">
          {started && (
            <button
              type="button"
              onClick={reset}
              aria-label="New conversation"
              className="press grid size-9 place-items-center rounded-full border border-border/70 bg-secondary/50"
            >
              <PenSquare className="size-4 text-primary" />
            </button>
          )}
          <HistorySheet onOpenConversation={openConversation} />
        </div>
      </header>

      <div className="mt-4 flex-1 space-y-6">
        {!started && (
          <>
            <AdvisorEmptyState onPick={ask} />

            <section>
              <SectionHeader title="Explore" />
              <TopicChips onPick={ask} />
            </section>

            <section>
              <SectionHeader title="Insights for you" action={<span>Swipe →</span>} />
              <InsightRail />
            </section>
          </>
        )}

        {started && (
          <>
            <section className="space-y-5">
              {messages.map((message) => (
                <ChatMessageRow key={message.id} message={message} />
              ))}
              {typing && <TypingIndicator />}
            </section>

            {!typing && (
              <section>
                <SectionHeader title="Follow up" />
                <TopicChips onPick={ask} />
              </section>
            )}
          </>
        )}

        <div ref={endRef} />
      </div>

      <AdvisorComposer
        ref={inputRef}
        value={input}
        onChange={setInput}
        onSubmit={() => ask(input)}
        disabled={typing}
      />
    </div>
  );
}
