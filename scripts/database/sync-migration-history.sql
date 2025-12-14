-- Migration History Sync Script
-- This script manually inserts the InitialCreate migration into history
-- Use this ONLY if you have existing tables from EnsureCreated()

-- Check if __EFMigrationsHistory table exists
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Insert InitialCreate migration as if it was already applied
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250104000000_InitialCreate', '8.0.11')
ON CONFLICT ("MigrationId") DO NOTHING;

-- Verify
SELECT * FROM "__EFMigrationsHistory";
