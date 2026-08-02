import { isMockApiMode } from "@/config/env";
import { notifications as mockNotifications } from "@/lib/mock-data";
import { BaseApiService } from "@/services/http/base-api-service";

export type NotificationView = {
  id: string;
  title: string;
  meta: string;
  unread: boolean;
};

export type NotificationsOverview = {
  items: NotificationView[];
  unreadCount: number;
};

type NotificationListItemDto = {
  id: string;
  title: string;
  type: number | string;
  status: number | string;
  createdAt: string;
  readAt?: string | null;
};

type NotificationListDto = {
  items: NotificationListItemDto[];
  totalCount: number;
};

type NotificationSummaryDto = {
  totalCount: number;
  unreadCount: number;
};

function relativeMeta(createdAt: string, type: number | string): string {
  const module = String(type) || "App";
  const d = new Date(createdAt);
  if (Number.isNaN(d.getTime())) return String(module);
  const mins = Math.round((Date.now() - d.getTime()) / 60_000);
  if (mins < 60) return `${module} · ${Math.max(mins, 1)}m ago`;
  if (mins < 1440) return `${module} · ${Math.round(mins / 60)}h ago`;
  return `${module} · ${Math.round(mins / 1440)}d ago`;
}

function isUnread(status: number | string, readAt?: string | null): boolean {
  if (readAt) return false;
  const k = String(status).toLowerCase();
  return k === "0" || k === "unread" || k === "pending";
}

class NotificationService extends BaseApiService {
  protected readonly serviceName = "NotificationService";

  async getOverview(): Promise<NotificationsOverview> {
    if (isMockApiMode()) {
      return {
        items: mockNotifications.map((n) => ({
          id: n.id,
          title: n.title,
          meta: n.meta,
          unread: true,
        })),
        unreadCount: mockNotifications.length,
      };
    }

    const [list, summary] = await Promise.all([
      this.get<NotificationListDto>("/notifications?pageSize=20"),
      this.get<NotificationSummaryDto>("/notifications/summary"),
    ]);

    const items = (list.items ?? []).map((n) => ({
      id: String(n.id),
      title: n.title,
      meta: relativeMeta(n.createdAt, n.type),
      unread: isUnread(n.status, n.readAt),
    }));

    return {
      items,
      unreadCount: summary.unreadCount ?? items.filter((i) => i.unread).length,
    };
  }

  async markRead(id: string): Promise<void> {
    if (isMockApiMode()) return;
    await this.put(`/notifications/${id}/read`);
  }
}

export const notificationService = new NotificationService();
