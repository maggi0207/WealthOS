/**
 * Goals module mock data (INR).
 * Backend-shaped fixtures: stable ids, ISO dates, plain rupee numbers.
 */

import { fmtINR, fmtINRShort } from "@/lib/wealth-data";
import { fmtDate, type ISODate } from "@/lib/business-data";

export { fmtINR, fmtINRShort, fmtDate };
export type { ISODate };

export type GoalCategory = "property" | "debt" | "safety" | "education" | "retirement";

export type Milestone = {
  id: string;
  label: string;
  atPct: number;
  reachedOn?: ISODate;
};

export type Goal = {
  id: string;
  name: string;
  category: GoalCategory;
  target: number;
  saved: number;
  monthlyContribution: number;
  targetDate: ISODate;
  startedOn: ISODate;
  note: string;
  milestones: Milestone[];
};

export const goals: Goal[] = [
  {
    id: "goal-house",
    name: "Buy second house",
    category: "property",
    target: 90_00_000,
    saved: 31_50_000,
    monthlyContribution: 85_000,
    targetDate: "2031-04-01",
    startedOn: "2023-04-01",
    note: "Down payment plus registration for a 2BHK in OMR corridor.",
    milestones: [
      { id: "m-h1", label: "Site shortlisted", atPct: 10, reachedOn: "2023-09-12" },
      { id: "m-h2", label: "25% saved", atPct: 25, reachedOn: "2025-02-04" },
      { id: "m-h3", label: "Half way", atPct: 50 },
      { id: "m-h4", label: "Down payment ready", atPct: 100 },
    ],
  },
  {
    id: "goal-loan-free",
    name: "Loan free",
    category: "debt",
    target: 45_47_000,
    saved: 40_40_000,
    monthlyContribution: 92_900,
    targetDate: "2033-05-05",
    startedOn: "2018-06-05",
    note: "Every EMI and prepayment counts toward clearing all three loans.",
    milestones: [
      { id: "m-l1", label: "Personal loan cleared", atPct: 60, reachedOn: "2025-11-20" },
      { id: "m-l2", label: "Jewel loan cleared", atPct: 80, reachedOn: "2026-06-18" },
      { id: "m-l3", label: "Home loan cleared", atPct: 100 },
    ],
  },
  {
    id: "goal-emergency",
    name: "Emergency fund",
    category: "safety",
    target: 12_00_000,
    saved: 12_00_000,
    monthlyContribution: 0,
    targetDate: "2026-06-30",
    startedOn: "2022-01-10",
    note: "Twelve months of household and business runway in liquid funds.",
    milestones: [
      { id: "m-e1", label: "3 months runway", atPct: 25, reachedOn: "2022-11-02" },
      { id: "m-e2", label: "6 months runway", atPct: 50, reachedOn: "2024-03-15" },
      { id: "m-e3", label: "12 months runway", atPct: 100, reachedOn: "2026-06-28" },
    ],
  },
  {
    id: "goal-education",
    name: "Daughter's education",
    category: "education",
    target: 65_00_000,
    saved: 18_20_000,
    monthlyContribution: 45_000,
    targetDate: "2038-06-01",
    startedOn: "2021-07-01",
    note: "Undergraduate abroad corpus, indexed at 8% education inflation.",
    milestones: [
      { id: "m-d1", label: "First ₹10 L", atPct: 15, reachedOn: "2024-08-19" },
      { id: "m-d2", label: "Quarter funded", atPct: 25 },
      { id: "m-d3", label: "Half funded", atPct: 50 },
      { id: "m-d4", label: "Fully funded", atPct: 100 },
    ],
  },
  {
    id: "goal-retirement",
    name: "Retirement corpus",
    category: "retirement",
    target: 6_00_00_000,
    saved: 1_42_00_000,
    monthlyContribution: 1_10_000,
    targetDate: "2045-03-31",
    startedOn: "2016-04-01",
    note: "Target corpus for a ₹2 L monthly lifestyle from age 58.",
    milestones: [
      { id: "m-r1", label: "First crore", atPct: 16, reachedOn: "2025-05-30" },
      { id: "m-r2", label: "Quarter corpus", atPct: 25 },
      { id: "m-r3", label: "Half corpus", atPct: 50 },
      { id: "m-r4", label: "Financially free", atPct: 100 },
    ],
  },
];

export function goalProgressPct(goal: Goal) {
  return Math.min(100, Math.round((goal.saved / goal.target) * 100));
}

export function monthsToTarget(goal: Goal) {
  const now = new Date("2026-08-01T00:00:00");
  const target = new Date(`${goal.targetDate}T00:00:00`);
  return Math.max(
    0,
    (target.getFullYear() - now.getFullYear()) * 12 + (target.getMonth() - now.getMonth()),
  );
}

/** Monthly contribution needed to land exactly on the target date. */
export function requiredMonthly(goal: Goal) {
  const months = monthsToTarget(goal);
  if (months === 0) return 0;
  return Math.max(0, Math.round((goal.target - goal.saved) / months));
}

export const goalsSummary = {
  totalTarget: goals.reduce((s, g) => s + g.target, 0),
  totalSaved: goals.reduce((s, g) => s + g.saved, 0),
  monthlyCommitted: goals.reduce((s, g) => s + g.monthlyContribution, 0),
  completed: goals.filter((g) => goalProgressPct(g) >= 100).length,
};

export const goalsOverallPct = Math.round((goalsSummary.totalSaved / goalsSummary.totalTarget) * 100);

export const goalCategoryLabel: Record<GoalCategory, string> = {
  property: "Property",
  debt: "Debt",
  safety: "Safety net",
  education: "Education",
  retirement: "Retirement",
};

/* ------------------------------- AI insights -------------------------------- */

export type GoalInsight = {
  id: string;
  tag: string;
  tone: "positive" | "caution" | "neutral";
  title: string;
  body: string;
  impact: string;
  action: string;
};

export const goalInsights: GoalInsight[] = [
  {
    id: "gi-1",
    tag: "On track",
    tone: "positive",
    title: "Emergency fund is fully funded",
    body: "Redirect the ₹20,000 you were parking here into the education goal — it is the furthest behind pace.",
    impact: "Closes gap 14 months earlier",
    action: "Reallocate",
  },
  {
    id: "gi-2",
    tag: "Behind",
    tone: "caution",
    title: "Second house needs ₹1.06 L a month",
    body: "You are contributing ₹85,000. Either raise the SIP or push the target date to late 2032.",
    impact: "₹21,000 monthly gap",
    action: "Adjust plan",
  },
  {
    id: "gi-3",
    tag: "Boost",
    tone: "neutral",
    title: "Step up retirement SIP by 10% yearly",
    body: "An annual step-up on your ₹1.1 L contribution reaches the ₹6 Cr corpus about four years sooner.",
    impact: "4 years earlier",
    action: "Enable step-up",
  },
];

/* ------------------------------- Quick actions ------------------------------- */

export const goalQuickActions = [
  { id: "new-goal", label: "New goal", hint: "Name, target, date" },
  { id: "add-funds", label: "Add funds", hint: "Log a contribution" },
  { id: "auto-invest", label: "Auto invest", hint: "Monthly SIP" },
  { id: "rebalance", label: "Rebalance", hint: "Shift between goals" },
] as const;
