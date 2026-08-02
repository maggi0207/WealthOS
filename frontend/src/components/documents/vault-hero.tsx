import { FolderLock, ShieldCheck } from "lucide-react";

import { vaultSummary } from "@/lib/documents-data";

/** Vault overview hero — counts, verification state and storage. */
export function VaultHero() {
  return (
    <section className="surface-hero p-4 sm:p-5">
      <div className="grid grid-cols-[minmax(0,1fr)_auto] items-start gap-3">
        <div className="min-w-0">
          <p className="text-[10px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
            Document vault
          </p>
          <p className="mt-1 font-display text-fluid-2xl font-semibold tabular-nums">{vaultSummary.total} files</p>
          <p className="mt-1 text-[12px] text-muted-foreground">{vaultSummary.storageLabel} used</p>
        </div>
        <span className="grid size-11 shrink-0 place-items-center rounded-xl bg-primary/10 text-primary">
          <FolderLock className="size-5" />
        </span>
      </div>

      <div className="mt-4 grid grid-cols-3 gap-2">
        {[
          { label: "Verified", value: `${vaultSummary.verified}` },
          { label: "Action", value: `${vaultSummary.actionNeeded}` },
          { label: "Categories", value: "6" },
        ].map((cell) => (
          <div key={cell.label} className="rounded-xl bg-muted/40 px-3 py-2">
            <p className="truncate text-[10px] font-semibold uppercase tracking-[0.1em] text-muted-foreground">
              {cell.label}
            </p>
            <p className="mt-0.5 truncate text-[13px] font-semibold tabular-nums sm:text-sm">{cell.value}</p>
          </div>
        ))}
      </div>

      <p className="mt-3 flex items-center gap-1.5 text-[11px] text-muted-foreground">
        <ShieldCheck className="size-3.5 shrink-0 text-success" />
        Stored locally in this demo — nothing leaves your device.
      </p>
    </section>
  );
}
