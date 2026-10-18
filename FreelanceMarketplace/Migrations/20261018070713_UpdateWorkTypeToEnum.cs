using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceMarketplace.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingCapacity",
                table: "FreelancerProfiles");

            migrationBuilder.AddColumn<int>(
                name: "WorkType",
                table: "FreelancerProfiles",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkType",
                table: "FreelancerProfiles");

            migrationBuilder.AddColumn<string>(
                name: "WorkingCapacity",
                table: "FreelancerProfiles",
                type: "text",
                nullable: true);
        }
    }
}
