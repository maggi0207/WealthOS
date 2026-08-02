import { Upload } from "lucide-react";
import { useState } from "react";

import { DocumentFormSheet } from "@/components/documents/document-form-sheet";

/** Sticky floating upload action — opens the document metadata form. */
export function UploadFab() {
  const [formOpen, setFormOpen] = useState(false);

  return (
    <>
      <div className="pointer-events-none sticky bottom-0 z-30 flex justify-end">
        <button
          type="button"
          onClick={() => setFormOpen(true)}
          className="press pointer-events-auto mb-[calc(5.5rem+env(safe-area-inset-bottom))] inline-flex h-12 items-center gap-2 rounded-full bg-primary px-5 text-[14px] font-semibold text-primary-foreground shadow-lg shadow-primary/25 md:mb-6"
        >
          <Upload className="size-5" />
          Add document
        </button>
      </div>

      <DocumentFormSheet open={formOpen} onOpenChange={setFormOpen} mode="create" />
    </>
  );
}
