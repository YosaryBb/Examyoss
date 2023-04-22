using Examyoss.Model;
using Examyoss.Services;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Examyoss.ViewModel
{
    public class NotasModelView
    {
        List<Notas> notasList = new List<Notas>();

        public async Task<List<Notas>> obtenerNotas()
        {
            var data = await Conexionfirebase.firebase
                .Child("notas")
                .OrderByKey()
                .OnceAsync<Notas>();

            foreach (var item in data)
            {
                Notas notas = new Notas();
                notas.Id = item.Key;
                notas.Descripcion = item.Object.Descripcion;
                notas.Fecha = item.Object.Fecha;
                notas.Audio = item.Object.Audio;
                notas.Foto= item.Object.Foto;

                notasList.Add(notas);
            }

            return notasList;
        }

        public async Task<bool> insertarNotas(Notas notas)
        {
            var data = await Conexionfirebase.firebase
                .Child("notas")
                .PostAsync(new Notas()
                {
                    Id = notas.Id,
                    Descripcion = notas.Descripcion,
                    Fecha = notas.Fecha,
                    Audio = notas.Audio,
                    Foto = notas.Foto,
                });

            return data != null;
        }
    }
}
