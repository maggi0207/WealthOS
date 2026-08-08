import { useEffect, useMemo, useState } from "react";
import {
  Bell,
  Download,
  Link2,
  Loader2,
  Lock,
  Palette,
  ShieldAlert,
  Upload,
  UserRound,
} from "lucide-react";
import { toast } from "sonner";

import { PageHeader } from "@/components/layout/page-header";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { TileSkeleton } from "@/components/ui-kit/skeletons";
import {
  useClearSettingsCache,
  useConnectAngelOne,
  useDeleteAccount,
  useExportSettings,
  useImportSettings,
  useSettings,
  useUpdateNotificationSettings,
  useUpdatePreferencesSettings,
  useUpdateProfileSettings,
  useUpdateSecuritySettings,
} from "@/hooks/api/use-settings";
import { toastMutationError } from "@/lib/form-utils";
import { useTheme, type ThemeMode } from "@/lib/theme";
import { cn } from "@/lib/utils";
import type { UserSettingsDto } from "@/services/settings/types";

function SectionCard({
  title,
  description,
  icon: Icon,
  children,
}: {
  title: string;
  description: string;
  icon: React.ComponentType<{ className?: string }>;
  children: React.ReactNode;
}) {
  return (
    <Card className="border-border/60 bg-card/80 shadow-sm">
      <CardHeader className="gap-2 space-y-0 pb-4">
        <div className="flex items-start gap-3">
          <div className="flex size-10 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary">
            <Icon className="size-5" aria-hidden />
          </div>
          <div className="min-w-0">
            <CardTitle className="text-base sm:text-lg">{title}</CardTitle>
            <CardDescription className="mt-1 text-[13px] leading-snug">{description}</CardDescription>
          </div>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">{children}</CardContent>
    </Card>
  );
}

function Field({
  label,
  htmlFor,
  children,
  hint,
}: {
  label: string;
  htmlFor: string;
  children: React.ReactNode;
  hint?: string;
}) {
  return (
    <div className="space-y-1.5">
      <Label htmlFor={htmlFor}>{label}</Label>
      {children}
      {hint ? <p className="text-xs text-muted-foreground">{hint}</p> : null}
    </div>
  );
}

function formatPreview(currency: string, numberFormat: string) {
  const value = 12345678.9;
  if (currency === "INR" || numberFormat === "indian") {
    return new Intl.NumberFormat("en-IN", {
      style: "currency",
      currency: currency === "INR" ? "INR" : currency,
      maximumFractionDigits: 2,
    }).format(value);
  }
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency,
    maximumFractionDigits: 2,
  }).format(value);
}

function downloadBase64File(fileName: string, contentType: string, contentBase64: string) {
  const binary = atob(contentBase64);
  const bytes = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i += 1) bytes[i] = binary.charCodeAt(i);
  const blob = new Blob([bytes], { type: contentType });
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = fileName;
  anchor.click();
  URL.revokeObjectURL(url);
}

