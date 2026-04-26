using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyCompass.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FamilyScopedTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FamilyId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PerformedByUserId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CreatedByUserId",
                table: "Transactions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FamilyId_Date",
                table: "Transactions",
                columns: new[] { "FamilyId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PerformedByUserId",
                table: "Transactions",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UpdatedByUserId",
                table: "Transactions",
                column: "UpdatedByUserId");

            migrationBuilder.Sql("""
                DO $$
                DECLARE
                    fallback_user_id uuid;
                    fallback_family_id uuid;
                BEGIN
                    IF EXISTS (SELECT 1 FROM "Transactions") THEN
                        SELECT "Id"
                        INTO fallback_user_id
                        FROM "Users"
                        ORDER BY "CreatedAt"
                        LIMIT 1;

                        IF fallback_user_id IS NULL THEN
                            fallback_user_id := '11111111-1111-1111-1111-111111111111';
                            INSERT INTO "Users" ("Id", "Username", "PasswordHash", "CreatedAt")
                            VALUES (fallback_user_id, 'migration-system-user', 'MIGRATION_PLACEHOLDER_HASH', NOW())
                            ON CONFLICT ("Id") DO NOTHING;
                        END IF;

                        SELECT "Id"
                        INTO fallback_family_id
                        FROM "Families"
                        ORDER BY "CreatedAt"
                        LIMIT 1;

                        IF fallback_family_id IS NULL THEN
                            fallback_family_id := '22222222-2222-2222-2222-222222222222';
                            INSERT INTO "Families" ("Id", "Name", "CreatedByUserId", "CreatedAt")
                            VALUES (fallback_family_id, 'Migrated Family', fallback_user_id, NOW())
                            ON CONFLICT ("Id") DO NOTHING;
                        END IF;

                        UPDATE "Transactions"
                        SET "FamilyId" = fallback_family_id
                        WHERE "FamilyId" = '00000000-0000-0000-0000-000000000000';

                        UPDATE "Transactions"
                        SET "PerformedByUserId" = fallback_user_id
                        WHERE "PerformedByUserId" = '00000000-0000-0000-0000-000000000000';

                        UPDATE "Transactions"
                        SET "CreatedByUserId" = fallback_user_id
                        WHERE "CreatedByUserId" = '00000000-0000-0000-0000-000000000000';

                        UPDATE "Transactions"
                        SET "UpdatedAt" = COALESCE("CreatedAt", NOW())
                        WHERE "UpdatedAt" = '-infinity'::timestamp with time zone;
                    END IF;
                END
                $$;
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Families_FamilyId",
                table: "Transactions",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_CreatedByUserId",
                table: "Transactions",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_PerformedByUserId",
                table: "Transactions",
                column: "PerformedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UpdatedByUserId",
                table: "Transactions",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Families_FamilyId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_CreatedByUserId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_PerformedByUserId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UpdatedByUserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CreatedByUserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_FamilyId_Date",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PerformedByUserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UpdatedByUserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FamilyId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PerformedByUserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Transactions");
        }
    }
}
