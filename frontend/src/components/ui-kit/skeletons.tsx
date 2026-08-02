import { Skeleton } from "@/components/ui/skeleton";
import { cn } from "@/lib/utils";

/** Shared loading skeletons so every screen loads with the same rhythm. */

export function TileSkeleton({ className }: { className?: string }) {
  return <Skeleton className={cn("h-24 w-full rounded-2xl", className)} />;
}

export function StatGridSkeleton({ count = 4 }: { count?: number }) {
  return (
    <div className="grid grid-cols-2 gap-3">
      {Array.from({ length: count }).map((_, i) => (
        <TileSkeleton key={i} className="h-20" />
      ))}
    </div>
  );
}

export function ListSkeleton({ rows = 4 }: { rows?: number }) {
  return (
    <div className="surface-tile divide-y divide-border/50 overflow-hidden">
      {Array.from({ length: rows }).map((_, i) => (
        <div key={i} className="grid grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-3 px-4 py-3">
          <Skeleton className="size-9 rounded-xl" />
          <div className="space-y-1.5">
            <Skeleton className="h-3 w-2/3" />
            <Skeleton className="h-2.5 w-1/3" />
          </div>
          <Skeleton className="h-3 w-14" />
        </div>
      ))}
    </div>
  );
}

export function ChartSkeleton({ height = 190 }: { height?: number }) {
  return <Skeleton className="w-full rounded-2xl" style={{ height }} />;
}

/** Hero card placeholder — headline number, delta chip and sparkline. */
export function HeroSkeleton({ className }: { className?: string }) {
  return (
    <div className={cn("surface-hero space-y-3 p-4 sm:p-5", className)}>
      <Skeleton className="h-2.5 w-24" />
      <Skeleton className="h-8 w-2/3" />
      <Skeleton className="h-4 w-40 rounded-full" />
      <Skeleton className="h-16 w-full rounded-2xl" />
    </div>
  );
}

/** Property gallery placeholder — chip row plus a 2-up photo grid. */
export function GallerySkeleton({ tiles = 4 }: { tiles?: number }) {
  return (
    <div className="space-y-2.5">
      <div className="flex gap-2">
        {Array.from({ length: 4 }).map((_, i) => (
          <Skeleton key={i} className="h-9 w-20 shrink-0 rounded-full" />
        ))}
      </div>
      <div className="grid grid-cols-2 gap-2">
        {Array.from({ length: tiles }).map((_, i) => (
          <div key={i} className="surface-tile overflow-hidden">
            <Skeleton className="aspect-[4/3] w-full rounded-none" />
            <div className="px-2.5 py-2">
              <Skeleton className="h-2.5 w-3/4" />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

/** AI chat placeholder — alternating user/assistant message blocks. */
export function ChatSkeleton({ turns = 3 }: { turns?: number }) {
  return (
    <div className="space-y-5">
      {Array.from({ length: turns }).map((_, i) => (
        <div key={i} className="space-y-5">
          <div className="flex justify-end">
            <Skeleton className="h-10 w-[60%] rounded-2xl rounded-br-md" />
          </div>
          <div className="flex gap-2.5">
            <Skeleton className="size-8 shrink-0 rounded-xl" />
            <div className="min-w-0 flex-1 space-y-2">
              <Skeleton className="h-3 w-full" />
              <Skeleton className="h-3 w-[85%]" />
              <Skeleton className="h-3 w-[60%]" />
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}
