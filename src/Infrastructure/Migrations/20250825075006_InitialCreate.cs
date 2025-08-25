using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    role_name = table.Column<string>(type: "VARCHAR2(50 CHAR)", nullable: false),
                    role_description = table.Column<string>(type: "VARCHAR2(500 CHAR)", nullable: true),
                    is_system_role = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "seasonal_events",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    event_name = table.Column<string>(type: "VARCHAR2(100)", nullable: false),
                    event_type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    description = table.Column<string>(type: "VARCHAR2(4000 CHAR)", nullable: true),
                    start_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    end_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    location = table.Column<string>(type: "VARCHAR2(100)", nullable: true),
                    budget = table.Column<decimal>(type: "NUMBER(12,2)", nullable: true),
                    max_capacity = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ticket_price = table.Column<decimal>(type: "NUMBER(8,2)", nullable: true),
                    status = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seasonal_events", x => x.event_id);
                    table.CheckConstraint("CK_seasonal_events_event_type_Enum", "\"event_type\" BETWEEN 0 AND 3");
                    table.CheckConstraint("CK_seasonal_events_status_Enum", "\"status\" BETWEEN 0 AND 3");
                });

            migrationBuilder.CreateTable(
                name: "ticket_types",
                columns: table => new
                {
                    ticket_type_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    type_name = table.Column<string>(type: "VARCHAR2(100 CHAR)", nullable: false),
                    description = table.Column<string>(type: "VARCHAR2(4000 CHAR)", nullable: true),
                    base_price = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    rules_text = table.Column<string>(type: "VARCHAR2(4000 CHAR)", nullable: true),
                    max_sale_limit = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    applicable_crowd = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_types", x => x.ticket_type_id);
                    table.CheckConstraint("CK_ticket_types_applicable_crowd_Enum", "\"applicable_crowd\" BETWEEN 0 AND 4");
                    table.CheckConstraint("CK_ticket_types_base_price_Range", "\"base_price\" >= 0.0");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    username = table.Column<string>(type: "VARCHAR2(50 CHAR)", nullable: false),
                    password_hash = table.Column<string>(type: "VARCHAR2(256 CHAR)", nullable: false),
                    email = table.Column<string>(type: "VARCHAR2(100 CHAR)", nullable: false),
                    display_name = table.Column<string>(type: "VARCHAR2(50 CHAR)", nullable: false),
                    phone_number = table.Column<string>(type: "VARCHAR2(20 CHAR)", nullable: true),
                    birth_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    gender = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    register_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    permission_level = table.Column<byte>(type: "NUMBER(2)", nullable: false, defaultValue: (byte)0),
                    role_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.CheckConstraint("CK_users_gender_Enum", "\"gender\" IN (0, 1)");
                    table.CheckConstraint("CK_users_permission_level_Range", "\"permission_level\" BETWEEN 0 AND 4");
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "visitors",
                columns: table => new
                {
                    visitor_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    visitor_type = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    points = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    member_level = table.Column<string>(type: "VARCHAR2(30 CHAR)", nullable: true),
                    member_since = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    is_blacklisted = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    height = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visitors", x => x.visitor_id);
                    table.CheckConstraint("CK_visitors_height_Range", "\"height\" BETWEEN 50 AND 300");
                    table.CheckConstraint("CK_visitors_points_Range", "\"points\" BETWEEN 0 AND 2147483647");
                    table.CheckConstraint("CK_visitors_visitor_type_Enum", "\"visitor_type\" IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_visitors_users_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "blacklist",
                columns: table => new
                {
                    visitor_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    blacklist_reason = table.Column<string>(type: "VARCHAR2(500 CHAR)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blacklist", x => x.visitor_id);
                    table.ForeignKey(
                        name: "FK_blacklist_visitors_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "visitors",
                        principalColumn: "visitor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "amusement_rides",
                columns: table => new
                {
                    ride_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ride_name = table.Column<string>(type: "VARCHAR2(100 CHAR)", nullable: false),
                    manager_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    location = table.Column<string>(type: "VARCHAR2(100 CHAR)", nullable: false),
                    description = table.Column<string>(type: "VARCHAR2(4000 CHAR)", nullable: true),
                    ride_status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    capacity = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    duration = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    height_limit_min = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    height_limit_max = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    open_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amusement_rides", x => x.ride_id);
                    table.CheckConstraint("CK_amusement_rides_capacity_Range", "\"capacity\" BETWEEN 1 AND 2147483647");
                    table.CheckConstraint("CK_amusement_rides_duration_Range", "\"duration\" BETWEEN 1 AND 2147483647");
                    table.CheckConstraint("CK_amusement_rides_height_limit_max_Range", "\"height_limit_max\" BETWEEN 50 AND 300");
                    table.CheckConstraint("CK_amusement_rides_height_limit_min_Range", "\"height_limit_min\" BETWEEN 50 AND 300");
                    table.CheckConstraint("CK_amusement_rides_ride_status_Enum", "\"ride_status\" BETWEEN 0 AND 3");
                });

            migrationBuilder.CreateTable(
                name: "ride_traffic_stats",
                columns: table => new
                {
                    ride_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    record_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    visitor_count = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    queue_length = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    waiting_time = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    is_crowded = table.Column<bool>(type: "NUMBER(1)", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ride_traffic_stats", x => new { x.ride_id, x.record_time });
                    table.ForeignKey(
                        name: "FK_ride_traffic_stats_amusement_rides_ride_id",
                        column: x => x.ride_id,
                        principalTable: "amusement_rides",
                        principalColumn: "ride_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attendances",
                columns: table => new
                {
                    attendance_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    employee_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    attendance_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    check_in_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    check_out_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    attendance_status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    leave_type = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendances", x => x.attendance_id);
                    table.CheckConstraint("CK_attendances_attendance_status_Enum", "\"attendance_status\" BETWEEN 0 AND 3");
                    table.CheckConstraint("CK_attendances_leave_type_Enum", "\"leave_type\" BETWEEN 0 AND 3");
                });

            migrationBuilder.CreateTable(
                name: "coupons",
                columns: table => new
                {
                    coupon_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    coupon_code = table.Column<string>(type: "VARCHAR2(50 CHAR)", nullable: false),
                    promotion_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    discount_type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    discount_value = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    min_purchase_amount = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false, defaultValue: 0m),
                    valid_from = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    valid_to = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    is_used = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    used_by = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    used_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupons", x => x.coupon_id);
                    table.CheckConstraint("CK_coupons_discount_type_Enum", "\"discount_type\" IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_coupons_visitors_used_by",
                        column: x => x.used_by,
                        principalTable: "visitors",
                        principalColumn: "visitor_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "employee_reviews",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    employee_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    period = table.Column<string>(type: "VARCHAR2(7)", nullable: false),
                    score = table.Column<decimal>(type: "NUMBER(5,2)", nullable: false),
                    evaluation_level = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    evaluator_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_reviews", x => x.review_id);
                    table.CheckConstraint("CK_employee_reviews_evaluation_level_Enum", "\"evaluation_level\" BETWEEN 0 AND 3");
                    table.CheckConstraint("CK_employee_reviews_score_Range", "\"score\" BETWEEN 0.0 AND 100.0");
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    staff_number = table.Column<string>(type: "VARCHAR2(20 CHAR)", nullable: false),
                    position = table.Column<string>(type: "VARCHAR2(50 CHAR)", nullable: false),
                    department_name = table.Column<string>(type: "VARCHAR2(50 CHAR)", nullable: true),
                    staff_type = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    team_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    hire_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    employment_status = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    manager_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    certification = table.Column<string>(type: "VARCHAR2(500 CHAR)", nullable: true),
                    responsibility_area = table.Column<string>(type: "VARCHAR2(100 CHAR)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.employee_id);
                    table.CheckConstraint("CK_employees_employment_status_Enum", "\"employment_status\" BETWEEN 0 AND 2");
                    table.CheckConstraint("CK_employees_staff_type_Enum", "\"staff_type\" BETWEEN 0 AND 3");
                    table.ForeignKey(
                        name: "FK_employees_employees_manager_id",
                        column: x => x.manager_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employees_users_employee_id",
                        column: x => x.employee_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "financial_records",
                columns: table => new
                {
                    record_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    transaction_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    amount = table.Column<decimal>(type: "NUMBER(12,2)", nullable: false),
                    transaction_type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    payment_method = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    responsible_employee_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    approved_by_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_records", x => x.record_id);
                    table.CheckConstraint("CK_financial_records_payment_method_Enum", "\"payment_method\" BETWEEN 0 AND 3");
                    table.CheckConstraint("CK_financial_records_transaction_type_Enum", "\"transaction_type\" BETWEEN 0 AND 3");
                    table.ForeignKey(
                        name: "FK_financial_records_employees_approved_by_id",
                        column: x => x.approved_by_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_financial_records_employees_responsible_employee_id",
                        column: x => x.responsible_employee_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "price_rules",
                columns: table => new
                {
                    price_rule_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ticket_type_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    rule_name = table.Column<string>(type: "VARCHAR2(100 CHAR)", nullable: false),
                    priority = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    effective_start_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    effective_end_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    min_quantity = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    max_quantity = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    price = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    created_by = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_rules", x => x.price_rule_id);
                    table.CheckConstraint("CK_price_rules_price_Range", "\"price\" >= 0");
                    table.ForeignKey(
                        name: "FK_price_rules_employees_created_by",
                        column: x => x.created_by,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_price_rules_ticket_types_ticket_type_id",
                        column: x => x.ticket_type_id,
                        principalTable: "ticket_types",
                        principalColumn: "ticket_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promotions",
                columns: table => new
                {
                    promotion_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    promotion_name = table.Column<string>(type: "VARCHAR2(100 CHAR)", nullable: false),
                    promotion_type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    description = table.Column<string>(type: "VARCHAR2(4000 CHAR)", nullable: true),
                    start_datetime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    end_datetime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    usage_limit_per_user = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    total_usage_limit = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    current_usage_count = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    display_priority = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 100),
                    applies_to_all_tickets = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: true),
                    is_combinable = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    employee_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotions", x => x.promotion_id);
                    table.CheckConstraint("CK_promotions_promotion_type_Enum", "\"promotion_type\" BETWEEN 0 AND 5");
                    table.ForeignKey(
                        name: "FK_promotions_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "salary_records",
                columns: table => new
                {
                    salary_record_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    employee_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    pay_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    salary = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_records", x => x.salary_record_id);
                    table.CheckConstraint("CK_salary_records_salary_Range", "\"salary\" >= 0");
                    table.ForeignKey(
                        name: "FK_salary_records_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "staff_teams",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    team_name = table.Column<string>(type: "VARCHAR2(50 CHAR)", nullable: false),
                    team_type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    leader_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_teams", x => x.team_id);
                    table.CheckConstraint("CK_staff_teams_team_type_Enum", "\"team_type\" BETWEEN 0 AND 2");
                    table.ForeignKey(
                        name: "FK_staff_teams_employees_leader_id",
                        column: x => x.leader_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "price_histories",
                columns: table => new
                {
                    price_history_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ticket_type_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    price_rule_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    old_price = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    new_price = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    change_datetime = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    employee_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    reason = table.Column<string>(type: "VARCHAR2(500 CHAR)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_histories", x => x.price_history_id);
                    table.ForeignKey(
                        name: "FK_price_histories_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_price_histories_price_rules_price_rule_id",
                        column: x => x.price_rule_id,
                        principalTable: "price_rules",
                        principalColumn: "price_rule_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_price_histories_ticket_types_ticket_type_id",
                        column: x => x.ticket_type_id,
                        principalTable: "ticket_types",
                        principalColumn: "ticket_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "promotion_actions",
                columns: table => new
                {
                    action_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    promotion_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    action_name = table.Column<string>(type: "VARCHAR2(100 CHAR)", nullable: false),
                    action_type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    target_ticket_type_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    discount_percentage = table.Column<decimal>(type: "NUMBER(5,2)", nullable: true),
                    discount_amount = table.Column<decimal>(type: "NUMBER(10,2)", nullable: true),
                    fixed_price = table.Column<decimal>(type: "NUMBER(10,2)", nullable: true),
                    points_awarded = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    free_ticket_type_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    free_ticket_quantity = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotion_actions", x => x.action_id);
                    table.CheckConstraint("CK_promotion_actions_action_type_Enum", "\"action_type\" BETWEEN 0 AND 4");
                    table.CheckConstraint("CK_promotion_actions_discount_amount_Range", "\"discount_amount\" >= 0.0");
                    table.CheckConstraint("CK_promotion_actions_discount_percentage_Range", "\"discount_percentage\" BETWEEN 0.0 AND 100.0");
                    table.CheckConstraint("CK_promotion_actions_fixed_price_Range", "\"fixed_price\" >= 0.0");
                    table.CheckConstraint("CK_promotion_actions_free_ticket_quantity_Range", "\"free_ticket_quantity\" BETWEEN 1 AND 2147483647");
                    table.CheckConstraint("CK_promotion_actions_points_awarded_Range", "\"points_awarded\" BETWEEN 0 AND 2147483647");
                    table.ForeignKey(
                        name: "FK_promotion_actions_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "promotions",
                        principalColumn: "promotion_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_promotion_actions_ticket_types_free_ticket_type_id",
                        column: x => x.free_ticket_type_id,
                        principalTable: "ticket_types",
                        principalColumn: "ticket_type_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_promotion_actions_ticket_types_target_ticket_type_id",
                        column: x => x.target_ticket_type_id,
                        principalTable: "ticket_types",
                        principalColumn: "ticket_type_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "promotion_conditions",
                columns: table => new
                {
                    condition_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    promotion_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    condition_name = table.Column<string>(type: "VARCHAR2(100 CHAR)", nullable: false),
                    condition_type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ticket_type_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    min_quantity = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    min_amount = table.Column<decimal>(type: "NUMBER(10,2)", nullable: true),
                    visitor_type = table.Column<string>(type: "VARCHAR2(30 CHAR)", nullable: true),
                    member_level = table.Column<string>(type: "VARCHAR2(30 CHAR)", nullable: true),
                    date_from = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    date_to = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    day_of_week = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    priority = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 10),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotion_conditions", x => x.condition_id);
                    table.CheckConstraint("CK_promotion_conditions_condition_type_Enum", "\"condition_type\" BETWEEN 0 AND 6");
                    table.ForeignKey(
                        name: "FK_promotion_conditions_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "promotions",
                        principalColumn: "promotion_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_promotion_conditions_ticket_types_ticket_type_id",
                        column: x => x.ticket_type_id,
                        principalTable: "ticket_types",
                        principalColumn: "ticket_type_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "promotion_ticket_types",
                columns: table => new
                {
                    promotion_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ticket_type_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promotion_ticket_types", x => new { x.promotion_id, x.ticket_type_id });
                    table.ForeignKey(
                        name: "FK_promotion_ticket_types_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "promotions",
                        principalColumn: "promotion_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_promotion_ticket_types_ticket_types_ticket_type_id",
                        column: x => x.ticket_type_id,
                        principalTable: "ticket_types",
                        principalColumn: "ticket_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    reservation_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    visitor_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    reservation_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    visit_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    discount_amount = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false, defaultValue: 0m),
                    total_amount = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    payment_status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    payment_method = table.Column<string>(type: "VARCHAR2(30 CHAR)", nullable: true),
                    promotion_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    special_requests = table.Column<string>(type: "VARCHAR2(500 CHAR)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservations", x => x.reservation_id);
                    table.CheckConstraint("CK_reservations_discount_amount_Range", "\"discount_amount\" >= 0");
                    table.CheckConstraint("CK_reservations_payment_status_Enum", "\"payment_status\" BETWEEN 0 AND 3");
                    table.CheckConstraint("CK_reservations_status_Enum", "\"status\" BETWEEN 0 AND 3");
                    table.CheckConstraint("CK_reservations_total_amount_Range", "\"total_amount\" >= 0");
                    table.ForeignKey(
                        name: "FK_reservations_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "promotions",
                        principalColumn: "promotion_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_reservations_visitors_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "visitors",
                        principalColumn: "visitor_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inspection_records",
                columns: table => new
                {
                    inspection_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ride_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    team_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    check_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    check_type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    is_passed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    issues_found = table.Column<string>(type: "VARCHAR2(4000 CHAR)", nullable: true),
                    recommendations = table.Column<string>(type: "VARCHAR2(4000 CHAR)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspection_records", x => x.inspection_id);
                    table.CheckConstraint("CK_inspection_records_check_type_Enum", "\"check_type\" BETWEEN 0 AND 3");
                    table.ForeignKey(
                        name: "FK_inspection_records_amusement_rides_ride_id",
                        column: x => x.ride_id,
                        principalTable: "amusement_rides",
                        principalColumn: "ride_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inspection_records_staff_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "staff_teams",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_records",
                columns: table => new
                {
                    maintenance_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ride_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    team_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    manager_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    maintenance_type = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    start_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    end_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    cost = table.Column<decimal>(type: "NUMBER(12,2)", nullable: false),
                    parts_replaced = table.Column<string>(type: "VARCHAR2(4000 CHAR)", nullable: true),
                    maintenance_details = table.Column<string>(type: "VARCHAR2(4000 CHAR)", nullable: true),
                    is_completed = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    is_accepted = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    acceptance_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    acceptance_comments = table.Column<string>(type: "VARCHAR2(1000)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maintenance_records", x => x.maintenance_id);
                    table.CheckConstraint("CK_maintenance_records_cost_Range", "\"cost\" >= 0");
                    table.CheckConstraint("CK_maintenance_records_maintenance_type_Enum", "\"maintenance_type\" BETWEEN 0 AND 3");
                    table.ForeignKey(
                        name: "FK_maintenance_records_amusement_rides_ride_id",
                        column: x => x.ride_id,
                        principalTable: "amusement_rides",
                        principalColumn: "ride_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_maintenance_records_employees_manager_id",
                        column: x => x.manager_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_maintenance_records_staff_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "staff_teams",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_members",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    employee_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    join_date = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_members", x => new { x.team_id, x.employee_id });
                    table.ForeignKey(
                        name: "FK_team_members_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_team_members_staff_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "staff_teams",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation_items",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    reservation_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ticket_type_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    quantity = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    unit_price = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    applied_price_rule_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    discount_amount = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false, defaultValue: 0m),
                    total_amount = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservation_items", x => x.item_id);
                    table.CheckConstraint("CK_reservation_items_discount_amount_Range", "\"discount_amount\" >= 0.0");
                    table.CheckConstraint("CK_reservation_items_quantity_Range", "\"quantity\" BETWEEN 1 AND 2147483647");
                    table.CheckConstraint("CK_reservation_items_total_amount_Range", "\"total_amount\" >= 0.0");
                    table.CheckConstraint("CK_reservation_items_unit_price_Range", "\"unit_price\" >= 0.0");
                    table.ForeignKey(
                        name: "FK_reservation_items_price_rules_applied_price_rule_id",
                        column: x => x.applied_price_rule_id,
                        principalTable: "price_rules",
                        principalColumn: "price_rule_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_reservation_items_reservations_reservation_id",
                        column: x => x.reservation_id,
                        principalTable: "reservations",
                        principalColumn: "reservation_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservation_items_ticket_types_ticket_type_id",
                        column: x => x.ticket_type_id,
                        principalTable: "ticket_types",
                        principalColumn: "ticket_type_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    ticket_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    reservation_item_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ticket_type_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    visitor_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    serial_number = table.Column<string>(type: "VARCHAR2(50 CHAR)", nullable: false),
                    valid_from = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    valid_to = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    used_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    ReservationItemItemId = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets", x => x.ticket_id);
                    table.CheckConstraint("CK_tickets_status_Enum", "\"status\" BETWEEN 0 AND 4");
                    table.ForeignKey(
                        name: "FK_tickets_reservation_items_ReservationItemItemId",
                        column: x => x.ReservationItemItemId,
                        principalTable: "reservation_items",
                        principalColumn: "item_id");
                    table.ForeignKey(
                        name: "FK_tickets_reservation_items_reservation_item_id",
                        column: x => x.reservation_item_id,
                        principalTable: "reservation_items",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tickets_ticket_types_ticket_type_id",
                        column: x => x.ticket_type_id,
                        principalTable: "ticket_types",
                        principalColumn: "ticket_type_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tickets_visitors_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "visitors",
                        principalColumn: "visitor_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "entry_records",
                columns: table => new
                {
                    entry_record_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    visitor_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    entry_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    exit_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    entry_gate = table.Column<string>(type: "VARCHAR2(30 CHAR)", nullable: false),
                    exit_gate = table.Column<string>(type: "VARCHAR2(30 CHAR)", nullable: true),
                    ticket_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entry_records", x => x.entry_record_id);
                    table.ForeignKey(
                        name: "FK_entry_records_tickets_ticket_id",
                        column: x => x.ticket_id,
                        principalTable: "tickets",
                        principalColumn: "ticket_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_entry_records_visitors_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "visitors",
                        principalColumn: "visitor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refund_records",
                columns: table => new
                {
                    refund_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ticket_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    visitor_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    refund_amount = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    refund_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    refund_reason = table.Column<string>(type: "VARCHAR2(500 CHAR)", nullable: true),
                    refund_status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    processor_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    processing_notes = table.Column<string>(type: "VARCHAR2(500 CHAR)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refund_records", x => x.refund_id);
                    table.CheckConstraint("CK_refund_records_refund_amount_Range", "\"refund_amount\" >= 0.0");
                    table.CheckConstraint("CK_refund_records_refund_status_Enum", "\"refund_status\" BETWEEN 0 AND 3");
                    table.ForeignKey(
                        name: "FK_refund_records_employees_processor_id",
                        column: x => x.processor_id,
                        principalTable: "employees",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_refund_records_tickets_ticket_id",
                        column: x => x.ticket_id,
                        principalTable: "tickets",
                        principalColumn: "ticket_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_refund_records_visitors_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "visitors",
                        principalColumn: "visitor_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_amusement_rides_manager_id",
                table: "amusement_rides",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_amusement_rides_ride_name",
                table: "amusement_rides",
                column: "ride_name");

            migrationBuilder.CreateIndex(
                name: "IX_amusement_rides_ride_status",
                table: "amusement_rides",
                column: "ride_status");

            migrationBuilder.CreateIndex(
                name: "IX_attendances_attendance_date",
                table: "attendances",
                column: "attendance_date");

            migrationBuilder.CreateIndex(
                name: "IX_attendances_attendance_status",
                table: "attendances",
                column: "attendance_status");

            migrationBuilder.CreateIndex(
                name: "IX_attendances_employee_id",
                table: "attendances",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_coupons_coupon_code",
                table: "coupons",
                column: "coupon_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coupons_is_used",
                table: "coupons",
                column: "is_used");

            migrationBuilder.CreateIndex(
                name: "IX_coupons_promotion_id",
                table: "coupons",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_coupons_used_by",
                table: "coupons",
                column: "used_by");

            migrationBuilder.CreateIndex(
                name: "IX_coupons_valid_from",
                table: "coupons",
                column: "valid_from");

            migrationBuilder.CreateIndex(
                name: "IX_coupons_valid_to",
                table: "coupons",
                column: "valid_to");

            migrationBuilder.CreateIndex(
                name: "IX_employee_reviews_employee_id",
                table: "employee_reviews",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_reviews_evaluator_id",
                table: "employee_reviews",
                column: "evaluator_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_reviews_period",
                table: "employee_reviews",
                column: "period");

            migrationBuilder.CreateIndex(
                name: "IX_employees_department_name",
                table: "employees",
                column: "department_name");

            migrationBuilder.CreateIndex(
                name: "IX_employees_manager_id",
                table: "employees",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_employees_staff_number",
                table: "employees",
                column: "staff_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_team_id",
                table: "employees",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_entry_records_entry_time",
                table: "entry_records",
                column: "entry_time");

            migrationBuilder.CreateIndex(
                name: "IX_entry_records_exit_time",
                table: "entry_records",
                column: "exit_time");

            migrationBuilder.CreateIndex(
                name: "IX_entry_records_ticket_id",
                table: "entry_records",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_entry_records_visitor_id",
                table: "entry_records",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "IX_financial_records_approved_by_id",
                table: "financial_records",
                column: "approved_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_financial_records_responsible_employee_id",
                table: "financial_records",
                column: "responsible_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_financial_records_transaction_date",
                table: "financial_records",
                column: "transaction_date");

            migrationBuilder.CreateIndex(
                name: "IX_financial_records_transaction_type",
                table: "financial_records",
                column: "transaction_type");

            migrationBuilder.CreateIndex(
                name: "IX_inspection_records_check_date",
                table: "inspection_records",
                column: "check_date");

            migrationBuilder.CreateIndex(
                name: "IX_inspection_records_is_passed",
                table: "inspection_records",
                column: "is_passed");

            migrationBuilder.CreateIndex(
                name: "IX_inspection_records_ride_id",
                table: "inspection_records",
                column: "ride_id");

            migrationBuilder.CreateIndex(
                name: "IX_inspection_records_team_id",
                table: "inspection_records",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_records_end_time",
                table: "maintenance_records",
                column: "end_time");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_records_is_accepted",
                table: "maintenance_records",
                column: "is_accepted");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_records_is_completed",
                table: "maintenance_records",
                column: "is_completed");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_records_manager_id",
                table: "maintenance_records",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_records_ride_id",
                table: "maintenance_records",
                column: "ride_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_records_start_time",
                table: "maintenance_records",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_records_team_id",
                table: "maintenance_records",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_price_histories_change_datetime",
                table: "price_histories",
                column: "change_datetime");

            migrationBuilder.CreateIndex(
                name: "IX_price_histories_employee_id",
                table: "price_histories",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_price_histories_price_rule_id",
                table: "price_histories",
                column: "price_rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_price_histories_ticket_type_id",
                table: "price_histories",
                column: "ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_price_rules_created_by",
                table: "price_rules",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_price_rules_effective_start_date",
                table: "price_rules",
                column: "effective_start_date");

            migrationBuilder.CreateIndex(
                name: "IX_price_rules_priority",
                table: "price_rules",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "IX_price_rules_ticket_type_id",
                table: "price_rules",
                column: "ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_actions_action_type",
                table: "promotion_actions",
                column: "action_type");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_actions_free_ticket_type_id",
                table: "promotion_actions",
                column: "free_ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_actions_promotion_id",
                table: "promotion_actions",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_actions_target_ticket_type_id",
                table: "promotion_actions",
                column: "target_ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_conditions_condition_type",
                table: "promotion_conditions",
                column: "condition_type");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_conditions_priority",
                table: "promotion_conditions",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_conditions_promotion_id",
                table: "promotion_conditions",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_conditions_ticket_type_id",
                table: "promotion_conditions",
                column: "ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_ticket_types_ticket_type_id",
                table: "promotion_ticket_types",
                column: "ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_applies_to_all_tickets",
                table: "promotions",
                column: "applies_to_all_tickets");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_display_priority",
                table: "promotions",
                column: "display_priority");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_employee_id",
                table: "promotions",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_end_datetime",
                table: "promotions",
                column: "end_datetime");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_is_active",
                table: "promotions",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_promotion_name",
                table: "promotions",
                column: "promotion_name");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_promotion_type",
                table: "promotions",
                column: "promotion_type");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_start_datetime",
                table: "promotions",
                column: "start_datetime");

            migrationBuilder.CreateIndex(
                name: "IX_refund_records_processor_id",
                table: "refund_records",
                column: "processor_id");

            migrationBuilder.CreateIndex(
                name: "IX_refund_records_refund_status",
                table: "refund_records",
                column: "refund_status");

            migrationBuilder.CreateIndex(
                name: "IX_refund_records_refund_time",
                table: "refund_records",
                column: "refund_time");

            migrationBuilder.CreateIndex(
                name: "IX_refund_records_ticket_id",
                table: "refund_records",
                column: "ticket_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refund_records_visitor_id",
                table: "refund_records",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_items_applied_price_rule_id",
                table: "reservation_items",
                column: "applied_price_rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_items_reservation_id",
                table: "reservation_items",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_items_ticket_type_id",
                table: "reservation_items",
                column: "ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_payment_status",
                table: "reservations",
                column: "payment_status");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_promotion_id",
                table: "reservations",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_reservation_time",
                table: "reservations",
                column: "reservation_time");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_status",
                table: "reservations",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_visit_date",
                table: "reservations",
                column: "visit_date");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_visitor_id",
                table: "reservations",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "IX_ride_traffic_stats_is_crowded",
                table: "ride_traffic_stats",
                column: "is_crowded");

            migrationBuilder.CreateIndex(
                name: "IX_roles_role_name",
                table: "roles",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_employee_id",
                table: "salary_records",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_pay_date",
                table: "salary_records",
                column: "pay_date");

            migrationBuilder.CreateIndex(
                name: "IX_seasonal_events_end_date",
                table: "seasonal_events",
                column: "end_date");

            migrationBuilder.CreateIndex(
                name: "IX_seasonal_events_event_name",
                table: "seasonal_events",
                column: "event_name");

            migrationBuilder.CreateIndex(
                name: "IX_seasonal_events_event_type",
                table: "seasonal_events",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "IX_seasonal_events_start_date",
                table: "seasonal_events",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "IX_seasonal_events_status",
                table: "seasonal_events",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_staff_teams_leader_id",
                table: "staff_teams",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_teams_team_type",
                table: "staff_teams",
                column: "team_type");

            migrationBuilder.CreateIndex(
                name: "IX_team_members_employee_id",
                table: "team_members",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_members_team_id",
                table: "team_members",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_types_type_name",
                table: "ticket_types",
                column: "type_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tickets_reservation_item_id",
                table: "tickets",
                column: "reservation_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_ReservationItemItemId",
                table: "tickets",
                column: "ReservationItemItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_serial_number",
                table: "tickets",
                column: "serial_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tickets_ticket_type_id",
                table: "tickets",
                column: "ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_visitor_id",
                table: "tickets",
                column: "visitor_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_created_at",
                table: "users",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_visitors_height",
                table: "visitors",
                column: "height");

            migrationBuilder.CreateIndex(
                name: "IX_visitors_is_blacklisted",
                table: "visitors",
                column: "is_blacklisted");

            migrationBuilder.CreateIndex(
                name: "IX_visitors_visitor_type",
                table: "visitors",
                column: "visitor_type");

            migrationBuilder.AddForeignKey(
                name: "FK_amusement_rides_employees_manager_id",
                table: "amusement_rides",
                column: "manager_id",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_attendances_employees_employee_id",
                table: "attendances",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_coupons_promotions_promotion_id",
                table: "coupons",
                column: "promotion_id",
                principalTable: "promotions",
                principalColumn: "promotion_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_reviews_employees_employee_id",
                table: "employee_reviews",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_reviews_employees_evaluator_id",
                table: "employee_reviews",
                column: "evaluator_id",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_staff_teams_team_id",
                table: "employees",
                column: "team_id",
                principalTable: "staff_teams",
                principalColumn: "team_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_staff_teams_employees_leader_id",
                table: "staff_teams");

            migrationBuilder.DropTable(
                name: "attendances");

            migrationBuilder.DropTable(
                name: "blacklist");

            migrationBuilder.DropTable(
                name: "coupons");

            migrationBuilder.DropTable(
                name: "employee_reviews");

            migrationBuilder.DropTable(
                name: "entry_records");

            migrationBuilder.DropTable(
                name: "financial_records");

            migrationBuilder.DropTable(
                name: "inspection_records");

            migrationBuilder.DropTable(
                name: "maintenance_records");

            migrationBuilder.DropTable(
                name: "price_histories");

            migrationBuilder.DropTable(
                name: "promotion_actions");

            migrationBuilder.DropTable(
                name: "promotion_conditions");

            migrationBuilder.DropTable(
                name: "promotion_ticket_types");

            migrationBuilder.DropTable(
                name: "refund_records");

            migrationBuilder.DropTable(
                name: "ride_traffic_stats");

            migrationBuilder.DropTable(
                name: "salary_records");

            migrationBuilder.DropTable(
                name: "seasonal_events");

            migrationBuilder.DropTable(
                name: "team_members");

            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "amusement_rides");

            migrationBuilder.DropTable(
                name: "reservation_items");

            migrationBuilder.DropTable(
                name: "price_rules");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "ticket_types");

            migrationBuilder.DropTable(
                name: "promotions");

            migrationBuilder.DropTable(
                name: "visitors");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "staff_teams");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
