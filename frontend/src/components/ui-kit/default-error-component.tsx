import { useRouter } from "@tanstack/react-router";
import { useEffect } from "react";

import { ErrorPage } from "@/components/ui-kit/error-page";
import { reportLovableError } from "@/lib/lovable-error-reporting";

/** Router-wide default error boundary with a working retry. */
export function DefaultErrorComponent({ error, reset }: { error: Error; reset: () => void }) {
  const router = useRouter();

  useEffect(() => {
    console.error(error);
    reportLovableError(error, { boundary: "tanstack_default_error_component" });
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
