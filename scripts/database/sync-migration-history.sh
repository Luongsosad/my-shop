#!/bin/bash

# Sync Migration History Script
# Use this to manually sync migration history when migrating from EnsureCreated()

if [ -f "../../.env" ]; then
    export $(cat ../../.env | grep -v '^#' | xargs)
fi

if [ -z "$DATABASE_URL" ]; then
    echo "? ERROR: DATABASE_URL not found"
    exit 1
fi

echo "?? Syncing migration history..."
echo "??  This will mark InitialCreate as applied"
echo ""
read -p "Continue? (yes/no): " CONFIRM

if [ "$CONFIRM" != "yes" ]; then
    echo "? Cancelled"
    exit 0
fi

# Parse DATABASE_URL
DB_URL_REGEX="postgresql://([^:]+):([^@]+)@([^:]+):([^/]+)/([^?]+)"
if [[ $DATABASE_URL =~ $DB_URL_REGEX ]]; then
    DB_USER="${BASH_REMATCH[1]}"
    DB_PASSWORD="${BASH_REMATCH[2]}"
    DB_HOST="${BASH_REMATCH[3]}"
    DB_PORT="${BASH_REMATCH[4]}"
    DB_NAME="${BASH_REMATCH[5]}"
fi

export PGPASSWORD="$DB_PASSWORD"

psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f sync-migration-history.sql

if [ $? -eq 0 ]; then
    echo "? Migration history synced!"
else
    echo "? Failed to sync"
    exit 1
fi

unset PGPASSWORD
