import { useState } from "react";
import { FileText, ShieldCheck, Upload } from "lucide-react";
import { toast } from "sonner";

import { vaultDocs, type VaultDoc } from "@/lib/property-data";

const statusStyles: Record<VaultDoc["status"], { label: string; className: string }> = {
  verified: { label: "Verified", className: "bg-success/12 text-success" },
  pending: { label: "Pending", className: "bg-warning/12 text-warning" },
  expiring: { label: "Renew soon", className: "bg-destructive/12 text-destructive" },
  missing: { label: "Upload", className: "bg-muted text-muted-foreground" },
};

/** Document vault — cards per document with an upload affordance. */
export function DocumentVault() {
  const [uploaded, setUploaded] = useState<string[]>([]);

  return (
    <section className="space-y-2">
      <div className="grid gap-2 sm:grid-cols-2">
        {vaultDocs.map((doc) => {
          const isUploaded = uploaded.includes(doc.id);
          const status = isUploaded ? statusStyles.pending : statusStyles[doc.status];
          return (
            <button
              key={doc.id}
              type="button"
              onClick={() => {
                if (doc.status === "missing" && !isUploaded) {
                  setUploaded((prev) => [...prev, doc.id]);
                  toast.success(`${doc.name} queued for upload`);
                } else {
                  toast.info(`${doc.name} — preview coming with the vault`);
                }
              }}
              className="surface-tile press flex w-full items-center gap-3 p-3 text-left transition-colors"
            >
              <span
                className={`grid size-10 shrink-0 place-items-center rounded-xl ${
                  doc.status === "missing" && !isUploaded
                    ? "bg-muted text-muted-foreground"
                    : "bg-primary/10 text-primary"
                }`}
              >
                {doc.status === "missing" && !isUploaded ? (
                  <Upload className="size-4" />
                ) : (
                  <FileText className="size-4" />
                )}
              </span>
              <span className="min-w-0 flex-1">
                <span className="block truncate text-[14px] font-medium">{doc.name}</span>
                <span className="block truncate text-[11px] text-muted-foreground">
                  {isUploaded ? "Uploaded just now" : doc.meta}
                </span>
              </span>
              <span
                className={`shrink-0 rounded-full px-2 py-0.5 text-[10px] font-semibold ${status.className}`}
              >
                {status.label}
              </span>
            </button>
          );
        })}
      </div>

      <p className="flex items-center gap-1.5 px-1 text-[11px] text-muted-foreground">
        <ShieldCheck className="size-3.5 shrink-0 text-success" />
        Stored locally in this demo — nothing leaves your device.
      </p>
    </section>
  );
}
