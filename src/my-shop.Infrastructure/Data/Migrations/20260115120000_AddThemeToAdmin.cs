using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace my_shop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddThemeToAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if theme column exists, if not add it
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name='admins' 
                        AND column_name='theme'
                    ) THEN
                        ALTER TABLE admins ADD COLUMN theme VARCHAR(20) DEFAULT 'Light';
                    END IF;
                END $$;
            ");

            // Update existing admins to have Light theme if null
            migrationBuilder.Sql(@"
                UPDATE admins 
                SET theme = 'Light' 
                WHERE theme IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name='admins' 
                        AND column_name='theme'
                    ) THEN
                        ALTER TABLE admins DROP COLUMN theme;
                    END IF;
                END $$;
            ");
        }
    }
}
