import { Compass, ExternalLink, Map, Navigation } from "lucide-react";
import { toast } from "sonner";

import { propertyDetail } from "@/lib/property-data";

const mapsQuery = encodeURIComponent(propertyDetail.address);

/** Location block — address, maps actions, distance and street view placeholders. */
export function LocationCard() {
  return (
    <section className="surface-tile overflow-hidden">
      <div className="grid grid-cols-[auto_minmax(0,1fr)] items-start gap-3 p-4">
        <span className="grid size-10 shrink-0 place-items-center rounded-xl bg-primary/12 text-primary">
          <Map className="size-5" />
        </span>
        <div className="min-w-0">
          <p className="text-[15px] font-semibold">Anna Avenue</p>
          <p className="text-[12px] text-muted-foreground">Adyar · Chennai 600020</p>
          <p className="mt-1.5 text-[11px] text-muted-foreground">Ward 175 · Zone 13 · 40 ft main road</p>
        </div>
      </div>

      <div className="grid grid-cols-2 gap-2 px-4">
        <a
          href={`https://www.google.com/maps/search/?api=1&query=${mapsQuery}`}
          target="_blank"
          rel="noreferrer"
          className="press inline-flex h-11 items-center justify-center gap-1.5 rounded-xl bg-primary text-[13px] font-semibold text-primary-foreground"
        >
          <ExternalLink className="size-4" />
          Google Maps
        </a>
        <a
          href={`https://www.google.com/maps/dir/?api=1&destination=${mapsQuery}`}
          target="_blank"
          rel="noreferrer"
          className="press inline-flex h-11 items-center justify-center gap-1.5 rounded-xl bg-muted text-[13px] font-semibold"
        >
          <Navigation className="size-4" />
          Directions
        </a>
      </div>

      <div className="mt-4 grid grid-cols-2 divide-x divide-border/70 border-t border-border/70">
        <div className="px-4 py-3">
          <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
            Distance from home
          </p>
          <p className="mt-1 font-display text-base font-semibold tabular-nums">—</p>
          <p className="text-[11px] text-muted-foreground">Enable location to calculate</p>
        </div>
        <button
          type="button"
          onClick={() => toast.info("Street View — coming with maps integration")}
          className="press px-4 py-3 text-left"
        >
          <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
            Street View
          </p>
          <p className="mt-1 inline-flex items-center gap-1 font-display text-base font-semibold">
            <Compass className="size-4 text-primary" />
            Preview
          </p>
          <p className="text-[11px] text-muted-foreground">Placeholder in this demo</p>
        </button>
      </div>
    </section>
  );
}
