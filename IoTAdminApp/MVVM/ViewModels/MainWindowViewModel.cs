using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAdminApp.MVVM.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private object _currentView;

        public RelayCommand BedroomViewCommand { get; set; }


        public BedroomViewModel BedroomViewModel { get; set; }



        public MainWindowViewModel()
        {

            BedroomViewModel = new BedroomViewModel();



            BedroomViewCommand = new RelayCommand(x => { CurrentView = BedroomViewModel; });


            CurrentView = BedroomViewModel;
        }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

    }
}
