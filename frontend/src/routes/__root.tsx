import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import {
  Outlet,
  createRootRouteWithContext,
  useRouter,
  HeadContent,
  Scripts,
} from "@tanstack/react-router";
import { useEffect, type ReactNode } from "react";

import appCss from "../styles.css?url";
import { reportLovableError } from "../lib/lovable-error-reporting";
import { AppErrorBoundary } from "@/components/layout/app-error-boundary";
import { SplashScreen } from "@/components/layout/splash-screen";
import { ErrorPage } from "@/components/ui-kit/error-page";
import { NotFoundPage } from "@/components/ui-kit/not-found-page";
import { AuthProvider } from "@/lib/mock-auth";
import { ThemeProvider } from "@/lib/theme";
import { Toaster } from "@/components/ui/sonner";

function NotFoundComponent() {
  return <NotFoundPage />;
}

function ErrorComponent({ error, reset }: { error: Error; reset: () => void }) {
  console.error(error);
  const router = useRouter();
  useEffect(() => {
    reportLovableError(error, { boundary: "tanstack_root_error_component" });
  }, [error]);

  return (
    <ErrorPage
      error={error}
      onRetry={() => {
        router.invalidate();
        reset();
      }}
    />
  );
}


export const Route = createRootRouteWithContext<{ queryClient: QueryClient }>()({
  head: () => ({
    meta: [
      { charSet: "utf-8" },
      { name: "viewport", content: "width=device-width, initial-scale=1" },
      { title: "WealthOS — Personal Wealth Operating System" },
      {
        name: "description",
        content: "WealthOS tracks assets, properties, loans, investments and cashflow in one workspace.",
      },
      { property: "og:title", content: "WealthOS — Personal Wealth Operating System" },
      {
        property: "og:description",
        content: "WealthOS tracks assets, properties, loans, investments and cashflow in one workspace.",
      },
      { property: "og:type", content: "website" },
      { name: "twitter:card", content: "summary_large_image" },
      { name: "theme-color", content: "#0b1418" },
      { name: "apple-mobile-web-app-capable", content: "yes" },
      { name: "apple-mobile-web-app-status-bar-style", content: "black-translucent" },
      { name: "apple-mobile-web-app-title", content: "WealthOS" },
      { name: "application-name", content: "WealthOS" },
      { name: "mobile-web-app-capable", content: "yes" },
      { name: "twitter:title", content: "WealthOS — Personal Wealth Operating System" },
      { name: "twitter:description", content: "WealthOS tracks assets, properties, loans, investments and cashflow in one workspace." },
      { property: "og:image", content: "https://pub-bb2e103a32db4e198524a2e9ed8f35b4.r2.dev/ec56f267-13a4-49e8-a3b4-015323a6f636/id-preview-5bd8c93d--64f73bd7-f827-4184-9a06-8969b525f694.lovable.app-1785652609481.png" },
      { name: "twitter:image", content: "https://pub-bb2e103a32db4e198524a2e9ed8f35b4.r2.dev/ec56f267-13a4-49e8-a3b4-015323a6f636/id-preview-5bd8c93d--64f73bd7-f827-4184-9a06-8969b525f694.lovable.app-1785652609481.png" },
    ],
    links: [
      { rel: "stylesheet", href: appCss },
      { rel: "preconnect", href: "https://fonts.googleapis.com" },
      { rel: "preconnect", href: "https://fonts.gstatic.com", crossOrigin: "anonymous" },
      {
        rel: "stylesheet",
        href: "https://fonts.googleapis.com/css2?family=Sora:wght@500;600;700&family=Inter+Tight:wght@400;500;600&display=swap",
      },
      { rel: "icon", type: "image/png", href: "/favicon.png" },
      { rel: "apple-touch-icon", sizes: "180x180", href: "/apple-touch-icon.png" },
      { rel: "manifest", href: "/manifest.webmanifest" },
    ],

  }),
  shellComponent: RootShell,
  component: RootComponent,
  notFoundComponent: NotFoundComponent,
  errorComponent: ErrorComponent,
});

function RootShell({ children }: { children: ReactNode }) {
  return (
    <html lang="en" className="dark">
      <head>
        <HeadContent />
      </head>
      <body>
        {children}
        <Scripts />
      </body>
    </html>
  );
}

function RootComponent() {
  const { queryClient } = Route.useRouteContext();

  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider>
        <AuthProvider>
          <AppErrorBoundary>
            {/* Required: nested routes render here. Removing <Outlet /> breaks all child routes. */}
            <Outlet />
          </AppErrorBoundary>
          <SplashScreen />
          <Toaster />
        </AuthProvider>
      </ThemeProvider>
    </QueryClientProvider>

  );
}
