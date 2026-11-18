using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using UcmTrackerApp.Data;
using UcmTrackerApp.Models;

namespace UcmTrackerApp
{
    public partial class MainWindow : Window
    {
        private readonly MarvelRepository _repositorio;
        private ProduccionMarvel? _seleccionado;

        public MainWindow()
        {
            InitializeComponent();
            _repositorio = new MarvelRepository();
            CargarDatos();
        }

        private void CargarDatos()
        {
            ListaProducciones.ItemsSource = _repositorio.ObtenerTodas();
        }

        private void MostrarMensaje(string texto, bool esError = false)
        {
            LblMensaje.Text = texto;
            LblMensaje.Foreground = esError ? Brushes.Salmon : Brushes.LightGreen;
        }

        private bool ValidarFormulario(out ProduccionMarvel modelo)
        {
            modelo = new ProduccionMarvel();

            string titulo = TxtTitulo.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(titulo))
            {
                MostrarMensaje("Error de validación: El título no puede estar vacío.", true);
                return false;
            }

            if (!int.TryParse(TxtOrden.Text?.Trim(), out int orden) || orden <= 0)
            {
                MostrarMensaje("Error de validación: El orden cronológico debe ser un número entero mayor a 0.", true);
                return false;
            }

            string calTexto = (TxtCalificacion.Text?.Trim() ?? "").Replace(',', '.');
            if (!double.TryParse(calTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out double calificacion)
                || calificacion < 0 || calificacion > 10)
            {
                MostrarMensaje("Error de validación: La calificación debe ser un número entre 0.0 y 10.0.", true);
                return false;
            }

            string tipo = (CmbTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Película";
            string estado = (CmbEstado.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Pendiente";

            modelo = new ProduccionMarvel(0, titulo, tipo, orden, estado, calificacion);
            return true;
        }

        private void LimpiarFormulario()
        {
            TxtTitulo.Text = string.Empty;
            TxtOrden.Text = string.Empty;
            TxtCalificacion.Text = string.Empty;
            CmbTipo.SelectedIndex = 0;
            CmbEstado.SelectedIndex = 0;
            _seleccionado = null;
            ListaProducciones.SelectedItem = null;
        }

        private void BtnGuardar_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidarFormulario(out var nuevo)) return;

                _repositorio.Agregar(nuevo);
                CargarDatos();
                LimpiarFormulario();
                MostrarMensaje($"[OK] '{nuevo.Titulo}' registrado correctamente en SQLite.");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al guardar: {ex.Message}", true);
            }
        }

        private void BtnActualizar_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (_seleccionado == null)
                {
                    MostrarMensaje("Selecciona primero una producción de la lista para actualizar.", true);
                    return;
                }

                if (!ValidarFormulario(out var editado)) return;

                editado.Id = _seleccionado.Id;
                _repositorio.Actualizar(editado);
                CargarDatos();
                LimpiarFormulario();
                MostrarMensaje($"[OK] Registro ID #{editado.Id} ('{editado.Titulo}') actualizado correctamente.");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al actualizar: {ex.Message}", true);
            }
        }

        private void BtnEliminar_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (_seleccionado == null)
                {
                    MostrarMensaje("Selecciona primero una producción de la lista para eliminar.", true);
                    return;
                }

                string tituloEliminado = _seleccionado.Titulo;
                _repositorio.Eliminar(_seleccionado.Id);
                CargarDatos();
                LimpiarFormulario();
                MostrarMensaje($"[OK] '{tituloEliminado}' eliminado de la base de datos.");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al eliminar: {ex.Message}", true);
            }
        }

        private void BtnLimpiar_Click(object? sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
            MostrarMensaje("Formulario limpiado.");
        }

        private void ListaProducciones_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (ListaProducciones.SelectedItem is ProduccionMarvel item)
            {
                _seleccionado = item;
                TxtTitulo.Text = item.Titulo;
                TxtOrden.Text = item.OrdenCronologico.ToString();
                TxtCalificacion.Text = item.Calificacion.ToString(CultureInfo.InvariantCulture);

                CmbTipo.SelectedIndex = item.Tipo switch
                {
                    "Película" => 0,
                    "Serie" => 1,
                    "Especial" => 2,
                    _ => 0
                };

                CmbEstado.SelectedIndex = item.Estado switch
                {
                    "Vista" => 0,
                    "En progreso" => 1,
                    "Pendiente" => 2,
                    _ => 0
                };

                MostrarMensaje($"Seleccionado: #{item.Id} - {item.Titulo}. Puedes editar o eliminar.");
            }
        }
    }
}