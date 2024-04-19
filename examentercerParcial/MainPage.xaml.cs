using Plugin.Media.Abstractions;
using Plugin.Media;
using System.IO;
using System.Text;
using Plugin.AudioRecorder;
using Firebase.Database;
using examentercerParcial.models;
using Firebase.Database.Query;

namespace examentercerParcial
{
	public partial class MainPage : ContentPage
	{
		FirebaseClient client = new FirebaseClient("https://examenfirebase-ec0bf-default-rtdb.firebaseio.com/");
		private byte[] photoBytes;
		byte[] audi;
		string filePath;
		AudioRecorderService recorder = new AudioRecorderService();

		public MainPage()
		{
			InitializeComponent();
			recorder.TotalAudioTimeout = TimeSpan.FromSeconds(10);
			recorder.StopRecordingOnSilence = false;
		}

		private async void TomarFoto_Clicked(object sender, EventArgs e)
		{
			var photo = await CrossMedia.Current.PickPhotoAsync();

			if (photo != null)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					await photo.GetStream().CopyToAsync(ms);
					photoBytes = ms.ToArray();
				}

				ImagenPreview.Source = ImageSource.FromStream(() => new MemoryStream(photoBytes));
			}
		}

		private async void Grabar_Clicked(object sender, EventArgs e)
		{
			var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
			if (status != PermissionStatus.Granted)
			{
				status = await Permissions.RequestAsync<Permissions.Microphone>();
				if (status != PermissionStatus.Granted)
				{
					await DisplayAlert("Permiso Requerido", "Se requieren permisos de micrófono para grabar audio.", "Aceptar");
					return;
				}
			}
			else
			{
				await recorder.StartRecording();
				Grabar.IsEnabled = false;
				Stop.IsEnabled = true;
			}
		}

		private async void Stop_Clicked(object sender, EventArgs e)
		{
			await recorder.StopRecording();
			filePath = recorder.GetAudioFilePath();

			if (!string.IsNullOrEmpty(filePath))
			{
				audi = File.ReadAllBytes(filePath);
			}
			else
			{
				// Manejar el caso en el que filePath es nulo o vacío
			}

			Grabar.IsEnabled = true;
			Stop.IsEnabled = false;
		}

		private async void GuardarDatosClick(object sender, EventArgs e)
		{
			if (photoBytes == null)
			{
				await DisplayAlert("Error", "Debes tomar una foto primero.", "Aceptar");
				return;
			}

			var tarea = new tareas
			{
				descripcion = DescripcionEntry.Text,
				fecha = Recuerdo.Date,
				Photo_Record = photoBytes,
				Audio_Record = audi
			};

			await client.Child("tareas").PostAsync(tarea);

			DescripcionEntry.Text = string.Empty;
			Recuerdo.Date = DateTime.Now;
			ImagenPreview.Source = null;

			await DisplayAlert("Éxito", "Datos guardados correctamente", "Aceptar");
		}
		private async void VerTareas_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new listaTareas()); // Esto te llevará a la página listaTareas
		}

	}
}
