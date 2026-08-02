import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { aiService } from "@/services/ai/ai-service";

export const aiKeys = {
  all: ["ai"] as const,
  history: () => [...aiKeys.all, "history"] as const,
  insights: () => [...aiKeys.all, "insights"] as const,
  suggestions: () => [...aiKeys.all, "suggestions"] as const,
};

export function useAiHistory() {
  return useQuery({
    queryKey: aiKeys.history(),
    queryFn: () => aiService.getHistory(),
  });
}

export function useAiInsights() {
  return useQuery({
    queryKey: aiKeys.insights(),
    queryFn: () => aiService.getInsights(),
  });
}

export function useAiChat() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      message,
      conversationId,
    }: {
      message: string;
      conversationId?: string;
    }) => aiService.chat(message, conversationId),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: aiKeys.history() });
    },
  });
}
