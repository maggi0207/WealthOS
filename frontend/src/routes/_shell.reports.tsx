import { createFileRoute } from "@tanstack/react-router";
import { PieChart } from "lucide-react";

import { ModulePlaceholder } from "@/components/layout/module-placeholder";

export const Route = createFileRoute("/_shell/reports")({
  head: () => ({
    meta: [
      { title: "Reports — WealthOS" },
      { name: "description", content: "Custom financial reports and exports." },
      { property: "og:title", content: "Reports — WealthOS" },
      { property: "og:description", content: "Custom financial reports and exports." },
    ],
  }),
  component: ReportsPage,
});

function ReportsPage() {
  return (
    <ModulePlaceholder
      title="Reports"
      description="Custom financial reports and exports."
      icon={PieChart}
      path="/reports"
      planned={["Net worth and cashflow statements", "Custom date range builder", "PDF and CSV exports", "Scheduled report delivery"]}
    />
  );
}
