using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PM2E2Grupo1.Models;

namespace PM2E2Grupo1.Controllers
{
    public class LugaresController
    {
        private const string ConnectionString = "Server=localhost;Database=lugares;Integrated Security=true;";


        public async Task<List<Lugar>> GetLugaresAsync()
        {
            List<Lugar> lugares = new List<Lugar>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT id, audio, latitud, longitud, imagen, descripcion FROM lugares";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Lugar lugar = new Lugar
                            {
                                Id = reader.GetInt32(0),
                                Audio = (byte[])reader["audio"],
                                Latitud = reader.GetString(2),
                                Longitud = reader.GetString(3),
                                Imagen = (byte[])reader["imagen"],
                                Descripcion = reader.GetString(5)
                            };
                            lugares.Add(lugar);
                        }
                    }
                }
            }

            return lugares;
        }

        public async Task<Lugar> GetLugarByIdAsync(string url, int id)
        {
            Lugar lugar = null;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT id, audio, latitud, longitud, imagen, descripcion FROM lugares WHERE id = @id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            lugar = new Lugar
                            {
                                Id = reader.GetInt32(0),
                                Audio = (byte[])reader["audio"],
                                Latitud = reader.GetString(2),
                                Longitud = reader.GetString(3),
                                Imagen = (byte[])reader["imagen"],
                                Descripcion = reader.GetString(5)
                            };
                        }
                    }
                }
            }

            return lugar;
        }

        public async Task<bool> InsertLugarAsync(string url, Lugar lugar)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    string query = "INSERT INTO lugares (audio, latitud, longitud, imagen, descripcion) " +
                        "VALUES (@audio, @latitud, @longitud, @imagen, @descripcion)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@audio", lugar.Audio);
                        command.Parameters.AddWithValue("@latitud", lugar.Latitud);
                        command.Parameters.AddWithValue("@longitud", lugar.Longitud);
                        command.Parameters.AddWithValue("@imagen", lugar.Imagen);
                        command.Parameters.AddWithValue("@descripcion", lugar.Descripcion);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateLugarAsync(Lugar lugar)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    string query = "UPDATE lugares SET audio = @audio, latitud = @latitud, longitud = @longitud, " +
                        "imagen = @imagen, descripcion = @descripcion WHERE id = @id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@audio", lugar.Audio);
                        command.Parameters.AddWithValue("@latitud", lugar.Latitud);
                        command.Parameters.AddWithValue("@longitud", lugar.Longitud);
                        command.Parameters.AddWithValue("@imagen", lugar.Imagen);
                        command.Parameters.AddWithValue("@descripcion", lugar.Descripcion);
                        command.Parameters.AddWithValue("@id", lugar.Id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteLugarAsync(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    string query = "DELETE FROM lugares WHERE id = @id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
