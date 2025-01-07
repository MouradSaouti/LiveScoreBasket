using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiveScore.Server.Migrations
{
    /// <inheritdoc />
    public partial class hope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EstEquipeDomicile = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coachs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coachs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coachs_Equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Joueurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Taille = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    estCapitaine = table.Column<bool>(type: "bit", nullable: false),
                    estEnJeu = table.Column<bool>(type: "bit", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquipeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Joueurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Joueurs_Equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matchs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateMatch = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EquipeDomicileId = table.Column<int>(type: "int", nullable: false),
                    EquipeExterieurId = table.Column<int>(type: "int", nullable: false),
                    ScoreDomicile = table.Column<int>(type: "int", nullable: false),
                    ScoreExterieur = table.Column<int>(type: "int", nullable: false),
                    CurrentQuarter = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matchs_Equipes_EquipeDomicileId",
                        column: x => x.EquipeDomicileId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchs_Equipes_EquipeExterieurId",
                        column: x => x.EquipeExterieurId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChronoMatchs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    TempsRestant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Etat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChronoMatchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChronoMatchs_Matchs_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CinqsDeBase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    EquipeId = table.Column<int>(type: "int", nullable: false),
                    JoueurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CinqsDeBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CinqsDeBase_Equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CinqsDeBase_Joueurs_JoueurId",
                        column: x => x.JoueurId,
                        principalTable: "Joueurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CinqsDeBase_Matchs_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurationsMatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomMatch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreQuartTemps = table.Column<int>(type: "int", nullable: false),
                    DureeQuartTemps = table.Column<TimeSpan>(type: "time", nullable: false),
                    DureeTimeout = table.Column<TimeSpan>(type: "time", nullable: false),
                    DateHeure = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EquipeDomicileId = table.Column<int>(type: "int", nullable: true),
                    EquipeExterieurId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MatchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationsMatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigurationsMatch_Equipes_EquipeDomicileId",
                        column: x => x.EquipeDomicileId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigurationsMatch_Equipes_EquipeExterieurId",
                        column: x => x.EquipeExterieurId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigurationsMatch_Matchs_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurationsMatch_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvenementsMatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: true),
                    JoueurId = table.Column<int>(type: "int", nullable: true),
                    TypeEvenement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: true),
                    TypeFaute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Temps = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EncodageUserId = table.Column<int>(type: "int", nullable: true),
                    EquipeId = table.Column<int>(type: "int", nullable: true),
                    JoueurSortantId = table.Column<int>(type: "int", nullable: true),
                    JoueurEntrantId = table.Column<int>(type: "int", nullable: true),
                    JoueurVictimeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvenementsMatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvenementsMatch_Equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "Equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvenementsMatch_Joueurs_JoueurId",
                        column: x => x.JoueurId,
                        principalTable: "Joueurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvenementsMatch_Joueurs_JoueurVictimeId",
                        column: x => x.JoueurVictimeId,
                        principalTable: "Joueurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvenementsMatch_Matchs_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvenementsMatch_Users_EncodageUserId",
                        column: x => x.EncodageUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    PlayerOutId = table.Column<int>(type: "int", nullable: false),
                    PlayerInId = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubPlayers_Joueurs_PlayerInId",
                        column: x => x.PlayerInId,
                        principalTable: "Joueurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubPlayers_Joueurs_PlayerOutId",
                        column: x => x.PlayerOutId,
                        principalTable: "Joueurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubPlayers_Matchs_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Timeouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timeouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timeouts_Matchs_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChronoMatchs_MatchId",
                table: "ChronoMatchs",
                column: "MatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CinqsDeBase_EquipeId",
                table: "CinqsDeBase",
                column: "EquipeId");

            migrationBuilder.CreateIndex(
                name: "IX_CinqsDeBase_JoueurId",
                table: "CinqsDeBase",
                column: "JoueurId");

            migrationBuilder.CreateIndex(
                name: "IX_CinqsDeBase_MatchId",
                table: "CinqsDeBase",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Coachs_EquipeId",
                table: "Coachs",
                column: "EquipeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationsMatch_EquipeDomicileId",
                table: "ConfigurationsMatch",
                column: "EquipeDomicileId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationsMatch_EquipeExterieurId",
                table: "ConfigurationsMatch",
                column: "EquipeExterieurId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationsMatch_MatchId",
                table: "ConfigurationsMatch",
                column: "MatchId",
                unique: true,
                filter: "[MatchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationsMatch_UserId",
                table: "ConfigurationsMatch",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EvenementsMatch_EncodageUserId",
                table: "EvenementsMatch",
                column: "EncodageUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EvenementsMatch_EquipeId",
                table: "EvenementsMatch",
                column: "EquipeId");

            migrationBuilder.CreateIndex(
                name: "IX_EvenementsMatch_JoueurId",
                table: "EvenementsMatch",
                column: "JoueurId");

            migrationBuilder.CreateIndex(
                name: "IX_EvenementsMatch_JoueurVictimeId",
                table: "EvenementsMatch",
                column: "JoueurVictimeId");

            migrationBuilder.CreateIndex(
                name: "IX_EvenementsMatch_MatchId",
                table: "EvenementsMatch",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Joueurs_EquipeId",
                table: "Joueurs",
                column: "EquipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_EquipeDomicileId",
                table: "Matchs",
                column: "EquipeDomicileId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_EquipeExterieurId",
                table: "Matchs",
                column: "EquipeExterieurId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPlayers_MatchId",
                table: "SubPlayers",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPlayers_PlayerInId",
                table: "SubPlayers",
                column: "PlayerInId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPlayers_PlayerOutId",
                table: "SubPlayers",
                column: "PlayerOutId");

            migrationBuilder.CreateIndex(
                name: "IX_Timeouts_MatchId",
                table: "Timeouts",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChronoMatchs");

            migrationBuilder.DropTable(
                name: "CinqsDeBase");

            migrationBuilder.DropTable(
                name: "Coachs");

            migrationBuilder.DropTable(
                name: "ConfigurationsMatch");

            migrationBuilder.DropTable(
                name: "EvenementsMatch");

            migrationBuilder.DropTable(
                name: "SubPlayers");

            migrationBuilder.DropTable(
                name: "Timeouts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Joueurs");

            migrationBuilder.DropTable(
                name: "Matchs");

            migrationBuilder.DropTable(
                name: "Equipes");
        }
    }
}
