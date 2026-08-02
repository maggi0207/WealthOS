import { useQuery } from "@tanstack/react-query";

import { documentService } from "@/services/documents/document-service";

export const documentKeys = {
  all: ["documents"] as const,
  overview: () => [...documentKeys.all, "overview"] as const,
};

export function useDocumentsOverview() {
  return useQuery({
    queryKey: documentKeys.overview(),
    queryFn: () => documentService.getOverview(),
  });
}

/** @deprecated Prefer useDocumentsOverview */
export function useDocuments() {
  return useDocumentsOverview();
}
