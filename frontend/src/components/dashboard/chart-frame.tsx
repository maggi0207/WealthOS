import { useEffect, useState, type ReactNode } from "react";

import { ChartSkeleton } from "@/components/ui-kit/skeletons";


/**
 * Recharts needs real DOM measurements, so charts mount after hydration.
 * Height is adaptive: `mobileHeight` applies below 768px so charts stay
 * readable on phones without forcing horizontal scroll.
 */
export function ChartFrame({
  height = 260,
  mobileHeight,
  children,
}: {
  height?: number;
  mobileHeight?: number;
  children: ReactNode;
}) {
  const [mounted, setMounted] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  useEffect(() => {
    setMounted(true);
    const query = window.matchMedia("(max-width: 767px)");
    const sync = () => setIsMobile(query.matches);
    sync();
    query.addEventListener("change", sync);
    return () => query.removeEventListener("change", sync);
  }, []);

  const shrunk = height > 200 ? Math.max(180, Math.round(height * 0.8)) : height;
  const resolved = isMobile ? (mobileHeight ?? shrunk) : height;


  if (!mounted) return <ChartSkeleton height={resolved} />;
  return (
    <div className="w-full min-w-0" style={{ height: resolved }}>
      {children}
    </div>
  );
}
