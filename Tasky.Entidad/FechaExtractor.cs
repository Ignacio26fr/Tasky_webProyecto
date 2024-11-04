using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tasky.Entidad
{
    public class FechaExtractor
    {
        public DateTime ExtraerFecha(string texto)
        {
            var regexFechaCorta = new Regex(@"\b(\d{1,2})/(\d{1,2})\b");
            var regexFechaLarga = new Regex(@"\b(\d{1,2}) de (enero|febrero|marzo|abril|mayo|junio|julio|agosto|septiembre|octubre|noviembre|diciembre)\b", RegexOptions.IgnoreCase);
            var regexProximaSemana = new Regex(@"próxima semana", RegexOptions.IgnoreCase);
            var regexFechaLargaConAño = new Regex(@"\b(\d{1,2}) de (enero|febrero|marzo|abril|mayo|junio|julio|agosto|septiembre|octubre|noviembre|diciembre) de (\d{4})\b", RegexOptions.IgnoreCase);
            var regexHora = new Regex(@"\b(\d{1,2}):(\d{2})\b");

            LocalDate fecha;
            LocalTime hora = LocalTime.Midnight;

            if (regexProximaSemana.IsMatch(texto))
            {
                fecha = LocalDate.FromDateTime(DateTime.Now.AddDays(7));
                return new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, hora.Second);
            }

            var matchFechaCorta = regexFechaCorta.Match(texto);
            if (matchFechaCorta.Success)
            {
                var dia = int.Parse(matchFechaCorta.Groups[1].Value);
                var mes = int.Parse(matchFechaCorta.Groups[2].Value);
                int año = DateTime.Now.Year;

                if (!EsFechaValida(dia, mes, año) || new LocalDate(año, mes, dia) < LocalDate.FromDateTime(DateTime.Now))
                {
                    año++;
                }

                if (EsFechaValida(dia, mes, año))
                {
                    fecha = new LocalDate(año, mes, dia);
                    return new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, hora.Second);
                }
            }

            var matchFechaLarga = regexFechaLarga.Match(texto);
            if (matchFechaLarga.Success)
            {
                var dia = int.Parse(matchFechaLarga.Groups[1].Value);
                var mesTexto = matchFechaLarga.Groups[2].Value.ToLower();
                var mes = ObtenerNumeroMes(mesTexto);
                int año = DateTime.Now.Year;

                if (!EsFechaValida(dia, mes, año) || new LocalDate(año, mes, dia) < LocalDate.FromDateTime(DateTime.Now))
                {
                    año++;
                }

                if (EsFechaValida(dia, mes, año))
                {
                    fecha = new LocalDate(año, mes, dia);
                    return new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, hora.Second);
                }
            }

            var matchFechaLargaConAño = regexFechaLargaConAño.Match(texto);
            if (matchFechaLargaConAño.Success)
            {
                var dia = int.Parse(matchFechaLargaConAño.Groups[1].Value);
                var mesTexto = matchFechaLargaConAño.Groups[2].Value.ToLower();
                var mes = ObtenerNumeroMes(mesTexto);
                var año = int.Parse(matchFechaLargaConAño.Groups[3].Value);

                if (EsFechaValida(dia, mes, año))
                {
                    fecha = new LocalDate(año, mes, dia);
                    return new DateTime(fecha.Year, fecha.Month, fecha.Day, hora.Hour, hora.Minute, hora.Second);
                }
            }

            return DateTime.Now; // Cambiar a null en caso de no encontrar fecha
        }


        public string FormatearFechaHora(LocalDate fecha, LocalTime hora)
        {
            var dateTime = fecha + hora;
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }


        public bool EsFechaValida(int dia, int mes, int año)
        {
            try
            {
                new LocalDate(año, mes, dia);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }



        private int ObtenerNumeroMes(string mesTexto)
        {
            return mesTexto switch
            {
                "enero" => 1,
                "febrero" => 2,
                "marzo" => 3,
                "abril" => 4,
                "mayo" => 5,
                "junio" => 6,
                "julio" => 7,
                "agosto" => 8,
                "septiembre" => 9,
                "octubre" => 10,
                "noviembre" => 11,
                "diciembre" => 12,
                _ => throw new ArgumentException("Mes no válido")
            };
        }

        private LocalTime ObtenerHoraSiExiste(string texto)
        {
            var regexHora = new Regex(@"\b(\d{1,2}):(\d{2})\b");
            var matchHora = regexHora.Match(texto);

            if (matchHora.Success)
            {
                var hora = int.Parse(matchHora.Groups[1].Value);
                var minuto = int.Parse(matchHora.Groups[2].Value);
                return new LocalTime(hora, minuto);
            }
            return LocalTime.Midnight;
        }
    }
}
