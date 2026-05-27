using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Infrastructure.Persistence.Configurations;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> errorLogBuilder)
    {
        errorLogBuilder.ToTable("ErrorLogs");

        errorLogBuilder.HasKey(errorLog => errorLog.ErrorLogId);

        errorLogBuilder.Property(errorLog => errorLog.Message)
            .IsRequired()
            .HasMaxLength(500);

        errorLogBuilder.Property(errorLog => errorLog.StackTrace)
            .HasMaxLength(4000);

        errorLogBuilder.Property(errorLog => errorLog.Path)
            .HasMaxLength(250);

        errorLogBuilder.Property(errorLog => errorLog.HttpMethod)
            .HasMaxLength(20);

        errorLogBuilder.Property(errorLog => errorLog.OccurredAt)
            .IsRequired();
    }
}