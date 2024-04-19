using examentercerParcial.models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;


namespace examentercerParcial
{
	public partial class listaTareas : ContentPage
	{
		FirebaseClient client = new FirebaseClient("https://examenfirebase-ec0bf-default-rtdb.firebaseio.com/");

		public listaTareas()
		{
			InitializeComponent();
			BindingContext = this;
			LoadTareas();
		}

		public ObservableCollection<tareas> Tareas { get; set; } = new ObservableCollection<tareas>();

		private async void LoadTareas()
		{
			var tareas = await client.Child("tareas").OnceAsync<tareas>();

			foreach (var tarea in tareas)
			{
				Tareas.Add(tarea.Object);
			}
		}
		private async void DescripcionEntry_TextChanged(object sender, TextChangedEventArgs e)
		{
			// Obtiene el Entry que desencadenó el evento
			Entry entry = sender as Entry;

			// Obtiene la tarea correspondiente al Entry
			tareas tarea = entry.BindingContext as tareas;

			// Actualiza la descripción de la tarea
			tarea.descripcion = e.NewTextValue;

			// Guarda los cambios en Firebase
			await client.Child("tareas").Child(tarea.Id.ToString()).PutAsync(tarea);
		}
		private async void Eliminar_Clicked(object sender, EventArgs e)
		{
			// Obtiene la tarea correspondiente al botón
			Button button = sender as Button;
			tareas tarea = button.CommandParameter as tareas;

			if (tarea != null)
			{
				// Elimina la tarea de la lista
				Tareas.Remove(tarea);

				// Elimina la tarea de Firebase
				await client.Child("tareas").Child(tarea.Id.ToString()).DeleteAsync();
			}
		}


		private async void CrearTarea_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync(); // Esto te llevará de vuelta a la MainPage
		}

	}
}
