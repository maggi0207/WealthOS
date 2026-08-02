import { useEffect, useRef, useState } from "react";
import { X } from "lucide-react";

import { galleryCategories, photos, type GalleryCategory } from "@/lib/property-data";

/** Categorised gallery of the real property photographs, with swipeable fullscreen preview. */
export function PropertyGallery() {
  const [active, setActive] = useState<GalleryCategory | "All">("All");
  const [openAt, setOpenAt] = useState<number | null>(null);
  const [index, setIndex] = useState(0);
  const railRef = useRef<HTMLDivElement>(null);

  const shown = active === "All" ? photos : photos.filter((p) => p.category === active);

  useEffect(() => {
    if (openAt === null) return;
    setIndex(openAt);
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") setOpenAt(null);
    };
    window.addEventListener("keydown", onKey);
    document.body.style.overflow = "hidden";
    // Jump the swipe rail to the tapped photo.
    requestAnimationFrame(() => {
      const el = railRef.current;
      if (el) el.scrollTo({ left: openAt * el.clientWidth, behavior: "instant" as ScrollBehavior });
    });
    return () => {
      window.removeEventListener("keydown", onKey);
      document.body.style.overflow = "";
    };
  }, [openAt]);

  return (
    <div className="space-y-2.5">
      <div className="bleed-gutter no-scrollbar flex gap-2 overflow-x-auto px-[var(--page-gutter)]">
        {(["All", ...galleryCategories] as const).map((cat) => (
          <button
            key={cat}
            type="button"
            onClick={() => setActive(cat)}
            aria-pressed={active === cat}
            className={`press h-11 shrink-0 whitespace-nowrap rounded-full px-4 text-[12px] font-semibold transition-colors ${
              active === cat ? "bg-primary text-primary-foreground" : "bg-muted text-muted-foreground"
            }`}
          >
            {cat}
          </button>
        ))}
      </div>

      <div className="grid grid-cols-2 gap-2">
        {shown.map((photo, i) => (
          <figure key={photo.id} className="surface-tile overflow-hidden">
            <button
              type="button"
              onClick={() => setOpenAt(i)}
              aria-label={`Open ${photo.caption} full screen`}
              className="press block aspect-[4/3] w-full overflow-hidden bg-muted"
            >
              <img
                src={photo.url}
                alt={photo.caption}
                loading="lazy"
                className="size-full object-cover transition-transform duration-500 hover:scale-105"
              />
            </button>
            <figcaption className="px-2.5 py-2 text-[11px] leading-snug text-muted-foreground">
              {photo.caption}
            </figcaption>
          </figure>
        ))}
      </div>

      {openAt !== null && (
        <div className="route-enter fixed inset-0 z-50 flex flex-col bg-background/95 backdrop-blur" role="dialog" aria-modal="true">
          <div className="flex items-center justify-between px-4 pt-[calc(0.75rem+env(safe-area-inset-top))]">
            <span className="text-[12px] font-medium tabular-nums text-muted-foreground">
              {index + 1} / {shown.length}
            </span>
            <button
              type="button"
              onClick={() => setOpenAt(null)}
              aria-label="Close photo"
              className="press grid size-11 place-items-center rounded-full bg-card ring-1 ring-border"
            >
              <X className="size-5" />
            </button>
          </div>
          <div
            ref={railRef}
            onScroll={(e) => {
              const el = e.currentTarget;
              setIndex(Math.round(el.scrollLeft / Math.max(el.clientWidth, 1)));
            }}
            className="no-scrollbar flex flex-1 snap-x snap-mandatory items-center overflow-x-auto"
          >
            {shown.map((photo) => (
              <div key={photo.id} className="w-full shrink-0 snap-center px-4">
                <img src={photo.url} alt={photo.caption} className="max-h-[70vh] w-full rounded-2xl object-contain" />
                <p className="mt-3 text-center text-[12px] text-muted-foreground">{photo.caption}</p>
              </div>
            ))}
          </div>
          <div className="flex justify-center gap-1.5 pb-[calc(1rem+env(safe-area-inset-bottom))]">
            {shown.map((photo, i) => (
              <span
                key={photo.id}
                className={`h-1.5 rounded-full transition-all duration-300 ${
                  i === index ? "w-5 bg-primary" : "w-1.5 bg-foreground/25"
                }`}
              />
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
