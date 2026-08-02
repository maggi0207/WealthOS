import { QueryClient } from "@tanstack/react-query";
import { createRouter } from "@tanstack/react-router";
import { routeTree } from "./routeTree.gen";
import { DefaultErrorComponent } from "./components/ui-kit/default-error-component";
import { NotFoundPage } from "./components/ui-kit/not-found-page";
import { createQueryClientOptions } from "@/services/http/query-defaults";

export const getRouter = () => {
  const queryClient = new QueryClient(createQueryClientOptions());

  const router = createRouter({
    routeTree,
    context: { queryClient },
    scrollRestoration: true,
    defaultPreloadStaleTime: 0,
    defaultErrorComponent: DefaultErrorComponent,
    defaultNotFoundComponent: NotFoundPage,
  });

  return router;
};

