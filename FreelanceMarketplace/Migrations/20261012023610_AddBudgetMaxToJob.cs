using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceMarketplace.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetMaxToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BudgetMin",
                table: "Jobs",
                type: "numeric(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "BudgetMax",
                table: "Jobs",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetMax",
                table: "Jobs");

            migrationBuilder.AlterColumn<decimal>(
                name: "BudgetMin",
                table: "Jobs",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldNullable: true);
        }
    }
}
