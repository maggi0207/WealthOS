import { Gem } from "lucide-react";
import type { ReactNode } from "react";

import { workspace } from "@/lib/mock-data";

export function AuthShell({
  title,
  subtitle,
  children,
}: {
  title: string;
  subtitle: string;
  children: ReactNode;
}) {
  return (
    <div className="bg-aurora flex min-h-screen items-center justify-center bg-background px-4 py-10">
      <div className="w-full max-w-md">
        <div className="mb-6 flex items-center justify-center gap-2.5">
          <span className="grid size-10 place-items-center rounded-xl bg-primary text-primary-foreground">
            <Gem className="size-5" />
          </span>
          <span className="font-display text-2xl font-semibold tracking-tight">{workspace.name}</span>
        </div>
        <div className="surface-panel p-6 sm:p-7">
          <h1 className="font-display text-xl font-semibold">{title}</h1>
          <p className="mt-1.5 text-sm text-muted-foreground">{subtitle}</p>
          <div className="mt-6">{children}</div>
        </div>
        <p className="mt-5 text-center text-xs text-muted-foreground">
          Frontend foundation build · mock data only
        </p>
      </div>
    </div>
  );
}
