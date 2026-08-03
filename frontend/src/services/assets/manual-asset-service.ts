import { BaseApiService } from "@/services/http/base-api-service";
import type {
  CreateManualAssetRequestDto,
  ManualAssetDto,
  ManualAssetListDto,
  UpdateManualAssetRequestDto,
} from "@/services/assets/types";

function buildQuery(params?: {
  page?: number;
  pageSize?: number;
  search?: string;
  type?: number;
}): string {
  const search = new URLSearchParams();
  if (params?.page != null) search.set("page", String(params.page));
  if (params?.pageSize != null) search.set("pageSize", String(params.pageSize));
  if (params?.search) search.set("search", params.search);
  if (params?.type != null) search.set("type", String(params.type));
  const qs = search.toString();
  return qs ? `?${qs}` : "";
}

class ManualAssetService extends BaseApiService {
  protected readonly serviceName = "ManualAssets";

  list(params?: {
    page?: number;
    pageSize?: number;
    search?: string;
    type?: number;
  }): Promise<ManualAssetListDto> {
    return this.get<ManualAssetListDto>(`/api/v1/assets/manual${buildQuery(params)}`);
  }

  getById(id: string): Promise<ManualAssetDto> {
    return this.get<ManualAssetDto>(`/api/v1/assets/manual/${id}`);
  }

  create(body: CreateManualAssetRequestDto): Promise<ManualAssetDto> {
    return this.post<ManualAssetDto>("/api/v1/assets/manual", body);
  }

  update(id: string, body: UpdateManualAssetRequestDto): Promise<ManualAssetDto> {
    return this.put<ManualAssetDto>(`/api/v1/assets/manual/${id}`, body);
  }

  remove(id: string): Promise<void> {
    return this.delete<void>(`/api/v1/assets/manual/${id}`);
  }
}

export const manualAssetService = new ManualAssetService();
