using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Goals.Entities;
using WealthOS.Infrastructure.Persistence.Configurations;

namespace WealthOS.Infrastructure.Goals.Configurations;

public sealed class FinancialGoalConfiguration : AuditableEntityConfiguration<FinancialGoal>
{
    public override void Configure(EntityTypeBuilder<FinancialGoal> builder)
    {
        base.Configure(builder);

        builder.ToTable("FinancialGoals");

        builder.HasIndex(goal => goal.UserId);
        builder.HasIndex(goal => new { goal.UserId, goal.Status });
        builder.HasIndex(goal => new { goal.UserId, goal.Category });
        builder.HasIndex(goal => new { goal.UserId, goal.Priority });
        builder.HasIndex(goal => goal.LinkedPropertyId);
        builder.HasIndex(goal => goal.LinkedInvestmentId);
        builder.HasIndex(goal => goal.LinkedLoanId);
        builder.HasIndex(goal => goal.LinkedIncomeSourceId);
        builder.HasIndex(goal => goal.TargetDate);

        builder.Property(goal => goal.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(goal => goal.Category)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(goal => goal.TargetAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(goal => goal.CurrentAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(goal => goal.MonthlyContribution)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(goal => goal.Priority)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(goal => goal.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(goal => goal.Description)
            .HasMaxLength(4000);

        builder.Property(goal => goal.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        // Soft GUID links only — no FK/cascade ownership of other modules' tables.
        builder.Property(goal => goal.LinkedPropertyId);
        builder.Property(goal => goal.LinkedInvestmentId);
        builder.Property(goal => goal.LinkedLoanId);
        builder.Property(goal => goal.LinkedIncomeSourceId);

        builder.HasMany(goal => goal.Contributions)
            .WithOne(contribution => contribution.Goal)
            .HasForeignKey(contribution => contribution.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(goal => goal.Milestones)
            .WithOne(milestone => milestone.Goal)
            .HasForeignKey(milestone => milestone.GoalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class GoalContributionConfiguration : AuditableEntityConfiguration<GoalContribution>
{
    public override void Configure(EntityTypeBuilder<GoalContribution> builder)
    {
        base.Configure(builder);

        builder.ToTable("GoalContributions");

        builder.HasIndex(contribution => contribution.GoalId);
        builder.HasIndex(contribution => new { contribution.GoalId, contribution.ContributedOn });

        builder.Property(contribution => contribution.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(contribution => contribution.Notes)
            .HasMaxLength(1000);

        builder.Property(contribution => contribution.Source)
            .HasMaxLength(128);
    }
}

public sealed class GoalMilestoneConfiguration : AuditableEntityConfiguration<GoalMilestone>
{
    public override void Configure(EntityTypeBuilder<GoalMilestone> builder)
    {
        base.Configure(builder);

        builder.ToTable("GoalMilestones");

        builder.HasIndex(milestone => milestone.GoalId);
        builder.HasIndex(milestone => new { milestone.GoalId, milestone.SortOrder });
        builder.HasIndex(milestone => new { milestone.GoalId, milestone.IsCompleted });

        builder.Property(milestone => milestone.Label)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(milestone => milestone.TargetPercent)
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(milestone => milestone.TargetAmount)
            .HasPrecision(18, 2);
    }
}
