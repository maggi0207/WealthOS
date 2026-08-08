import { createFileRoute } from "@tanstack/react-router";

import { SettingsModule } from "@/components/settings/settings-module";

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
  return <SettingsModule />;
}
