import { useState } from "react";
import { Link, useRouterState } from "@tanstack/react-router";
import { Bot, Building2, Coins, LayoutDashboard, MoreHorizontal } from "lucide-react";

import { cn } from "@/lib/utils";
import { navGroups } from "@/lib/navigation";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";

/** The 5 native-feeling primary tabs. Everything else lives in "More". */
const primary = [
  { title: "Dashboard", url: "/dashboard", icon: LayoutDashboard },
  { title: "Assets", url: "/assets", icon: Coins },
  { title: "Properties", url: "/properties", icon: Building2 },
  { title: "AI", url: "/ai-advisor", icon: Bot },
] as const;

const primaryUrls = new Set<string>(primary.map((item) => item.url));

/**
 * Mobile-first primary navigation.
 * Thumb-reachable bottom bar (44px+ targets, iOS safe-area aware) plus a
 * drawer holding the remaining modules. Hidden from md upwards.
 */
export function BottomNav() {
  const pathname = useRouterState({ select: (r) => r.location.pathname });
  const [open, setOpen] = useState(false);
  const isActive = (url: string) => pathname === url || pathname.startsWith(`${url}/`);
  const moreActive = !primary.some((item) => isActive(item.url));

  const moreGroups = navGroups
    .map((group) => ({ ...group, items: group.items.filter((item) => !primaryUrls.has(item.url)) }))
    .filter((group) => group.items.length > 0);
  const moreCount = moreGroups.reduce((sum, group) => sum + group.items.length, 0);

  return (
    <nav
      aria-label="Primary"
      className="fixed inset-x-0 bottom-0 z-40 border-t border-border/60 bg-background/92 pb-safe backdrop-blur-xl md:hidden"
    >
      <ul className="grid grid-cols-5">
        {primary.map((item) => {
          const active = isActive(item.url);
          return (
            <li key={item.url}>
              <Link
                to={item.url}
                aria-current={active ? "page" : undefined}
                className={cn(
                  "flex min-h-[3.25rem] flex-col items-center justify-center gap-[3px] px-1 pb-1.5 pt-2 text-[10px] font-medium tracking-tight transition-colors active:opacity-70",
                  active ? "text-primary" : "text-muted-foreground",
                )}
              >
                <item.icon
                  className={cn("size-[22px] transition-transform", active && "scale-105")}
                  strokeWidth={active ? 2.4 : 1.8}
                  aria-hidden
                />
                <span className="max-w-full truncate">{item.title}</span>
              </Link>
            </li>
          );
        })}
        <li>
          <Sheet open={open} onOpenChange={setOpen}>
            <SheetTrigger
              className={cn(
                "flex min-h-[3.25rem] w-full flex-col items-center justify-center gap-[3px] px-1 pb-1.5 pt-2 text-[10px] font-medium tracking-tight transition-colors active:opacity-70",
                moreActive ? "text-primary" : "text-muted-foreground",
              )}
            >
              <MoreHorizontal className="size-[22px]" strokeWidth={moreActive ? 2.4 : 1.8} aria-hidden />
              <span>More</span>
            </SheetTrigger>
            <SheetContent side="bottom" className="max-h-[85svh] overflow-y-auto rounded-t-3xl pb-safe">
              <SheetHeader className="px-4 text-left">
                <SheetTitle className="font-display text-base">Modules</SheetTitle>
                <SheetDescription className="text-xs">{moreCount} more WealthOS modules</SheetDescription>
              </SheetHeader>
              <div className="space-y-4 px-4 pb-6">
                {moreGroups.map((group) => (
                  <div key={group.label}>
                    <p className="mb-2 text-[10px] font-semibold uppercase tracking-[0.12em] text-muted-foreground">
                      {group.label}
                    </p>
                    <div className="grid grid-cols-2 gap-2">
                      {group.items.map((item) => (
                        <Link
                          key={item.url}
                          to={item.url}
                          onClick={() => setOpen(false)}
                          className={cn(
                            "flex min-h-12 items-center gap-2.5 rounded-2xl border border-border/60 px-3 py-2 text-sm font-medium active:opacity-70",
                            isActive(item.url)
                              ? "border-primary/40 bg-primary/10 text-primary"
                              : "bg-card text-foreground",
                          )}
                        >
                          <item.icon className="size-4 shrink-0" aria-hidden />
                          <span className="truncate">{item.title}</span>
                        </Link>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            </SheetContent>
          </Sheet>
        </li>
      </ul>
    </nav>
  );
}
