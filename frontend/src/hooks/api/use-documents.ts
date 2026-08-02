import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { documentService } from "@/services/documents/document-service";
import type {
  CreateDocumentRequestDto,
  UpdateDocumentRequestDto,
} from "@/services/documents/types";

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

function invalidateDocuments(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: documentKeys.all });
  void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
}

export function useCreateDocument() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateDocumentRequestDto) => documentService.create(body),
    onSuccess: () => invalidateDocuments(queryClient),
  });
}

export function useUpdateDocument() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: UpdateDocumentRequestDto }) =>
      documentService.update(id, body),
    onSuccess: () => invalidateDocuments(queryClient),
  });
}

export function useDeleteDocument() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => documentService.remove(id),
    onSuccess: () => invalidateDocuments(queryClient),
  });
}
