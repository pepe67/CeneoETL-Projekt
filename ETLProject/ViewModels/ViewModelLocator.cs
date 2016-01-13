using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ETLProject.ViewModels
{
    /// <summary>
    /// Lokalizator ViewModeli do aplikacji. Potrzebny do implementacji modelu MVVM
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        //public DevicesViewModel Devices => ServiceLocator.Current.GetInstance<DevicesViewModel>();
    }
}
