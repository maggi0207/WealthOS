import { isMockApiMode } from "@/config/env";
import { BaseApiService } from "@/services/http/base-api-service";
import { ApiError } from "@/services/http/problem-details";
import {
  mapMockPropertyDashboard,
  mapMockPropertyDetail,
  mapMockPropertyList,
  mapMockPropertySummary,
  mapPropertyDashboardResponse,
  mapPropertyListResponse,
  mapPropertyResponse,
  mapPropertySummaryResponse,
} from "@/services/properties/property-mapper";
import type {
  CreatePropertyRequestDto,
  PropertyDashboardDto,
  PropertyDetailView,
  PropertyDto,
  PropertyListDto,
  PropertyListQuery,
  PropertyListView,
  PropertySummaryDto,
  PropertySummaryView,
  UpdatePropertyRequestDto,
} from "@/services/properties/types";

function buildListQuery(params: PropertyListQuery = {}): string {
  const search = new URLSearchParams();
  if (params.page != null) search.set("page", String(params.page));
  if (params.pageSize != null) search.set("pageSize", String(params.pageSize));
  if (params.search) search.set("search", params.search);
  if (params.status != null) search.set("status", String(params.status));
  if (params.type != null) search.set("type", String(params.type));
  const qs = search.toString();
  return qs ? `?${qs}` : "";
}

/**
 * Properties API service — `/api/v{version}/properties*`.
 * Transparent mock fallback when `VITE_API_MODE=mock`.
 *
 * Detail page uses `getDashboard(id)` (`GET .../properties/{id}/dashboard`)
 * for equity estimate + related stub counts.
 */
class PropertyService extends BaseApiService {
  protected readonly serviceName = "PropertyService";

  async list(params: PropertyListQuery = {}): Promise<PropertyListView> {
    if (isMockApiMode()) {
      return mapMockPropertyList();
    }
    const dto = await this.get<PropertyListDto>(
      `/properties${buildListQuery(params)}`,
    );
    return mapPropertyListResponse(dto);
  }

  async getById(id: string): Promise<PropertyDetailView> {
    if (isMockApiMode()) {
      return mapMockPropertyDetail();
    }
    const dto = await this.get<PropertyDto>(`/properties/${id}`);
    return mapPropertyResponse(dto);
  }

  async getSummary(): Promise<PropertySummaryView> {
    if (isMockApiMode()) {
      return mapMockPropertySummary();
    }
    const dto = await this.get<PropertySummaryDto>("/properties/summary");
    return mapPropertySummaryResponse(dto);
  }

  /**
   * Per-property dashboard snapshot.
   * Endpoint: GET /api/v1/properties/{id}/dashboard
   */
  async getDashboard(id: string): Promise<PropertyDetailView> {
    if (isMockApiMode()) {
      return mapMockPropertyDashboard();
    }
    const dto = await this.get<PropertyDashboardDto>(
      `/properties/${id}/dashboard`,
    );
    return mapPropertyDashboardResponse(dto);
  }

  /**
   * Resolves the primary property for the passport page (no `:id` in route).
   * Uses list → dashboard so equity is available.
   */
  async getPrimary(): Promise<PropertyDetailView | null> {
    if (isMockApiMode()) {
      return mapMockPropertyDashboard();
    }

    const list = await this.list({ page: 1, pageSize: 1 });
    const first = list.items[0];
    if (!first) {
      return null;
    }
    return this.getDashboard(first.id);
  }

  async create(body: CreatePropertyRequestDto): Promise<PropertyDetailView> {
    if (isMockApiMode()) {
      return mapMockPropertyDetail();
    }
    const dto = await this.post<PropertyDto>("/properties", body);
    return mapPropertyResponse(dto);
  }

  async update(
    id: string,
    body: UpdatePropertyRequestDto,
  ): Promise<PropertyDetailView> {
    if (isMockApiMode()) {
      return mapMockPropertyDetail();
    }
    const dto = await this.put<PropertyDto>(`/properties/${id}`, body);
    return mapPropertyResponse(dto);
  }

  async remove(id: string): Promise<void> {
    if (isMockApiMode()) {
      return;
    }
    await this.delete<unknown>(`/properties/${id}`);
  }
}

export const propertyService = new PropertyService();
