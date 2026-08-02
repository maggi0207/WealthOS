import { Plus } from "lucide-react";
import { toast } from "sonner";

/**
 * Sticky floating action — sits above the mobile bottom nav and the iOS
 * home indicator, and docks to the bottom-right corner on desktop.
 */
export function AddAssetFab({ label = "Add asset" }: { label?: string }) {
  return (
    <div className="pointer-events-none sticky bottom-0 z-30 flex justify-end">
      <button
        type="button"
        onClick={() => toast.success(`${label} — coming with the asset editor`)}
        className="press pointer-events-auto mb-[calc(5.5rem+env(safe-area-inset-bottom))] inline-flex h-12 items-center gap-2 rounded-full bg-primary px-5 text-[14px] font-semibold text-primary-foreground shadow-lg shadow-primary/25 md:mb-6"
      >
        <Plus className="size-5" />
        {label}
      </button>
    </div>
  );
}
