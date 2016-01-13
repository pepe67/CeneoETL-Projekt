using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ETLProject.Models
{
    class CsvExport<T> where T : class
    {
        private const string ListSeparator = ";";

        public IList<T> Objects;
        private List<object> list;

        public CsvExport(IList<T> objects)
        {
            Objects = objects;
        }

        public CsvExport(List<object> list)
        {
            this.list = list;
        }

        public string Export()
        {
            return Export(true);
        }

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

        //export to a file. 
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

        //export as binary data. 
        public byte[] ExportToBytes()
        {
            return Encoding.UTF8.GetBytes(Export());
        }

        //get the csv value for field. 
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