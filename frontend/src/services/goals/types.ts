/** Goal API DTOs — aligned with WealthOS.Application.Goals */

export type GoalCategoryDto = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7;

export type GoalStatusDto = 0 | 1 | 2 | 3;

export type GoalPriorityDto = 0 | 1 | 2 | 3;

export type GoalDto = {
  id: string;
  name: string;
  category: GoalCategoryDto | string;
  targetAmount: number;
  currentAmount: number;
  targetDate: string;
  startedOn: string;
  monthlyContribution: number;
  priority?: GoalPriorityDto | string;
  status?: GoalStatusDto | string;
  description?: string | null;
  currencyCode?: string;
};

export type CreateGoalRequestDto = {
  name: string;
  category: GoalCategoryDto;
  targetAmount: number;
  currentAmount: number;
  targetDate: string;
  startedOn: string;
  monthlyContribution: number;
  priority?: GoalPriorityDto;
  status?: GoalStatusDto;
  description?: string | null;
  currencyCode?: string;
};

export type UpdateGoalRequestDto = CreateGoalRequestDto;

export type RecordGoalContributionRequestDto = {
  amount: number;
  contributedOn: string;
  notes?: string | null;
  source?: string | null;
};

export type GoalContributionDto = {
  id: string;
  goalId: string;
  amount: number;
  contributedOn: string;
  notes?: string | null;
  source?: string | null;
};
