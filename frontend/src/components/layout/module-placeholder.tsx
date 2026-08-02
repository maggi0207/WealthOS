import { Link } from "@tanstack/react-router";
import { Bot, type LucideIcon } from "lucide-react";


import { PageHeader } from "@/components/layout/page-header";
import { EmptyState } from "@/components/ui-kit/empty-state";
import { SectionHeader } from "@/components/ui-kit/section-header";
import { Badge } from "@/components/ui/badge";
import { moduleStatus } from "@/lib/mock-data";

/**
 * Clean empty state for a module that will be implemented later.
 * Keeps every route visually consistent while modules land one by one.
 */
export function ModulePlaceholder({
  title,
  description,
  icon: Icon,
  path,
  planned,
}: {
  title: string;
  description: string;
  icon: LucideIcon;
  path: string;
  planned: string[];
}) {
  const status = moduleStatus[path];

  return (
    <div className="space-y-4">
      <PageHeader title={title} description={description} actions={<Badge variant="secondary">Coming soon</Badge>} />

      <EmptyState
        icon={Icon}
        title={`${title} is on the way`}
        description={
          status
            ? `${status.records} mock records ready · last synced ${status.lastSync}.`
            : "This module is scaffolded and ready for its build."
        }
        action={
          <div className="flex flex-wrap items-center justify-center gap-2">
            <Link
              to="/ai-advisor"
              className="press inline-flex min-h-11 items-center gap-1.5 rounded-full bg-primary px-4 text-[13px] font-semibold text-primary-foreground"
            >
              <Bot className="size-4" />
              Ask the AI Advisor
            </Link>
            <Link
              to="/dashboard"
              className="press inline-flex min-h-11 items-center rounded-full border border-border/70 bg-secondary/50 px-4 text-[13px] font-semibold text-foreground"
            >
              Back to home
            </Link>
          </div>
        }
      />


      <div>
        <SectionHeader title="Planned in this module" />
        <ul className="grid gap-2 sm:grid-cols-2">
          {planned.map((item) => (
            <li
              key={item}
              className="flex items-start gap-2.5 surface-tile px-3.5 py-3 text-sm"
            >
              <span className="mt-1.5 size-1.5 shrink-0 rounded-full bg-primary" />
              <span className="min-w-0">{item}</span>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
