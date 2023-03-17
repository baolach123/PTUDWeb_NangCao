using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings
{
    public class PostMap : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x=> x.ShortDescription)
                .HasMaxLength(5000)
                .IsRequired() ;

            builder.Property(x=>x.Description)
                .HasMaxLength (5000)
                .IsRequired() ;

            builder.Property(x=>x.UrlSlug)
                .HasMaxLength(200)
                .IsRequired() ;

            builder.Property(x=>x.Meta)
                .HasMaxLength(1000)
                .IsRequired() ;

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.ViewCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x=>x.Published)
                .IsRequired ()
                .HasDefaultValue(false);

            builder.Property(x => x.PostedDate)
                .HasColumnType("datetime");

            builder.Property(x => x.ModifiedDate)
                .HasColumnType("datetime");

            builder.HasOne(x => x.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(x => x.CategotyId)
                .HasConstraintName("FK_Posts_Categories")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Author)
                .WithMany(c => c.Posts)
                .HasForeignKey(x => x.AuthorId)
                .HasConstraintName("FK_Posts_Authors")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Tags)
                .WithMany(x => x.posts)
                .UsingEntity(pt => pt.ToTable("PostTags"));
        }
    }
}
