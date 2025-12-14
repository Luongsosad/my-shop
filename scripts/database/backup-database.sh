#!/bin/bash

# PostgreSQL Database Backup Script
# Usage: ./backup-database.sh [backup-name]

# Load environment variables
if [ -f "../../.env" ]; then
    export $(cat ../../.env | grep -v '^#' | xargs)
fi

# Parse DATABASE_URL
if [ -z "$DATABASE_URL" ]; then
    echo "? ERROR: DATABASE_URL not found in environment"
    exit 1
fi

# Extract connection details from DATABASE_URL
# Format: postgresql://user:password@host:port/database
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

# Create backups directory if not exists
BACKUP_DIR="../../backups"
mkdir -p "$BACKUP_DIR"

# Generate backup filename
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_NAME="${1:-myshop}"
BACKUP_FILE="$BACKUP_DIR/${BACKUP_NAME}_${TIMESTAMP}.sql"
BACKUP_FILE_GZ="${BACKUP_FILE}.gz"

echo "?? Starting database backup..."
echo "?? Host: $DB_HOST:$DB_PORT"
echo "???  Database: $DB_NAME"
echo "?? Backup file: $BACKUP_FILE_GZ"

# Export password for pg_dump
export PGPASSWORD="$DB_PASSWORD"

# Perform backup with pg_dump
pg_dump -h "$DB_HOST" \
        -p "$DB_PORT" \
        -U "$DB_USER" \
        -d "$DB_NAME" \
        --no-owner \
        --no-acl \
        --clean \
        --if-exists \
        -F p \
        -f "$BACKUP_FILE"

# Check if backup was successful
if [ $? -eq 0 ]; then
    # Compress backup
    gzip "$BACKUP_FILE"
    
    # Get file size
    BACKUP_SIZE=$(du -h "$BACKUP_FILE_GZ" | cut -f1)
    
    echo "? Backup completed successfully!"
    echo "?? File: $BACKUP_FILE_GZ"
    echo "?? Size: $BACKUP_SIZE"
    
    # List recent backups
    echo ""
    echo "?? Recent backups:"
    ls -lht "$BACKUP_DIR" | head -6
else
    echo "? Backup failed!"
    rm -f "$BACKUP_FILE" "$BACKUP_FILE_GZ"
    exit 1
fi

# Clean up old backups (keep last 7 days)
echo ""
echo "?? Cleaning up old backups (keeping last 7 days)..."
find "$BACKUP_DIR" -name "*.sql.gz" -type f -mtime +7 -delete
echo "? Cleanup completed!"

# Unset password
unset PGPASSWORD
