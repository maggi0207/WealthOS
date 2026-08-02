import { useEffect, useState } from "react";

const HOLD_MS = 900;
const FADE_MS = 420;

/**
 * Premium branded splash. Rendered with the very first paint (server included,
 * so there is no hydration mismatch) and faded out shortly after the app
 * hydrates. It lives in the root component, so client-side tab navigation
 * never replays it — only a full page load does.
 */
export function SplashScreen() {
  const [phase, setPhase] = useState<"visible" | "leaving" | "hidden">("visible");

  useEffect(() => {
    const leave = window.setTimeout(() => setPhase("leaving"), HOLD_MS);
    const done = window.setTimeout(() => setPhase("hidden"), HOLD_MS + FADE_MS);
    return () => {
      window.clearTimeout(leave);
      window.clearTimeout(done);
    };
  }, []);

  if (phase === "hidden") return null;

  return (
    <div
      aria-hidden="true"
      className="splash-root"
      data-state={phase}
      style={{ ["--splash-fade" as string]: `${FADE_MS}ms` }}
    >
      <div className="splash-inner">
        <span className="splash-mark">
          <img src="/icon-192.png" alt="" width={192} height={192} className="size-full rounded-[22%]" />
        </span>
        <p className="splash-word font-display text-fluid-xl font-semibold tracking-tight">WealthOS</p>
        <p className="splash-tag text-xs font-medium uppercase tracking-[0.22em] text-muted-foreground">
          Personal wealth OS
        </p>
        <span className="splash-bar" />
      </div>
    </div>
  );
}
