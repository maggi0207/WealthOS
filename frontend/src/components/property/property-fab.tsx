import { useState } from "react";
import { FilePlus2, Pencil, Plus, Receipt, Share2, X } from "lucide-react";
import { toast } from "sonner";

const actions = [
  { icon: FilePlus2, label: "Add document", msg: "Add document — coming with the vault" },
  { icon: Receipt, label: "Add expense", msg: "Add expense — coming with the expense tracker" },
  { icon: Pencil, label: "Edit property", msg: "Edit property — coming with the property editor" },
  { icon: Share2, label: "Share property", msg: "Share link copied (demo)" },
];

/** Floating action: document, expense, edit and share (mock actions). */
export function PropertyFab() {
  const [open, setOpen] = useState(false);

  const act = (message: string) => {
    setOpen(false);
    toast.success(message);
  };

  return (
    <div className="pointer-events-none sticky bottom-0 z-30 flex flex-col items-end gap-2">
      {open && (
        <div className="pointer-events-auto flex flex-col items-end gap-2">
          {actions.map((action) => (
            <button
              key={action.label}
              type="button"
              onClick={() => act(action.msg)}
              className="press inline-flex h-11 animate-in fade-in slide-in-from-bottom-2 items-center gap-2 rounded-full bg-card px-4 text-[13px] font-semibold shadow-lg ring-1 ring-border"
            >
              <action.icon className="size-4 text-primary" />
              {action.label}
            </button>
          ))}
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
  );
}
