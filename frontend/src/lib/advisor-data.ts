/**
 * AI Advisor mock data (INR).
 * Pure frontend fixtures — no backend, no LLM, no network.
 */

import {
  Building2,
  Coins,
  Landmark,
  PiggyBank,
  Receipt,
  Target,
  TrendingUp,
  type LucideIcon,
} from "lucide-react";

export type AdvisorAction = {
  label: string;
  /** Existing app route opened from an AI answer (mock navigation). */
  to: string;
};

export type ChatMessage = {
  id: string;
  role: "user" | "assistant";
  text: string;
  /** Optional bullet highlights rendered as a compact list. */
  points?: string[];
  actions?: AdvisorAction[];
  time: string;
};

export type Conversation = {
  id: string;
  title: string;
  preview: string;
  /** Grouping bucket for the history sheet. */
  bucket: "Today" | "Yesterday" | "Earlier";
  time: string;
  messages: ChatMessage[];
};

export type TopicChip = {
  id: string;
  label: string;
  icon: LucideIcon;
  prompt: string;
};

export type SuggestedPrompt = {
  id: string;
  text: string;
  icon: LucideIcon;
};

export const advisorTopics: TopicChip[] = [
  { id: "property", label: "Property", icon: Building2, prompt: "How is my property performing?" },
  { id: "loans", label: "Loans", icon: Landmark, prompt: "Should I prepay my home loan?" },
  { id: "investments", label: "Investments", icon: TrendingUp, prompt: "Review my investment allocation" },
  { id: "taxes", label: "Taxes", icon: Receipt, prompt: "How can I save tax this year?" },
  { id: "goals", label: "Goals", icon: Target, prompt: "Am I on track for my goals?" },
];

export const suggestedPrompts: SuggestedPrompt[] = [
  { id: "p1", text: "Can I buy another property?", icon: Building2 },
  { id: "p2", text: "Should I prepay my home loan?", icon: Landmark },
  { id: "p3", text: "Show my net worth", icon: Coins },
  { id: "p4", text: "How can I reduce my debt?", icon: PiggyBank },
];

export type InsightCard = {
  id: string;
  tag: string;
  title: string;
  body: string;
  impact: string;
  tone: "positive" | "caution" | "neutral";
  action: AdvisorAction;
};

export const advisorInsights: InsightCard[] = [
  {
    id: "i1",
    tag: "Debt",
    title: "Prepay ₹4 L on your home loan",
    body: "A one-time prepayment in the current rate cycle trims 19 EMIs from your Adyar flat loan.",
    impact: "Saves ₹7.4 L interest",
    tone: "positive",
    action: { label: "Open Loans", to: "/loans" },
  },
  {
    id: "i2",
    tag: "Allocation",
    title: "Property is 61% of net worth",
    body: "Concentration is high. Routing new surplus into equity and debt funds rebalances you over 8 months.",
    impact: "Target 48% by Mar 2027",
    tone: "caution",
    action: { label: "Open Wealth", to: "/assets" },
  },
  {
    id: "i3",
    tag: "Cashflow",
    title: "₹38,000 idle in savings",
    body: "Your average monthly surplus is sitting at 3% interest. A liquid fund or sweep-in FD is a same-day switch.",
    impact: "+₹21,600 a year",
    tone: "neutral",
    action: { label: "Open Investments", to: "/investments" },
  },
];

/* ------------------------------ Mock answers ------------------------------ */

type Answer = Omit<ChatMessage, "id" | "role" | "time">;

