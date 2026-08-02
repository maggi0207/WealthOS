import { Clock3 } from "lucide-react";
import { toast } from "sonner";

import { ListSkeleton } from "@/components/ui-kit/skeletons";
import { useDocumentsOverview } from "@/hooks/api/use-documents";
import { fmtDate } from "@/lib/documents-data";

/** Recently updated documents rail. */
export function RecentDocuments() {
  const { data, isPending, isError, refetch, isFetching } = useDocumentsOverview();

  if (isPending) return <ListSkeleton rows={2} />;
  if (isError || !data) {
    return (
      <div className="surface-tile px-4 py-6 text-center">
        <p className="text-sm font-medium">Unable to load recent documents</p>
        <button
          type="button"
          onClick={() => void refetch()}
          disabled={isFetching}
          className="press mt-3 inline-flex min-h-11 items-center rounded-full bg-primary px-4 text-xs font-semibold text-primary-foreground"
        >
          Retry
        </button>
      </div>
    );
  }

  return (
    <div className="no-scrollbar bleed-gutter page-gutter flex snap-x snap-mandatory gap-2.5 overflow-x-auto pb-1 sm:mx-0 sm:grid sm:grid-cols-3 sm:px-0 lg:grid-cols-5">
      {data.recent.map((doc) => (
        <button
          key={doc.id}
          type="button"
          onClick={() => toast.info(`${doc.name} — preview arrives with the vault`)}
          className="surface-tile press w-[62vw] max-w-[220px] shrink-0 snap-start p-3 text-left sm:w-auto sm:max-w-none"
        >
          <span className="inline-flex items-center gap-1.5 rounded-full bg-muted/60 px-2 py-0.5 text-[10px] font-semibold text-muted-foreground">
            <Clock3 className="size-3" />
            {fmtDate(doc.updatedOn)}
          </span>
          <p className="mt-2 line-clamp-2 text-[13px] font-semibold leading-snug">{doc.name}</p>
          <p className="mt-1 truncate text-[11px] text-muted-foreground">
            {doc.fileType} · {doc.sizeLabel}
          </p>
        </button>
      ))}
    </div>
  );
}
