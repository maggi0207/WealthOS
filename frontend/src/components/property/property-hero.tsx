import { useEffect, useRef, useState } from "react";
import { ArrowUpRight, Expand, MapPin, X } from "lucide-react";

import { appreciation, fmtINR, fmtINRShort, photos, propertyDetail } from "@/lib/property-data";

/** Hero: swipeable real-photo gallery + value headline. */
export function PropertyHero() {
  const railRef = useRef<HTMLDivElement>(null);
  const [index, setIndex] = useState(0);
  const [full, setFull] = useState<number | null>(null);

  useEffect(() => {
    const el = railRef.current;
    if (!el) return;
    const onScroll = () => {
      const i = Math.round(el.scrollLeft / Math.max(el.clientWidth, 1));
      setIndex(Math.min(Math.max(i, 0), photos.length - 1));
    };
    el.addEventListener("scroll", onScroll, { passive: true });
    return () => el.removeEventListener("scroll", onScroll);
  }, []);

  useEffect(() => {
    if (full === null) return;
    const onKey = (e: KeyboardEvent) => e.key === "Escape" && setFull(null);
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [full]);

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
          {propertyDetail.type}
        </span>

        <span className="absolute right-3 top-3 inline-flex items-center gap-1 rounded-full bg-background/70 px-2.5 py-1 text-[11px] font-semibold tabular-nums text-foreground backdrop-blur">
          <Expand className="size-3.5" />
          {index + 1}/{photos.length}
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
          {propertyDetail.name}
          <span className="ml-2 align-middle text-[11px] font-medium text-muted-foreground">
            {propertyDetail.doorNumber}
          </span>
        </h1>
        <p className="mt-1 flex items-start gap-1.5 text-[12px] leading-snug text-muted-foreground">
          <MapPin className="mt-0.5 size-3.5 shrink-0" />
          <span className="min-w-0">{propertyDetail.address}</span>
        </p>

        <p className="mt-3 text-[11px] font-semibold uppercase tracking-[0.14em] text-muted-foreground">
          Current market value
        </p>
        <p className="mt-1 font-display text-[1.75rem] font-semibold leading-none tabular-nums sm:text-4xl">
          {fmtINR(propertyDetail.currentValue)}
        </p>
        <p className="mt-2 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs">
          <span className="inline-flex items-center gap-0.5 rounded-full bg-success/12 px-2 py-0.5 font-semibold tabular-nums text-success">
            <ArrowUpRight className="size-3.5" />
            {fmtINRShort(appreciation.absolute)} ({appreciation.pct.toFixed(0)}%)
          </span>
          <span className="text-muted-foreground">
            since {propertyDetail.purchaseYear} · {appreciation.cagrPct}% CAGR · owned by{" "}
            {propertyDetail.owner}
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
