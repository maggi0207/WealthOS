import { Link } from "@tanstack/react-router";
import { Pencil, Trash2 } from "lucide-react";
import { useState } from "react";

import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { ManualAssetFormSheet } from "@/components/wealth/manual-asset-form-sheet";
import { Button } from "@/components/ui/button";
import { useDeleteManualAsset, useManualAssets } from "@/hooks/api/use-manual-assets";
import {
  formatAssetMoney,
  formatRelativeDate,
  type UnifiedAsset,
} from "@/lib/assets-utils";
import { toastMutationError } from "@/lib/form-utils";
import { cn } from "@/lib/utils";
import { toast } from "sonner";

/** Section 3 — merged derived + manual asset cards. */
export function AssetsList({
  assets,
  currencyCode,
  loading,
}: {
  assets: UnifiedAsset[];
  currencyCode: string;
  loading?: boolean;
}) {
  const { data: manuals } = useManualAssets({ pageSize: 100 });
  const deleteMutation = useDeleteManualAsset();
  const [editId, setEditId] = useState<string | null>(null);

  const editAsset = manuals?.items.find((a) => a.id === editId) ?? null;

  if (loading) {
    return (
      <div className="grid gap-2.5 sm:grid-cols-2 xl:grid-cols-3">
        {Array.from({ length: 6 }).map((_, i) => (
          <div key={i} className="surface-tile h-[148px] animate-pulse bg-muted/40" />
        ))}
      </div>
    );
  }

  if (assets.length === 0) {
    return (
      <div className="surface-tile flex min-h-[160px] flex-col items-center justify-center gap-1 p-8 text-center">
        <p className="text-sm font-medium">No assets yet</p>
        <p className="max-w-sm text-[12px] text-muted-foreground">
          Add a property, investment or manual holding to start building your wealth view.
        </p>
      </div>
    );
  }

  return (
    <>
      <div className="grid gap-2.5 sm:grid-cols-2 xl:grid-cols-3">
        {assets.map((asset) => {
          const gainPositive = asset.gainLoss >= 0;
          return (
            <article
              key={asset.id}
              className="surface-tile flex min-h-[148px] flex-col justify-between p-3.5 transition-colors hover:bg-muted/15"
            >
              <div className="flex items-start justify-between gap-2">
                <div className="min-w-0">
                  <p className="truncate text-[14px] font-semibold">{asset.name}</p>
                  <span className="mt-1 inline-flex rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-semibold text-primary">
                    {asset.categoryLabel}
                  </span>
                </div>
                <span className="shrink-0 rounded-full bg-muted/70 px-2 py-0.5 text-[10px] font-medium text-muted-foreground">
                  {asset.source === "manual" ? "Manual" : "Linked"}
                </span>
              </div>

              <div className="mt-3 space-y-1">
                <p className="text-[18px] font-semibold tabular-nums">
                  {formatAssetMoney(asset.currentValue, currencyCode)}
                </p>
                <p className="text-[11px] text-muted-foreground">
                  Purchase {formatAssetMoney(asset.purchaseValue, currencyCode)}
                  {asset.gainLossPercent != null ? (
                    <span
                      className={cn(
                        "ml-1.5 font-medium",
                        gainPositive ? "text-success" : "text-destructive",
                      )}
                    >
                      {gainPositive ? "+" : ""}
                      {asset.gainLossPercent.toFixed(1)}%
                    </span>
                  ) : null}
                </p>
                <p className="text-[11px] text-muted-foreground">
                  Updated {formatRelativeDate(asset.updatedAt)}
                </p>
              </div>

              <div className="mt-3 flex gap-1.5">
                {asset.source === "manual" ? (
                  <>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      className="h-9 flex-1 rounded-full"
                      onClick={() => setEditId(asset.sourceId)}
                    >
                      <Pencil className="size-3.5" />
                      Edit
                    </Button>
                    <ConfirmDialog
                      title="Delete asset?"
                      description={`Remove “${asset.name}” from your wealth summary?`}
                      confirmLabel="Delete"
                      destructive
                      onConfirm={() => {
                        void deleteMutation
                          .mutateAsync(asset.sourceId)
                          .then(() => toast.success("Asset deleted"))
                          .catch((err) => toastMutationError(err, "Could not delete asset"));
                      }}
                      trigger={
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          className="h-9 rounded-full text-destructive hover:bg-destructive/10 hover:text-destructive"
                        >
                          <Trash2 className="size-3.5" />
                        </Button>
                      }
                    />
                  </>
                ) : (
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    className="h-9 flex-1 rounded-full"
                    asChild
                  >
                    <Link to={asset.source === "property" ? "/properties" : "/investments"}>
                      <Pencil className="size-3.5" />
                      Edit in module
                    </Link>
                  </Button>
                )}
              </div>
            </article>
          );
        })}
      </div>

      <ManualAssetFormSheet
        open={Boolean(editId)}
        onOpenChange={(open) => {
          if (!open) setEditId(null);
        }}
        mode="edit"
        asset={editAsset}
      />
    </>
  );
}
