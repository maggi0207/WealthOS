import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { propertyService } from "@/services/properties/property-service";
import type {
  CreatePropertyRequestDto,
  PropertyListQuery,
  UpdatePropertyRequestDto,
} from "@/services/properties/types";

export const propertyKeys = {
  all: ["properties"] as const,
  lists: () => [...propertyKeys.all, "list"] as const,
  list: (params: PropertyListQuery = {}) =>
    [...propertyKeys.lists(), params] as const,
  details: () => [...propertyKeys.all, "detail"] as const,
  detail: (id: string) => [...propertyKeys.details(), id] as const,
  summary: () => [...propertyKeys.all, "summary"] as const,
  dashboards: () => [...propertyKeys.all, "dashboard"] as const,
  dashboard: (id: string) => [...propertyKeys.dashboards(), id] as const,
  primary: () => [...propertyKeys.all, "primary"] as const,
};

/** Paginated property list — GET /api/v1/properties */
export function useProperties(params: PropertyListQuery = {}) {
  return useQuery({
    queryKey: propertyKeys.list(params),
    queryFn: () => propertyService.list(params),
  });
}

/** Single property detail — GET /api/v1/properties/{id} */
export function useProperty(id: string) {
  return useQuery({
    queryKey: propertyKeys.detail(id),
    queryFn: () => propertyService.getById(id),
    enabled: Boolean(id),
  });
}

/** Portfolio summary — GET /api/v1/properties/summary */
export function usePropertySummary() {
  return useQuery({
    queryKey: propertyKeys.summary(),
    queryFn: () => propertyService.getSummary(),
  });
}

/**
 * Per-property dashboard — GET /api/v1/properties/{id}/dashboard
 * Preferred for the passport page (includes equity estimate).
 */
export function usePropertyDashboard(id: string) {
  return useQuery({
    queryKey: propertyKeys.dashboard(id),
    queryFn: () => propertyService.getDashboard(id),
    enabled: Boolean(id),
  });
}

/**
 * Primary property for `/properties` (no route id).
 * Mock mode → property-data fixtures; API mode → first list item dashboard.
 */
export function usePrimaryProperty() {
  return useQuery({
    queryKey: propertyKeys.primary(),
    queryFn: () => propertyService.getPrimary(),
  });
}

export function useCreateProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreatePropertyRequestDto) => propertyService.create(body),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: propertyKeys.all });
      void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
    },
  });
}

export function useUpdateProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      body,
    }: {
      id: string;
      body: UpdatePropertyRequestDto;
    }) => propertyService.update(id, body),
    onSuccess: (_data, variables) => {
      void queryClient.invalidateQueries({ queryKey: propertyKeys.all });
      void queryClient.invalidateQueries({
        queryKey: propertyKeys.detail(variables.id),
      });
      void queryClient.invalidateQueries({
        queryKey: propertyKeys.dashboard(variables.id),
      });
      void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
    },
  });
}

export function useDeleteProperty() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => propertyService.remove(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: propertyKeys.all });
      void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
    },
  });
}
