import {
  GraduationCap,
  Hospital,
  Plane,
  TrainFront,
  TramFront,
  Waves,
  type LucideIcon,
} from "lucide-react";

import { nearbyPlaces } from "@/lib/property-data";

const icons: Record<string, LucideIcon> = {
  School: GraduationCap,
  Hospital: Hospital,
  Metro: TramFront,
  Beach: Waves,
  Railway: TrainFront,
  Airport: Plane,
};

/** Nearby places with mock distances. */
export function NearbyPlaces() {
  return (
    <div className="grid grid-cols-2 gap-2">
      {nearbyPlaces.map((place) => {
        const Icon = icons[place.kind] ?? GraduationCap;
        return (
          <div key={place.id} className="surface-tile flex items-start gap-2.5 p-3">
            <span className="grid size-8 shrink-0 place-items-center rounded-lg bg-primary/10 text-primary">
              <Icon className="size-4" />
            </span>
            <div className="min-w-0">
              <p className="text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
                {place.kind}
              </p>
              <p className="truncate text-[13px] font-medium leading-snug">{place.name}</p>
              <p className="text-[12px] font-semibold tabular-nums text-primary">{place.distance}</p>
            </div>
          </div>
        );
      })}
    </div>
  );
}
