import { Link2, Loader2, PlugZap, RefreshCw, Unplug } from "lucide-react";
import { useMemo, useState } from "react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ListSkeleton } from "@/components/ui-kit/skeletons";
import {
  useConnectInvestmentProvider,
  useCreateInvestmentAccount,
  useDisconnectInvestmentProvider,
  useInvestmentProviders,
  useInvestmentsOverview,
  useSyncInvestmentProvider,
} from "@/hooks/api/use-investments";
import { toastMutationError } from "@/lib/form-utils";
import { fmtINRShort, type InvestmentAccount } from "@/lib/investments-data";
import { cn } from "@/lib/utils";
import { settingsService } from "@/services/settings/settings-service";

function isAngelAccount(account: InvestmentAccount): boolean {
  const kind = String(account.providerKind ?? "");
  const name = `${account.providerName ?? ""} ${account.name} ${account.kind}`.toLowerCase();
  return kind === "1" || kind === "AngelOne" || name.includes("angel");
}

/**
 * Live Angel One broker link panel — connect, sync holdings, disconnect,
 * and show live account/holdings summary on the Investments page.
 */
export function AngelOneLinkCard() {
  const { data, isPending, isError, refetch, isFetching } = useInvestmentsOverview();
  const { data: providers } = useInvestmentProviders();
  const createAccount = useCreateInvestmentAccount();
  const connect = useConnectInvestmentProvider();
  const sync = useSyncInvestmentProvider();
  const disconnect = useDisconnectInvestmentProvider();
  const [busy, setBusy] = useState<"connect" | "sync" | "disconnect" | null>(null);

  const angelAccounts = useMemo(
    () => (data?.accounts ?? []).filter(isAngelAccount),
    [data?.accounts],
  );

  const angelHoldings = useMemo(() => {
    const ids = new Set(angelAccounts.map((a) => a.id));
    return (data?.holdings ?? []).filter((h) => ids.has(h.accountId));
  }, [angelAccounts, data?.holdings]);

  const angelProvider = useMemo(() => {
    const items = providers?.items ?? [];
    return items.find((p) => {
      const kind = String(p.kind);
      return kind === "1" || kind === "AngelOne" || p.name.toLowerCase().includes("angel");
    });
  }, [providers?.items]);

  const connected = angelAccounts.some((a) => a.status === "connected");
  const totalValue = angelAccounts.reduce((sum, a) => sum + a.value, 0);
  const totalHoldings = angelAccounts.reduce((sum, a) => sum + a.holdings, 0);
  const lastSync =
    angelAccounts
      .map((a) => a.lastSyncedAt)
      .filter(Boolean)
      .sort()
      .at(-1) ?? null;

  const ensureAccount = async (): Promise<string> => {
    if (angelAccounts[0]) return angelAccounts[0].id;
    if (!angelProvider?.id) {
      throw new Error("Angel One provider is not available yet. Redeploy the API and try again.");
    }
    const created = await createAccount.mutateAsync({
      providerId: angelProvider.id,
      name: "Angel One",
      ownerName: "Primary",
      kindLabel: "Broker · Stocks & MF",
      status: 3,
    });
    return created.id;
  };

  const onConnect = async () => {
    setBusy("connect");
    try {
      const accountId = await ensureAccount();
      await connect.mutateAsync(accountId);
      await settingsService.connectAngelOne(true).catch(() => undefined);
      toast.success("Angel One connected");
      await refetch();
    } catch (error) {
      toastMutationError(error, "Could not connect Angel One");
    } finally {
      setBusy(null);
    }
  };

  const onSync = async () => {
    if (angelAccounts.length === 0) {
      toast.error("Connect Angel One first");
      return;
    }
    setBusy("sync");
    try {
      for (const account of angelAccounts) {
        await sync.mutateAsync({ accountId: account.id, target: "holdings" });
        await sync.mutateAsync({ accountId: account.id, target: "portfolio" });
      }
      toast.success("Angel One holdings refreshed");
      await refetch();
    } catch (error) {
      toastMutationError(error, "Angel One sync failed");
    } finally {
      setBusy(null);
    }
  };

  const onDisconnect = async () => {
    if (angelAccounts.length === 0) return;
    setBusy("disconnect");
    try {
      for (const account of angelAccounts) {
        await disconnect.mutateAsync(account.id);
      }
      await settingsService.connectAngelOne(false).catch(() => undefined);
      toast.success("Angel One disconnected");
      await refetch();
    } catch (error) {
      toastMutationError(error, "Could not disconnect Angel One");
    } finally {
      setBusy(null);
    }
  };

  if (isPending) return <ListSkeleton rows={2} />;

  if (isError || !data) {
    return (
      <Card className="border-border/60 bg-card/80">
        <CardContent className="flex flex-col items-center gap-3 py-8 text-center">
          <p className="text-sm font-medium">Unable to load Angel One status</p>
          <Button type="button" variant="outline" onClick={() => void refetch()} disabled={isFetching}>
            Retry
          </Button>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="border-border/60 bg-card/80 shadow-sm">
      <CardHeader className="gap-2 space-y-0 pb-3">
        <div className="flex items-start justify-between gap-3">
          <div className="flex min-w-0 items-start gap-3">
            <div className="flex size-10 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary">
              <Link2 className="size-5" aria-hidden />
            </div>
            <div className="min-w-0">
              <CardTitle className="text-base sm:text-lg">Angel One</CardTitle>
              <CardDescription className="mt-1 text-[13px] leading-snug">
                Live broker link for stocks, mutual funds and portfolio sync.
              </CardDescription>
            </div>
          </div>
          <span
            className={cn(
              "inline-flex shrink-0 items-center rounded-full px-2.5 py-1 text-[11px] font-semibold",
              connected ? "bg-success/12 text-success" : "bg-secondary text-muted-foreground",
            )}
          >
            {connected ? "Connected" : "Not connected"}
          </span>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
          <Metric label="Accounts" value={String(angelAccounts.length)} />
          <Metric label="Holdings" value={String(totalHoldings || angelHoldings.length)} />
          <Metric label="Value" value={fmtINRShort(totalValue)} />
          <Metric
            label="Last sync"
            value={
              lastSync
                ? new Date(lastSync).toLocaleString("en-IN", {
                    day: "numeric",
                    month: "short",
                    hour: "2-digit",
                    minute: "2-digit",
                  })
                : "Never"
            }
          />
        </div>

        {angelAccounts.length > 0 ? (
          <ul className="divide-y divide-border/50 overflow-hidden rounded-xl border border-border/60">
            {angelAccounts.map((account) => (
              <li key={account.id} className="flex items-center justify-between gap-3 px-3 py-2.5 text-sm">
                <div className="min-w-0">
                  <p className="truncate font-medium">
                    {account.name} · {account.owner}
                  </p>
                  <p className="text-xs text-muted-foreground">
                    {account.status} · {account.lastSync} · {account.holdings} holdings
                  </p>
                </div>
                <p className="shrink-0 font-display text-sm font-semibold tabular-nums">
                  {fmtINRShort(account.value)}
                </p>
              </li>
            ))}
          </ul>
        ) : (
          <p className="rounded-xl border border-dashed border-border/70 bg-muted/20 px-4 py-5 text-center text-sm text-muted-foreground">
            No Angel One account linked yet. Connect to show live holdings here.
          </p>
        )}

        {angelHoldings.length > 0 ? (
          <div className="space-y-2">
            <p className="text-sm font-medium">Live holdings preview</p>
            <ul className="divide-y divide-border/50 overflow-hidden rounded-xl border border-border/60">
              {angelHoldings.slice(0, 6).map((holding) => (
                <li key={holding.id} className="flex items-center justify-between gap-3 px-3 py-2 text-sm">
                  <div className="min-w-0">
                    <p className="truncate font-medium">{holding.name}</p>
                    <p className="text-xs text-muted-foreground">
                      {holding.ticker || holding.category}
                    </p>
                  </div>
                  <p className="shrink-0 tabular-nums font-medium">{fmtINRShort(holding.value)}</p>
                </li>
              ))}
            </ul>
            {angelHoldings.length > 6 ? (
              <p className="text-xs text-muted-foreground">
                +{angelHoldings.length - 6} more in Holdings below
              </p>
            ) : null}
          </div>
        ) : null}

        <div className="flex flex-wrap gap-2">
          {!connected ? (
            <Button type="button" disabled={busy !== null} onClick={() => void onConnect()}>
              {busy === "connect" ? <Loader2 className="size-4 animate-spin" /> : <PlugZap className="size-4" />}
              Connect Angel One
            </Button>
          ) : (
            <>
              <Button type="button" disabled={busy !== null} onClick={() => void onSync()}>
                {busy === "sync" ? <Loader2 className="size-4 animate-spin" /> : <RefreshCw className="size-4" />}
                Sync live
              </Button>
              <Button
                type="button"
                variant="outline"
                disabled={busy !== null}
                onClick={() => void onDisconnect()}
              >
                {busy === "disconnect" ? <Loader2 className="size-4 animate-spin" /> : <Unplug className="size-4" />}
                Disconnect
              </Button>
            </>
          )}
        </div>
      </CardContent>
    </Card>
  );
}

function Metric({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-xl border border-border/50 bg-muted/15 px-3 py-2">
      <p className="text-[11px] text-muted-foreground">{label}</p>
      <p className="mt-0.5 truncate text-sm font-semibold tabular-nums">{value}</p>
    </div>
  );
}
