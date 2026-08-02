import { createFileRoute } from "@tanstack/react-router";
import { Settings } from "lucide-react";

import { ModulePlaceholder } from "@/components/layout/module-placeholder";

export const Route = createFileRoute("/_shell/settings")({
  head: () => ({
    meta: [
      { title: "Settings — WealthOS" },
      { name: "description", content: "Preferences, currency and workspace options." },
      { property: "og:title", content: "Settings — WealthOS" },
      { property: "og:description", content: "Preferences, currency and workspace options." },
    ],
  }),
  component: SettingsPage,
});

function SettingsPage() {
  return (
    <ModulePlaceholder
      title="Settings"
      description="Preferences, currency and workspace options."
      icon={Settings}
      path="/settings"
      planned={["Profile and workspace preferences", "Currency, locale and number formats", "Theme and layout density", "Data import and export"]}
    />
  );
}
