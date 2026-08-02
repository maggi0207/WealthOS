import { isMockApiMode } from "@/config/env";
import {
  goals as mockGoals,
  goalsOverallPct as mockOverallPct,
  goalsSummary as mockSummary,
  type Goal,
  type GoalCategory,
  type Milestone,
} from "@/lib/goals-data";
import { BaseApiService } from "@/services/http/base-api-service";

export type GoalsSummaryView = {
  totalSaved: number;
  totalTarget: number;
  monthlyCommitted: number;
  completed: number;
  active: number;
  overallPct: number;
  goalCount: number;
};

export type GoalsOverview = {
  summary: GoalsSummaryView;
  goals: Goal[];
};

type GoalDashboardDto = {
  activeGoals: number;
  completedGoals: number;
  pausedGoals: number;
  totalGoalValue: number;
  totalSaved: number;
  overallProgressPercent: number;
  monthlyCommitted: number;
};

type GoalMilestoneDto = {
  id: string;
  label: string;
  targetPercent: number;
  reachedOn?: string | null;
  completedOn?: string | null;
};

type GoalListItemDto = {
  id: string;
  name: string;
  category: number | string;
  targetAmount: number;
  currentAmount: number;
  monthlyContribution: number;
  targetDate: string;
  startedOn: string;
  description?: string | null;
  milestones?: GoalMilestoneDto[];
};

type GoalListDto = { items: GoalListItemDto[] };

type GoalDetailDto = GoalListItemDto & {
  milestones: GoalMilestoneDto[];
};

function n(v: unknown, f = 0) {
  const x = typeof v === "number" ? v : Number(v);
  return Number.isFinite(x) ? x : f;
}

function mapCategory(c: number | string): GoalCategory {
  const k = String(c);
  if (k === "0" || k.includes("House") || k.includes("property")) return "property";
  if (k === "2" || k.includes("Loan") || k.includes("debt")) return "debt";
  if (k === "1" || k.includes("Emergency") || k.includes("safety")) return "safety";
  if (k === "3" || k.includes("Education") || k.includes("education")) return "education";
  return "retirement";
}

function mapMilestone(m: GoalMilestoneDto): Milestone {
  return {
    id: String(m.id),
    label: m.label,
    atPct: n(m.targetPercent),
    reachedOn: m.reachedOn?.slice(0, 10) || m.completedOn?.slice(0, 10),
  };
}

function mapGoal(dto: GoalListItemDto | GoalDetailDto): Goal {
  return {
    id: String(dto.id),
    name: dto.name,
    category: mapCategory(dto.category),
    target: n(dto.targetAmount),
    saved: n(dto.currentAmount),
    monthlyContribution: n(dto.monthlyContribution),
    targetDate: dto.targetDate?.slice(0, 10) || "",
    startedOn: dto.startedOn?.slice(0, 10) || "",
    note: dto.description || "",
    milestones: (dto.milestones ?? []).map(mapMilestone),
  };
}

function mapMock(): GoalsOverview {
  return {
    summary: {
      totalSaved: mockSummary.totalSaved,
      totalTarget: mockSummary.totalTarget,
      monthlyCommitted: mockSummary.monthlyCommitted,
      completed: mockSummary.completed,
      active: mockGoals.length - mockSummary.completed,
      overallPct: mockOverallPct,
      goalCount: mockGoals.length,
    },
    goals: mockGoals.map((g) => ({
      ...g,
      milestones: g.milestones.map((m) => ({ ...m })),
    })),
  };
}

class GoalService extends BaseApiService {
  protected readonly serviceName = "GoalService";

  async getOverview(): Promise<GoalsOverview> {
    if (isMockApiMode()) return mapMock();

    const [dashboard, list] = await Promise.all([
      this.get<GoalDashboardDto>("/goals/dashboard"),
      this.get<GoalListDto>("/goals?pageSize=50"),
    ]);

    const items = list.items ?? [];
    // Fetch details for milestones when list is thin — best-effort parallel.
    const detailed = await Promise.all(
      items.slice(0, 12).map(async (item) => {
        try {
          return await this.get<GoalDetailDto>(`/goals/${item.id}`);
        } catch {
          return item;
        }
      }),
    );

    const goals = detailed.map(mapGoal);
    return {
      summary: {
        totalSaved: n(dashboard.totalSaved),
        totalTarget: n(dashboard.totalGoalValue),
        monthlyCommitted: n(dashboard.monthlyCommitted),
        completed: n(dashboard.completedGoals),
        active: n(dashboard.activeGoals),
        overallPct: Math.round(n(dashboard.overallProgressPercent)),
        goalCount: goals.length,
      },
      goals,
    };
  }
}

export const goalService = new GoalService();
