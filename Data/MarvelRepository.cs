using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using UcmTrackerApp.Models;

namespace UcmTrackerApp.Data
{
    public class MarvelRepository
    {
        private readonly string _connectionString;

        public MarvelRepository(string dbPath = "ucm_tracker.db")
        {
            _connectionString = $"Data Source={dbPath}";
            InicializarBaseDeDatos();
        }

        private void InicializarBaseDeDatos()
        {
            using var conexion = new SqliteConnection(_connectionString);
            conexion.Open();

            string sqlCrearTabla = @"
                CREATE TABLE IF NOT EXISTS ProduccionesMarvel (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Titulo TEXT NOT NULL,
                    Tipo TEXT NOT NULL,
                    OrdenCronologico INTEGER NOT NULL,
                    Estado TEXT NOT NULL,
                    Calificacion REAL NOT NULL
                );";

            using var comando = new SqliteCommand(sqlCrearTabla, conexion);
            comando.ExecuteNonQuery();
        }

        // CREATE
        public void Agregar(ProduccionMarvel item)
        {
            using var conexion = new SqliteConnection(_connectionString);
            conexion.Open();

            string sql = @"
                INSERT INTO ProduccionesMarvel (Titulo, Tipo, OrdenCronologico, Estado, Calificacion)
                VALUES (@Titulo, @Tipo, @OrdenCronologico, @Estado, @Calificacion);";

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Titulo", item.Titulo);
            comando.Parameters.AddWithValue("@Tipo", item.Tipo);
            comando.Parameters.AddWithValue("@OrdenCronologico", item.OrdenCronologico);
            comando.Parameters.AddWithValue("@Estado", item.Estado);
            comando.Parameters.AddWithValue("@Calificacion", item.Calificacion);
            comando.ExecuteNonQuery();
        }

        // READ
        public List<ProduccionMarvel> ObtenerTodas()
        {
            var lista = new List<ProduccionMarvel>();

            using var conexion = new SqliteConnection(_connectionString);
            conexion.Open();

            string sql = "SELECT Id, Titulo, Tipo, OrdenCronologico, Estado, Calificacion FROM ProduccionesMarvel ORDER BY OrdenCronologico ASC;";

            using var comando = new SqliteCommand(sql, conexion);
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                lista.Add(new ProduccionMarvel(
                    id: lector.GetInt32(0),
                    titulo: lector.GetString(1),
                    tipo: lector.GetString(2),
                    ordenCronologico: lector.GetInt32(3),
                    estado: lector.GetString(4),
                    calificacion: lector.GetDouble(5)
                ));
            }

            return lista;
        }

        // UPDATE
        public void Actualizar(ProduccionMarvel item)
        {
            using var conexion = new SqliteConnection(_connectionString);
            conexion.Open();

            string sql = @"
                UPDATE ProduccionesMarvel
                SET Titulo = @Titulo,
                    Tipo = @Tipo,
                    OrdenCronologico = @OrdenCronologico,
                    Estado = @Estado,
                    Calificacion = @Calificacion
                WHERE Id = @Id;";

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Id", item.Id);
            comando.Parameters.AddWithValue("@Titulo", item.Titulo);
            comando.Parameters.AddWithValue("@Tipo", item.Tipo);
            comando.Parameters.AddWithValue("@OrdenCronologico", item.OrdenCronologico);
            comando.Parameters.AddWithValue("@Estado", item.Estado);
            comando.Parameters.AddWithValue("@Calificacion", item.Calificacion);
            comando.ExecuteNonQuery();
        }

        // DELETE
        public void Eliminar(int id)
        {
            using var conexion = new SqliteConnection(_connectionString);
            conexion.Open();

            string sql = "DELETE FROM ProduccionesMarvel WHERE Id = @Id;";

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@Id", id);
            comando.ExecuteNonQuery();
        }
    }
}