import { Navigate, Outlet, createFileRoute } from "@tanstack/react-router";

import { AppSidebar } from "@/components/layout/app-sidebar";
import { BottomNav } from "@/components/layout/bottom-nav";
import { PageTransition } from "@/components/layout/page-transition";
import { Topbar } from "@/components/layout/topbar";

import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar";
import { Skeleton } from "@/components/ui/skeleton";
import { HeroSkeleton, ListSkeleton, StatGridSkeleton } from "@/components/ui-kit/skeletons";
import { useAuth } from "@/lib/mock-auth";


export const Route = createFileRoute("/_shell")({
  component: ShellLayout,
});

function ShellLayout() {
  const { user, isReady } = useAuth();

  if (!isReady) {
    return (
      <div className="flex min-h-svh w-full gap-4 bg-background p-4">
        <Skeleton className="hidden h-[calc(100svh-2rem)] w-64 rounded-2xl md:block" />
        <div className="min-w-0 flex-1 space-y-4">
          <Skeleton className="h-12 w-full rounded-2xl" />
          <HeroSkeleton />
          <StatGridSkeleton count={4} />
          <ListSkeleton rows={3} />
        </div>
      </div>
    );
  }


  if (!user) return <Navigate to="/login" />;

  return (
    <SidebarProvider className="h-svh min-h-0 overflow-hidden">
      <div className="flex h-svh w-full overflow-hidden bg-background">
        <AppSidebar />
        <SidebarInset className="min-h-0 min-w-0 overflow-hidden bg-transparent">
          <Topbar />
          <div className="min-h-0 min-w-0 flex-1 overflow-y-auto overscroll-y-contain page-gutter py-4 sm:py-6">
            <div className="mx-auto w-full min-w-0 max-w-7xl pb-[calc(4.75rem+env(safe-area-inset-bottom))] md:pb-0">
              <PageTransition>
                <Outlet />
              </PageTransition>
            </div>
          </div>

          <BottomNav />
        </SidebarInset>
      </div>
    </SidebarProvider>
  );
}

