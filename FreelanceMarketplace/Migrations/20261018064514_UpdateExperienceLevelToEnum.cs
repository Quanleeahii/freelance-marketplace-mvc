using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceMarketplace.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExperienceLevelToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""FreelancerProfiles"" 
                ALTER COLUMN ""ExperienceLevel"" TYPE integer 
                USING CASE 
                    WHEN ""ExperienceLevel"" ~ '^[0-9]+$' THEN ""ExperienceLevel""::integer 
                    ELSE NULL 
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""FreelancerProfiles"" 
                ALTER COLUMN ""ExperienceLevel"" TYPE text 
                USING ""ExperienceLevel""::text;
            ");
        }
    }
}