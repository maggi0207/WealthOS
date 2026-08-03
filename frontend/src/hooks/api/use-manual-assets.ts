import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { manualAssetService } from "@/services/assets/manual-asset-service";
import type {
  CreateManualAssetRequestDto,
  UpdateManualAssetRequestDto,
} from "@/services/assets/types";

export const manualAssetKeys = {
  all: ["manual-assets"] as const,
  lists: () => [...manualAssetKeys.all, "list"] as const,
  list: (params?: { page?: number; pageSize?: number; search?: string; type?: number }) =>
    [...manualAssetKeys.lists(), params ?? {}] as const,
  details: () => [...manualAssetKeys.all, "detail"] as const,
  detail: (id: string) => [...manualAssetKeys.details(), id] as const,
};

function invalidateAssets(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: manualAssetKeys.all });
  void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
}

export function useManualAssets(params?: {
  page?: number;
  pageSize?: number;
  search?: string;
  type?: number;
}) {
  return useQuery({
    queryKey: manualAssetKeys.list(params),
    queryFn: () =>
      manualAssetService.list({
        page: params?.page ?? 1,
        pageSize: params?.pageSize ?? 100,
        search: params?.search,
        type: params?.type,
      }),
  });
}

export function useManualAsset(id: string) {
  return useQuery({
    queryKey: manualAssetKeys.detail(id),
    queryFn: () => manualAssetService.getById(id),
    enabled: Boolean(id),
  });
}

export function useCreateManualAsset() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateManualAssetRequestDto) => manualAssetService.create(body),
    onSuccess: () => invalidateAssets(queryClient),
  });
}

export function useUpdateManualAsset() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: UpdateManualAssetRequestDto }) =>
      manualAssetService.update(id, body),
    onSuccess: () => invalidateAssets(queryClient),
  });
}

export function useDeleteManualAsset() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => manualAssetService.remove(id),
    onSuccess: () => invalidateAssets(queryClient),
  });
}
