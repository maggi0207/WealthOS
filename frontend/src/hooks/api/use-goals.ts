import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { goalService } from "@/services/goals/goal-service";
import type {
  CreateGoalRequestDto,
  RecordGoalContributionRequestDto,
  UpdateGoalRequestDto,
} from "@/services/goals/types";

export const goalKeys = {
  all: ["goals"] as const,
  overview: () => [...goalKeys.all, "overview"] as const,
};

export function useGoalsOverview() {
  return useQuery({
    queryKey: goalKeys.overview(),
    queryFn: () => goalService.getOverview(),
  });
}

function invalidateGoals(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: goalKeys.all });
  void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
}

export function useCreateGoal() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateGoalRequestDto) => goalService.create(body),
    onSuccess: () => invalidateGoals(queryClient),
  });
}

export function useUpdateGoal() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, body }: { id: string; body: UpdateGoalRequestDto }) =>
      goalService.update(id, body),
    onSuccess: () => invalidateGoals(queryClient),
  });
}

export function useDeleteGoal() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => goalService.remove(id),
    onSuccess: () => invalidateGoals(queryClient),
  });
}

export function useRecordGoalContribution() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      goalId,
      body,
    }: {
      goalId: string;
      body: RecordGoalContributionRequestDto;
    }) => goalService.recordContribution(goalId, body),
    onSuccess: () => invalidateGoals(queryClient),
  });
}
