using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Tasky.Datos.EF;

#nullable disable

namespace Tasky.Datos.Migrations
{
    public partial class addTaskyTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "GoogleSessions",
               columns: table => new
               {
                   id = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                   UserId =  table.Column<string>(type: "nvarchar(450)", nullable: false),

               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_GoogleSession", x => x.id);
                   table.ForeignKey(
                       name: "FK_GoogleSession_AspNetUsers",
                       column: x => x.UserId,
                       principalTable: "AspNetUsers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                 
               });

            migrationBuilder.CreateTable(
                name: "TablerosTrellos",
                columns: table => new
                {
                    IdTablero = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrelloBoardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TablerosTrella", x => x.IdTablero);
                });

           

        

            migrationBuilder.CreateTable(
                name: "ListasTrellos",
                columns: table => new
                {
                    IdLista = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrelloListId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TableroId = table.Column<int>(type: "int", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasTrellos", x => x.IdLista);
                    table.ForeignKey(
                        name: "FK_ListasTrellos_TablerosTrella_TableroId",
                        column: x => x.TableroId,
                        principalTable: "TablerosTrellos",
                        principalColumn: "IdTablero",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskyObjects",
                columns: table => new
                {
                    IdObject = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Subjectt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Spam = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<TaskyPriority>(type: "int", nullable: false),
                    Delete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskyObjects", x => x.IdObject);
                });

            migrationBuilder.CreateTable(
                name: "EventosCalendars",
                columns: table => new
                {
                    IdEventoCalendar = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoogleEventId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ZonaHoraria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Localizacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sincronizado = table.Column<bool>(type: "bit", nullable: false),
                    IdObject = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosCalendars", x => x.IdEventoCalendar);
                    table.ForeignKey(
                        name: "FK_EventosCalendars_TaskyObjects_IdObject",
                        column: x => x.IdObject,
                        principalTable: "TaskyObjects",
                        principalColumn: "IdObject",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TareasTrellos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrelloCardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdLista = table.Column<int>(type: "int", nullable: true),
                    IdObject = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreadoEn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareasTrella", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TareasTrella_ListasTrellos_IdLista",
                        column: x => x.IdLista,
                        principalTable: "ListasTrellos",
                        principalColumn: "IdLista",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TareasTrella_TaskyObjects_IdObject",
                        column: x => x.IdObject,
                        principalTable: "TaskyObjects",
                        principalColumn: "IdObject",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventosCalendars_IdObject",
                table: "EventosCalendars",
                column: "IdObject");

            migrationBuilder.CreateIndex(
                name: "IX_ListasTrellos_TableroId",
                table: "ListasTrellos",
                column: "TableroId");

            migrationBuilder.CreateIndex(
                name: "IX_TareasTrella_IdLista",
                table: "TareasTrellos",
                column: "IdLista");

            migrationBuilder.CreateIndex(
                name: "IX_TareasTrella_IdObject",
                table: "TareasTrellos",
                column: "IdObject");

            migrationBuilder.CreateIndex(
                name: "IX_GoogleSession_UserId",
                table: "GoogleSessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventosCalendars");

            migrationBuilder.DropTable(
                name: "TareasTrellos");

            migrationBuilder.DropTable(
                name: "ListasTrellos");

            migrationBuilder.DropTable(
                name: "TaskyObjects");

            migrationBuilder.DropTable(
                name: "TablerosTrellos");

            migrationBuilder.DropTable(
                name: "GoogleSessions");
        }
    }
}
