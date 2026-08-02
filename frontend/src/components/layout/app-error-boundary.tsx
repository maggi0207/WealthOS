import { Component, type ErrorInfo, type ReactNode } from "react";

import { ErrorPage } from "@/components/ui-kit/error-page";
import { reportLovableError } from "@/lib/lovable-error-reporting";

type Props = { children: ReactNode };
type State = { error: Error | null };

/**
 * Application-level error boundary. Catches render/runtime errors anywhere in
 * the tree (router boundaries only cover route work) and offers a retry.
 */
export class AppErrorBoundary extends Component<Props, State> {
  override state: State = { error: null };

  static getDerivedStateFromError(error: Error): State {
    return { error };
  }

  override componentDidCatch(error: Error, info: ErrorInfo) {
    console.error(error);
    reportLovableError(error, { boundary: "app_error_boundary", componentStack: info.componentStack });
  }

  private reset = () => this.setState({ error: null });

  override render() {
    if (this.state.error) {
      return <ErrorPage error={this.state.error} onRetry={this.reset} />;
    }
    return this.props.children;
  }
}
