import { isMockApiMode } from "@/config/env";
import {
  makeUserMessage,
  mockAdvisorReply,
  type ChatMessage,
  type Conversation,
  conversationHistory as mockHistory,
  advisorInsights as mockInsights,
  suggestedPrompts as mockPrompts,
} from "@/lib/advisor-data";
import { BaseApiService } from "@/services/http/base-api-service";

export type AiChatResult = {
  conversationId: string;
  reply: ChatMessage;
};

type AIChatResponseDto = {
  conversationId: string;
  assistantMessageId: string;
  reply: string;
  isPlaceholder: boolean;
  modulesUsed?: string[];
};

type AIConversationSummaryDto = {
  id: string;
  title: string;
  lastMessageAt?: string | null;
  messageCount: number;
};

type AIHistoryDto = {
  items: AIConversationSummaryDto[];
};

type AIConversationDto = {
  id: string;
  title: string;
  messages: Array<{
    id: string;
    role: number | string;
    content: string;
    createdAt: string;
  }>;
};

type AISuggestionsDto = { suggestions: string[] };

type AIInsightsDto = {
  insights?: Array<{
    id: string;
    tag?: string;
    title: string;
    body: string;
    impact?: string;
    severity?: string;
  }>;
};

function nowTime(): string {
  return new Date().toLocaleTimeString("en-IN", {
    hour: "2-digit",
    minute: "2-digit",
  });
}

function mapRole(role: number | string): ChatMessage["role"] {
  const k = String(role).toLowerCase();
  if (k === "0" || k === "user") return "user";
  return "assistant";
}

class AiService extends BaseApiService {
  protected readonly serviceName = "AiService";

  async chat(message: string, conversationId?: string): Promise<AiChatResult> {
    if (isMockApiMode()) {
      return {
        conversationId: conversationId || "mock-conversation",
        reply: mockAdvisorReply(message),
      };
    }

    const dto = await this.post<AIChatResponseDto>("/ai/chat", {
      message,
      conversationId: conversationId || null,
    });

    return {
      conversationId: String(dto.conversationId),
      reply: {
        id: String(dto.assistantMessageId),
        role: "assistant",
        text: dto.reply,
        points: dto.modulesUsed?.length
          ? dto.modulesUsed.map((m) => `Used ${m} context`)
          : undefined,
        time: nowTime(),
      },
    };
  }

  async getHistory(): Promise<Conversation[]> {
    if (isMockApiMode()) {
      return mockHistory.map((c) => ({
        ...c,
        messages: c.messages.map((m) => ({ ...m })),
      }));
    }

    const history = await this.get<AIHistoryDto>("/ai/history?pageSize=20");
    return (history.items ?? []).map((item) => ({
      id: String(item.id),
      title: item.title,
      preview: `${item.messageCount} messages`,
      bucket: "Earlier" as const,
      time: item.lastMessageAt
        ? new Date(item.lastMessageAt).toLocaleDateString("en-IN", {
            day: "numeric",
            month: "short",
          })
        : "",
      messages: [],
    }));
  }

  async getConversation(id: string): Promise<Conversation> {
    if (isMockApiMode()) {
      const found = mockHistory.find((c) => c.id === id);
      return (
        found ?? {
          id,
          title: "Conversation",
          preview: "",
          bucket: "Earlier",
          time: "",
          messages: [],
        }
      );
    }

    // History list items are summaries; open with empty messages until chat continues.
    return {
      id,
      title: "Conversation",
      preview: "",
      bucket: "Earlier",
      time: "",
      messages: [],
    };
  }

  async getSuggestions(): Promise<string[]> {
    if (isMockApiMode()) return mockPrompts.map((p) => p.text);
    const dto = await this.get<AISuggestionsDto>("/ai/suggestions");
    return dto.suggestions ?? [];
  }

  async getInsights() {
    if (isMockApiMode()) return mockInsights;
    const dto = await this.get<AIInsightsDto>("/ai/insights");
    return (dto.insights ?? []).map((i) => ({
      id: String(i.id),
      tag: i.tag || "Insight",
      title: i.title,
      body: i.body,
      impact: i.impact || "",
      tone:
        i.severity === "High" || i.severity === "high"
          ? ("caution" as const)
          : ("neutral" as const),
      action: { label: "Open", to: "/dashboard" },
    }));
  }

  createUserMessage(text: string): ChatMessage {
    return makeUserMessage(text);
  }
}

export const aiService = new AiService();
