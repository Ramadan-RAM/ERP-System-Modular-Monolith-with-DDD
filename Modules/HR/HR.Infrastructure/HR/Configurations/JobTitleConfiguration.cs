using HR.Domain.HR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.HR.Configurations
{
    public class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
    {
        public void Configure(EntityTypeBuilder<JobTitle> builder)
        {
            builder.ToTable("JobTitles");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(j => j.Description)
                   .HasMaxLength(500);

            // ✅ علاقة مع القسم
            builder.HasOne(j => j.Department)
                   .WithMany(d => d.JobTitles)
                   .HasForeignKey(j => j.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(j => !j.IsDeleted);
        }
    }

}