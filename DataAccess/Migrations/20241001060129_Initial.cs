using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
	/// <inheritdoc />
	public partial class Initial : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoles", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "CalendarRoles",
				columns: table => new
				{
					CalendarRoleId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CalendarRoles", x => x.CalendarRoleId);
				});

			migrationBuilder.CreateTable(
				name: "Calendars",
				columns: table => new
				{
					CalendarId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CalendarName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Color = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Calendars", x => x.CalendarId);
				});

			migrationBuilder.CreateTable(
				name: "Departments",
				columns: table => new
				{
					DepartmentId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Acronym = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Departments", x => x.DepartmentId);
				});

			migrationBuilder.CreateTable(
				name: "Locations",
				columns: table => new
				{
					LocationID = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Alias = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Building = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					RoomNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
					Street1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
					Street2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
					City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Locations", x => x.LocationID);
				});

			migrationBuilder.CreateTable(
				name: "RecurringRules",
				columns: table => new
				{
					RecurringRuleId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Skip = table.Column<int>(type: "int", nullable: false),
					EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
					WeekdayBitMap = table.Column<byte>(type: "tinyint", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_RecurringRules", x => x.RecurringRuleId);
				});

			migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ToDos",
				columns: table => new
				{
					ToDoId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CalendarId = table.Column<int>(type: "int", nullable: false),
					Completed = table.Column<bool>(type: "bit", nullable: false),
					DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ToDos", x => x.ToDoId);
					table.ForeignKey(
						name: "FK_ToDos_Calendars_CalendarId",
						column: x => x.CalendarId,
						principalTable: "Calendars",
						principalColumn: "CalendarId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
					FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
					LocationId = table.Column<int>(type: "int", nullable: true),
					ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
					PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
					SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
					TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
					AccessFailedCount = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUsers", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetUsers_Locations_LocationId",
						column: x => x.LocationId,
						principalTable: "Locations",
						principalColumn: "LocationID");
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new
				{
					LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					table.ForeignKey(
						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new
				{
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new
				{
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					table.ForeignKey(
						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "CalendarAccesses",
				columns: table => new
				{
					ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					CalendarId = table.Column<int>(type: "int", nullable: false),
					CalendarRoleId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CalendarAccesses", x => new { x.ApplicationUserId, x.CalendarId, x.CalendarRoleId });
					table.ForeignKey(
						name: "FK_CalendarAccesses_AspNetUsers_ApplicationUserId",
						column: x => x.ApplicationUserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_CalendarAccesses_CalendarRoles_CalendarRoleId",
						column: x => x.CalendarRoleId,
						principalTable: "CalendarRoles",
						principalColumn: "CalendarRoleId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_CalendarAccesses_Calendars_CalendarId",
						column: x => x.CalendarId,
						principalTable: "Calendars",
						principalColumn: "CalendarId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "CalendarEvents",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
					Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
					StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_CalendarEvents", x => x.Id);
					table.ForeignKey(
						name: "FK_CalendarEvents_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "Classes",
				columns: table => new
				{
					ClassId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					DepartmentId = table.Column<int>(type: "int", nullable: false),
					InstructorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Building = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
					RoomNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
					CalendarId = table.Column<int>(type: "int", nullable: false),
					CourseNumber = table.Column<int>(type: "int", nullable: false),
					Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					CreditHours = table.Column<int>(type: "int", nullable: false),
					Capacity = table.Column<int>(type: "int", nullable: false),
					StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Classes", x => x.ClassId);
					table.ForeignKey(
						name: "FK_Classes_AspNetUsers_InstructorId",
						column: x => x.InstructorId,
						principalTable: "AspNetUsers",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Classes_Calendars_CalendarId",
						column: x => x.CalendarId,
						principalTable: "Calendars",
						principalColumn: "CalendarId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Classes_Departments_DepartmentId",
						column: x => x.DepartmentId,
						principalTable: "Departments",
						principalColumn: "DepartmentId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Events",
				columns: table => new
				{
					EventId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					CalendarId = table.Column<int>(type: "int", nullable: false),
					Start = table.Column<DateTime>(type: "datetime2", nullable: false),
					End = table.Column<DateTime>(type: "datetime2", nullable: false),
					Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
					Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
					CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					RecurringRuleId = table.Column<int>(type: "int", nullable: true),
					CanDelete = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Events", x => x.EventId);
					table.ForeignKey(
						name: "FK_Events_AspNetUsers_CreatorId",
						column: x => x.CreatorId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Events_Calendars_CalendarId",
						column: x => x.CalendarId,
						principalTable: "Calendars",
						principalColumn: "CalendarId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Events_RecurringRules_RecurringRuleId",
						column: x => x.RecurringRuleId,
						principalTable: "RecurringRules",
						principalColumn: "RecurringRuleId");
				});

			migrationBuilder.CreateTable(
				name: "Assignments",
				columns: table => new
				{
					AssignmentId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					ClassId = table.Column<int>(type: "int", nullable: false),
					ToDoId = table.Column<int>(type: "int", nullable: false),
					Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					DueDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
					Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
					SubmissionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					Points = table.Column<float>(type: "real", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Assignments", x => x.AssignmentId);
					table.ForeignKey(
						name: "FK_Assignments_Classes_ClassId",
						column: x => x.ClassId,
						principalTable: "Classes",
						principalColumn: "ClassId",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Assignments_ToDos_ToDoId",
						column: x => x.ToDoId,
						principalTable: "ToDos",
						principalColumn: "ToDoId");
				});

			migrationBuilder.CreateTable(
				name: "StudentRegistrations",
				columns: table => new
				{
					RegistrationId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ClassId = table.Column<int>(type: "int", nullable: false),
					RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_StudentRegistrations", x => x.RegistrationId);
					table.ForeignKey(
						name: "FK_StudentRegistrations_AspNetUsers_StudentId",
						column: x => x.StudentId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_StudentRegistrations_Classes_ClassId",
						column: x => x.ClassId,
						principalTable: "Classes",
						principalColumn: "ClassId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AssignmentAttachments",
				columns: table => new
				{
					AssignmentAttachmentId = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					AssignmentId = table.Column<int>(type: "int", nullable: false),
					FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AssignmentAttachments", x => x.AssignmentAttachmentId);
					table.ForeignKey(
						name: "FK_AssignmentAttachments_Assignments_AssignmentId",
						column: x => x.AssignmentId,
						principalTable: "Assignments",
						principalColumn: "AssignmentId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_AspNetRoleClaims_RoleId",
				table: "AspNetRoleClaims",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "AspNetRoles",
				column: "NormalizedName",
				unique: true,
				filter: "[NormalizedName] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserClaims_UserId",
				table: "AspNetUserClaims",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserLogins_UserId",
				table: "AspNetUserLogins",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserRoles_RoleId",
				table: "AspNetUserRoles",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "EmailIndex",
				table: "AspNetUsers",
				column: "NormalizedEmail");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUsers_LocationId",
				table: "AspNetUsers",
				column: "LocationId");

			migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "AspNetUsers",
				column: "NormalizedUserName",
				unique: true,
				filter: "[NormalizedUserName] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_AssignmentAttachments_AssignmentId",
				table: "AssignmentAttachments",
				column: "AssignmentId");

			migrationBuilder.CreateIndex(
				name: "IX_Assignments_ClassId",
				table: "Assignments",
				column: "ClassId");

			migrationBuilder.CreateIndex(
				name: "IX_Assignments_ToDoId",
				table: "Assignments",
				column: "ToDoId");

			migrationBuilder.CreateIndex(
				name: "IX_CalendarAccesses_CalendarId",
				table: "CalendarAccesses",
				column: "CalendarId");

			migrationBuilder.CreateIndex(
				name: "IX_CalendarAccesses_CalendarRoleId",
				table: "CalendarAccesses",
				column: "CalendarRoleId");

			migrationBuilder.CreateIndex(
				name: "IX_CalendarEvents_UserId",
				table: "CalendarEvents",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_Classes_CalendarId",
				table: "Classes",
				column: "CalendarId");

			migrationBuilder.CreateIndex(
				name: "IX_Classes_DepartmentId",
				table: "Classes",
				column: "DepartmentId");

			migrationBuilder.CreateIndex(
				name: "IX_Classes_InstructorId",
				table: "Classes",
				column: "InstructorId");

			migrationBuilder.CreateIndex(
				name: "IX_Events_CalendarId",
				table: "Events",
				column: "CalendarId");

			migrationBuilder.CreateIndex(
				name: "IX_Events_CreatorId",
				table: "Events",
				column: "CreatorId");

			migrationBuilder.CreateIndex(
				name: "IX_Events_RecurringRuleId",
				table: "Events",
				column: "RecurringRuleId");

			migrationBuilder.CreateIndex(
				name: "IX_StudentRegistrations_ClassId",
				table: "StudentRegistrations",
				column: "ClassId");

			migrationBuilder.CreateIndex(
				name: "IX_StudentRegistrations_StudentId",
				table: "StudentRegistrations",
				column: "StudentId");

			migrationBuilder.CreateIndex(
				name: "IX_ToDos_CalendarId",
				table: "ToDos",
				column: "CalendarId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AspNetRoleClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserLogins");

			migrationBuilder.DropTable(
				name: "AspNetUserRoles");

			migrationBuilder.DropTable(
				name: "AspNetUserTokens");

			migrationBuilder.DropTable(
				name: "AssignmentAttachments");

			migrationBuilder.DropTable(
				name: "CalendarAccesses");

			migrationBuilder.DropTable(
				name: "CalendarEvents");

			migrationBuilder.DropTable(
				name: "Events");

			migrationBuilder.DropTable(
				name: "StudentRegistrations");

			migrationBuilder.DropTable(
				name: "AspNetRoles");

			migrationBuilder.DropTable(
				name: "Assignments");

			migrationBuilder.DropTable(
				name: "CalendarRoles");

			migrationBuilder.DropTable(
				name: "RecurringRules");

			migrationBuilder.DropTable(
				name: "Classes");

			migrationBuilder.DropTable(
				name: "ToDos");

			migrationBuilder.DropTable(
				name: "AspNetUsers");

			migrationBuilder.DropTable(
				name: "Departments");

			migrationBuilder.DropTable(
				name: "Calendars");

			migrationBuilder.DropTable(
				name: "Locations");
		}
	}
}