export function SettingsModule() {
  const { data, isPending, isError, refetch, isFetching } = useSettings();
  const { setTheme } = useTheme();

  const updateProfile = useUpdateProfileSettings();
  const updatePreferences = useUpdatePreferencesSettings();
  const updateNotifications = useUpdateNotificationSettings();
  const updateSecurity = useUpdateSecuritySettings();
  const connectAngel = useConnectAngelOne();
  const exportSettings = useExportSettings();
  const importSettings = useImportSettings();
  const clearCache = useClearSettingsCache();
  const deleteAccount = useDeleteAccount();

  const [draft, setDraft] = useState<UserSettingsDto | null>(null);
  const [password, setPassword] = useState({ current: "", next: "", confirm: "" });
  const [savingSection, setSavingSection] = useState<string | null>(null);

  useEffect(() => {
    if (data) setDraft(structuredClone(data));
  }, [data]);

  const dirtyProfile = useMemo(() => {
    if (!data || !draft) return false;
    return (
      data.firstName !== draft.firstName ||
      data.lastName !== draft.lastName ||
      data.workspaceName !== draft.workspaceName ||
      data.timezone !== draft.timezone ||
      data.country !== draft.country
    );
  }, [data, draft]);

  const dirtyAppearance = useMemo(() => {
    if (!data || !draft) return false;
    return (
      data.theme !== draft.theme ||
      data.layoutDensity !== draft.layoutDensity ||
      data.sidebarCollapsed !== draft.sidebarCollapsed
    );
  }, [data, draft]);

  const dirtyRegional = useMemo(() => {
    if (!data || !draft) return false;
    return (
      data.currencyCode !== draft.currencyCode ||
      data.locale !== draft.locale ||
      data.dateFormat !== draft.dateFormat ||
      data.numberFormat !== draft.numberFormat
    );
  }, [data, draft]);

  const dirtyNotifications = useMemo(() => {
    if (!data || !draft) return false;
    return (
      data.emailNotifications !== draft.emailNotifications ||
      data.pushNotifications !== draft.pushNotifications ||
      data.goalReminders !== draft.goalReminders ||
      data.loanEmiReminders !== draft.loanEmiReminders ||
      data.investmentAlerts !== draft.investmentAlerts ||
      data.aiAdvisorInsights !== draft.aiAdvisorInsights ||
      data.weeklySummary !== draft.weeklySummary ||
      data.monthlyReport !== draft.monthlyReport
    );
  }, [data, draft]);

  if (isPending || !draft) {
    return (
      <div className="space-y-4">
        <PageHeader title="Settings" description="Preferences, currency and workspace options." />
        <TileSkeleton className="h-40" />
        <TileSkeleton className="h-52" />
        <TileSkeleton className="h-52" />
      </div>
    );
  }

  if (isError) {
    return (
      <div className="space-y-4">
        <PageHeader title="Settings" description="Preferences, currency and workspace options." />
        <Card>
          <CardContent className="flex flex-col items-center gap-3 py-10 text-center">
            <p className="text-sm font-medium">Unable to load settings</p>
            <Button type="button" variant="outline" onClick={() => void refetch()} disabled={isFetching}>
              Retry
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  const patch = <K extends keyof UserSettingsDto>(key: K, value: UserSettingsDto[K]) =>
    setDraft((prev) => (prev ? { ...prev, [key]: value } : prev));

  const saveProfile = async () => {
    setSavingSection("profile");
    try {
      await updateProfile.mutateAsync({
        firstName: draft.firstName,
        lastName: draft.lastName,
        workspaceName: draft.workspaceName,
        avatarUrl: draft.avatarUrl,
        timezone: draft.timezone,
        country: draft.country,
      });
      toast.success("Profile saved");
    } catch (error) {
      toastMutationError(error, "Could not save profile");
    } finally {
      setSavingSection(null);
    }
  };

  const savePreferences = async (section: "appearance" | "regional") => {
    setSavingSection(section);
    try {
      const next = await updatePreferences.mutateAsync({
        theme: draft.theme,
        layoutDensity: draft.layoutDensity,
        sidebarCollapsed: draft.sidebarCollapsed,
        currencyCode: draft.currencyCode,
        locale: draft.locale,
        dateFormat: draft.dateFormat,
        numberFormat: draft.numberFormat,
      });
      if (section === "appearance") {
        setTheme((next.theme as ThemeMode) || "dark");
        document.documentElement.dataset.density = next.layoutDensity;
      }
      toast.success(section === "appearance" ? "Appearance saved" : "Regional settings saved");
    } catch (error) {
      toastMutationError(error, "Could not save preferences");
    } finally {
      setSavingSection(null);
    }
  };

  const saveNotifications = async () => {
    setSavingSection("notifications");
    try {
      await updateNotifications.mutateAsync({
        emailNotifications: draft.emailNotifications,
        pushNotifications: draft.pushNotifications,
        goalReminders: draft.goalReminders,
        loanEmiReminders: draft.loanEmiReminders,
        investmentAlerts: draft.investmentAlerts,
        aiAdvisorInsights: draft.aiAdvisorInsights,
        weeklySummary: draft.weeklySummary,
        monthlyReport: draft.monthlyReport,
      });
      toast.success("Notification preferences saved");
    } catch (error) {
      toastMutationError(error, "Could not save notifications");
    } finally {
      setSavingSection(null);
    }
  };

  const saveSecurity = async () => {
    if (password.next && password.next !== password.confirm) {
      toast.error("New password and confirmation do not match");
      return;
    }
    setSavingSection("security");
    try {
      await updateSecurity.mutateAsync({
        currentPassword: password.current || undefined,
        newPassword: password.next || undefined,
        confirmPassword: password.confirm || undefined,
        twoFactorEnabled: draft.twoFactorEnabled,
      });
      setPassword({ current: "", next: "", confirm: "" });
      toast.success("Security settings updated");
    } catch (error) {
      toastMutationError(error, "Could not update security settings");
    } finally {
      setSavingSection(null);
    }
  };

  const runExport = async (scope: string) => {
    try {
      const file = await exportSettings.mutateAsync(scope);
      downloadBase64File(file.fileName, file.contentType, file.contentBase64);
      toast.success(`Exported ${scope}`);
    } catch (error) {
      toastMutationError(error, "Export failed");
    }
  };

  const onImportFile = async (file: File | null) => {
    if (!file) return;
    const buffer = await file.arrayBuffer();
    const bytes = new Uint8Array(buffer);
    let binary = "";
    bytes.forEach((b) => {
      binary += String.fromCharCode(b);
    });
    try {
      await importSettings.mutateAsync({
        contentBase64: btoa(binary),
        fileName: file.name,
      });
      toast.success("Import completed");
    } catch (error) {
      toastMutationError(error, "Import failed");
    }
  };

  const notificationToggles: Array<{ key: keyof UserSettingsDto; label: string }> = [
    { key: "emailNotifications", label: "Email notifications" },
    { key: "pushNotifications", label: "Push notifications" },
    { key: "goalReminders", label: "Goal reminders" },
    { key: "loanEmiReminders", label: "Loan EMI reminders" },
    { key: "investmentAlerts", label: "Investment alerts" },
    { key: "aiAdvisorInsights", label: "AI advisor insights" },
    { key: "weeklySummary", label: "Weekly summary" },
    { key: "monthlyReport", label: "Monthly report" },
  ];

  return (
    <div className="mx-auto max-w-4xl space-y-5 pb-8">
      <PageHeader
        title="Settings"
        description="Manage profile, appearance, regional formats, notifications and security."
      />

      <SectionCard title="Profile & Workspace" description="Your identity and workspace defaults." icon={UserRound}>
        <div className="grid gap-4 sm:grid-cols-2">
          <Field label="Full name (first)" htmlFor="firstName">
            <Input
              id="firstName"
              value={draft.firstName}
              onChange={(e) => patch("firstName", e.target.value)}
              autoComplete="given-name"
            />
          </Field>
          <Field label="Full name (last)" htmlFor="lastName">
            <Input
              id="lastName"
              value={draft.lastName}
              onChange={(e) => patch("lastName", e.target.value)}
              autoComplete="family-name"
            />
          </Field>
          <Field label="Email" htmlFor="email" hint="Managed by your account login.">
            <Input id="email" value={draft.email} readOnly disabled />
          </Field>
          <Field label="Workspace name" htmlFor="workspaceName">
            <Input
              id="workspaceName"
              value={draft.workspaceName}
              onChange={(e) => patch("workspaceName", e.target.value)}
            />
          </Field>
          <Field label="Timezone" htmlFor="timezone">
            <Input id="timezone" value={draft.timezone} onChange={(e) => patch("timezone", e.target.value)} />
          </Field>
          <Field label="Country" htmlFor="country">
            <Input id="country" value={draft.country} onChange={(e) => patch("country", e.target.value)} />
          </Field>
        </div>
        <div className="rounded-xl border border-dashed border-border/70 bg-muted/20 px-4 py-6 text-center text-sm text-muted-foreground">
          Avatar upload placeholder — drag & drop coming soon.
        </div>
        <div className="flex flex-wrap gap-2">
          <Button type="button" disabled={!dirtyProfile || savingSection === "profile"} onClick={() => void saveProfile()}>
            {savingSection === "profile" ? <Loader2 className="size-4 animate-spin" /> : null}
            Save changes
          </Button>
          <Button
            type="button"
            variant="outline"
            disabled={!dirtyProfile}
            onClick={() => data && setDraft(structuredClone(data))}
          >
            Cancel
          </Button>
        </div>
      </SectionCard>

      <SectionCard title="Appearance" description="Theme, density and sidebar defaults." icon={Palette}>
        <div className="grid gap-4 sm:grid-cols-3">
          <Field label="Theme" htmlFor="theme">
            <select
              id="theme"
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm"
              value={draft.theme}
              onChange={(e) => {
                patch("theme", e.target.value);
                setTheme(e.target.value as ThemeMode);
              }}
            >
              <option value="system">System</option>
              <option value="light">Light</option>
              <option value="dark">Dark</option>
            </select>
          </Field>
          <Field label="Layout density" htmlFor="density">
            <select
              id="density"
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm"
              value={draft.layoutDensity}
              onChange={(e) => patch("layoutDensity", e.target.value)}
            >
              <option value="comfortable">Comfortable</option>
              <option value="compact">Compact</option>
            </select>
          </Field>
          <div className="flex items-end justify-between gap-3 rounded-xl border border-border/60 px-3 py-2">
            <div>
              <p className="text-sm font-medium">Sidebar collapsed by default</p>
              <p className="text-xs text-muted-foreground">Applies on next visit</p>
            </div>
            <Switch
              checked={draft.sidebarCollapsed}
              onCheckedChange={(checked) => patch("sidebarCollapsed", checked)}
              aria-label="Sidebar collapsed by default"
            />
          </div>
        </div>
        <div
          className={cn(
            "rounded-xl border border-border/60 bg-muted/20 p-4 text-sm",
            draft.layoutDensity === "compact" ? "space-y-1" : "space-y-3",
          )}
        >
          <p className="font-medium">Live preview</p>
          <p className="text-muted-foreground">
            Theme: {draft.theme} · Density: {draft.layoutDensity} · Sidebar:{" "}
            {draft.sidebarCollapsed ? "collapsed" : "expanded"}
          </p>
        </div>
        <Button
          type="button"
          disabled={!dirtyAppearance || savingSection === "appearance"}
          onClick={() => void savePreferences("appearance")}
        >
          {savingSection === "appearance" ? <Loader2 className="size-4 animate-spin" /> : null}
          Save appearance
        </Button>
      </SectionCard>

      <SectionCard title="Regional settings" description="Currency, locale and number formats." icon={Download}>
        <div className="grid gap-4 sm:grid-cols-2">
          <Field label="Currency" htmlFor="currency">
            <select
              id="currency"
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm"
              value={draft.currencyCode}
              onChange={(e) => patch("currencyCode", e.target.value)}
            >
              {["INR", "USD", "EUR", "GBP"].map((code) => (
                <option key={code} value={code}>
                  {code}
                </option>
              ))}
            </select>
          </Field>
          <Field label="Locale" htmlFor="locale">
            <select
              id="locale"
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm"
              value={draft.locale}
              onChange={(e) => patch("locale", e.target.value)}
            >
              {["en-IN", "en-US", "en-GB"].map((locale) => (
                <option key={locale} value={locale}>
                  {locale}
                </option>
              ))}
            </select>
          </Field>
          <Field label="Date format" htmlFor="dateFormat">
            <select
              id="dateFormat"
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm"
              value={draft.dateFormat}
              onChange={(e) => patch("dateFormat", e.target.value)}
            >
              {["DD/MM/YYYY", "MM/DD/YYYY", "YYYY-MM-DD"].map((format) => (
                <option key={format} value={format}>
                  {format}
                </option>
              ))}
            </select>
          </Field>
          <Field label="Number format" htmlFor="numberFormat">
            <select
              id="numberFormat"
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm"
              value={draft.numberFormat}
              onChange={(e) => patch("numberFormat", e.target.value)}
            >
              <option value="indian">Indian</option>
              <option value="international">International</option>
            </select>
          </Field>
        </div>
        <div className="rounded-xl border border-border/60 bg-muted/20 px-4 py-3 text-sm">
          <p className="font-medium">Preview</p>
          <p className="mt-1 text-muted-foreground">{formatPreview(draft.currencyCode, draft.numberFormat)}</p>
          <p className="text-muted-foreground">Date example uses {draft.dateFormat}</p>
        </div>
        <Button
          type="button"
          disabled={!dirtyRegional || savingSection === "regional"}
          onClick={() => void savePreferences("regional")}
        >
          {savingSection === "regional" ? <Loader2 className="size-4 animate-spin" /> : null}
          Save regional settings
        </Button>
      </SectionCard>

      <SectionCard title="Notifications" description="Choose which alerts reach you." icon={Bell}>
        <div className="space-y-3">
          {notificationToggles.map((item) => (
            <div
              key={item.key}
              className="flex items-center justify-between gap-3 rounded-xl border border-border/50 px-3 py-2.5"
            >
              <span className="text-sm font-medium">{item.label}</span>
              <Switch
                checked={Boolean(draft[item.key])}
                onCheckedChange={(checked) => patch(item.key, checked as never)}
                aria-label={item.label}
              />
            </div>
          ))}
        </div>
        <Button
          type="button"
          disabled={!dirtyNotifications || savingSection === "notifications"}
          onClick={() => void saveNotifications()}
        >
          {savingSection === "notifications" ? <Loader2 className="size-4 animate-spin" /> : null}
          Save preferences
        </Button>
      </SectionCard>

      <SectionCard title="Data import & export" description="Move data in and out of WealthOS." icon={Upload}>
        <div className="grid gap-3 sm:grid-cols-2">
          <label className="cursor-pointer rounded-xl border border-dashed border-border/70 bg-muted/20 px-4 py-6 text-center text-sm text-muted-foreground hover:bg-muted/30">
            CSV upload placeholder
            <input
              type="file"
              accept=".csv,.json"
              className="sr-only"
              onChange={(e) => void onImportFile(e.target.files?.[0] ?? null)}
            />
          </label>
          <div className="rounded-xl border border-dashed border-border/70 bg-muted/20 px-4 py-6 text-center text-sm text-muted-foreground">
            Excel upload placeholder — future integration
          </div>
        </div>
        <div className="flex flex-wrap gap-2">
          {["all", "investments", "loans", "properties", "reports"].map((scope) => (
            <Button
              key={scope}
              type="button"
              variant="outline"
              disabled={exportSettings.isPending}
              onClick={() => void runExport(scope)}
            >
              Export {scope}
            </Button>
          ))}
        </div>
      </SectionCard>

      <SectionCard title="Security" description="Password, sessions and 2FA." icon={Lock}>
        <div className="grid gap-4 sm:grid-cols-3">
          <Field label="Current password" htmlFor="currentPassword">
            <Input
              id="currentPassword"
              type="password"
              value={password.current}
              onChange={(e) => setPassword((p) => ({ ...p, current: e.target.value }))}
              autoComplete="current-password"
            />
          </Field>
          <Field label="New password" htmlFor="newPassword">
            <Input
              id="newPassword"
              type="password"
              value={password.next}
              onChange={(e) => setPassword((p) => ({ ...p, next: e.target.value }))}
              autoComplete="new-password"
            />
          </Field>
          <Field label="Confirm password" htmlFor="confirmPassword">
            <Input
              id="confirmPassword"
              type="password"
              value={password.confirm}
              onChange={(e) => setPassword((p) => ({ ...p, confirm: e.target.value }))}
              autoComplete="new-password"
            />
          </Field>
        </div>
        <div className="flex items-center justify-between gap-3 rounded-xl border border-border/50 px-3 py-2.5">
          <div>
            <p className="text-sm font-medium">Two-factor authentication</p>
            <p className="text-xs text-muted-foreground">Placeholder — authenticator apps coming soon</p>
          </div>
          <Switch
            checked={draft.twoFactorEnabled}
            onCheckedChange={(checked) => patch("twoFactorEnabled", checked)}
            aria-label="Enable two-factor authentication"
          />
        </div>
        <div className="space-y-2">
          <p className="text-sm font-medium">Active sessions</p>
          {draft.activeSessions.map((session) => (
            <div key={session.id} className="rounded-xl border border-border/50 px-3 py-2 text-sm">
              <p className="font-medium">{session.device}</p>
              <p className="text-xs text-muted-foreground">
                {session.location} · {new Date(session.lastActiveAt).toLocaleString()}
                {session.isCurrent ? " · Current" : ""}
              </p>
            </div>
          ))}
        </div>
        <div className="flex flex-wrap gap-2">
          <Button type="button" disabled={savingSection === "security"} onClick={() => void saveSecurity()}>
            {savingSection === "security" ? <Loader2 className="size-4 animate-spin" /> : null}
            Save security
          </Button>
          <Button
            type="button"
            variant="outline"
            onClick={() =>
              void updateSecurity
                .mutateAsync({ signOutAllDevices: true })
                .then(() => toast.success("Signed out other devices"))
                .catch((error) => toastMutationError(error, "Could not sign out devices"))
            }
          >
            Sign out from all devices
          </Button>
        </div>
      </SectionCard>

      <SectionCard title="Connected accounts" description="Broker and bank integrations." icon={Link2}>
        <div className="grid gap-3 sm:grid-cols-2">
          {[
            { key: "angelOneConnected" as const, name: "Angel One", live: true },
            { key: "indiaBondsConnected" as const, name: "IndiaBonds", live: false },
            { key: "bankSyncConnected" as const, name: "Bank Sync", live: false },
            { key: "upiConnected" as const, name: "UPI", live: false },
          ].map((item) => (
            <div key={item.name} className="rounded-xl border border-border/60 p-4">
              <div className="flex items-start justify-between gap-2">
                <div>
                  <p className="font-medium">{item.name}</p>
                  <p className="text-xs text-muted-foreground">
                    {draft[item.key] ? "Connected" : "Not connected"}
                    {!item.live ? " · Coming soon" : ""}
                  </p>
                </div>
                {item.live ? (
                  <Button
                    type="button"
                    size="sm"
                    variant={draft[item.key] ? "outline" : "default"}
                    disabled={connectAngel.isPending}
                    onClick={() =>
                      void connectAngel
                        .mutateAsync(!draft[item.key])
                        .then(() => toast.success(draft[item.key] ? "Disconnected" : "Connected"))
                        .catch((error) => toastMutationError(error, "Integration update failed"))
                    }
                  >
                    {draft[item.key] ? "Disconnect" : "Connect"}
                  </Button>
                ) : (
                  <Button type="button" size="sm" variant="outline" disabled>
                    Soon
                  </Button>
                )}
              </div>
            </div>
          ))}
        </div>
      </SectionCard>

      <SectionCard title="Advanced" description="Danger zone and cache controls." icon={ShieldAlert}>
        <div className="flex flex-wrap gap-2">
          <Button
            type="button"
            variant="outline"
            onClick={() =>
              void clearCache
                .mutateAsync()
                .then(() => {
                  window.localStorage.removeItem("wealthos.query-cache");
                  toast.success("Local cache cleared");
                })
                .catch((error) => toastMutationError(error, "Could not clear cache"))
            }
          >
            Delete local cached data
          </Button>
          <Button
            type="button"
            variant="destructive"
            onClick={() => {
              const confirmed = window.confirm(
                "Delete your WealthOS account? This soft-deletes the account and revokes sessions.",
              );
              if (!confirmed) return;
              void deleteAccount
                .mutateAsync()
                .then(() => toast.success("Account deleted"))
                .catch((error) => toastMutationError(error, "Could not delete account"));
            }}
          >
            Delete account
          </Button>
        </div>
      </SectionCard>
    </div>
  );
}
