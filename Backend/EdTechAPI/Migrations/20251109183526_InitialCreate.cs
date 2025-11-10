using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EdTechAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_list",
                columns: table => new
                {
                    course_id = table.Column<string>(type: "TEXT", nullable: false),
                    creator_id = table.Column<string>(type: "TEXT", nullable: false),
                    course_title = table.Column<string>(type: "TEXT", nullable: false),
                    course_image_url = table.Column<string>(type: "TEXT", nullable: false),
                    course_description = table.Column<string>(type: "TEXT", nullable: false),
                    course_content_url = table.Column<string>(type: "TEXT", nullable: false),
                    is_published = table.Column<int>(type: "INTEGER", nullable: false),
                    course_size_bytes = table.Column<int>(type: "INTEGER", nullable: false),
                    course_type = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_list", x => x.course_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_list");
        }
    }
}
