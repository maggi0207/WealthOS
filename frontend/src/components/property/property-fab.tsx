import { useState } from "react";
import { FilePlus2, Pencil, Plus, Share2, Trash2, X } from "lucide-react";
import { toast } from "sonner";

import { PropertyFormSheet } from "@/components/property/property-form-sheet";
import { ConfirmDialog } from "@/components/ui-kit/confirm-dialog";
import { useDeleteProperty, usePrimaryProperty } from "@/hooks/api/use-properties";
import { dashboardKeys } from "@/hooks/api/use-dashboard";
import { toastMutationError } from "@/lib/form-utils";
import { useQueryClient } from "@tanstack/react-query";

/** Floating action: add / edit / delete property + document placeholder. */
export function PropertyFab() {
  const [open, setOpen] = useState(false);
  const [formMode, setFormMode] = useState<"create" | "edit" | null>(null);
  const { data: property } = usePrimaryProperty();
  const deleteMutation = useDeleteProperty();
  const queryClient = useQueryClient();

  const closeMenu = () => setOpen(false);

  return (
    <>
      <div className="pointer-events-none sticky bottom-0 z-30 flex flex-col items-end gap-2">
        {open && (
          <div className="pointer-events-auto flex flex-col items-end gap-2">
            <button
              type="button"
              onClick={() => {
                closeMenu();
                setFormMode("create");
              }}
              className="press inline-flex h-11 animate-in fade-in slide-in-from-bottom-2 items-center gap-2 rounded-full bg-card px-4 text-[13px] font-semibold shadow-lg ring-1 ring-border"
            >
              <Plus className="size-4 text-primary" />
              Add property
            </button>
            {property ? (
              <>
                <button
                  type="button"
                  onClick={() => {
                    closeMenu();
                    setFormMode("edit");
                  }}
                  className="press inline-flex h-11 animate-in fade-in slide-in-from-bottom-2 items-center gap-2 rounded-full bg-card px-4 text-[13px] font-semibold shadow-lg ring-1 ring-border"
                >
                  <Pencil className="size-4 text-primary" />
                  Edit property
                </button>
                <ConfirmDialog
                  title="Delete property?"
                  description={`Remove “${property.name}” from your portfolio?`}
                  confirmLabel="Delete"
                  destructive
                  onConfirm={() => {
                    closeMenu();
                    void deleteMutation
                      .mutateAsync(property.id)
                      .then(() => {
                        toast.success("Property deleted");
                        void queryClient.invalidateQueries({ queryKey: dashboardKeys.all });
                      })
                      .catch((err) => toastMutationError(err, "Could not delete property"));
                  }}
                  trigger={
                    <button
                      type="button"
                      className="press inline-flex h-11 animate-in fade-in slide-in-from-bottom-2 items-center gap-2 rounded-full bg-card px-4 text-[13px] font-semibold shadow-lg ring-1 ring-border"
                    >
                      <Trash2 className="size-4 text-destructive" />
                      Delete property
                    </button>
                  }
                />
              </>
            ) : null}
            <button
              type="button"
              onClick={() => {
                closeMenu();
                toast.info("Upload placeholder — add metadata from Documents");
              }}
              className="press inline-flex h-11 animate-in fade-in slide-in-from-bottom-2 items-center gap-2 rounded-full bg-card px-4 text-[13px] font-semibold shadow-lg ring-1 ring-border"
            >
              <FilePlus2 className="size-4 text-primary" />
              Upload photo
            </button>
            <button
              type="button"
              onClick={() => {
                closeMenu();
                if (property?.googleMapsUrl) {
                  window.open(property.googleMapsUrl, "_blank", "noopener,noreferrer");
                } else {
                  toast.info("Add a Google Maps URL when editing the property");
                }
              }}
              className="press inline-flex h-11 animate-in fade-in slide-in-from-bottom-2 items-center gap-2 rounded-full bg-card px-4 text-[13px] font-semibold shadow-lg ring-1 ring-border"
            >
              <Share2 className="size-4 text-primary" />
              Open in Maps
            </button>
          </div>
        )}

        <button
          type="button"
          aria-expanded={open}
          aria-label={open ? "Close actions" : "Property actions"}
          onClick={() => setOpen((v) => !v)}
          className="press pointer-events-auto mb-[calc(5.5rem+env(safe-area-inset-bottom))] inline-flex h-12 items-center gap-2 rounded-full bg-primary px-5 text-[14px] font-semibold text-primary-foreground shadow-lg shadow-primary/25 md:mb-6"
        >
          {open ? <X className="size-5" /> : <Plus className="size-5" />}
          {open ? "Close" : "Actions"}
        </button>
      </div>

      <PropertyFormSheet
        open={formMode !== null}
        onOpenChange={(next) => {
          if (!next) setFormMode(null);
        }}
        mode={formMode === "edit" ? "edit" : "create"}
        propertyId={property?.id}
        initial={property}
      />
    </>
  );
}
