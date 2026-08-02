import {
  Banknote,
  Bot,
  Building2,
  Coins,
  FileText,
  LayoutDashboard,
  Landmark,
  PieChart,
  Receipt,
  Settings,
  Target,
  TrendingUp,
  type LucideIcon,
} from "lucide-react";

export type NavItem = {
  title: string;
  url: string;
  icon: LucideIcon;
  description: string;
};

export type NavGroup = {
  label: string;
  items: NavItem[];
};

export const navGroups: NavGroup[] = [
  {
    label: "Overview",
    items: [
      {
        title: "Dashboard",
        url: "/dashboard",
        icon: LayoutDashboard,
        description: "Net worth snapshot, cashflow pulse and portfolio highlights.",
      },
    ],
  },
  {
    label: "Balance sheet",
    items: [
      { title: "Assets", url: "/assets", icon: Coins, description: "Every asset you own, valued and categorised." },
      { title: "Properties", url: "/properties", icon: Building2, description: "Real estate holdings, equity and rental yield." },
      { title: "Loans", url: "/loans", icon: Landmark, description: "Mortgages and credit lines with amortisation views." },
      { title: "Investments", url: "/investments", icon: TrendingUp, description: "Brokerage, retirement and alternative positions." },
    ],
  },
  {
    label: "Cashflow",
    items: [
      {
        title: "Income & Business",
        url: "/income",
        icon: Banknote,
        description: "Salary, client revenue, developer payroll and monthly profit.",
      },
      { title: "Expenses", url: "/expenses", icon: Receipt, description: "Spending categories, trends and recurring costs." },
      { title: "Goals", url: "/goals", icon: Target, description: "Savings targets and progress tracking." },
    ],
  },
  {
    label: "Intelligence",
    items: [
      { title: "Documents", url: "/documents", icon: FileText, description: "Statements, deeds and contracts in one vault." },
      { title: "Reports", url: "/reports", icon: PieChart, description: "Custom financial reports and exports." },
      { title: "AI Advisor", url: "/ai-advisor", icon: Bot, description: "Conversational guidance across your finances." },
      { title: "Settings", url: "/settings", icon: Settings, description: "Preferences, currency and workspace options." },
    ],
  },
];

export const navItems: NavItem[] = navGroups.flatMap((group) => group.items);

export function findNavItem(pathname: string): NavItem | undefined {
  return navItems.find((item) => pathname === item.url || pathname.startsWith(`${item.url}/`));
}
