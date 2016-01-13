using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ETLProject.Models
{
    /// <summary>
    /// Klasa zajmująca się eksportem danych do pliku CSV.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CsvExport<T> where T : class
    {
        /// <summary>
        /// Separator używany w pliku CSV
        /// </summary>
        private const string ListSeparator = ";";

        /// <summary>
        /// Obiekt listy przekazywany w konstruktorze klasy
        /// </summary>
        private List<object> list;


        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        /// <param name="list">Lista przyjmowana przez konstruktor</param>
        public CsvExport(List<object> list)
        {
            this.list = list;
        }
        /// <summary>
        /// metoda wywołująca metodę Export(boolincludeHeaderLine)
        /// </summary>
        /// <returns>Zwraca wynik wywoływanej metody</returns>
        public string Export()
        {
            return Export(true);
        }
        /// <summary>
        /// Metoda exportu danych z listy. Dołącza nagłówki (tj. nazwy pól z bazy danych)
        /// </summary>
        /// <param name="includeHeaderLine">Jeżeli true - dołącz nagłówki, jeżeli False - nagłówki nie są dołączane</param>
        /// <returns>Zwraca ciąg znaków String zapisywany w pliku</returns>
        public string Export(bool includeHeaderLine)
        {

            var sb = new StringBuilder();

            //Get properties using reflection. 
            var propertyInfos = typeof(T).GetTypeInfo();

            if (includeHeaderLine)
            {
                //add header line. 
                foreach (var propertyInfo in propertyInfos.DeclaredProperties)
                {
                    sb.Append(propertyInfo.Name).Append(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                }
                sb.Remove(sb.Length - 1, 1).AppendLine();
            }

            //add value for each property. 
            foreach (T obj in list)
            {
                foreach (var propertyInfo in propertyInfos.DeclaredProperties)
                {
                    sb.Append(MakeValueCsvFriendly(propertyInfo.GetValue(obj, null))).Append(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                }

                sb.Remove(sb.Length - 1, 1).AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Metoda zajmująca się eksportem danych do pliku.
        /// Ustawiany jest FileSavePicker, w tym rozszeżenie zapisywanego pliku, oraz jego sugerowana nazwa
        /// Następnie sprawdzany jest warunek wyboru scieżki i pliku. Jeżeli plik został wybrany następuje zapis.
        /// Jeżeli nie, metoda kończy się bez efektu.
        /// </summary>
        /// <param name="path">Nazwa pliku przekazywana przy kliknięciu przycisku wywołującego metodę.</param>
        public async void ExportToFile(string path)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("CSV (.csv)", new List<string>() { ".csv" });
            savePicker.SuggestedFileName = path + ".csv";
            //var storageFolder = KnownFolders.DocumentsLibrary;

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, Export());
            }
            
        }


        /// <summary>
        /// Metoda sprawdzająca czy pole jest typu DateTime i przekształca je do wartości String.
        /// </summary>
        /// <param name="value">Zawartość sprawdzanego pola.</param>
        /// <returns>Zwracana przekształcone dane.</returns>
        private string MakeValueCsvFriendly(object value)
        {
            if (value == null) return "";

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            string output = value.ToString();

            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';

            return output;

        }
    }
}