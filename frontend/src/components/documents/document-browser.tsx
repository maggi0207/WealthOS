import { useEffect, useMemo, useState } from "react";
import { FileText, Search, SlidersHorizontal } from "lucide-react";
import { toast } from "sonner";

import { EmptyState } from "@/components/ui-kit/empty-state";
import { ListSkeleton } from "@/components/ui-kit/skeletons";
import {
  docCategories,
  docStatusLabel,
  documents,
  fmtDate,
  type DocCategory,
  type DocStatus,
} from "@/lib/documents-data";

const statusTone: Record<DocStatus, string> = {
  verified: "bg-success/12 text-success",
  pending: "bg-warning/12 text-warning",
  expiring: "bg-amber-500/12 text-amber-500",
  expired: "bg-destructive/12 text-destructive",
};

/** Searchable, filterable document browser with loading and empty states. */
export function DocumentBrowser({
  category,
  onCategoryChange,
}: {
  category: DocCategory | "all";
  onCategoryChange: (category: DocCategory | "all") => void;
}) {
  const [query, setQuery] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const timer = window.setTimeout(() => setLoading(false), 520);
    return () => window.clearTimeout(timer);
  }, []);

  const results = useMemo(() => {
    const q = query.trim().toLowerCase();
    return documents.filter((doc) => {
      const inCategory = category === "all" || doc.category === category;
      const inQuery =
        !q ||
        doc.name.toLowerCase().includes(q) ||
        doc.tags.some((tag) => tag.includes(q)) ||
        (doc.linkedTo?.toLowerCase().includes(q) ?? false);
      return inCategory && inQuery;
    });
  }, [query, category]);

  return (
    <div className="space-y-2.5">
      <div className="grid grid-cols-[minmax(0,1fr)_auto] gap-2">
        <div className="surface-tile flex min-h-11 items-center gap-2 px-3">
          <Search className="size-4 shrink-0 text-muted-foreground" />
          <input
            value={query}
            onChange={(event) => setQuery(event.target.value)}
            placeholder="Search documents or tags"
            aria-label="Search documents"
            className="min-w-0 flex-1 bg-transparent text-[14px] outline-none placeholder:text-muted-foreground"
          />
        </div>
        <button
          type="button"
          aria-label="More filters"
          onClick={() => toast.info("Advanced filters arrive with the vault editor")}
          className="surface-tile press grid size-11 shrink-0 place-items-center text-muted-foreground"
        >
          <SlidersHorizontal className="size-4" />
        </button>
      </div>

      <div className="no-scrollbar bleed-gutter page-gutter flex gap-2 overflow-x-auto pb-1 sm:mx-0 sm:flex-wrap sm:px-0">
        {[{ id: "all" as const, label: "All" }, ...docCategories].map((chip) => (
          <button
            key={chip.id}
            type="button"
            onClick={() => onCategoryChange(chip.id)}
            aria-pressed={category === chip.id}
            className={`press inline-flex min-h-11 shrink-0 items-center rounded-full px-4 text-[13px] font-semibold transition-colors ${
              category === chip.id ? "bg-primary text-primary-foreground" : "bg-muted/60 text-muted-foreground"
            }`}
          >
            {chip.label}
          </button>
        ))}
      </div>

      {loading ? (
        <ListSkeleton rows={5} />
      ) : results.length === 0 ? (
        <EmptyState
          icon={FileText}
          title="No documents found"
          description="Try a different search term or category, or upload a new file to the vault."
        />
      ) : (
        <ul className="surface-tile divide-y divide-border/50 overflow-hidden">
          {results.map((doc) => (
            <li key={doc.id}>
              <button
                type="button"
                onClick={() => toast.info(`${doc.name} — preview arrives with the vault`)}
                className="press grid w-full grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 px-4 py-3 text-left"
              >
                <span className="grid size-9 shrink-0 place-items-center rounded-xl bg-primary/10 text-primary">
                  <FileText className="size-4" />
                </span>
                <span className="min-w-0">
                  <span className="block truncate text-[14px] font-medium">{doc.name}</span>
                  <span className="block truncate text-[11px] text-muted-foreground">
                    {doc.fileType} · {doc.sizeLabel} · updated {fmtDate(doc.updatedOn)}
                    {doc.linkedTo ? ` · ${doc.linkedTo}` : ""}
                  </span>
                  <span className="mt-1 flex flex-wrap gap-1">
                    {doc.tags.map((tag) => (
                      <span
                        key={tag}
                        className="rounded-full bg-muted/60 px-2 py-0.5 text-[10px] text-muted-foreground"
                      >
                        #{tag}
                      </span>
                    ))}
                  </span>
                </span>
                <span
                  className={`shrink-0 rounded-full px-2 py-0.5 text-[10px] font-semibold ${statusTone[doc.status]}`}
                >
                  {docStatusLabel[doc.status]}
                </span>
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
