using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProject.ViewModels;

namespace ETLProject.Models
{
    class Load
    {
        public async Task<long> InsertDevice(string deviceName, string manufacturer, string others)
        {
            long deviceId = await CreateDatabase.InsertDevice(App.conn, deviceName, manufacturer, others);
            return deviceId;
        }
        public async void InsertCommentForDevice(int id, string zalety, string wady, string autor, string tekstOpinii, string gwiazdki, string data, string polecam, string przydatnosc, string pochodzenie)
        {
            await CreateDatabase.InsertComment(App.conn, id, zalety, wady, autor, tekstOpinii, gwiazdki, data, polecam, przydatnosc, pochodzenie);
        }
    }
}
