using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceMarketplace.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJobModelEnumsAndBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BudgetMax",
                table: "Jobs",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkMode",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkMode",
                table: "Jobs");

            migrationBuilder.AlterColumn<decimal>(
                name: "BudgetMax",
                table: "Jobs",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);
        }
    }
}
