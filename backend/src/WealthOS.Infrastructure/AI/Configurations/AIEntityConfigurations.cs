using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.AI.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.AI.Configurations;

public sealed class AIConversationConfiguration : AuditableEntityConfiguration<AIConversation>
{
    public override void Configure(EntityTypeBuilder<AIConversation> builder)
    {
        base.Configure(builder);

        builder.ToTable("AIConversations");
        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => new { entity.UserId, entity.Status });
        builder.HasIndex(entity => new { entity.UserId, entity.LastMessageAt });

        builder.Property(entity => entity.Title).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.ProviderKind).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.Summary).HasMaxLength(4000);

        builder.HasMany(entity => entity.Messages)
            .WithOne(message => message.Conversation)
            .HasForeignKey(message => message.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.Sessions)
            .WithOne(session => session.Conversation)
            .HasForeignKey(session => session.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(entity => entity.ToolExecutions)
            .WithOne(execution => execution.Conversation)
            .HasForeignKey(execution => execution.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class AIMessageConfiguration : AuditableEntityConfiguration<AIMessage>
{
    public override void Configure(EntityTypeBuilder<AIMessage> builder)
    {
        base.Configure(builder);

        builder.ToTable("AIMessages");
        builder.HasIndex(entity => new { entity.ConversationId, entity.Sequence }).IsUnique();
        builder.Property(entity => entity.Role).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.Content).HasMaxLength(16000).IsRequired();
        builder.Property(entity => entity.MetadataJson).HasMaxLength(8000);
    }
}

public sealed class ConversationSessionConfiguration : AuditableEntityConfiguration<ConversationSession>
{
    public override void Configure(EntityTypeBuilder<ConversationSession> builder)
    {
        base.Configure(builder);

        builder.ToTable("ConversationSessions");
        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => new { entity.UserId, entity.Status });
        builder.HasIndex(entity => entity.SessionKey).IsUnique();
        builder.Property(entity => entity.SessionKey).HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
    }
}

public sealed class AIToolConfiguration : AuditableEntityConfiguration<AITool>
{
    public override void Configure(EntityTypeBuilder<AITool> builder)
    {
        base.Configure(builder);

        builder.ToTable("AITools");
        builder.HasIndex(entity => entity.Code).IsUnique();
        builder.Property(entity => entity.Code).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Name).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Description).HasMaxLength(2000).IsRequired();
        builder.Property(entity => entity.Category).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.InputSchemaJson).HasMaxLength(8000);
    }
}

public sealed class AIToolExecutionConfiguration : AuditableEntityConfiguration<AIToolExecution>
{
    public override void Configure(EntityTypeBuilder<AIToolExecution> builder)
    {
        base.Configure(builder);

        builder.ToTable("AIToolExecutions");
        builder.HasIndex(entity => entity.ConversationId);
        builder.HasIndex(entity => entity.ToolCode);
        builder.Property(entity => entity.ToolCode).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.InputJson).HasMaxLength(8000);
        builder.Property(entity => entity.OutputJson).HasMaxLength(16000);
        builder.Property(entity => entity.ErrorMessage).HasMaxLength(2000);
    }
}

public sealed class AIContextConfiguration : AuditableEntityConfiguration<AIContext>
{
    public override void Configure(EntityTypeBuilder<AIContext> builder)
    {
        base.Configure(builder);

        builder.ToTable("AIContexts");
        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => new { entity.UserId, entity.BuiltAt });
        builder.Property(entity => entity.ContextJson).HasColumnType("text").IsRequired();
        builder.Property(entity => entity.ModulesIncluded).HasMaxLength(500);
    }
}

public sealed class AIMemoryConfiguration : AuditableEntityConfiguration<AIMemory>
{
    public override void Configure(EntityTypeBuilder<AIMemory> builder)
    {
        base.Configure(builder);

        builder.ToTable("AIMemories");
        builder.HasIndex(entity => new { entity.UserId, entity.Key }).IsUnique();
        builder.HasIndex(entity => new { entity.UserId, entity.MemoryType });
        builder.Property(entity => entity.MemoryType).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.Key).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Content).HasMaxLength(8000).IsRequired();
        builder.Property(entity => entity.MetadataJson).HasMaxLength(8000);
    }
}

public sealed class AIRecommendationConfiguration : AuditableEntityConfiguration<AIRecommendation>
{
    public override void Configure(EntityTypeBuilder<AIRecommendation> builder)
    {
        base.Configure(builder);

        builder.ToTable("AIRecommendations");
        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => new { entity.UserId, entity.Status });
        builder.Property(entity => entity.Title).HasMaxLength(300).IsRequired();
        builder.Property(entity => entity.Body).HasMaxLength(4000).IsRequired();
        builder.Property(entity => entity.Category).HasMaxLength(100);
        builder.Property(entity => entity.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.PayloadJson).HasMaxLength(8000);
    }
}

public sealed class AIInsightConfiguration : AuditableEntityConfiguration<AIInsight>
{
    public override void Configure(EntityTypeBuilder<AIInsight> builder)
    {
        base.Configure(builder);

        builder.ToTable("AIInsights");
        builder.HasIndex(entity => entity.UserId);
        builder.HasIndex(entity => new { entity.UserId, entity.Severity });
        builder.Property(entity => entity.Title).HasMaxLength(300).IsRequired();
        builder.Property(entity => entity.Body).HasMaxLength(4000).IsRequired();
        builder.Property(entity => entity.Module).HasMaxLength(64);
        builder.Property(entity => entity.Severity).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.PayloadJson).HasMaxLength(8000);
    }
}

public sealed class PromptTemplateConfiguration : AuditableEntityConfiguration<PromptTemplate>
{
    public override void Configure(EntityTypeBuilder<PromptTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("PromptTemplates");
        builder.HasIndex(entity => entity.Code).IsUnique();
        builder.HasIndex(entity => entity.Category);
        builder.Property(entity => entity.Code).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Name).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.Category).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.SystemPrompt).HasMaxLength(8000).IsRequired();
        builder.Property(entity => entity.UserPromptTemplate).HasMaxLength(8000).IsRequired();
        builder.Property(entity => entity.Description).HasMaxLength(1000);
    }
}
