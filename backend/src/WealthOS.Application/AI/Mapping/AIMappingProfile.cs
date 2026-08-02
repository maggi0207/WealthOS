using AutoMapper;
using WealthOS.Application.AI.DTOs.Responses;
using WealthOS.Domain.AI.Entities;
using WealthOS.Domain.AI.Models;

namespace WealthOS.Application.AI.Mapping;

/// <summary>AutoMapper profile for AI advisor entities and DTOs.</summary>
public sealed class AIMappingProfile : Profile
{
    public AIMappingProfile()
    {
        CreateMap<AIMessage, AIMessageResponse>();
        CreateMap<AIConversationSummary, AIConversationSummaryResponse>();
        CreateMap<AIMemory, AIMemoryResponse>();
        CreateMap<AIRecommendation, AIRecommendationResponse>();
        CreateMap<AIInsight, AIInsightItemResponse>();
        CreateMap<PromptTemplate, PromptTemplateResponse>();
    }
}