const answers: { match: RegExp; answer: Answer }[] = [
  {
    match: /net ?worth|how much.*worth|portfolio value/i,
    answer: {
      text: "Your net worth is ₹2.86 Cr, up ₹1,24,500 (0.44%) today and 14.2% for the year.",
      points: [
        "Assets ₹3.54 Cr across property, equity, gold, bonds and cash",
        "Liabilities ₹67.8 L, all of it the Adyar flat home loan",
        "Property is the largest block at 61% of the total",
      ],
      actions: [
        { label: "Open Wealth", to: "/assets" },
        { label: "See reports", to: "/reports" },
      ],
    },
  },
  {
    match: /prepay|foreclose|close.*loan early/i,
    answer: {
      text: "Yes — a partial prepayment is the highest-return move available to you right now.",
      points: [
        "Outstanding ₹67.8 L at 8.6% for 14 years remaining",
        "Prepaying ₹4 L cuts 19 EMIs and saves about ₹7.4 L in interest",
        "Keep 6 months of EMI (₹3.6 L) as your buffer before prepaying",
      ],
      actions: [
        { label: "Open Loans", to: "/loans" },
        { label: "Check goals", to: "/goals" },
      ],
    },
  },
  {
    match: /another property|buy.*property|second (flat|home)/i,
    answer: {
      text: "You can, but it stretches you. A second property is affordable from mid-2027 without touching your emergency corpus.",
      points: [
        "Eligible EMI headroom today is ₹52,000/month against a ₹41,000 existing EMI",
        "A ₹95 L purchase needs ₹19 L down payment — you hold ₹11.2 L in liquid assets",
        "Real estate would rise to 71% of net worth, well above the 50% comfort band",
      ],
      actions: [
        { label: "Open Properties", to: "/properties" },
        { label: "Open Loans", to: "/loans" },
      ],
    },
  },
  {
    match: /reduce.*debt|debt free|pay off/i,
    answer: {
      text: "You are 34% of the way to debt-free. Three changes get you there by Aug 2033 instead of 2040.",
      points: [
        "Step up your EMI by 5% each year as salary grows",
        "Route the annual bonus (₹2.4 L) straight into principal",
        "Avoid new consumption credit — your card cycle is already fully used each month",
      ],
      actions: [
        { label: "Open Loans", to: "/loans" },
        { label: "Review expenses", to: "/expenses" },
      ],
    },
  },
  {
    match: /tax|80c|deduction/i,
    answer: {
      text: "You have ₹68,000 of unused deductions for FY 2026-27.",
      points: [
        "80C shortfall ₹48,000 — ELSS or PPF top-up closes it",
        "80D health cover for parents adds ₹20,000 of relief",
        "Home loan interest of ₹2 L under 24(b) is already fully claimed",
      ],
      actions: [{ label: "Open Reports", to: "/reports" }],
    },
  },
  {
    match: /investment|allocation|portfolio|sip|equity/i,
    answer: {
      text: "Your investable portfolio is ₹64.5 L with a 12.8% XIRR — ahead of your 11% plan rate.",
      points: [
        "Equity 54%, corporate bond 21%, gold 16%, cash 9%",
        "Two SIPs totalling ₹35,000/month are running on autopay",
        "Gold is 4 points above target after the recent rally — trim on the next high",
      ],
      actions: [{ label: "Open Investments", to: "/investments" }],
    },
  },
  {
    match: /property (perform|value)|adyar|rent/i,
    answer: {
      text: "The Adyar flat (970 sq ft) is valued at ₹1.74 Cr, up 9.1% year on year.",
      points: [
        "Equity in the property is ₹1.06 Cr after the outstanding loan",
        "Rental yield is 2.4% — typical for Chennai residential",
        "Registration-value comparables in the block support the current estimate",
      ],
      actions: [{ label: "Open Properties", to: "/properties" }],
    },
  },
  {
    match: /goal|retire|target/i,
    answer: {
      text: "Two of your three goals are on track at the current savings rate of ₹86,000 a month.",
      points: [
        "Emergency fund 78% funded — ₹2.4 L to go",
        "Child education corpus on track for 2038",
        "Retirement corpus is short by 11% — an extra ₹9,000/month SIP closes the gap",
      ],
      actions: [{ label: "Open Goals", to: "/goals" }],
    },
  },
  {
    match: /expense|spend|budget/i,
    answer: {
      text: "You spent ₹1,42,300 last month, ₹8,400 above your three-month average.",
      points: [
        "Dining and delivery rose 31% to ₹18,600",
        "Fixed costs (EMI, school fees, utilities) are steady at ₹94,000",
        "Trimming discretionary spend by 10% frees ₹11,000/month for prepayment",
      ],
      actions: [{ label: "Open Expenses", to: "/expenses" }],
    },
  },
];

