import { Compass, ExternalLink, Map, Navigation } from "lucide-react";
import { toast } from "sonner";

import { TileSkeleton } from "@/components/ui-kit/skeletons";
import { usePrimaryProperty } from "@/hooks/api/use-properties";

/** Location block — address, maps actions, distance and street view placeholders. */
export function LocationCard() {
  const { data, isPending, isError, refetch, isFetching } = usePrimaryProperty();

  if (isPending) {
    return <TileSkeleton className="h-48" />;
  }

  if (isError || !data) {
    return (
      <section className="surface-tile px-4 py-5 text-center">
        <p className="text-sm font-medium">Unable to load location</p>
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

  const mapsQuery = encodeURIComponent(data.address);
  const mapsHref =
    data.googleMapsUrl ||
    `https://www.google.com/maps/search/?api=1&query=${mapsQuery}`;
  const directionsHref = `https://www.google.com/maps/dir/?api=1&destination=${mapsQuery}`;
  const title = data.locality || data.city || data.name;
  const subtitle = [data.city, data.postalCode].filter(Boolean).join(" · ");

  return (
    <section className="surface-tile overflow-hidden">
      <div className="grid grid-cols-[auto_minmax(0,1fr)] items-start gap-3 p-4">
        <span className="grid size-10 shrink-0 place-items-center rounded-xl bg-primary/12 text-primary">
          <Map className="size-5" />
        </span>
        <div className="min-w-0">
          <p className="text-[15px] font-semibold">{title}</p>
          {subtitle ? (
            <p className="text-[12px] text-muted-foreground">{subtitle}</p>
          ) : null}
          {data.state ? (
            <p className="mt-1.5 text-[11px] text-muted-foreground">{data.state}</p>
          ) : null}
        </div>
      </div>

      <div className="grid grid-cols-2 gap-2 px-4">
        <a
          href={mapsHref}
          target="_blank"
          rel="noreferrer"
          className="press inline-flex h-11 items-center justify-center gap-1.5 rounded-xl bg-primary text-[13px] font-semibold text-primary-foreground"
        >
          <ExternalLink className="size-4" />
          Google Maps
        </a>
        <a
          href={directionsHref}
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
