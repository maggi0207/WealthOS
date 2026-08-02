import { isMockApiMode } from "@/config/env";
import { BaseApiService } from "@/services/http/base-api-service";
import {
  mapActivitiesResponse,
  mapDashboardHealthResponse,
  mapDashboardResponse,
  mapMockActivities,
  mapMockDashboardHealth,
  mapMockDashboardSummary,
  mapMockHealthScore,
  mapMockNetWorth,
  mapNetWorthResponse,
} from "@/services/dashboard/dashboard-mapper";
import type {
  ActivityView,
  DashboardHealthResponseDto,
  DashboardHealthView,
  DashboardResponseDto,
  DashboardSummaryView,
  HealthScoreView,
  NetWorthResponseDto,
  NetWorthView,
  RecentActivityDto,
} from "@/services/dashboard/types";

/**
 * Dashboard API service — GET /api/v{version}/dashboard*.
 * Transparent mock fallback when `VITE_API_MODE=mock`.
 */
class DashboardService extends BaseApiService {
  protected readonly serviceName = "DashboardService";

  /** Full dashboard summary (KPIs, health score, activities, quick actions). */
  async getDashboard(): Promise<DashboardSummaryView> {
    if (isMockApiMode()) {
      return mapMockDashboardSummary();
    }
    const dto = await this.get<DashboardResponseDto>("/dashboard");
    return mapDashboardResponse(dto);
  }

  /** Net-worth / assets / liabilities slice. */
  async getNetWorth(): Promise<NetWorthView> {
    if (isMockApiMode()) {
      return mapMockNetWorth();
    }
    const dto = await this.get<NetWorthResponseDto>("/dashboard/net-worth");
    return mapNetWorthResponse(dto);
  }

  /** Recent portfolio activity feed. */
  async getActivities(limit = 10): Promise<ActivityView[]> {
    if (isMockApiMode()) {
      return mapMockActivities(limit);
    }
    const dto = await this.get<RecentActivityDto[]>(
      `/dashboard/activities?limit=${limit}`,
    );
    return mapActivitiesResponse(dto);
  }

  /**
   * Financial health score (from summary payload).
   * Prefer `getDashboard()` when the page already needs the full summary.
   */
  async getHealthScore(): Promise<HealthScoreView> {
    if (isMockApiMode()) {
      return mapMockHealthScore();
    }
    const dto = await this.get<DashboardResponseDto>("/dashboard");
    return mapDashboardResponse(dto).healthScore;
  }

  /** Module / provider readiness (`GET /dashboard/health`). */
  async getHealth(): Promise<DashboardHealthView> {
    if (isMockApiMode()) {
      return mapMockDashboardHealth();
    }
    const dto = await this.get<DashboardHealthResponseDto>("/dashboard/health");
    return mapDashboardHealthResponse(dto);
  }
}

export const dashboardService = new DashboardService();