const fallback: Answer = {
  text: "Here's what I can see across your finances right now — net worth ₹2.86 Cr, ₹67.8 L of debt and a ₹86,000 monthly surplus. Ask me about property, loans, investments, taxes or goals and I'll go deeper.",
  actions: [
    { label: "Open Wealth", to: "/assets" },
    { label: "Open Dashboard", to: "/dashboard" },
  ],
};

export const nowLabel = () =>
  new Date().toLocaleTimeString("en-IN", { hour: "numeric", minute: "2-digit" });

let seq = 0;
export const nextId = (prefix: string) => `${prefix}-${Date.now()}-${seq++}`;

/** Deterministic mock "model" — keyword routed, no network. */
export function mockAdvisorReply(question: string): ChatMessage {
  const hit = answers.find((a) => a.match.test(question));
  const answer = hit?.answer ?? fallback;
  return { id: nextId("a"), role: "assistant", time: nowLabel(), ...answer };
}

export function makeUserMessage(text: string): ChatMessage {
  return { id: nextId("u"), role: "user", text, time: nowLabel() };
}

/* --------------------------- Conversation history -------------------------- */

export const conversationHistory: Conversation[] = [
  {
    id: "c1",
    title: "Should I prepay my home loan?",
    preview: "Prepaying ₹4 L cuts 19 EMIs and saves ₹7.4 L…",
    bucket: "Today",
    time: "9:12 AM",
    messages: [
      { id: "c1-u", role: "user", text: "Should I prepay my home loan?", time: "9:12 AM" },
      { id: "c1-a", role: "assistant", time: "9:12 AM", ...answers[1]!.answer },
    ],
  },
  {
    id: "c2",
    title: "Show my net worth",
    preview: "₹2.86 Cr, up 14.2% for the year…",
    bucket: "Today",
    time: "8:04 AM",
    messages: [
      { id: "c2-u", role: "user", text: "Show my net worth", time: "8:04 AM" },
      { id: "c2-a", role: "assistant", time: "8:04 AM", ...answers[0]!.answer },
    ],
  },
  {
    id: "c3",
    title: "Can I buy another property?",
    preview: "Affordable from mid-2027 without touching…",
    bucket: "Yesterday",
    time: "7:41 PM",
    messages: [
      { id: "c3-u", role: "user", text: "Can I buy another property?", time: "7:41 PM" },
      { id: "c3-a", role: "assistant", time: "7:41 PM", ...answers[2]!.answer },
    ],
  },
  {
    id: "c4",
    title: "How can I save tax this year?",
    preview: "₹68,000 of unused deductions for FY 2026-27…",
    bucket: "Earlier",
    time: "28 Jul",
    messages: [
      { id: "c4-u", role: "user", text: "How can I save tax this year?", time: "28 Jul" },
      { id: "c4-a", role: "assistant", time: "28 Jul", ...answers[4]!.answer },
    ],
  },
  {
    id: "c5",
    title: "Review my investment allocation",
    preview: "₹64.5 L invested at a 12.8% XIRR…",
    bucket: "Earlier",
    time: "24 Jul",
    messages: [
      { id: "c5-u", role: "user", text: "Review my investment allocation", time: "24 Jul" },
      { id: "c5-a", role: "assistant", time: "24 Jul", ...answers[5]!.answer },
    ],
  },
];

export const historyBuckets: Conversation["bucket"][] = ["Today", "Yesterday", "Earlier"];
