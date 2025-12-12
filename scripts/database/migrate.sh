#!/bin/bash

# Database Migration Management Tool
# Usage: ./migrate.sh [command] [options]

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_PROJECT="$SCRIPT_DIR/../../src/my-shop.API"
INFRA_PROJECT="$SCRIPT_DIR/../../src/my-shop.Infrastructure"

# Colors
RED='\033[0,31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

show_help() {
    echo "?? Database Migration Management Tool"
    echo ""
    echo "Usage: ./migrate.sh [command] [options]"
    echo ""
    echo "Commands:"
    echo "  add <name>        Create a new migration"
    echo "  list              List all migrations"
    echo "  apply             Apply pending migrations"
    echo "  rollback <name>   Rollback to specific migration"
    echo "  status            Show migration status"
    echo "  script            Generate SQL script for pending migrations"
    echo "  remove            Remove last migration (if not applied)"
    echo "  help              Show this help message"
    echo ""
    echo "Examples:"
    echo "  ./migrate.sh add AddProductImages"
    echo "  ./migrate.sh list"
    echo "  ./migrate.sh apply"
    echo "  ./migrate.sh status"
}

check_ef_tools() {
    if ! command -v dotnet-ef &> /dev/null; then
        echo -e "${RED}? dotnet-ef tool not found${NC}"
        echo "Installing dotnet-ef globally..."
        dotnet tool install --global dotnet-ef
    fi
}

migrate_add() {
    if [ -z "$1" ]; then
        echo -e "${RED}? Migration name required${NC}"
        echo "Usage: ./migrate.sh add <MigrationName>"
        exit 1
    fi

    MIGRATION_NAME="$1"
    echo -e "${BLUE}?? Creating migration: $MIGRATION_NAME${NC}"
    
    cd "$INFRA_PROJECT"
    dotnet ef migrations add "$MIGRATION_NAME" \
        --startup-project "$API_PROJECT" \
        --output-dir Data/Migrations
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}? Migration created successfully!${NC}"
        echo "?? Location: src/my-shop.Infrastructure/Data/Migrations/"
    else
        echo -e "${RED}? Failed to create migration${NC}"
        exit 1
    fi
}

migrate_list() {
    echo -e "${BLUE}?? Listing all migrations${NC}"
    cd "$INFRA_PROJECT"
    dotnet ef migrations list \
        --startup-project "$API_PROJECT" \
        --no-build
}

migrate_apply() {
    echo -e "${YELLOW}??  Applying pending migrations...${NC}"
    cd "$INFRA_PROJECT"
    dotnet ef database update \
        --startup-project "$API_PROJECT"
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}? Migrations applied successfully!${NC}"
    else
        echo -e "${RED}? Failed to apply migrations${NC}"
        exit 1
    fi
}

migrate_rollback() {
    if [ -z "$1" ]; then
        echo -e "${RED}? Target migration required${NC}"
        echo "Usage: ./migrate.sh rollback <MigrationName>"
        exit 1
    fi

    TARGET="$1"
    echo -e "${YELLOW}??  Rolling back to: $TARGET${NC}"
    echo -e "${RED}WARNING: This will delete data! Make sure you have a backup!${NC}"
    read -p "Are you sure? (yes/no): " CONFIRM
    
    if [ "$CONFIRM" != "yes" ]; then
        echo "? Rollback cancelled"
        exit 0
    fi

    cd "$INFRA_PROJECT"
    dotnet ef database update "$TARGET" \
        --startup-project "$API_PROJECT"
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}? Rollback completed${NC}"
    else
        echo -e "${RED}? Rollback failed${NC}"
        exit 1
    fi
}

migrate_status() {
    echo -e "${BLUE}?? Migration Status${NC}"
    cd "$INFRA_PROJECT"
    
    echo ""
    echo -e "${YELLOW}Applied Migrations:${NC}"
    dotnet ef migrations list --startup-project "$API_PROJECT" --no-build 2>/dev/null | grep "Applied"
    
    echo ""
    echo -e "${YELLOW}Pending Migrations:${NC}"
    dotnet ef migrations list --startup-project "$API_PROJECT" --no-build 2>/dev/null | grep "Pending"
}

migrate_script() {
    echo -e "${BLUE}?? Generating SQL script for pending migrations${NC}"
    
    OUTPUT_FILE="$SCRIPT_DIR/../../migration-script.sql"
    
    cd "$INFRA_PROJECT"
    dotnet ef migrations script \
        --startup-project "$API_PROJECT" \
        --idempotent \
        --output "$OUTPUT_FILE"
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}? SQL script generated!${NC}"
        echo "?? File: $OUTPUT_FILE"
    else
        echo -e "${RED}? Failed to generate script${NC}"
        exit 1
    fi
}

migrate_remove() {
    echo -e "${YELLOW}??  Removing last migration...${NC}"
    echo -e "${RED}WARNING: This will delete the last migration file!${NC}"
    read -p "Are you sure? (yes/no): " CONFIRM
    
    if [ "$CONFIRM" != "yes" ]; then
        echo "? Operation cancelled"
        exit 0
    fi

    cd "$INFRA_PROJECT"
    dotnet ef migrations remove \
        --startup-project "$API_PROJECT"
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}? Migration removed${NC}"
    else
        echo -e "${RED}? Failed to remove migration${NC}"
        exit 1
    fi
}

# Main
check_ef_tools

case "$1" in
    add)
        migrate_add "$2"
        ;;
    list)
        migrate_list
        ;;
    apply)
        migrate_apply
        ;;
    rollback)
        migrate_rollback "$2"
        ;;
    status)
        migrate_status
        ;;
    script)
        migrate_script
        ;;
    remove)
        migrate_remove
        ;;
    help|--help|-h|"")
        show_help
        ;;
    *)
        echo -e "${RED}? Unknown command: $1${NC}"
        show_help
        exit 1
        ;;
esac
