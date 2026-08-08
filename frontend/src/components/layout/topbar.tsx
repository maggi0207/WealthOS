import { Link, useNavigate, useRouterState } from "@tanstack/react-router";
import { Bell, LogOut, Moon, Search, Sun, User } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { SidebarTrigger } from "@/components/ui/sidebar";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useNotifications } from "@/hooks/api/use-notifications";
import { useTheme } from "@/lib/theme";
import { useAuth } from "@/lib/mock-auth";
import { findNavItem } from "@/lib/navigation";

export function Topbar() {
  const { resolved, toggle } = useTheme();
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const pathname = useRouterState({ select: (r) => r.location.pathname });
  const current = findNavItem(pathname);
  const { data: notifications } = useNotifications();
  const items = notifications?.items ?? [];
  const unread = notifications?.unreadCount ?? 0;

  return (
    <header className="sticky top-0 z-30 grid grid-cols-[minmax(0,1fr)_auto] items-center gap-3 border-b border-border/70 bg-background/80 py-2 pt-safe page-gutter backdrop-blur-xl sm:flex sm:justify-between sm:py-3">
      <div className="flex min-w-0 items-center gap-2 sm:gap-3">
        <SidebarTrigger className="hidden shrink-0 md:flex" />

        <div className="min-w-0">
          <p className="truncate font-display text-[15px] font-semibold leading-tight sm:text-base">
            {current?.title ?? "WealthOS"}
          </p>
          <p className="hidden truncate text-xs text-muted-foreground sm:block">
            {current?.description ?? "Personal wealth operating system"}
          </p>
        </div>
      </div>

      <div className="flex shrink-0 items-center gap-0.5 sm:gap-1.5">
        <div className="relative hidden lg:block">
          <Search className="pointer-events-none absolute left-2.5 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input placeholder="Search anything…" className="h-9 w-56 pl-8" />
        </div>

        <Button variant="ghost" size="icon" aria-label="Toggle theme" className="size-11 md:size-9" onClick={toggle}>
          {resolved === "dark" ? <Sun className="size-4" /> : <Moon className="size-4" />}
        </Button>

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="icon" aria-label="Notifications" className="relative size-11 md:size-9">
              <Bell className="size-4" />
              {unread > 0 ? (
                <span className="absolute right-1.5 top-1.5 size-1.5 rounded-full bg-primary" />
              ) : null}
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-72">
            <DropdownMenuLabel className="flex items-center justify-between">
              Notifications
              <Badge variant="secondary">{unread || items.length}</Badge>
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            {items.length === 0 ? (
              <DropdownMenuItem disabled className="text-muted-foreground">
                No notifications
              </DropdownMenuItem>
            ) : (
              items.map((n) => (
                <DropdownMenuItem key={n.id} className="flex-col items-start gap-0.5">
                  <span className="text-sm">{n.title}</span>
                  <span className="text-xs text-muted-foreground">{n.meta}</span>
                </DropdownMenuItem>
              ))
            )}
          </DropdownMenuContent>
        </DropdownMenu>

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="h-11 gap-2 px-2 md:h-9">
              <span className="grid size-6 place-items-center rounded-md bg-primary text-[11px] font-semibold text-primary-foreground">
                {user?.initials ?? "WO"}
              </span>
              <span className="hidden max-w-28 truncate text-sm sm:inline">{user?.name}</span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-56">
            <DropdownMenuLabel className="flex flex-col gap-0.5">
              <span className="truncate">{user?.name}</span>
              <span className="truncate text-xs font-normal text-muted-foreground">{user?.email}</span>
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem asChild>
              <Link to="/settings">
                <User className="size-4" /> Settings
              </Link>
            </DropdownMenuItem>
            <DropdownMenuItem
              onSelect={() => {
                void logout().then(() => navigate({ to: "/login" }));
              }}
            >
              <LogOut className="size-4" /> Sign out
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  );
}
