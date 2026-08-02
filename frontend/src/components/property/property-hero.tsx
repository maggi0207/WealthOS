import { useEffect, useRef, useState } from "react";
import { ArrowUpRight, Expand, MapPin, X } from "lucide-react";

import { HeroSkeleton } from "@/components/ui-kit/skeletons";
import { usePrimaryProperty } from "@/hooks/api/use-properties";
import { fmtINR, fmtINRShort } from "@/lib/property-data";

/** Hero: swipeable real-photo gallery + value headline. */
export function PropertyHero() {
  const { data, isPending, isError, refetch, isFetching } = usePrimaryProperty();
  const railRef = useRef<HTMLDivElement>(null);
  const [index, setIndex] = useState(0);
  const [full, setFull] = useState<number | null>(null);

  const photos = data?.photos ?? [];

  useEffect(() => {
    const el = railRef.current;
    if (!el) return;
    const onScroll = () => {
      const i = Math.round(el.scrollLeft / Math.max(el.clientWidth, 1));
      setIndex(Math.min(Math.max(i, 0), Math.max(photos.length - 1, 0)));
    };
    el.addEventListener("scroll", onScroll, { passive: true });
    return () => el.removeEventListener("scroll", onScroll);
  }, [photos.length]);

  useEffect(() => {
    if (full === null) return;
    const onKey = (e: KeyboardEvent) => e.key === "Escape" && setFull(null);
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [full]);

  if (isPending) {
    return <HeroSkeleton className="min-h-[16rem]" />;
  }

  if (isError || !data) {
    return (
      <section className="surface-hero relative overflow-hidden p-4 sm:p-5">
        <p className="text-sm font-medium">Unable to load property</p>
        <p className="mt-1 text-xs text-muted-foreground">
          Check your connection and try again.
        </p>
        <button
          type="button"
          onClick={() => void refetch()}
          disabled={isFetching}
          className="press mt-3 inline-flex min-h-11 items-center rounded-full bg-primary px-4 text-xs font-semibold text-primary-foreground"
        >
          Retry
        </button>
      </section>
    );
  }

  return (
    <section className="surface-hero overflow-hidden">
      <div className="relative">
        <div
          ref={railRef}
          className="no-scrollbar flex snap-x snap-mandatory overflow-x-auto"
          aria-label="Property photos"
        >
          {photos.map((photo, i) => (
            <button
              key={photo.id}
              type="button"
              onClick={() => setFull(i)}
              aria-label={`Open ${photo.caption} full screen`}
              className="relative aspect-[16/10] w-full shrink-0 snap-center overflow-hidden bg-muted sm:aspect-[21/9]"
            >
              <img
                src={photo.url}
                alt={photo.caption}
                loading={i === 0 ? "eager" : "lazy"}
                className="size-full object-cover"
              />
            </button>
          ))}
        </div>

        <div className="pointer-events-none absolute inset-x-0 bottom-0 h-24 bg-gradient-to-t from-card via-card/50 to-transparent" />

        <span className="absolute left-3 top-3 rounded-full bg-background/70 px-2.5 py-1 text-[10px] font-semibold uppercase tracking-[0.12em] text-foreground backdrop-blur">
          {data.typeLabel}
        </span>

        <span className="absolute right-3 top-3 inline-flex items-center gap-1 rounded-full bg-background/70 px-2.5 py-1 text-[11px] font-semibold tabular-nums text-foreground backdrop-blur">
          <Expand className="size-3.5" />
          {photos.length > 0 ? `${index + 1}/${photos.length}` : "0/0"}
        </span>

        <div className="absolute inset-x-0 bottom-2 flex justify-center gap-1.5">
          {photos.map((photo, i) => (
            <span
              key={photo.id}
              className={`h-1.5 rounded-full transition-all duration-300 ${
                i === index ? "w-5 bg-primary" : "w-1.5 bg-foreground/25"
              }`}
            />
          ))}
        </div>
      </div>

      <div className="relative z-10 px-4 pb-4 pt-3 sm:px-5">
        <h1 className="font-display text-lg font-semibold">
          {data.name}
          {data.doorNumber ? (
            <span className="ml-2 align-middle text-[11px] font-medium text-muted-foreground">
              {data.doorNumber}
            </span>
          ) : null}
        </h1>
        <p className="mt-1 flex items-start gap-1.5 text-[12px] leading-snug text-muted-foreground">
          <MapPin className="mt-0.5 size-3.5 shrink-0" />
          <span className="min-w-0">{data.address}</span>
        </p>

        <p className="mt-3 text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
          Current market value
        </p>
        <p className="mt-1 font-display text-[1.75rem] font-semibold leading-none tabular-nums sm:text-4xl">
          {fmtINR(data.currentValue)}
        </p>
        <p className="mt-2 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs">
          <span className="inline-flex items-center gap-0.5 rounded-full bg-success/12 px-2 py-0.5 font-semibold tabular-nums text-success">
            <ArrowUpRight className="size-3.5" />
            {fmtINRShort(data.appreciationAbsolute)} ({data.appreciationPct.toFixed(0)}%)
          </span>
          <span className="text-muted-foreground">
            {data.purchaseYear
              ? `since ${data.purchaseYear} · ${data.cagrPct}% CAGR · owned by ${data.owner}`
              : `owned by ${data.owner}`}
          </span>
        </p>
      </div>

      {full !== null && (
        <div
          className="fixed inset-0 z-50 flex flex-col bg-background/95 backdrop-blur"
          role="dialog"
          aria-modal="true"
        >
          <div className="flex items-center justify-between px-4 pt-[calc(0.75rem+env(safe-area-inset-top))]">
            <span className="text-[12px] font-medium text-muted-foreground tabular-nums">
              {full + 1} / {photos.length}
            </span>
            <button
              type="button"
              onClick={() => setFull(null)}
              aria-label="Close photo"
              className="press grid size-11 place-items-center rounded-full bg-card ring-1 ring-border"
            >
              <X className="size-5" />
            </button>
          </div>
          <div className="no-scrollbar flex flex-1 snap-x snap-mandatory items-center overflow-x-auto">
            {photos.map((photo) => (
              <div key={photo.id} className="w-full shrink-0 snap-center px-4">
                <img src={photo.url} alt={photo.caption} className="max-h-[70vh] w-full rounded-2xl object-contain" />
                <p className="mt-3 text-center text-[12px] text-muted-foreground">{photo.caption}</p>
              </div>
            ))}
          </div>
          <div className="h-[calc(1rem+env(safe-area-inset-bottom))]" />
        </div>
      )}
    </section>
  );
}
