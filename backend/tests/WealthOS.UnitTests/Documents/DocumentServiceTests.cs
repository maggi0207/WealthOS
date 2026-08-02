using AutoMapper;
using FluentAssertions;
using Moq;
using WealthOS.Application.Common.Interfaces;
using WealthOS.Application.Common.Models;
using WealthOS.Application.Documents.DTOs.Requests;
using WealthOS.Application.Documents.Mapping;
using WealthOS.Application.Documents.Services;
using WealthOS.Domain.Common.Abstractions.Repositories;
using WealthOS.Domain.Documents.Entities;
using WealthOS.Domain.Documents.Enums;
using WealthOS.Domain.Documents.Repositories;

namespace WealthOS.UnitTests.Documents;

/// <summary>
/// Unit tests for DocumentService create/status resolution paths.
/// </summary>
public sealed class DocumentServiceTests
{
    private readonly Mock<IDocumentRepository> _documentRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly DocumentService _sut;

    public DocumentServiceTests()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<DocumentMappingProfile>())
            .CreateMapper();

        var userId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(true);
        _currentUser.SetupGet(user => user.UserId).Returns(userId);

        _documentRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _documentRepository
            .Setup(repo => repo.GetByIdWithDetailsAsync(
                It.IsAny<Guid>(),
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, Guid _, CancellationToken _) => new Document(id)
            {
                UserId = userId,
                Title = "Sale deed",
                Owner = "Magesh",
                Category = DocumentCategory.Property,
                Status = DocumentStatus.Verified,
                Tags = { new DocumentTag { Name = "deed" } },
            });

        _sut = new DocumentService(
            _documentRepository.Object,
            _unitOfWork.Object,
            _currentUser.Object,
            mapper);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ShouldPersistAndReturnDocument()
    {
        var result = await _sut.CreateAsync(new CreateDocumentRequest
        {
            Title = "Sale deed",
            Owner = "Magesh",
            Category = DocumentCategory.Property,
            Status = DocumentStatus.Verified,
            OriginalFileName = "deed.pdf",
            ContentType = "application/pdf",
            FileSizeBytes = 1000,
            StorageProvider = DocumentStorageProvider.LocalPlaceholder,
            Tags = ["deed", "adyar"],
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Sale deed");
        _documentRepository.Verify(
            repo => repo.AddAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenPastExpiry_ShouldMarkExpired()
    {
        Document? captured = null;
        _documentRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()))
            .Callback<Document, CancellationToken>((document, _) => captured = document)
            .Returns(Task.CompletedTask);

        _documentRepository
            .Setup(repo => repo.GetByIdWithDetailsAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, Guid userId, CancellationToken _) =>
            {
                captured!.Id.Should().NotBeEmpty();
                return captured;
            });

        var result = await _sut.CreateAsync(new CreateDocumentRequest
        {
            Title = "Car insurance",
            Owner = "Magesh",
            Category = DocumentCategory.Insurance,
            Status = DocumentStatus.Verified,
            ExpiryDate = new DateOnly(2020, 1, 1),
        });

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.Status.Should().Be(DocumentStatus.Expired);
    }

    [Fact]
    public async Task CreateAsync_WhenUnauthenticated_ShouldFail()
    {
        _currentUser.SetupGet(user => user.IsAuthenticated).Returns(false);
        _currentUser.SetupGet(user => user.UserId).Returns((Guid?)null);

        var result = await _sut.CreateAsync(new CreateDocumentRequest
        {
            Title = "Test",
            Owner = "Magesh",
        });

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("unauthorized");
    }

    [Fact]
    public async Task CreateAsync_WhenReferenceModuleWithoutId_ShouldFail()
    {
        var result = await _sut.CreateAsync(new CreateDocumentRequest
        {
            Title = "Linked",
            Owner = "Magesh",
            ReferenceModule = DocumentReferenceModule.Loan,
            ReferenceId = null,
        });

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("validation_error");
    }
}
