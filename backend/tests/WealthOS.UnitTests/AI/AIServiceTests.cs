using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using WealthOS.Application.AI.DTOs.Requests;
using WealthOS.Application.AI.Interfaces;
using WealthOS.Application.AI.Options;
using WealthOS.Application.AI.Services;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Domain.AI.Entities;
using WealthOS.Domain.AI.Enums;
using WealthOS.Domain.AI.Models;
using WealthOS.Domain.AI.Repositories;
using WealthOS.Domain.Common.Abstractions.Repositories;

namespace WealthOS.UnitTests.AI;

/// <summary>
/// Unit test skeleton for AIService chat orchestration (provider/tool stubs).
/// </summary>
public sealed class AIServiceTests
{
    private readonly Mock<IAIConversationRepository> _conversationRepository = new();
    private readonly Mock<IAIMessageRepository> _messageRepository = new();
    private readonly Mock<IConversationSessionRepository> _sessionRepository = new();
    private readonly Mock<IAIToolExecutionRepository> _toolExecutionRepository = new();
    private readonly Mock<IAIInsightRepository> _insightRepository = new();
    private readonly Mock<IAIContextBuilder> _contextBuilder = new();
    private readonly Mock<IAIToolRegistry> _toolRegistry = new();
    private readonly Mock<IAIPromptService> _promptService = new();
    private readonly Mock<IAIRecommendationService> _recommendationService = new();
    private readonly Mock<IAIProvider> _provider = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly AIService _sut;
    private readonly Guid _userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public AIServiceTests()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(_userId);

        _unitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _conversationRepository
            .Setup(repo => repo.AddAsync(It.IsAny<AIConversation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _sessionRepository
            .Setup(repo => repo.AddAsync(It.IsAny<ConversationSession>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _messageRepository
            .Setup(repo => repo.AddAsync(It.IsAny<AIMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _toolExecutionRepository
            .Setup(repo => repo.AddAsync(It.IsAny<AIToolExecution>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _provider.SetupGet(provider => provider.Kind).Returns(AIProviderKind.OpenAI);
        _provider
            .Setup(provider => provider.GenerateResponseAsync(
                It.IsAny<AIProviderRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success("[Placeholder:OpenAI] Hello"));

        _contextBuilder
            .Setup(builder => builder.BuildAsync(
                _userId,
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new AIContextSnapshot
            {
                UserId = _userId,
                BuiltAt = DateTime.UtcNow,
                ContextJson = "{}",
                ModulesIncluded = new[] { "Dashboard" },
            }));

        _toolRegistry
            .Setup(registry => registry.ExecuteRelevantAsync(
                It.IsAny<AIToolExecutionContext>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<AIToolResultDto>());

        _messageRepository
            .Setup(repo => repo.GetNextSequenceAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        _sessionRepository
            .Setup(repo => repo.GetOpenSessionAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConversationSession?)null);

        _sut = new AIService(
            _conversationRepository.Object,
            _messageRepository.Object,
            _sessionRepository.Object,
            _toolExecutionRepository.Object,
            _insightRepository.Object,
            _contextBuilder.Object,
            _toolRegistry.Object,
            _promptService.Object,
            _recommendationService.Object,
            new[] { _provider.Object },
            _unitOfWork.Object,
            _currentUser.Object,
            Options.Create(new AIOptions()));
    }

    [Fact]
    public async Task StartConversationAsync_WhenAuthenticated_ShouldSucceed()
    {
        var result = await _sut.StartConversationAsync(new StartConversationRequest
        {
            Title = "Net worth chat",
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.IsPlaceholder.Should().BeTrue();
        result.Value.ConversationId.Should().NotBeEmpty();
        _conversationRepository.Verify(
            repo => repo.AddAsync(It.IsAny<AIConversation>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_WhenConversationExists_ShouldReturnPlaceholderReply()
    {
        var conversationId = Guid.NewGuid();
        var conversation = new AIConversation(conversationId)
        {
            UserId = _userId,
            Title = "Existing",
            Status = AIConversationStatus.Active,
            ProviderKind = AIProviderKind.OpenAI,
        };

        _conversationRepository
            .Setup(repo => repo.GetByIdForUserAsync(conversationId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _sut.SendMessageAsync(new SendMessageRequest
        {
            ConversationId = conversationId,
            Message = "Summarize my dashboard",
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Reply.Should().Contain("Placeholder");
        result.Value.IsPlaceholder.Should().BeTrue();
        _contextBuilder.Verify(
            builder => builder.BuildAsync(_userId, conversationId, It.IsAny<CancellationToken>()),
            Times.Once);
        _toolRegistry.Verify(
            registry => registry.ExecuteRelevantAsync(
                It.IsAny<AIToolExecutionContext>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetSuggestionsAsync_ShouldReturnStaticSuggestions()
    {
        var result = await _sut.GetSuggestionsAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Suggestions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetHistoryAsync_WhenAuthenticated_ShouldReturnPagedHistory()
    {
        _conversationRepository
            .Setup(repo => repo.ListSummariesForUserAsync(
                _userId,
                1,
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                new List<AIConversationSummary>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Title = "Chat",
                        Status = AIConversationStatus.Active,
                        MessageCount = 2,
                    },
                },
                1));

        var result = await _sut.GetHistoryAsync(1, 20);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(1);
        result.Value.Items.Should().HaveCount(1);
    }
}
