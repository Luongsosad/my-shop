#!/bin/bash

# PostgreSQL Database Restore Script
# Usage: ./restore-database.sh <backup-file>

if [ -z "$1" ]; then
    echo "? ERROR: No backup file specified"
    echo "Usage: ./restore-database.sh <backup-file>"
    echo ""
    echo "Available backups:"
    ls -lht ../../backups/*.sql.gz 2>/dev/null || echo "No backups found"
    exit 1
fi

BACKUP_FILE="$1"

if [ ! -f "$BACKUP_FILE" ]; then
    echo "? ERROR: Backup file not found: $BACKUP_FILE"
    exit 1
fi

# Load environment variables
if [ -f "../../.env" ]; then
    export $(cat ../../.env | grep -v '^#' | xargs)
fi

if [ -z "$DATABASE_URL" ]; then
    echo "? ERROR: DATABASE_URL not found in environment"
    exit 1
fi

# Parse DATABASE_URL
DB_URL_REGEX="postgresql://([^:]+):([^@]+)@([^:]+):([^/]+)/([^?]+)"
if [[ $DATABASE_URL =~ $DB_URL_REGEX ]]; then
    DB_USER="${BASH_REMATCH[1]}"
    DB_PASSWORD="${BASH_REMATCH[2]}"
    DB_HOST="${BASH_REMATCH[3]}"
    DB_PORT="${BASH_REMATCH[4]}"
    DB_NAME="${BASH_REMATCH[5]}"
else
    echo "? ERROR: Invalid DATABASE_URL format"
    exit 1
fi

echo "??  WARNING: This will DROP and RECREATE all tables in the database!"
echo "?? Host: $DB_HOST:$DB_PORT"
echo "???  Database: $DB_NAME"
echo "?? Backup file: $BACKUP_FILE"
echo ""
read -p "Are you sure you want to continue? (yes/no): " CONFIRM

if [ "$CONFIRM" != "yes" ]; then
    echo "? Restore cancelled"
    exit 0
fi

# Export password
export PGPASSWORD="$DB_PASSWORD"

# Check if file is gzipped
if [[ "$BACKUP_FILE" == *.gz ]]; then
    echo "?? Decompressing backup file..."
    TEMP_FILE="${BACKUP_FILE%.gz}"
    gunzip -c "$BACKUP_FILE" > "$TEMP_FILE"
    RESTORE_FILE="$TEMP_FILE"
else
    RESTORE_FILE="$BACKUP_FILE"
fi

echo "?? Restoring database..."

# Restore database
psql -h "$DB_HOST" \
     -p "$DB_PORT" \
     -U "$DB_USER" \
     -d "$DB_NAME" \
     -f "$RESTORE_FILE"

if [ $? -eq 0 ]; then
    echo "? Database restored successfully!"
else
    echo "? Restore failed!"
    [ -f "$TEMP_FILE" ] && rm -f "$TEMP_FILE"
    exit 1
fi

# Clean up temporary file
if [[ "$BACKUP_FILE" == *.gz ]]; then
    rm -f "$TEMP_FILE"
fi

# Unset password
unset PGPASSWORD

echo ""
echo "? Restore completed!"
echo "?? Verify the restore with:"
echo "   cd src/my-shop.API"
echo "   dotnet run"
