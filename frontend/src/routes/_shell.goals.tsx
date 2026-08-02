import { createFileRoute, Link } from "@tanstack/react-router";

import { GoalCards } from "@/components/goals/goal-cards";
import { GoalInsights } from "@/components/goals/goal-insights";
import { GoalQuickActions } from "@/components/goals/goal-quick-actions";
import { GoalsHero } from "@/components/goals/goals-hero";
import { MilestoneTimeline } from "@/components/goals/milestone-timeline";
import { DefaultErrorComponent } from "@/components/ui-kit/default-error-component";
import { SectionHeader } from "@/components/ui-kit/section-header";
import { useGoalsOverview } from "@/hooks/api/use-goals";
import { fmtINR } from "@/lib/goals-data";

const description =
  "Track savings goals — house, loan free, emergency fund, education and retirement — with milestones and AI pacing suggestions.";

export const Route = createFileRoute("/_shell/goals")({
  head: () => ({
    meta: [
      { title: "Goals — WealthOS" },
      { name: "description", content: description },
      { property: "og:title", content: "Goals — WealthOS" },
      { property: "og:description", content: description },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
    ],
  }),
  errorComponent: DefaultErrorComponent,
  component: GoalsPage,
});

function GoalsPage() {
  const { data } = useGoalsOverview();
  const monthly = data?.summary.monthlyCommitted ?? 0;

  return (
    <div className="space-y-6">
      <h1 className="sr-only">Goals</h1>

      <GoalsHero />

      <section>
        <SectionHeader title="Quick actions" />
        <GoalQuickActions />
      </section>

      <section>
        <SectionHeader
          title="Your goals"
          action={<span className="tabular-nums">{fmtINR(monthly)}/mo</span>}
        />
        <GoalCards />
      </section>

      <section>
        <SectionHeader title="Milestones reached" />
        <MilestoneTimeline />
      </section>

      <section>
        <SectionHeader
          title="AI suggestions"
          action={
            <Link to="/ai-advisor" className="press -my-2 inline-flex min-h-11 items-center">
              Ask AI
            </Link>
          }
        />
        <GoalInsights />
      </section>
    </div>
  );
}
