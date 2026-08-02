import { createFileRoute } from "@tanstack/react-router";
import { Receipt } from "lucide-react";

import { ModulePlaceholder } from "@/components/layout/module-placeholder";

export const Route = createFileRoute("/_shell/expenses")({
  head: () => ({
    meta: [
      { title: "Expenses — WealthOS" },
      { name: "description", content: "Spending categories, trends and recurring costs." },
      { property: "og:title", content: "Expenses — WealthOS" },
      { property: "og:description", content: "Spending categories, trends and recurring costs." },
    ],
  }),
  component: ExpensesPage,
});

function ExpensesPage() {
  return (
    <ModulePlaceholder
      title="Expenses"
      description="Spending categories, trends and recurring costs."
      icon={Receipt}
      path="/expenses"
      planned={["Category budgets and burn rate", "Recurring subscription detection", "Merchant and trend analytics", "Anomaly flags"]}
    />
  );
}
