/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string;
  readonly VITE_API_MODE: "mock" | "api";
  readonly VITE_API_VERSION: string;
  /** Set to "true" to allow mock mode in production builds (demo only). */
  readonly VITE_ALLOW_MOCK_PROD?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
