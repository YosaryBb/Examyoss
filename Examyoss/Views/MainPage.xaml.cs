using Plugin.AudioRecorder;
using Plugin.Media;
using System;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Examyoss.Model;
using Examyoss.ViewModel;

namespace Examyoss
{
    public partial class MainPage : ContentPage
    {
        public String nombre;
        int audio = 0;
        int min, seg;
        bool Grabando;
        bool GraAudio = false;
        byte[] ImagenSave;

        public readonly AudioRecorderService audioRecorderService = new AudioRecorderService()
        {
            StopRecordingAfterTimeout = false,
            TotalAudioTimeout = TimeSpan.FromSeconds(180)
        };

        public readonly AudioPlayer audioPlayer = new AudioPlayer();

        public MainPage()
        {
            InitializeComponent();
            descripcion.Text = "";
            UbiImagen.Source = null;
            nombre = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DateTime.Now.ToString("ddMMyyyymmss").Trim() + "Audio_.wav");
        }

        private async void btnguardar_Clicked(object sender, EventArgs e)
        {
            
                    if (String.IsNullOrEmpty(descripcion.Text) || descripcion.Text.Length > 100 || lblstatus.Text == "¡No hay grabación!" || ImagenSave == null)
                    {
                        await DisplayAlert("Advertencia", "¡Hay uno o mas campos vacios!", "Ok");
                    }
                    else
                    {
                        string pathBase64Imagen = Convert.ToBase64String(ImagenSave);
                        string audio = nombre;
                        byte[] fileByte = System.IO.File.ReadAllBytes(audio);
                        string pathBase64Audio = Convert.ToBase64String(fileByte);
                        Notas notas = new Notas
                        {
                            Descripcion = descripcion.Text,
                            Foto = pathBase64Imagen,
                            Audio = pathBase64Audio,
                            Fecha = DateTime.Today
                        };

                    NotasModelView notasModelView = new NotasModelView();

                    var respuesta = await notasModelView.insertarNotas(notas);

                if (respuesta)
                {

                    await DisplayAlert("Notificación", "¡Los datos se guardaron satisfactoriamente!", "ok");

                    descripcion.Text = "";
                    ImagenSave = null;
                    //AudioSave = null;
                    lblstatus.Text = "¡No hay grabación!";
                    UbiImagen.Source = null;
                    lblmin.Text = "00";
                    lblseg.Text = "00";
                }
                else
                {
                    await DisplayAlert("Error", "No se guardaron los datos", "ok");
                }
            }
        }

        private async void btnDetener_Clicked(object sender, EventArgs e)
        {
            Grabar.IsEnabled = true;
            btnDetener.IsEnabled = false;

            Grabando = false;
            lblmin.Text = "00";
            lblseg.Text = "00";
            lblstatus.Text = "Grabación finalizada";

            descripcion.IsEnabled = true;
            btnguardar.IsEnabled = true;
            btnlista.IsEnabled = true;

            await audioRecorderService.StopRecording();
            var stream = audioRecorderService.GetAudioFileStream();
            bool NoExist = File.Exists(nombre);
            if (!NoExist)
            {
                /////////Audio Existente por Defecto 
            }
            else
            {
                String[] FRecord = nombre.Split('/');
                int tamfile = FRecord.Length;
                String nombreA = FRecord[tamfile - 1];
                String valorResultante = new String(nombreA.Where(Char.IsDigit).ToArray());

                if (!String.IsNullOrEmpty(valorResultante))
                {
                    int num = Int32.Parse(valorResultante);
                    nombre = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DateTime.Now.ToString("ddMMyyyymmss").Trim() + "Audio_.wav");
                }
            }

            using (var audiofile = new FileStream(nombre, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(audiofile);
            }
            await DisplayAlert("Notificación", "El audio se guardo satisfactoriamente", "Ok");
        }

        private async void Grabar_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(nombre))
            {

            }
            else
            {
                if (!audioRecorderService.IsRecording)
                {
                    min = 0;
                    seg = 0;
                    Grabando = true;

                    Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                        seg++;

                        if (seg.ToString().Length == 1)
                        {
                            lblseg.Text = "0" + seg.ToString();
                        }
                        else
                        {
                            lblseg.Text = seg.ToString();
                        }
                        if (seg == 60)
                        {
                            min++;
                            seg = 0;

                            if (min.ToString().Length == 1)
                            {
                                lblmin.Text = "0" + min.ToString();
                            }
                            else
                            {
                                lblmin.Text = min.ToString();
                            }

                            lblseg.Text = "00";
                        }
                        return Grabando;
                    });

                    lblstatus.Text = "Grabando...";
                    var grab = await audioRecorderService.StartRecording();
                    Grabar.IsEnabled = false;
                    btnDetener.IsEnabled = true;

                    await grab;

                    GraAudio = true;
                }
            }
        }

        private async void foto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var TomarFoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Pictures",
                    Name = DateTime.Now.ToString() + "_IMG.jpg",
                    SaveToAlbum = true,
                    CompressionQuality = 40
                });

                if (TomarFoto != null)
                {
                    ImagenSave = null;
                    MemoryStream memoryStream = new MemoryStream();

                    TomarFoto.GetStream().CopyTo(memoryStream);
                    ImagenSave = memoryStream.ToArray();

                    UbiImagen.Source = ImageSource.FromStream(() => { return TomarFoto.GetStream(); });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
