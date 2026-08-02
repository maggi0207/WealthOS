import { Link } from "@tanstack/react-router";
import { ArrowUpRight, Check, Copy, Sprout } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

import type { ChatMessage } from "@/lib/advisor-data";
import { cn } from "@/lib/utils";

/** Copy-to-clipboard control shown under every assistant answer. */
function CopyButton({ text }: { text: string }) {
  const [copied, setCopied] = useState(false);

  const copy = async () => {
    try {
      await navigator.clipboard.writeText(text);
      setCopied(true);
      toast.success("Answer copied");
      setTimeout(() => setCopied(false), 1600);
    } catch {
      toast.error("Couldn't copy — try selecting the text");
    }
  };

  return (
    <button
      type="button"
      onClick={copy}
      aria-label={copied ? "Copied" : "Copy answer"}
      className="press inline-flex min-h-11 items-center gap-1.5 rounded-full px-2 text-[11px] font-medium text-muted-foreground hover:text-foreground"
    >
      {copied ? <Check className="size-3.5 text-success" /> : <Copy className="size-3.5" />}
      {copied ? "Copied" : "Copy"}
    </button>
  );
}

/** One turn of the advisor conversation. Assistant turns are unbubbled. */
export function ChatMessageRow({ message }: { message: ChatMessage }) {
  const isUser = message.role === "user";

  if (isUser) {
    return (
      <div className="msg-enter flex justify-end">
        <div className="max-w-[85%] rounded-2xl rounded-br-md bg-primary px-3.5 py-2.5 text-[14px] leading-relaxed text-primary-foreground">
          {message.text}
          <span className="mt-1 block text-[10px] font-medium text-primary-foreground/70">{message.time}</span>
        </div>
      </div>
    );
  }

  const plain = [message.text, ...(message.points ?? []).map((p) => `• ${p}`)].join("\n");

  return (
    <div className="msg-enter flex gap-2.5">
      <span className="mt-0.5 grid size-8 shrink-0 place-items-center rounded-xl bg-primary/12 text-primary">
        <Sprout className="size-4" />
      </span>
      <div className="min-w-0 flex-1">
        <p className="text-[14px] leading-relaxed text-foreground">{message.text}</p>

        {message.points && (
          <ul className="mt-2.5 space-y-1.5">
            {message.points.map((point) => (
              <li key={point} className="flex gap-2 text-[13px] leading-relaxed text-muted-foreground">
                <span className="mt-[7px] size-1.5 shrink-0 rounded-full bg-primary/60" />
                <span className="min-w-0">{point}</span>
              </li>
            ))}
          </ul>
        )}

        {message.actions && message.actions.length > 0 && (
          <div className="mt-3 flex flex-wrap gap-2">
            {message.actions.map((action) => (
              <Link
                key={action.to + action.label}
                to={action.to}
                className={cn(
                  "press inline-flex min-h-11 items-center gap-1.5 rounded-full border border-border/70 bg-secondary/60",
                  "px-3.5 text-[12.5px] font-semibold text-foreground",
                )}
              >
                {action.label}
                <ArrowUpRight className="size-3.5 text-primary" />
              </Link>
            ))}
          </div>
        )}

        <div className="mt-1.5 flex items-center gap-1">
          <span className="text-[10px] font-medium text-muted-foreground/70">{message.time}</span>
          <CopyButton text={plain} />
        </div>
      </div>
    </div>
  );
}

/** Streaming placeholder shown while the mock advisor composes an answer. */
export function TypingIndicator() {
  return (
    <div className="msg-enter flex gap-2.5" aria-live="polite">
      <span className="mt-0.5 grid size-8 shrink-0 place-items-center rounded-xl bg-primary/12 text-primary">
        <Sprout className="size-4" />
      </span>
      <div className="min-w-0 flex-1">
        <div className="inline-flex items-center gap-1.5 rounded-2xl rounded-bl-md bg-secondary/60 px-3.5 py-3">
          {[0, 1, 2].map((i) => (
            <span
              key={i}
              className="size-1.5 animate-bounce rounded-full bg-primary/70"
              style={{ animationDelay: `${i * 140}ms`, animationDuration: "900ms" }}
            />
          ))}
          <span className="sr-only">Advisor is typing</span>
        </div>
        {/* Streaming placeholder lines keep the layout stable while "thinking". */}
        <div className="mt-2 space-y-1.5" aria-hidden>
          <span className="block h-2.5 w-[70%] animate-pulse rounded-full bg-muted" />
          <span className="block h-2.5 w-[45%] animate-pulse rounded-full bg-muted" />
        </div>
      </div>
    </div>
  );
}
