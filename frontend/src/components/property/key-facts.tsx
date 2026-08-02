import { TileSkeleton } from "@/components/ui-kit/skeletons";
import { usePrimaryProperty } from "@/hooks/api/use-properties";

/** Key facts as scrollable chips — full-bleed on mobile so nothing clips. */
export function KeyFacts() {
  const { data, isPending, isError } = usePrimaryProperty();

  if (isPending) {
    return (
      <div className="bleed-gutter no-scrollbar flex gap-2 overflow-x-auto px-[var(--page-gutter)]">
        {Array.from({ length: 4 }).map((_, i) => (
          <TileSkeleton key={i} className="h-14 w-28 shrink-0" />
        ))}
      </div>
    );
  }

  if (isError || !data?.keyFacts.length) {
    return null;
  }

  return (
    <div className="bleed-gutter no-scrollbar flex snap-x snap-mandatory gap-2 overflow-x-auto px-[var(--page-gutter)] pb-0.5">
      {data.keyFacts.map((fact) => (
        <div
          key={fact.label}
          className="surface-tile snap-start whitespace-nowrap px-3 py-2"
        >
          <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
            {fact.label}
          </p>
          <p className="mt-0.5 text-[13px] font-semibold tabular-nums">{fact.value}</p>
        </div>
      ))}
    </div>
  );
}
