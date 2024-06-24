using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyConverterApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableColumnDataTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RateKey",
                table: "ConversionsHistory",
                type: "varchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExchangeRate",
                table: "ConversionsHistory",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3)",
                oldMaxLength: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RateKey",
                table: "ConversionsHistory",
                type: "varchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)",
                oldMaxLength: 6);

            migrationBuilder.AlterColumn<string>(
                name: "ExchangeRate",
                table: "ConversionsHistory",
                type: "varchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
