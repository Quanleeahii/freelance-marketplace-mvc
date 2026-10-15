using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceMarketplace.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberToProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Proposals",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Proposals");
        }
    }
}
