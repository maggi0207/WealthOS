namespace WealthOS.Domain.AI.Enums;

/// <summary>Role of a message within an AI conversation.</summary>
public enum AIMessageRole
{
    System = 0,
    User = 1,
    Assistant = 2,
    Tool = 3,
}

/// <summary>Lifecycle status of an AI conversation.</summary>
public enum AIConversationStatus
{
    Active = 0,
    Archived = 1,
    Cleared = 2,
}

/// <summary>Lifecycle status of a conversation session.</summary>
public enum ConversationSessionStatus
{
    Open = 0,
    Closed = 1,
    Expired = 2,
}

/// <summary>Category of a registered AI tool.</summary>
public enum AIToolCategory
{
    Dashboard = 0,
    Property = 1,
    Loan = 2,
    Investment = 3,
    Income = 4,
    Goal = 5,
    Document = 6,
    Notification = 7,
    General = 8,
}

/// <summary>Execution outcome for a tool invocation.</summary>
public enum AIToolExecutionStatus
{
    Pending = 0,
    Running = 1,
    Succeeded = 2,
    Failed = 3,
    Skipped = 4,
}

/// <summary>Kinds of long-term / short-term AI memory.</summary>
public enum AIMemoryType
{
    Conversation = 0,
    UserPreference = 1,
    FinancialContext = 2,
    RecentActivity = 3,
    ImportantFact = 4,
}

/// <summary>Configured LLM / orchestration provider kinds (placeholders).</summary>
public enum AIProviderKind
{
    OpenAI = 0,
    AzureOpenAI = 1,
    Anthropic = 2,
    Gemini = 3,
    MCP = 4,
}

/// <summary>Severity of a generated insight.</summary>
public enum AIInsightSeverity
{
    Info = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4,
}

/// <summary>Status of an AI recommendation.</summary>
public enum AIRecommendationStatus
{
    Draft = 0,
    Active = 1,
    Accepted = 2,
    Dismissed = 3,
    Expired = 4,
}

/// <summary>Reusable prompt template categories.</summary>
public enum PromptTemplateCategory
{
    FinancialSummary = 0,
    InvestmentAnalysis = 1,
    LoanAnalysis = 2,
    CashFlowAnalysis = 3,
    GoalPlanning = 4,
    BusinessAnalysis = 5,
    DocumentSearch = 6,
}
