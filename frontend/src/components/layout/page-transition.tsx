import { useRouterState } from "@tanstack/react-router";
import type { ReactNode } from "react";

/**
 * Premium tab transition: fade + subtle upward slide (220ms) on every
 * pathname change. Keyed remount of the wrapper only — routes keep their own
 * state/scroll restoration handled by the router.
 */
export function PageTransition({ children }: { children: ReactNode }) {
  const pathname = useRouterState({ select: (r) => r.location.pathname });

  return (
    <div key={pathname} className="route-enter min-w-0">
      {children}
    </div>
  );
}
